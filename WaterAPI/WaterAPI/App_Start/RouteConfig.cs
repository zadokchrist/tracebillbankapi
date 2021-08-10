using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WaterAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Hello",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Customers", id = UrlParameter.Optional }
            );
            routes.MapRoute(
            "Customer", "Customer/{name}", new
            {
                controller = "Home",
                action = "Customers",
                name = UrlParameter.Optional
            });

        }
    }
}
