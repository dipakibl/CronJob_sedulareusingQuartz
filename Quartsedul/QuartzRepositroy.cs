using Quartsedul.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quartsedul
{
    public class QuartzRepositroy : IJob
    {
        private readonly QuartzContext _quartz;
        public QuartzRepositroy(QuartzContext quartz)
        {
            _quartz = quartz;
        }
        public Task Execute(IJobExecutionContext context)
        {
            User user = new User()
            {
                Name = "Nitin",
                Contect = "5464964865",
                Date = DateTime.Now,
            };
            _quartz.Users.Add(user);
            _quartz.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
