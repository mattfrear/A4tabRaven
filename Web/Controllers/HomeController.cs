using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : RavenController
    {
        public ActionResult Index()
        {
            var tabs = RavenSession.Query<Tab>().OrderByDescending(x => x.CreatedOn).Take(10).ToList();
            return View(tabs);
        }

        [HttpPost]
        public ActionResult Search(string s)
        {
            ViewBag.SearchTerms = s;
            
            var results = RavenSession.Query<Tab>().Where(x => x.Artist.StartsWith(s) || x.Name.StartsWith(s)).ToList();
            return View(results);
        }
    }
}
