using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using jcamacho_bugtracker.Models;
using Microsoft.AspNet.Identity;

namespace jcamacho_bugtracker.Controllers
{
    public class ProjectsController : Controller
    {
        // GET: Projects
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(DashboardViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = User.Identity.GetUserId();

            List<Project> relevantProjects = new List<Project>();
            List<Project> allProjects = new List<Project>();

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
                }

                allProjects.Add(project);
            }

            model.RelevantProjects = relevantProjects;

            if (User.IsInRole("Administrator") || User.IsInRole("Project Manager"))
            {
                model.AllProjects = allProjects;
            }

            return View(model);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectName,ProjectDescription,CreatedDate,UpdatedDate,AuthorId")] Project project, string authorId)
        {
            if (ModelState.IsValid)
            {
                string AuthorId = User.Identity.GetUserId();
                authorId = AuthorId;
                project.CreatedDate = DateTimeOffset.Now;
                db.Projects.Add(project);

                AssignHelper helper = new AssignHelper(db);
                helper.AddProjectToUser(project.Id, authorId);

                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectName,ProjectDescription,CreatedDate,UpdatedDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.UpdatedDate = DateTimeOffset.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            List<Ticket> tickets = db.Tickets.Where(t => t.TicketStatuses.Name != "Resolved" && t.ProjectId == id).ToList();
            if (tickets.Count() > 0)
            {
                project.Tickets = tickets;
                return View(project);
            }
            else
            {
                project.IsDeleted = true;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            //db.Projects.Remove(project);
            project.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        // GET: Projects/Assign/5
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Assign(int id)
        {
            var project = db.Projects.Find(id);
            AssignHelper helper = new AssignHelper(db);
            var model = new AssignUsersViewModel();

            model.Project = project;
            model.SelectedUsers = helper.ListAssignedUsers(id).Select(u => u.Id).ToArray();
            model.Users = new MultiSelectList(db.Users.Where(u => (u.DisplayName != "N/A" && u.DisplayName != "(Remove Assigned User)")).OrderBy(u => u.FirstName), "Id", "DisplayName", model.SelectedUsers);

            return View(model);
        }

        // POST: Projects/Assign/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult Assign(AssignUsersViewModel model)
        {
            var project = db.Projects.Find(model.Project.Id);
            AssignHelper helper = new AssignHelper(db);

            foreach (var user in db.Users.Select(r => r.Id).ToList())
            {
                helper.RemoveProjectFromUser(project.Id, user);
            }
            if (model.SelectedUsers != null)
            {
                foreach (var user in model.SelectedUsers)
                {
                    helper.AddProjectToUser(project.Id, user);
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}