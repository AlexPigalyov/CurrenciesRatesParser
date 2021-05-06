using CurrenciesRatesParser.Model;
using Quartz;
using ratesRatesParser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrenciesRatesParser.Jobs
{
    public class CoinRatesParserJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Task<List<CoinsRate>> coinsRatesZolotoyZapasTask = ParserService.GetCoinsRatesZolotoyZapas();
            Task<List<CoinsRate>> coinsRatesZolotoMDTask = ParserService.GetCoinsRatesZolotoMD();
            Task<List<CoinsRate>> coinsRatesZolotoyClubTask = ParserService.GetCoinsRatesZolotoyClub();
            Task<List<CoinsRate>> coinsRatesMotenaInvestTask = ParserService.GetCoinsRatesMonetaInvest();
            Task<List<CoinsRate>> coinsRatesVFBankTask = ParserService.GetCoinsRatesVFBank();
            Task<List<CoinsRate>> coinsRates9999dTask = ParserService.GetCoinsRates9999dRu();
            Task<List<CoinsRate>> coinsRatesRicGoldComTask = ParserService.GetCoinsRatesRicgoldCom();
            Task<List<CoinsRate>> coinsRatesRshbRuTask = ParserService.GetCoinsRatesRshbRu();
            Task<List<CoinsRate>> coinsRatesSberbankTask = ParserService.GetCoinsRatesSberbank();
            Task<List<CoinsRate>> coinsRatesLantaRuTask = ParserService.GetCoinsRatesLantaRu();
            Task<List<CoinsRate>> coinsRatesTsbnkTask = ParserService.GetCoinsRateTsBnk();
            Task<List<CoinsRate>> coinsRatesZolotoidvorTask = ParserService.GetCoinsRatesZolotoyDvor();
            Task<List<CoinsRate>> coinsRatesMkdRuTask = ParserService.GetCoinsRatesMkdRu();
            Task<List<CoinsRate>> coinsRatesTkbbankTask = ParserService.GetCoinsRatesTkbbank();






            await Task.WhenAll(
                coinsRatesZolotoyZapasTask,
                coinsRatesZolotoyClubTask,
                coinsRatesZolotoMDTask,
                coinsRatesMotenaInvestTask,
                coinsRatesVFBankTask,
                coinsRates9999dTask,
                coinsRatesRicGoldComTask,
                coinsRatesRshbRuTask,
                coinsRatesSberbankTask,
                coinsRatesLantaRuTask,
                coinsRatesTsbnkTask,
                coinsRatesZolotoidvorTask,
                coinsRatesMkdRuTask,
                coinsRatesTkbbankTask);

            List<CoinsRate> coinsRatesZolotoyZapas = await coinsRatesZolotoyZapasTask;
            List<CoinsRate> coinsRatesZolotoMD = await coinsRatesZolotoMDTask;
            List<CoinsRate> coinsRatesZolotoyClub = await coinsRatesZolotoyClubTask;
            List<CoinsRate> coinsRatesMotenaInvest = await coinsRatesMotenaInvestTask;
            List<CoinsRate> coinsRatesVFBank = await coinsRatesVFBankTask;
            List<CoinsRate> coinsRates9999d = await coinsRates9999dTask;
            List<CoinsRate> coinsRatesRicGoldCom = await coinsRatesRicGoldComTask;
            List<CoinsRate> coinsRatesRshbRu = await coinsRatesRshbRuTask;
            List<CoinsRate> coinsRatesSberbank = await coinsRatesSberbankTask;
            List<CoinsRate> coinsRatesLantaRu = await coinsRatesLantaRuTask;
            List<CoinsRate> coinsRatesTsbnk = await coinsRatesTsbnkTask;
            List<CoinsRate> coinsRatesZolotoidvor = await coinsRatesZolotoidvorTask;
            List<CoinsRate> coinsRatesMkdRu = await coinsRatesMkdRuTask;
            List<CoinsRate> coinsRatesTkbbank = await coinsRatesTkbbankTask;




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
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesRicGoldCom);
            Console.WriteLine("Coins rates ricgold.com saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesRshbRu);
            Console.WriteLine("Coins rates rshb.ru saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesSberbank);
            Console.WriteLine("Coins rates sberbank.ru saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesLantaRu);
            Console.WriteLine("Coins rates Lanta.ru saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesTsbnk);
            Console.WriteLine("Coins rates TsBnk saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesZolotoidvor);
            Console.WriteLine("Coins rates ZolotoiDvor saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesMkdRu);
            Console.WriteLine("Coins rates MkdRu saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));
            CoinsRatesDataHelper.AddCoinsRatesRange(coinsRatesTkbbank);
            Console.WriteLine("Coins rates Tkbbank saved. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));

            // Save to GoldTech DataBase Georgiy Pobedonosec gold coin price 
            // id = 46
            try
            {
                decimal gpmPrice = Convert.ToDecimal(coinsRatesZolotoyZapas.FirstOrDefault(x => x.Acronim == "GPM").Sell - 100);
                GoldTechDataHelper.UpdateCoinPrice(46, gpmPrice);
                Console.WriteLine("Save to goldTech price Georgiy Pobedonosec OK.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Save to goldTech price Georgiy Pobedonosec ERROR.{0}", e.Message);
            }
        }
    }
}
