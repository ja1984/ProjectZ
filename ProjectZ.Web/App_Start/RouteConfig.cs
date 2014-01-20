using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectZ.Web
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");





            routes.Add("subdomain", new SubdomainRoute("{controller}/{action}/{*id}")
            {
                Defaults = new RouteValueDictionary(new { action = "Index", id = UrlParameter.Optional }),
                DataTokens = new RouteValueDictionary()
            });

            routes.Add("projectsubdomain", new SubdomainRoute("{action}")
            {
                Defaults = new RouteValueDictionary(new { controller = "Project", action = "Details" }),
                DataTokens = new RouteValueDictionary()
            });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Project", action = "Index", id = UrlParameter.Optional }
            );


        }

    }



    class SubdomainRoute : Route
    {
        public SubdomainRoute(string url) : base(url, new MvcRouteHandler()) { }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = base.GetRouteData(httpContext);
            if (routeData == null) return null; // Only look at the subdomain if this route matches in the first place.
            string subdomain = httpContext.Request.Params["subdomain"]; // A subdomain specified as a query parameter takes precedence over the hostname.
            if (subdomain == null)
            {
                string host = httpContext.Request.Headers["Host"];
                int index = host.IndexOf('.');
                if (index >= 0)
                    subdomain = host.Substring(0, index);
            }
            if (subdomain != null)
                routeData.Values["subdomain"] = subdomain;

            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            object subdomainParam = requestContext.HttpContext.Request.Params["subdomain"];
            if (subdomainParam != null)
                values["subdomain"] = subdomainParam;
            return base.GetVirtualPath(requestContext, values);
        }
    }
}