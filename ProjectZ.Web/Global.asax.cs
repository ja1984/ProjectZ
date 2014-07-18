using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace ProjectZ.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public static class RavenDb
    {
        public static DocumentStore CreateAndInitalizeStore()
        {
            var store = new DocumentStore() { ConnectionStringName = "RavenDb" };
            //store.Conventions.IdentityPartsSeparator = "-";
            store.Conventions.SaveEnumsAsIntegers = true;
            store.Initialize();
            //IndexCreation.CreateIndexes(typeof(CountOfPostForTripMenu).Assembly, store);
            //Raven.Client.MvcIntegration.RavenProfiler.InitializeFor(store);
            return store;
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        public static DocumentStore Store;
        protected void Application_Start()
        {

            Store = RavenDb.CreateAndInitalizeStore();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}