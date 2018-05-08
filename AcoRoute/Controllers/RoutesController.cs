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
using System.Globalization;

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
                
                var lat = double.Parse(point[0], CultureInfo.InvariantCulture);
                var lon = double.Parse(point[1], CultureInfo.InvariantCulture);
                var personId = double.Parse(point[2], CultureInfo.InvariantCulture);

                pointsList.Add(new double[] {lat, lon, personId });
            }

            var startLat = double.Parse(param.StartCoord[0], CultureInfo.InvariantCulture);
            var startLon = double.Parse(param.StartCoord[1], CultureInfo.InvariantCulture);
            var startPersonId = double.Parse(param.StartCoord[2], CultureInfo.InvariantCulture);
            var endLat = double.Parse(param.EndCoord[0], CultureInfo.InvariantCulture);
            var endLon = double.Parse(param.EndCoord[1], CultureInfo.InvariantCulture);
            var endPersonId = double.Parse(param.EndCoord[2], CultureInfo.InvariantCulture);

            double[][] pointsArray = pointsList.ToArray();
            double[] startingPoint = new double[] { startLat, startLon, startPersonId };
            double[] endPoint = new double[] { endLat, endLon, endPersonId };

            var problem = new Problem(pointsArray, startingPoint, colonySize: 50, iterations: 10, endPoint: endPoint);
            var route = problem.FindRoute();
            List<Result> routeResult = new List<Result>();

            foreach (double[] node in route)
            {
                Result res = new Result
                {
                    Latitude = node[0],
                    Longitude = node[1],
                    PersonId = (int)node[2]
                };
                routeResult.Add(res);
            }

            return Json(new { result = "Redirect", url = Url.Action("DrawRoute", "Routes"), routeResult = routeResult });
        }

        public ActionResult DrawRoute(List<Result> result)
        {
            //return View("~/Views/Home/Index.cshtml");
            return View("~/Views/Routes/RouteResult.cshtml", result);
        }

    }
}