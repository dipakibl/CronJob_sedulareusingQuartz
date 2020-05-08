using Quartsedul.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quartsedul.Repository
{
    public class QuartzRepo:IQuartzRepo
    {
        QuartzContext _context = new QuartzContext();
        public QuartzRepo()
        {

        }
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
    }
}
