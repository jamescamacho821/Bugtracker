//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace jcamacho_bugtracker.Controllers
//{
//    public class HomeController : Controller
//    {
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult About()
//        {
//            ViewBag.Message = "Your application description page.";

//            return View();
//        }

//        public ActionResult Contact()
//        {
//            ViewBag.Message = "Your contact page.";

//            return View();
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jcamacho_bugtracker.Models;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace jcamacho_bugtracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(DashboardViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = User.Identity.GetUserId();

            List<Project> relevantProjects = new List<Project>();
            List<Project> irrelevantProjects = new List<Project>();
            List<Ticket> relevantTickets = new List<Ticket>();
            List<Ticket> irrelevantTickets = new List<Ticket>();

            foreach (var project in db.Projects.ToList())
            {
                bool relevantProjectflag = false;

                foreach (var projectUser in project.Users)
                {
                    if (userId == projectUser.Id)
                    {
                        relevantProjectflag = true;
                    }
                }

                if (relevantProjectflag)
                {
                    relevantProjects.Add(project);

                    foreach (var ticket in project.Tickets)
                    {
                        if (((User.IsInRole("Administrator") && ticket.TicketStatusId != 1) || (User.IsInRole("Project Manager") && ticket.TicketStatusId != 1))
                        || ((User.IsInRole("Developer") && userId == ticket.AssignedUserId) || User.IsInRole("Developer") && userId == ticket.AuthorId) || (User.IsInRole("Submitter") && userId == ticket.AuthorId))
                        {
                            relevantTickets.Add(ticket);
                        }
                        else if (ticket.TicketStatusId == 1)
                        {
                            irrelevantTickets.Add(ticket);
                        }
                    }
                }
                else
                {
                    if (User.IsInRole("Administrator") || User.IsInRole("Project Manager"))
                    {
                        irrelevantProjects.Add(project);
                        foreach (var ticket in project.Tickets)
                        {
                            if ((User.IsInRole("Administrator") && ticket.TicketStatusId != 1) || (User.IsInRole("Project Manager") && ticket.TicketStatusId != 1 && userId == ticket.AuthorId))
                            {
                                relevantTickets.Add(ticket);
                            }
                            else if ((User.IsInRole("Administrator") && ticket.TicketStatusId == 1) || (User.IsInRole("Project Manager") && ticket.TicketStatusId == 1 && userId == ticket.AuthorId))
                            {
                                irrelevantTickets.Add(ticket);
                            }
                        }

                    }
                }
            }

            model.RelevantProjects = relevantProjects;
            model.RelevantTickets = relevantTickets;

            if (User.IsInRole("Administrator") || User.IsInRole("Project Manager"))
            {
                model.IrrelevantProjects = irrelevantProjects;
                model.IrrelevantTickets = irrelevantTickets;
            }

            return View(model);
        }

        //public ActionResult AdminIndex(DashboardViewModel model)
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    var userId = User.Identity.GetUserId();

        //    List<Project> allProjects = new List<Project>();
        //    //List<Project> relevantProjects = new List<Project>();
        //    //List<Project> irrelevantProjects = new List<Project>();
        //    List<Ticket> relevantTickets = new List<Ticket>();
        //    List<Ticket> irrelevantTickets = new List<Ticket>();

        //    foreach (var project in db.Projects.ToList())
        //    {
        //        allProjects.Add(project);
        //        //bool relevantProjectflag = false;

        //        //foreach (var projectUser in project.Users)
        //        //{
        //        //    if (userId == projectUser.Id)
        //        //    {
        //        //        relevantProjectflag = true;
        //        //    }
        //        //}

        //        //if (relevantProjectflag)
        //        //{
        //        //    relevantProjects.Add(project);

        //            foreach (var ticket in project.Tickets)
        //            {
        //                if (ticket.StatusId == 1)
        //                {
        //                    irrelevantTickets.Add(ticket);
        //                }
        //                else if (((User.IsInRole("Admin") || User.IsInRole("Project Manager") && ((ticket.StatusId == 2)))
        //                || ((User.IsInRole("Developer") && userId == ticket.AssignedToUserId) || User.IsInRole("Developer") && userId == ticket.AuthorUserId) || (User.IsInRole("Submitter") && userId == ticket.AuthorUserId)))
        //                {
        //                    relevantTickets.Add(ticket);
        //                }
        //            }
        //        //}
        //        //else
        //        //{
        //        //    if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
        //        //    {
        //        //        irrelevantProjects.Add(project);
        //        //    }
        //        //}
        //    }

        //    //model.RelevantProjects = relevantProjects;
        //    model.RelevantTickets = relevantTickets;

        //    if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
        //    {
        //        //model.IrrelevantProjects = irrelevantProjects;
        //        model.AllProjects = allProjects;
        //        model.IrrelevantTickets = irrelevantTickets;
        //    }

        //    return View(model);
        //    //return View(db.Projects.ToList().Where(p => p.Deleted == false));
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}