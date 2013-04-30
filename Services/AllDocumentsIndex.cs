using System.Linq;
using Domain;
using Raven.Client.Indexes;

namespace Services
{
    public class AllDocumentsIndex : AbstractIndexCreationTask<Tab>
    {
        public AllDocumentsIndex()
        {
            Map = tabs => tabs.Select(x => new { });
        }
    }
}