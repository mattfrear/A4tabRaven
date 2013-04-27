using Domain;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models;

namespace Web.Indexes
{
    public class AllDocumentsIndex : AbstractIndexCreationTask<Tab>
    {
        public AllDocumentsIndex()
        {
            Map = tabs => tabs.Select(x => new { });
        }
    }
}