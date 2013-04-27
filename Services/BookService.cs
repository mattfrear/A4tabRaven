using Domain;
using Raven.Client;
using System.Collections.Generic;

namespace Services
{
    public class BookService
    {
        public void AddTabToBook(int tabId, int bookId, IDocumentSession ravenSession)
        {
            var book = ravenSession.Load<Book>(bookId);
            if (book.TabIds == null)
            {
                book.TabIds = new List<int>();
            }

            if (!book.TabIds.Contains(tabId))
            {
                book.TabIds.Add(tabId);
            }
        }
    }
}