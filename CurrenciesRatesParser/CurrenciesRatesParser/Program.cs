
﻿using CurrenciesRatesParser.Necromant24;
﻿using CurrenciesRatesParser.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;

namespace CurrenciesRatesParser
{
    class Program
    {
        static void Main(string[] args)
        {
            // Necroman24 9999d.ru parser use
            Parser.Parse();

            Console.WriteLine("Service start. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));

            #region Exchange
            //EXCHANGE JOB
            IJobDetail exchangeJob = JobBuilder.Create<ExchangeRatesParserJob>()
                .WithIdentity("ExchangeRatesParserJob")
                .Build();
            //EXCHANGE TRIGGER
            ITrigger exchangeTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(int.Parse(ConfigurationManager.AppSettings["ExchangeParserDelay"]))
                    .RepeatForever())
                .Build();
            #endregion

            #region Metal
            //METAL JOB
            IJobDetail metalJob = JobBuilder.Create<MetalRatesParserJob>()
                .WithIdentity("MetalRatesParserJob")
                .Build();
            //METAL TRIGGER
            ITrigger metalTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(int.Parse(ConfigurationManager.AppSettings["MetalParserDelay"]))
                    .RepeatForever())
                .Build();
            #endregion

            #region Coin
            //COIN JOB
            IJobDetail coinJob = JobBuilder.Create<CoinRatesParserJob>()
                .WithIdentity("CoinRatesParserJob")
                .Build();
            //COIN TRIGGER
            ITrigger coinTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(int.Parse(ConfigurationManager.AppSettings["CoinParserDelay"]))
                    .RepeatForever())
                .Build();
            #endregion

            IScheduler scheduler = new StdSchedulerFactory().GetScheduler().Result;

            scheduler.ScheduleJob(exchangeJob, exchangeTrigger);
            scheduler.ScheduleJob(metalJob, metalTrigger);
            scheduler.ScheduleJob(coinJob, coinTrigger);

            scheduler.Start();

            Console.ReadLine();
        }
    }
}
