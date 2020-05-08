using Quartsedul.Models;
using Quartsedul.Repository;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quartsedul.Tasks
{
    public class QuartzJob : IJob
    {
        
        IQuartzRepo _repo = new QuartzRepo();
        public Task Execute(IJobExecutionContext context)
        {
            _repo.TransferData();
            return Task.CompletedTask;
        }
    }
}