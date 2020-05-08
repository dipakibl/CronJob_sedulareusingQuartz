using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartsedul.Repository;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Quartsedul.Tasks
{
    public class QuartStart
    {
        public async static Task QuartSet()
        {
            IConfigurationSection configurationSection = GetConnectionSection();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.AddScoped<QuartzJob>();
            serviceCollection.AddScoped<IQuartzRepo,QuartzRepo>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            await ScheduleJob(serviceProvider);
        }
        private static IConfigurationSection GetConnectionSection()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", true, true);
            var configuration = builder.Build();
            var connectionSection = configuration.GetSection("ConnectionStrings:DefaultConnection");
            return connectionSection;
        }
        private static async Task ScheduleJob(IServiceProvider serviceProvider)
        {
            var Propertis = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            
            var factory = new StdSchedulerFactory(Propertis);
            var sched = await factory.GetScheduler();
            sched.JobFactory = new JobFactory(serviceProvider);
            await sched.Start();
        
            var job = JobBuilder.Create<QuartzJob>()
                .WithIdentity("myJob", "group1")
                .Build();
            //  var trigger = TriggerBuilder.Create()
            //    .WithIdentity("myTrigger", "group1")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInHours(11)
            //        .RepeatForever())
            //.Build();


            ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("Test")
                        .WithSchedule(CronScheduleBuilder
                        .CronSchedule("0 0 9 1/1 * ? *"))
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(2)
                        .WithRepeatCount(10))
                        .Build();
            await sched.ScheduleJob(job, trigger);

        }
    }
}
