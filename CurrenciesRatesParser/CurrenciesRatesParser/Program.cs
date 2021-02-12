using Quartz;
using Quartz.Impl;
using System;

namespace CurrenciesRatesParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Srvice start. Time: {0}", DateTime.Now.ToString("HH:mm:ss"));

            ISchedulerFactory schedFact = new StdSchedulerFactory();

            IScheduler sched = schedFact.GetScheduler().Result;
            sched.Start();

            IJobDetail job = JobBuilder.Create<CurrenciesRatesParserJob>()
                .WithIdentity("CurrenciesRatesParserJob")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(60 * 4 + 30)
                    .RepeatForever())
                .Build();

            _ = sched.ScheduleJob(job, trigger).Result;
            Console.ReadLine();
        }
    }
}
