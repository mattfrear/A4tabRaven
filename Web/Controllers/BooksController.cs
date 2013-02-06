using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers
{   
    public class BooksController : RavenController
    {
		//
        // GET: /Books/

        public ViewResult Index()
        {
            var model = RavenSession.Query<Book>().OrderBy(x => x.Name).ToList();
            return View(model);
        }

        //
        // GET: /Books/Details/5

        public ViewResult Details(int id)
        {
            Book book = RavenSession.Include<Book>(x => x.TabIds).Load<Book>(id);

            if (book == null)
            {
                return PageNotFound();
            }

            var tabs = new List<Tab>();
            if (book.TabIds != null)
            {
                foreach (var tabId in book.TabIds)
                {
                    tabs.Add(RavenSession.Load<Tab>(tabId));
                }
            }

            ViewBag.BookName = book.Name;
            return View(tabs);
        }

        //
        // GET: /Books/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Books/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var item = new Book();
            var whiteList = new string[] { "Name" }; // todo - add editable properties to this list
            if (TryUpdateModel(item, whiteList, collection.ToValueProvider()))
			{
				if (ModelState.IsValid)
				{
                    if (RavenSession.Query<Book>().FirstOrDefault(x => x.Name == item.Name) != null)
                    {
                        ModelState.AddModelError("Name", "Sorry, but a book with that name already exists. Please try another name.");
                        return View(item);
                    }

                    RavenSession.Store(item);

					TempData.Add(Constants.TempDataMessage, "Added successfully");
					return RedirectToAction("Index");  
				}
			}
            return View(item);
        }
        
        //
        // GET: /Books/Delete/5
        [Authorize] // todo, admin roles/claim
        public ActionResult Delete(int id)
        {
            Book book = RavenSession.Load<Book>(id);
            return View(book);
        }

        //
        // POST: /Books/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize] // todo, admin role
        public ActionResult DeleteConfirmed(int id)
        {
            var book = RavenSession.Load<Book>(id);
            RavenSession.Delete<Book>(book);
            return RedirectToAction("Index");
        }
    }
}