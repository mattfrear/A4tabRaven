using System.Linq;
using System.Web.Mvc;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{   
    public class ArtistsController : RavenController
    {
		//
        // GET: /Artists/

        public ViewResult Index()
        {
            // todo figure out map reduce with Raven

            var artists = RavenSession.Query<Tab>().OrderBy(t => t.Artist)
                .ToList()
                .GroupBy(x => x.Artist)
                .Select(y => new Artist { ArtistName = y.Key, TabCount = y.Count() })
                .ToList();

            if (!artists.Any())
                return PageNotFound();

            return View(artists);
        }

        //
        // GET: /Artists/Details/Bob%20Marley

        public ViewResult Details(string id)
        {
            var tabs = RavenSession.Query<Tab>().Where(x => x.Artist.Equals(id)).OrderBy(x => x.Name).ToList();

            if (!tabs.Any())
                return PageNotFound();

            return View(tabs);
        }
    }
}