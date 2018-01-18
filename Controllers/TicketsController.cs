using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jcamacho_bugtracker.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using static jcamacho_bugtracker.ApplicationSignInManager;

namespace jcamacho_bugtracker.Controllers
{
    public class TicketsController : Controller
    {
        // GET: Tickets
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
            List<Ticket> allTickets = new List<Ticket>();

            foreach (var project in db.Projects.ToList())
            {
                bool relevantProjectflag = false;

                foreach (var ticket in project.Tickets)
                {
                    allTickets.Add(ticket);
                }

                // Find Projects that users are assigned to and mark them Relevant
                foreach (var projectUser in project.Users)
                {
                    if (userId == projectUser.Id)
                    {
                        relevantProjectflag = true;
                    }
                }

                // Filter tickets ONLY for projects that are assigned to users
                // Find tickets ON Projects that are assigned to the PM
                // Find tickets that are assigned to Devs and created by Subs
                if (relevantProjectflag)
                {
                    relevantProjects.Add(project);

                    foreach (var ticket in project.Tickets)
                    {
                        if (User.IsInRole("Project Manager")
                        || ((User.IsInRole("Developer") && userId == ticket.AssignedUserId) || User.IsInRole("Developer") && userId == ticket.AuthorId) || (User.IsInRole("Submitter") && userId == ticket.AuthorId))
                        {
                            relevantTickets.Add(ticket);
                        }
                    }
                }

                // Filter tickets ONLY for projects that are NOT assigned to users
                // Find tickets that were created by the PM
                else
                {
                    if (User.IsInRole("Project Manager"))
                    {
                        irrelevantProjects.Add(project);
                        foreach (var ticket in project.Tickets)
                        {
                            if (User.IsInRole("Project Manager") && userId == ticket.AuthorId)
                            {
                                relevantTickets.Add(ticket);
                            }
                        }
                    }
                }
            }
            model.RelevantProjects = relevantProjects;
            model.RelevantTickets = relevantTickets;

            if (User.IsInRole("Administrator"))
            {
                model.AllTickets = allTickets;
            }

            return View(model);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create(int id)
        {
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.AuthorUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.ProjectId = id;
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,CreatedDate,UpdatedDate,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,AssignedUserId")] Ticket ticket, int projectId)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                ticket.AuthorId = userId;
                ticket.ProjectId = projectId;
                ticket.CreatedDate = DateTimeOffset.Now;
                ticket.TicketStatusId = 1; //unassigned
                ticket.TicketPriorityId = 1; //green
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedUserId);
            //ViewBag.AuthorUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ApplicationDbContext context = new ApplicationDbContext();
            Project project = db.Projects.Find(ticket.ProjectId);
            var projectUsers = context.Users.Where(u => u.Projects.Any(p => p.ProjectName == project.ProjectName));
            var role = context.Roles.SingleOrDefault(u => u.Name == "Developer");
            var usersInRole = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            var displayUsers = usersInRole.Where(u => u.Projects.Any(p => (p.ProjectName == project.ProjectName)));
            var removeUser = db.Users.Where(u => (u.DisplayName != "N/A" && u.DisplayName != "(Remove Assigned User)"));

            //ViewBag.AssignedUserId = new SelectList(displayUsers, "Id", "FirstName", ticket.AssignedUserId);
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedUserId);
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Body,CreatedDate,UpdatedDate,ProjectId,TicketTypeId,TicketPriorityId,TicketStatus,TicketStatusId,AuthorId,AssignedUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                if (ticket.TicketStatusId == 4) //resolved
                {
                    ticket.IsDeleted = true;
                }
                if (ticket.AssignedUserId != null && ticket.TicketStatusId < 2)
                {
                    ticket.TicketStatusId = 2;
                }
                Ticket oldTicket = new ApplicationDbContext().Tickets.Find(ticket.Id);
                if (oldTicket.AssignedUserId != ticket.AssignedUserId)
                {
                    var history = new TicketHistory
                    {
                        ChangedDate = new DateTimeOffset(DateTime.Now),
                        TicketId = ticket.Id,
                        UserId = User.Identity.GetUserId(),
                        Property = "The assigned user has been changed to",
                        NewValue = db.Users.Find(ticket.AssignedUserId).FirstName,
                    };
                    db.TicketHistories.Add(history);

                    ticket.UpdatedDate = DateTimeOffset.Now;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    await Notify(ticket.Id, history);
                }

                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    var history = new TicketHistory
                    {
                        ChangedDate = new DateTimeOffset(DateTime.Now),
                        TicketId = ticket.Id,
                        UserId = User.Identity.GetUserId(),
                        Property = "The status has been set to",
                        OldValue = oldTicket.TicketStatuses.Name,
                        NewValue = db.TicketStatuses.Find(ticket.TicketStatusId).Name
                    };
                    db.TicketHistories.Add(history);

