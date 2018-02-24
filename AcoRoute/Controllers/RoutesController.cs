using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AcoRoute.Models;
using AcoEngine;

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

        public class RouteParams
        {
            public string[][] Points { get; set; }
            public string[] StartCoord { get; set; }
            public string[] EndCoord { get; set; }
        }

        [HttpPost]
        public ActionResult CalculateRoute(RouteParams param)
        {
            var multiplier = 1E7;

            List<long[]> pointsList = new List<long[]>();

            foreach (var point in param.Points)
            {
                var lat = Math.Truncate(Convert.ToDouble(point[0]) * multiplier);
                var lon = Math.Truncate(Convert.ToDouble(point[1]) * multiplier);

                pointsList.Add(new long[] {(long)lat, (long)lon });
            }

            long startLat = (long)Math.Truncate(Convert.ToDouble(param.StartCoord[0]) * multiplier);
            long startLon = (long)Math.Truncate(Convert.ToDouble(param.StartCoord[1]) * multiplier);
            long endLat = (long)Math.Truncate(Convert.ToDouble(param.EndCoord[0]) * multiplier);
            long endLon = (long)Math.Truncate(Convert.ToDouble(param.EndCoord[1]) * multiplier);

            long[][] pointsArray = pointsList.ToArray();
            long[] startingPoint = new long[] { startLat, startLon };
            long[] endPoint = new long[] { endLat, endLon };

            //var problem = new Problem(pointsArray, startingPoint, colonySize: 50, iterations: 10, endPoint: endPoint);
            //var route = problem.FindRoute();
            //List<double[]> routeList = new List<double[]>();

            //foreach (var point in route)
            //{
            //    var lat = point[0] / multiplier;
            //    var lon = point[1] / multiplier;

            //    routeList.Add(new double[] { lat, lon });
            //}

            //var routeArray = routeList.ToArray();

            return View("~/Views/Routes/Route.cshtml", null);
        }

        
    }
}