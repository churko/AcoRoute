using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AcoRoute.Models;

namespace AcoRoute.Controllers
{
    public class RoutesController : Controller
    {
        private AcoRouteContext db = new AcoRouteContext();

        public ActionResult Index()
        {
            var peopleList = db.People.OrderBy(x => x.Surname).ThenBy(x => x.Name).ToList();
            
            return View("~/Views/Routes/Calculate.cshtml", peopleList);
            
        }
    }
}