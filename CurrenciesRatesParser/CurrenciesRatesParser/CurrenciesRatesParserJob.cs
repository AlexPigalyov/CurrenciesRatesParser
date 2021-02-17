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
            Task<List<Rates>> echangeRatesTask = ParserService.GetExchangeRates();
            Task<List<Rates>> metalRatesTask = ParserService.GetMetalRates();
            Task<List<CoinsRates>> coinsRatesZolotoyZapasTask = ParserService.GetCoinsRatesZolotoyZapas();
            Task<List<CoinsRates>> coinsRatesZolotoMDTask = ParserService.GetCoinsRatesZolotoMD();
            Task<List<CoinsRates>> coinsRatesZolotoyClubTask = ParserService.GetCoinsRatesZolotoyClub();
            Task<List<CoinsRates>> coinsRatesMonetaInvestTask = ParserService.GetCoinsRatesMonetaInvest();


            await Task.WhenAll(
                echangeRatesTask,
                metalRatesTask,
                coinsRatesZolotoyZapasTask,
                coinsRatesZolotoyClubTask,
                coinsRatesZolotoMDTask,
                coinsRatesMonetaInvestTask);

            List<Rates> exchangeyRates = await echangeRatesTask;
            List<Rates> metalRates = await metalRatesTask;
            List<CoinsRates> coinsRatesZolotoyZapas = await coinsRatesZolotoyZapasTask;
            List<CoinsRates> coinsRatesZolotoMD = await coinsRatesZolotoMDTask;
            List<CoinsRates> coinsRatesZolotoyClub = await coinsRatesZolotoyClubTask;
            List<CoinsRates> coinsRatesMonetaInvest = await coinsRatesMonetaInvestTask;


            RatesDataHelper.AddRatesRange(exchangeyRates);
            RatesDataHelper.AddRatesRange(metalRates);
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesZolotoyZapas);
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesZolotoMD);
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesZolotoyClub);
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesMonetaInvest);
        }
    }
}
