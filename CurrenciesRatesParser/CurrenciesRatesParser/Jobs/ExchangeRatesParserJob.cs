using CurrenciesRatesParser.Model;
using Quartz;
using ratesRatesParser.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrenciesRatesParser.Jobs
{
    public class ExchangeRatesParserJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            List<Rate> echangeRates = await ParserService.GetExchangeRates();

            RatesDataHelper.AddRatesRange(echangeRates);

            Console.WriteLine("Exchange rates saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
