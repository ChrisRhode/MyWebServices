using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HumansDataMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // since we are using string IDs we need to set up special routes
            //routes.MapRoute(
              //name: "Details",
               //url: "{controller}/Details/{id}",
               //defaults: new { controller = "Humans", action = "Details", id = @"\w+" });
               // Default always comes last
           routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
