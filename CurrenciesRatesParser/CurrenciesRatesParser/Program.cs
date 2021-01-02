using Quartz;
using Quartz.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrenciesRatesParser
{
    class Program
    {
        static void Main(string[] args)
        {
            
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
