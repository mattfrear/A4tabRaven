using Domain;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Services
{
    public class ImportService
    {
        public void Import()
        {
            Console.WriteLine("Starting import");

            var importFolder = Path.Combine(Directory.GetCurrentDirectory(), "Import");
            
            if (!Directory.Exists(importFolder))
            {
                Console.WriteLine("Nothing to import, no Import folder: " + importFolder);
                return;
            }

            if (!Directory.GetFiles(importFolder).Any())
            {
                Console.WriteLine("Nothing to import, no files present");
                return;
            }

            IDocumentStore documentStore = new EmbeddableDocumentStore {ConnectionStringName = "RavenDB"};
            documentStore.Conventions.IdentityPartsSeparator = "-";
            documentStore.Initialize();
            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), documentStore);

            using (var session = documentStore.OpenSession())
            {
                session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("AllDocumentsIndex", new IndexQuery());

                ImportFolder(importFolder, session);

                foreach (var folderName in Directory.GetDirectories(importFolder))
                {
                    ImportFolder(folderName, session, new DirectoryInfo(folderName).Name);
                }

                session.SaveChanges();
            }

            Console.WriteLine("Finished import, press ENTER to quit");
            Console.ReadLine();
        }

        private static void ImportFolder(string folderName, IDocumentSession session, string bookName = "")
        {
            Console.WriteLine("Importing folder " + folderName);
            Book book = null;
            if (!string.IsNullOrEmpty(bookName))
            {
                var books = session.Query<Book>().Where(x => x.Name.Equals(bookName)).ToList();
                book = books.FirstOrDefault();
                if (book != null)
                {
                    Console.WriteLine("Deleting book " + book.Name);
                    session.Delete<Book>(book);
                }

                book = new Book() { Name = bookName };
                session.Store(book);
                Console.WriteLine("Saved book " + book.Name);
            }

            foreach (var filePath in Directory.GetFiles(folderName))
            {
                ImportFile(session, filePath, book);
                //File.Delete(filePath);
            }
        }

        private static void ImportFile(IDocumentSession session, string filePath, Book book)
        {
            var filename = Path.GetFileNameWithoutExtension(filePath);
            Console.WriteLine("Importing " + filename);

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
                var bookTasks = new BookService();
                bookTasks.AddTabToBook(tab.Id, book.Id, session); // add song to Singalong book
            }
        }
    }
}