using ESerranoEcoConnect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESerranoEcoConnect.Controllers
{
    public class EventController : Controller
    {
        private EcoConnectDbContext db = new EcoConnectDbContext();
        // GET: Event
        public ActionResult Index()
        {
            var eventList = db.Events.ToList();
            return View(eventList);
        }
    }
}