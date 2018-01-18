using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using jcamacho_bugtracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace jcamacho_bugtracker.Controllers
{
    [Authorize]
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users via PagedList
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Index(int? page, string sortOrder)
        {
            ViewBag.NameSortParm = sortOrder == "FirstName_D" ? "FirstName" : "FirstName_D";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "LastName_D" : "LastName";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "Email_D" : "Email";
            ViewBag.PhoneSortParm = sortOrder == "Phone" ? "Phone_D" : "Phone";

            var usersList = db.Users.ToList();
            ViewBag.sortparam = sortOrder;
            switch (sortOrder)
            {
                case ("FirstName"):
                    usersList = usersList.OrderBy(u => u.FirstName).ToList();
                    break;
                case ("FirstName_D"):
                    usersList = usersList.OrderByDescending(u => u.FirstName).ToList();
                    break;
                case ("LastName"):
                    usersList = usersList.OrderBy(u => u.LastName).ToList();
                    break;
                case ("LastName_D"):
                    usersList = usersList.OrderByDescending(u => u.LastName).ToList();
                    break;
                case ("Email"):
                    usersList = usersList.OrderBy(u => u.Email).ToList();
                    break;
                case ("Email_D"):
                    usersList = usersList.OrderByDescending(u => u.Email).ToList();
                    break;
                default:
                    usersList = usersList.OrderBy(u => u.FirstName).ToList();
                    break;
            }
            var pageNumber = page ?? 1;
            ViewBag.pageNumber = pageNumber;
            return View(usersList.ToPagedList(pageNumber, 5));
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Users/Manage
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Manage(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new AssignPageModel();
            var theUser = db.Users.Find(id);
            // send a select list of projects the user is NOT on
            // send a select list of tickets the user is NOT on
            // send a select list of roles the user is NOT on

            // send a list of projects, tickets, roles the user is on
            var roles = new List<IdentityRole>();
            foreach (var rol in db.Roles)
            {
                if (!theUser.Roles.Any(r => r.RoleId == rol.Id))
                {
                    roles.Add(rol);
                }
            }
            var tick = db.Tickets;
            var helper = new ProjectAssignHelper();
            var helperP = new Helper.ProjectAssignHelper(db);
            model.TicketOwner = id;
            model.DisplayName = theUser.DisplayName;

            var UNP = helperP.ListUserNOTProjects(theUser.Id);
            model.UserNotProjects = new MultiSelectList(UNP.OrderBy(p => p.ProjectName), "Id", "Name");

            var UNT = db.Tickets.Where(t => t.AssignedUser.Id != theUser.Id).OrderBy(m => m.Title);
            model.UserNotTickets = new MultiSelectList(UNT, "Id", "Title");

            model.UserNotRoles = new MultiSelectList(roles, "Id", "Name");

            var UP = theUser.Projects.OrderBy(p => p.ProjectName);
            model.UserProjects = new MultiSelectList(UP, "Id", "Name");

            //model.UserProjects = null;

            var UT = theUser.Tickets.OrderBy(t => t.Title);
            model.UserTickets = new MultiSelectList(UT, "Id", "Title");
            //model.UserTickets = null;

            model.UserRoles = new MultiSelectList(helper.ListUserRoles(theUser.Id));
            //model.UserRoles = null;

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Users/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Manage(AssignPageModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.TicketOwner);
                if (model.newTickets != null)
                {
                    foreach (var newTicket in model.newTickets)
                    {
                        // assigning a ticket
                        if (newTicket != "" && newTicket != null)
                        {
                            int tickId = Convert.ToInt32(newTicket);
                            var tick = db.Tickets.Find(tickId);
                            // add to user's tickets
                            user.Tickets.Add(tick);
                            // add ticket's project to user's projects if not currently on the project
                            if (!user.Projects.Contains(tick.Project))
                            {
                                user.Projects.Add(tick.Project);
                            }
                            // assign the ticket
                            tick.AssignedUser = user;
                            tick.AssignedUserId = model.TicketOwner;
                            //create the new ticket notification
                            var tn = new TicketNotification()
                            {
                                TicketId = tick.Id,
                                UserId = user.Id,
                                Ticket = tick,
                            };
                            // notify the new ticket owner
                            var helper = new ProjectAssignHelper();
                            helper.Notify(tn, "Add");
                            // save the ticket
                            db.Entry(tick).State = EntityState.Modified;
                        }
                    }
                }
                // assigning a project
                if (model.newProjects != null)
                {
                    foreach (var newProject in model.newProjects)
                    {
                        if (newProject != "" && newProject != null)
                        {
                            int projId = Convert.ToInt32(newProject);
                            var proj = db.Projects.Find(projId);
                            //add the project to the user's projects
                            user.Projects.Add(proj);
                            // add the user to the project's users
                            proj.Users.Add(user);
                            db.Entry(proj).State = EntityState.Modified;
                        }
                    }
                }
                if (model.newRoles != null)
                {
                    foreach (var newRole in model.newRoles)
                    {
                        if (newRole != "" && newRole != null)
                        {
                            string roleId = newRole;
                            var role = db.Roles.Find(roleId);
                            var helper = new ProjectAssignHelper();
                            //add user to role
                            helper.AddUserToRole(user.Id, db.Roles.Find(roleId).Name);
                        }
                    }
                }
                if (model.remTickets != null)
                {
                    foreach (var remTicket in model.remTickets)
                    {
                        if (remTicket != "" && remTicket != null)
                        {
                            int tickId = Convert.ToInt32(remTicket);
                            var tick = db.Tickets.Find(tickId);
                            // create new ticketNotification
                            var tn = new TicketNotification()
                            {
                                TicketId = tick.Id,
                                UserId = user.Id,
                            };
                            var helper = new ProjectAssignHelper();
                            //notify the user who is removed
                            helper.Notify(tn, "Remove");
                            user.Tickets.Remove(tick);
                            // assign ticket to project's project manager
                            tick.AssignedUser = tick.Project.Manager;
                            tick.AssignedUserId = tick.Project.ManagerId;
                            tick.Project.Manager.Tickets.Add(tick);
                            //save changes
                            db.Entry(tick).State = EntityState.Modified;

                        }
                    }
                }
                if (model.remProjects != null)
                {
                    foreach (var remProject in model.remProjects)
                    {
                        if (remProject != "" && remProject != null)
                        {
                            int projId = Convert.ToInt32(remProject);
                            var proj = db.Projects.Find(projId);
                            //find tickets on project
                            var theTicketsToRemove = new List<Ticket>();
                            foreach (var tick in user.Tickets)
                                if (tick.ProjectId == projId)
                                    theTicketsToRemove.Add(tick);

                            // reassign user's tickets to not include any on project
                            user.Tickets = user.Tickets.Except(theTicketsToRemove).ToList();
                            user.Projects.Remove(proj);
                            proj.Users.Remove(user);
                            db.Entry(proj).State = EntityState.Modified;
                        }
                    }
                }
                if (model.remRoles != null)
                {
                    foreach (var remRole in model.remRoles)
                    {
                        if (remRole != "" && remRole != null)
                        {
                            string roleName = remRole;
                            var helper = new ProjectAssignHelper();
                            helper.RemoveUserFromRole(user.Id, roleName);
                        }
                    }
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
        // GET: Users/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(ApplicationUser applicationUser)
        {

            db.Users.Attach(applicationUser);

            db.Entry(applicationUser).Property(u => u.DisplayName).IsModified = true;

            if (ModelState.IsValid)
            {
                //db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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