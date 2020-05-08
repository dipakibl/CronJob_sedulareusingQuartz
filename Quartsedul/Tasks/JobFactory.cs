using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartsedul.Models;
using Quartsedul.Repository;
using Quartz;
using Quartz.Spi;

namespace Quartsedul.Tasks
{
    public class JobFactory:IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
           
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService<QuartzJob>();
        }

        public void ReturnJob(IJob job)
        {
            var disosable = job as IDisposable;
            disosable?.Dispose();

        }
    }
}
