using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Infrastructure;
using Web.Models;


namespace Web.Controllers
{
    public class UploadController : RavenController
    {
        public ActionResult Index()
        {
            //var books = _readOnlyDb.All<Book>();
            //ViewBag.Books = new SelectList(books, "Id", "Name");
            var books = RavenSession.Query<Book>().ToList();
            ViewBag.Books = new SelectList(books, "Id", "Name");

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(FormCollection collection)
        {
            var item = new Upload();
            var whiteList = new string[] { "Artist", "Name", "Tab", "BookId" };
            if (TryUpdateModel(item, whiteList, collection.ToValueProvider()))
            {
                if (ModelState.IsValid)
                {
                    //_uploadService.Upload(item.Artist, item.Name, item.BookId, item.Tab, User.Identity.Name);

                    var tab = new Tab { Artist = item.Artist, Name = item.Name, Content = item.Tab, CreatedOn = DateTime.UtcNow, AuthorName = User.Identity.Name };
                    RavenSession.Store(tab);

                    if (item.BookId > 0)
                    {
                        var bookTasks = new BookTasks();
                        bookTasks.AddTabToBook(tab.Id, item.BookId.Value);
                    }

                    TempData.Add(Constants.TempDataMessage, "Upload successful, thanks!");
                    
                    return RedirectToAction("Index", "Songs");
                }
            }
            return View();
        }
    }
}
