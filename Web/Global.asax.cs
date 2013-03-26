using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.Controllers;
using Web.Tasks;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static IDocumentStore DocumentStore { get; set; }

        public MvcApplication()
        {
            BeginRequest += (sender, args) =>
            {
                HttpContext.Current.Items["CurrentRequestRavenSession"] = RavenController.DocumentStore.OpenSession();
            };

            EndRequest += (sender, args) =>
            {
                using (var session = (IDocumentSession)HttpContext.Current.Items["CurrentRequestRavenSession"])
                {
                    if (session == null)
                        return;

                    if (Server.GetLastError() != null)
                        return;

                    session.SaveChanges();
                }
                //TaskExecutor.StartExecuting();
            };
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleMobileConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            InitialiseDocumentStore();

            var import = new ImportTasks();
            import.Import();
        }

        private static void InitialiseDocumentStore()
        {
            var useEmbeddedRavenServer = Boolean.Parse(ConfigurationManager.AppSettings["UseEmbeddedRavenServer"]);
            if (useEmbeddedRavenServer)
            {
                DocumentStore = new EmbeddableDocumentStore { ConnectionStringName = "RavenDB", UseEmbeddedHttpServer = true };
            }
            else
            {
                DocumentStore = new DocumentStore { ConnectionStringName = "RavenDB" };
            }
                
            DocumentStore.Conventions.IdentityPartsSeparator = "-";
            DocumentStore.Initialize();

            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), DocumentStore);

            RavenController.DocumentStore = DocumentStore;
        }
    }
}