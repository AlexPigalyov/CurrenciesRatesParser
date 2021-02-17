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
            Task<List<CoinsRates>> coinsRatesZolotoyZapasTask = ParserService.GetCoinsRatesZolotoyZapas();
            Task<List<CoinsRates>> coinsRatesMonetaInvestTask = ParserService.GetCoinsRatesMonetaInvest();

            await Task.WhenAll(currencyRatesTask, metalRatesTask, coinsRatesZolotoyZapasTask, coinsRatesMonetaInvestTask);

            List<Rates> currencyRates = await currencyRatesTask;
            List<Rates> metalRates = await metalRatesTask;
            List<CoinsRates> coinsRatesZolotoyZapas = await coinsRatesZolotoyZapasTask;
            List<CoinsRates> coinsRatesMonetaInvest = await coinsRatesMonetaInvestTask;


            RatesDataHelper.AddRatesRange(currencyRates);
            RatesDataHelper.AddRatesRange(metalRates);
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesZolotoyZapas);
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesMonetaInvest);
        }
    }
}
