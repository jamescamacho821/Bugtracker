using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace jcamacho_bugtracker
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //enable route mapping
            //routes.MapMvcAttributeRoutes();
            //routes.MapRoute(
            //    name: "TicketDetail", 
            //    url: "Tickets/Details/{TicketId}",
            //    defaults: new { controller = "Tickets", action = "Details", TicketId = UrlParameter.Optional }
            //);
            //routes.MapRoute(
            //    name: "NewSlug", url: "Blog/{slug}",
            //    defaults: new { controller = "Posts", action = "Details", slug = UrlParameter.Optional }
            // //);
           // routes.MapRoute(
           //    name: "TicketHistories", 
           //    url: "TicketHistories/Index/{ticketId}",
           //    defaults: new { controller = "TicketHistories", action = "Index", ticketId = UrlParameter.Optional }
           //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
