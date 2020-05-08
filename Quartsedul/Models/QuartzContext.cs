using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quartsedul.Models
{
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
}
