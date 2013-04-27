using Domain;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models;
using Web.ViewModels;

namespace Web.Indexes
{
    public class ArtistsIndex : AbstractIndexCreationTask<Tab, Artist> 
    {
        public ArtistsIndex() 
        {
            Map = tabs => from t in tabs
                          select new Artist { ArtistName = t.Artist, TabCount = 1 };


            Reduce = results => from result in results 
                                group result by result.ArtistName 
                                into g 
                                orderby g.Key
                                select new Artist
                                           { 
                                               ArtistName = g.Key, 
                                               TabCount = g.Sum(x => x.TabCount) 
                                           };
        } 
    }
}