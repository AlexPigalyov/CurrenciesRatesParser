using Quartz;
using Quartz.Impl;
using ratesRatesParser.Services;
using System;
using System.Configuration;

namespace CurrenciesRatesParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Service start. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));

            ISchedulerFactory schedFact = new StdSchedulerFactory();

            IScheduler sched = schedFact.GetScheduler().Result;
            sched.Start();

            IJobDetail job = JobBuilder.Create<CurrenciesRatesParserJob>()
                .WithIdentity("CurrenciesRatesParserJob")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(int.Parse(ConfigurationManager.AppSettings["ParserDelay"]))
                    .RepeatForever())
                .Build();

            _ = sched.ScheduleJob(job, trigger).Result;

            Console.ReadLine();
        }
    }
}
