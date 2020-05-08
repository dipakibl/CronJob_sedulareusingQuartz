using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartsedul.Models;
using Quartsedul.Repository;
using Quartsedul.Tasks;
using Quartz;

namespace Quartsedul.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IQuartzRepo _repo;
       
        public HomeController(ILogger<HomeController> logger, IQuartzRepo repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public  IActionResult ReserveTickets()
        {
            _repo.TransferData();
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
