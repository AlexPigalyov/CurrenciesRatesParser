using CurrenciesRatesParser.Model;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrenciesRatesParser
{
    public class CurrenciesRatesParserJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Task<Dictionary<string, double>> currencyRatesTask = RatesDataHelper.GetCurrencyRates();
            Task<Dictionary<string, double>> metalRatesTask = RatesDataHelper.GetMetalRates();

            await Task.WhenAll(currencyRatesTask, metalRatesTask);

            Dictionary<string, double> currencyRates = await currencyRatesTask;
            Dictionary<string, double> metalRates = await metalRatesTask;

            Dictionary<string, double> allRates = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> metalCurrency in metalRates)
            {
                allRates.Add(metalCurrency.Key, metalCurrency.Value);
            }
            allRates.Add("USD", 1);
            foreach (KeyValuePair<string, double> valuteCurrency in currencyRates)
            {
                allRates.Add(valuteCurrency.Key, valuteCurrency.Value);
            }

            RatesDataHelper.AddRates(new Rates()
            {
                Date = DateTime.Now,
                Value = JsonConvert.SerializeObject(allRates)
            });
        }
    }
}