                    await Notify(ticket.Id, history);
                }

                if (oldTicket.Title != ticket.Title)
                {
                    var history = new TicketHistory
                    {
                        ChangedDate = new DateTimeOffset(DateTime.Now),
                        TicketId = ticket.Id,
                        UserId = User.Identity.GetUserId(),
                        Property = "The title has been changed to",
                        OldValue = oldTicket.Title,
                        NewValue = ticket.Title
                    };
                    db.TicketHistories.Add(history);

                    await Notify(ticket.Id, history);
                }

                if (oldTicket.Body != ticket.Body)
                {
                    var history = new TicketHistory
                    {
                        ChangedDate = new DateTimeOffset(DateTime.Now),
                        TicketId = ticket.Id,
                        UserId = User.Identity.GetUserId(),
                        Property = "The description has been changed to",
                        OldValue = oldTicket.Body,
                        NewValue = ticket.Body
                    };
                    db.TicketHistories.Add(history);

                    await Notify(ticket.Id, history);
                }

                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    var history = new TicketHistory
                    {
                        ChangedDate = new DateTimeOffset(DateTime.Now),
                        TicketId = ticket.Id,
                        UserId = User.Identity.GetUserId(),
                        Property = "The ticket type has been changed to",
                        OldValue = oldTicket.TicketTypes.Name,
                        NewValue = db.TicketTypes.Find(ticket.TicketTypeId).Name
                    };
                    db.TicketHistories.Add(history);

                    await Notify(ticket.Id, history);
                }

                if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    var history = new TicketHistory
                    {
                        ChangedDate = new DateTimeOffset(DateTime.Now),
                        TicketId = ticket.Id,
                        UserId = User.Identity.GetUserId(),
                        Property = "The priority has been set to",
                        OldValue = oldTicket.TicketPriorities.Name,
                        NewValue = db.TicketPriorities.Find(ticket.TicketPriorityId).Name
                    };
                    db.TicketHistories.Add(history);

                    await Notify(ticket.Id, history);
                }

                ApplicationDbContext context = new ApplicationDbContext();
                Project project = db.Projects.Find(ticket.ProjectId);
                var projectUsers = context.Users.Where(u => u.Projects.Any(p => p.ProjectName == project.ProjectName));
                var role = context.Roles.SingleOrDefault(u => u.Name == "Developer");
                var usersInRole = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
                var displayUsers = usersInRole.Where(u => u.Projects.Any(p => (p.ProjectName == project.ProjectName)));
                var removeUser = db.Users.Where(u => (u.DisplayName != "N/A" && u.DisplayName != "(Remove Assigned User)"));

                ViewBag.AssignedUserId = new SelectList(displayUsers, "Id", "FirstName", ticket.AssignedUserId);
                ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", ticket.ProjectId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

                ticket.UpdatedDate = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(ticket);
        }

        // POST: Ticket/Notifications
        public async Task Notify(int ticketId, TicketHistory history)
        {
            var db = new ApplicationDbContext();
            var ticket = db.Tickets.Find(ticketId);
            //var ticket = db.Tickets.Include("Project").FirstOrDefault(t => t.Id == ticketId);
            var displayName = db.Users.Find(history.UserId).DisplayName;
            var emailService = new PersonalEmail();

            // If the ticket is already assigned to a user and the user is not being changed
            if (ticket.AssignedUserId != null && history.Property != "Assigned / Active")
            {
                await emailService.SendAsync(new MailMessage(ConfigurationManager.AppSettings["username"], ticket.AssignedUser.UserName)
                {
                    Subject = "A ticket that you are assigned to has been changed.",
                    //Body = ticket.AssignedToUser.FirstName + ", </p>" + displayName + " has changed the " + history.Property + " of ticket <i>" + ticket.Title + "</i> from " + history.OldValue + " to <i>" + history.NewValue + "</i>.",
                    Body = ticket.AssignedUser.FirstName + ", </p>" + displayName + " has made a change to the ticket <i>" + ticket.Title + ".</i></p>" + history.Property + "<i> " + history.NewValue + ".<i>",
                    IsBodyHtml = true
                });
            }

            // If the ticket is being assigned for the first time or being assigned to a different user
            if (history.Property == "Assigned / Active")
            {
                await emailService.SendAsync(new MailMessage(ConfigurationManager.AppSettings["username"], ticket.AssignedUser.UserName)
                {
                    Subject = "You have been assigned a new ticket.",
                    Body = ticket.AssignedUser.FirstName + ", </p>" + displayName + " has assigned you to work on a new ticket, <i>" + ticket.Title + "</i>.",
                    //Body = User.Identity.GetUserName() + ", </p><b>" + displayName + "</b> has changed the <b>" + ticket.Title + "</b> from <mark>" + history.OldValue + "</mark> to <mark>" + history.NewValue + "</mark>.",
                    IsBodyHtml = true
                });
            }
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            //db.Tickets.Remove(ticket);
            ticket.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: Tickets/Assign/5
        //[Authorize(Roles = "Admin, Project Manager")]
        //public ActionResult Assign(int id)
        //{
        //    var ticket = db.Tickets.Find(id);
        //    var project = ticket.ProjectId;
        //    AssignHelper helper = new AssignHelper(db);
        //    var model = new AssignUsersViewModel();

        //    model.Ticket = ticket;
        //    model.SelectedUsers = helper.ListAssignedUsers(id).Select(u => u.Id).ToArray();
        //    model.Users = new SelectList(db.Users.Where(u => (u.DisplayName != "N/A" && u.DisplayName != "(Remove Assigned User)")).OrderBy(u => u.FirstName), "Id", "DisplayName", model.SelectedUsers);

        //    return View(model);
        //}

        // POST: Tickets/Assign/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[Authorize(Roles = "Admin, Project Manager")]
        //public ActionResult Assign(Ticket ticket)
        //{

        //}

        //{
        //    var ticket = db.Tickets.Find(model.Ticket.Id);
        //    AssignHelper helper = new AssignHelper(db);

        //    foreach (var user in db.Users.Select(r => r.Id).ToList())
        //    {
        //        helper.RemoveProjectFromUser(ticket.Id, user);
        //    }
        //    if (model.SelectedUser != null)
        //    {
        //        foreach (var user in model.SelectedUser)
        //        {
        //            helper.AddProjectToUser(ticket.Id, user);
        //        }
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
