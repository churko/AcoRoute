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

        [HttpPost]
        public ActionResult CalculateRoute(RouteParams param)
        {
            var multiplier = 1E7;

            List<int[]> pointsList = new List<int[]>();

            foreach(var point in param.points)
            {
                var lat = Math.Truncate(point[0] * multiplier);
                var lon = Math.Truncate(point[1] * multiplier);

                pointsList.Add(new int[] { (int)lat, (int)lon });
            }

            int startLat = (int)Math.Truncate(param.startCoord[0] * multiplier);
            int startLon = (int)Math.Truncate(param.startCoord[1] * multiplier);
            int endLat = (int)Math.Truncate(param.endCoord[0] * multiplier);
            int endLon = (int)Math.Truncate(param.endCoord[1] * multiplier);

            int[][] pointsArray = pointsList.ToArray();
            int[] startingPoint = new int[] { startLat, startLon };
            int[] endPoint = new int[] { endLat, endLon };

            var problem = new Problem(pointsArray, startingPoint, colonySize: 50, iterations: 10, endPoint: endPoint);
            var route = problem.FindRoute();
            List<double[]> routeList = new List<double[]>();

            foreach (var point in route)
            {
                var lat = point[0] / multiplier;
                var lon = point[1] / multiplier;

                routeList.Add(new double[] { lat, lon });
            }

            var routeArray = routeList.ToArray();

            return View("~/Views/Routes/Route.cshtml", routeArray);
        }

        public class RouteParams
        {
            public double[][] points;
            public double[] startCoord;
            public double[] endCoord;
        }
    }
}