using CurrenciesRatesParser.Model;
using Newtonsoft.Json;
using Quartz;
using ratesRatesParser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrenciesRatesParser
{
    public class CurrenciesRatesParserJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Task<List<Rates>> currencyRatesTask = ParserService.GetCurrencyRates();
            Task<List<Rates>> metalRatesTask = ParserService.GetMetalRates();

            await Task.WhenAll(currencyRatesTask, metalRatesTask);

            List<Rates> currencyRates = await currencyRatesTask;
            List<Rates> metalRates = await metalRatesTask;


            RatesDataHelper.AddRatesRange(currencyRates);
            RatesDataHelper.AddRatesRange(metalRates);
        }
    }
}
