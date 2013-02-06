using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Web.Infrastructure;
using Web.Models;
using Web.Tasks;

namespace Web.Controllers
{   
    public class SongsController : RavenController
    {
        public ViewResult Index()
        {
            var tabs = RavenSession.Query<Tab>().OrderBy(t => t.Artist).ThenBy(t => t.Name).ToList();
            return View(tabs);
        }

        //
        // GET: /Songs/Details/5

        public ViewResult Details(int id)
        {
            var tab = RavenSession.Load<Tab>(id);

            if (tab == null)
            {
                return PageNotFound();
            }

            tab.Content = tab.Content ?? string.Empty;

            ViewBag.Keywords = string.Format("tab, {0}, {1}, guitar tab, chords", tab.Artist, tab.Name);
            ViewBag.Description = string.Format("tab {0} {1} guitar", tab.Artist, tab.Name);

            var allBbooks = RavenSession.Query<Book>().ToList();
            ViewBag.AllBooks = new SelectList(allBbooks, "Id", "Name");

            ViewBag.Books = allBbooks.Where(x => x.TabIds != null && x.TabIds.Contains(id)).ToList();

            return View(tab);
        }

        //
        // GET: /Songs/Download/5

        public ActionResult Download(int id)
        {
            var tab = RavenSession.Load<Tab>(id);

            if (tab == null)
            {
                return PageNotFound();
            }
            
            return File(Encoding.ASCII.GetBytes(tab.Content), "text/plain", string.Format("{0} - {1}.txt", tab.Artist, tab.Name));
        }

        //
        // GET: /Songs/Delete/5

        public ActionResult Delete(int id)
        {
            var tab = RavenSession.Load<Tab>(id);

            if (tab == null)
            {
                return PageNotFound();
            }

            return View(tab);
        }

        //
        // POST: /Songs/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var tab = RavenSession.Load<Tab>(id);
            RavenSession.Delete<Tab>(tab);
            
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public ActionResult AddToBook(int id, int bookId)
        {
            var bookTasks = new BookTasks();
            bookTasks.AddTabToBook(id, bookId);
            TempData[Constants.TempDataMessage] = "Added to Book.";

            return RedirectToAction("Details", new { id = id });
        }
    }
}