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
            List<double[]> pointsList = new List<double[]>();

            foreach (var point in param.Points)
            {
                var lat = Convert.ToDouble(point[0]);
                var lon = Convert.ToDouble(point[1]);

                pointsList.Add(new double[] {lat, lon });
            }

            var startLat = Convert.ToDouble(param.StartCoord[0]);
            var startLon = Convert.ToDouble(param.StartCoord[1]);
            var endLat = Convert.ToDouble(param.EndCoord[0]);
            var endLon = Convert.ToDouble(param.EndCoord[1]);

            double[][] pointsArray = pointsList.ToArray();
            double[] startingPoint = new double[] { startLat, startLon };
            double[] endPoint = new double[] { endLat, endLon };

            var problem = new Problem(pointsArray, startingPoint, colonySize: 50, iterations: 10, endPoint: endPoint);
            var route = problem.FindRoute();
            

            return View("~/Views/Routes/Route.cshtml", route);
        }

        
    }
}