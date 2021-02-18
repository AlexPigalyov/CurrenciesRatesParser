using CurrenciesRatesParser.Model;
using Quartz;
using ratesRatesParser.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrenciesRatesParser.Jobs
{
    public class CoinRatesParserJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Task<List<CoinsRates>> coinsRatesZolotoyZapasTask = ParserService.GetCoinsRatesZolotoyZapas();
            Task<List<CoinsRates>> coinsRatesZolotoMDTask = ParserService.GetCoinsRatesZolotoMD();
            Task<List<CoinsRates>> coinsRatesZolotoyClubTask = ParserService.GetCoinsRatesZolotoyClub();
            Task<List<CoinsRates>> coinsRatesMotenaInvestTask = ParserService.GetCoinsRatesMonetaInvest();
            Task<List<CoinsRates>> coinsRatesVFBankTask = ParserService.GetCoinsRatesVFBank();

            Task<List<CoinsRates>> coinRates9999d_ruTask = Necromant24.Parser.Get_9999d_ru_coins();

            await Task.WhenAll(
                coinsRatesZolotoyZapasTask,
                coinsRatesZolotoyClubTask,
                coinsRatesZolotoMDTask,
                coinsRatesMotenaInvestTask,
                coinsRatesVFBankTask,
                coinRates9999d_ruTask);

            List<CoinsRates> coinsRatesZolotoyZapas = await coinsRatesZolotoyZapasTask;
            List<CoinsRates> coinsRatesZolotoMD = await coinsRatesZolotoMDTask;
            List<CoinsRates> coinsRatesZolotoyClub = await coinsRatesZolotoyClubTask;
            List<CoinsRates> coinsRatesMotenaInvest = await coinsRatesMotenaInvestTask;
            List<CoinsRates> coinsRatesVFBank = await coinsRatesVFBankTask;

            List<CoinsRates> coinsRates9999d = await coinRates9999d_ruTask;

            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesZolotoyZapas);
            Console.WriteLine("Coins rates zolotoy zapas saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesZolotoMD);
            Console.WriteLine("Coins rates zoloto md saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesZolotoyClub);
            Console.WriteLine("Coins rates zolotoy club saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesMotenaInvest);
            Console.WriteLine("Coins rates moneta invest saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesVFBank);
            Console.WriteLine("Coins rates vfbank saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));

            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRates9999d);
            Console.WriteLine("Coins rates 9999d.ru saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));

        }
    }
}
