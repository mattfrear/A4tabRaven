using System.Collections.Generic;
using Web.Controllers;
using Web.Models;

namespace Web.Tasks
{
    public class BookTasks
    {
        public void AddTabToBook(int tabId, int bookId)
        {
            var ravenSession = RavenController.DocumentStore.OpenSession();
            var book = ravenSession.Load<Book>(bookId);
            if (book.TabIds == null)
            {
                book.TabIds = new List<int>();
            }

            if (!book.TabIds.Contains(tabId))
            {
                book.TabIds.Add(tabId);
            }

            ravenSession.SaveChanges();
        }
    }
}