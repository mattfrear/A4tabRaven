using System.Linq;
using System.Web.Mvc;
using Web.Indexes;
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
            var artists = RavenSession.Query<Artist, ArtistsIndex>().OrderBy(x => x.ArtistName);

            if (!artists.Any())
            {
                return PageNotFound();
            }

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