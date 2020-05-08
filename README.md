# CronJob_sedulareusingQuartz

==========how to create this Quartz Scheduler project.=============
1) Create  .net core project.
2) Create User class in models folder.
		public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contect { get; set; }
        public DateTime Date { get; set; }
    }

3) Create TempUser class in models folder.
	 public class TempUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contect { get; set; }
        public DateTime Date { get; set; }
    }

4) create QuartzContext class in models folder.
	 public class QuartzContext:DbContext
    {
        public QuartzContext()
        {

        }
        public QuartzContext(DbContextOptions<QuartzContext> options):base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<TempUser> TempUsers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-UEHO8OV\\SQLEXPRESS;Database=Schedule; Trusted_Connection=True;");
        }
    }

5) Migration. 
	- Set up a connection string in appsettings.json.
	- Set up AddDbContext in the ConfigureServices action within the Startup class.
	
			services.AddDbContext<QuartzContext>(options =>
            options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
			
	- after that, Open the Package Manager console for that. And follow the command below.
			- Add-Migration <migration_Name>
			- Update_database
	
6) install the Quartz from Manage Nuget Packages.

7) Create a new folder named Task.

8) create QuartStart class. in that create QuartSet static method. in that, set the configurationSection.
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

9) create JobFactory class in Task Folder. Set the IJobFactory interface in that class. create NewJob and ReturnJob method.

		  public class JobFactory:IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
           
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            
        }

        public void ReturnJob(IJob job)
        {
            var disosable = job as IDisposable;
            disosable?.Dispose();

        }
    }
	
10) create QuartzJob class in Task folder. Set the IJob interface in it.

  public class QuartzJob : IJob
    {
        
        public Task Execute(IJobExecutionContext context)
        {
           
            return Task.CompletedTask;
        }
    }	
	
11) Refer to QuartzJob in the NewJob method in the JobFactory class.

		 public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetService<QuartzJob>();
        }
			
			
12) create ScheduleJob method in QuartStart class. in that, setup the factory, job and Trigger.
	- Refer to the JobFactory class in sched.JobFactory. Refer to the QuartzJobclass in JobBuilder.Create
	- Schedule in me trigger is repeated 10 times every 15 minutes at 9 am. 
	
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
           

            ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("Test")
                        .WithSchedule(CronScheduleBuilder
                        .CronSchedule("0 0 9 1/1 * ? *"))
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(15)
                        .WithRepeatCount(10))
                        .Build();
            await sched.ScheduleJob(job, trigger);

        }
		
13) Create a new folder named Repository.

14) Create IQuartzRepo class in Repository folder. in that, create void action.

	 public interface IQuartzRepo
    {
        void TransferData();
    }
	
15) Create QuartRepo class in Repository folder. And set the interface of IQuartzRepo in it.
	after that, crate a TransferData action for update data temp table.
	
	  public void TransferData()
        {
            try
            {
               
                List<User> data = _context.Users.ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    User user = data[i];
                    var tempUser = _context.TempUsers.Where(x => x.Name == data[i].Name).FirstOrDefault();
                    if (tempUser == null)
                    {
                        TempUser temp = new TempUser()
                        {
                            Name = data[i].Name,
                            Contect = data[i].Contect,
                            Date = DateTime.Now
                        };
                        _context.TempUsers.Add(temp);
                        _context.SaveChanges();

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
15) Refer to the TransferData action in QuartzJob.
	
	  IQuartzRepo _repo = new QuartzRepo();
        public Task Execute(IJobExecutionContext context)
        {
            _repo.TransferData();
            return Task.CompletedTask;
        }
		
16) Refer to QuartStart.QuartSet () in the Configure action of the Startup class.

	   app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
			
             QuartStart.QuartSet();
        }
	