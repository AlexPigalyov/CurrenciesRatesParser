using CurrenciesRatesParser.Model;
using Quartz;
using ratesRatesParser.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrenciesRatesParser.Jobs
{
    public class MetalRatesParserJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            List<Rates> echangeRates = await ParserService.GetMetalRates();

            RatesDataHelper.AddRatesRange(echangeRates);

            Console.WriteLine("Metal rates saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
