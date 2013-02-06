using System.Configuration;
using System.IO;
using Web.Controllers;
using Web.Models;
using System.Linq;
using System;

namespace Web.Tasks
{
    public class ImportTasks
    {
        public void Import()
        {
            var path = ConfigurationManager.AppSettings["ImportFolder"];
            
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var physicalPath = System.Web.HttpContext.Current.Server.MapPath(path);

            using (var session = RavenController.DocumentStore.OpenSession())
            {
                var book = session.Query<Book>().Where(x => x.Name.Equals("Singalong")).FirstOrDefault();
                var bookTasks = new BookTasks();

                foreach (var filePath in Directory.GetFiles(physicalPath))
                {
                    var filename = Path.GetFileNameWithoutExtension(filePath);
                    var parts = filename.Split(new[] { " - " }, System.StringSplitOptions.None);
                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    var artist = parts[0].Trim();
                    var name = parts[1].Trim();

                    var content = File.ReadAllText(filePath);

                    var tab = new Tab { Artist = artist, Name = name, Content = content, CreatedOn = DateTime.Now };

                    session.Store(tab);

                    bookTasks.AddTabToBook(tab.Id, book.Id);

                    File.Delete(filePath);
                }

                session.SaveChanges();
            }
        }
    }
}