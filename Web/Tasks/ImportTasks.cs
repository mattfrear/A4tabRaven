using System.Configuration;
using System.IO;
using Web.Controllers;
using Web.Models;
using System.Linq;
using System;
using Web.Infrastructure;
using Raven.Client;
using Raven.Abstractions.Data;

namespace Web.Tasks
{
    public class ImportTasks
    {
        public void Import()
        {
            new LogEvent("Starting import").Raise();

            var physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Import");
            if (!Directory.GetFiles(physicalPath).Any())
            {
                new LogEvent("Nothing to import").Raise();
                return;
            }

            using (var session = RavenController.DocumentStore.OpenSession())
            {
                session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("AllDocumentsIndex", new IndexQuery());
                
                ImportFolder(physicalPath, session);

                foreach (var folderName in Directory.GetDirectories(physicalPath))
                {
                    ImportFolder(folderName, session, new DirectoryInfo(folderName).Name);
                }

                session.SaveChanges();
            }

            new LogEvent("Finished import").Raise();
        }

        private static void ImportFolder(string folderName, Raven.Client.IDocumentSession session, string bookName = "")
        {
            var book = session.Query<Book>().Where(x => x.Name.Equals(bookName)).FirstOrDefault();
            if (!string.IsNullOrEmpty(bookName))
            {
                if (book != null)
                {
                    session.Delete<Book>(book);
                }

                book = new Book() { Name = bookName };
                session.Store(book);
                session.SaveChanges();
            }

            foreach (var filePath in Directory.GetFiles(folderName))
            {
                ImportFile(session, filePath, book);

                File.Delete(filePath);
            }
        }

        private static void ImportFile(IDocumentSession session, string filePath, Book book)
        {
            var bookTasks = new BookTasks();
            
            var filename = Path.GetFileNameWithoutExtension(filePath);
            var parts = filename.Split(new[] { " - " }, System.StringSplitOptions.None);
            if (parts.Length < 2)
            {
                return;
            }

            var artist = parts.First().Trim();
            var name = parts.Last().Trim();

            var content = File.ReadAllText(filePath);

            var tab = new Tab { Artist = artist, Name = name, Content = content, CreatedOn = DateTime.Now };

            session.Store(tab);

            if (book != null)
            {
                bookTasks.AddTabToBook(tab.Id, book.Id); // add song to Singalong book
            }
        }
    }
}