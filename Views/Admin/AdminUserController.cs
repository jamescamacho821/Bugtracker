using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using jcamacho_bugtracker.Models;

namespace jcamacho_bugtracker.Views.Admin
{
    [Authorize]
    //[RequireHttps]
    public class AdminController  : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin
        [Authorize(Roles = "Administrator")]
        public ActionResult UserList()
        {
            List<AdminUserListModel> users = new List<AdminUserListModel>();
            ProjectAssignHelper helper = new ProjectAssignHelper();
            
            foreach ( var user in db.Users.ToList())
            {
                var eachUser = new AdminUserListModel();

                eachUser.Roles = new List<string>();
                eachUser.user = user;
                eachUser.Roles = helper.ListUserRoles(user.Id).ToList();

                users.Add(eachUser);
            }

            ApplicationDbContext context = new ApplicationDbContext();

            var role1 = context.Roles.SingleOrDefault(u => u.Name == "Administrator");
            var admins = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role1.Id)));

            var role2 = context.Roles.SingleOrDefault(u => u.Name == "Project Manager");
            var projectManagers = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role2.Id)));

            var role3 = context.Roles.SingleOrDefault(u => u.Name == "Developer");
            var developers = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role3.Id)));

            var role4 = context.Roles.SingleOrDefault(u => u.Name == "Submitter");
            var submitters = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role4.Id)));

            
            var number1 = admins.Count();
            var number2 = projectManagers.Count();
            var number3 = developers.Count();
            var number4 = submitters.Count();
            
            ViewBag.intNumber1 = number1;
            ViewBag.intNumber2 = number2;
            ViewBag.intNumber3 = number3;
            ViewBag.intNumber4 = number4;
           
            int[] projectTickets = { 1, 2, 3, 4 };
            ViewBag.intArray = projectTickets;

            return View(users);
        }

        // GET: User List
        [Authorize(Roles = "Administrator")]
        public ActionResult UserRoles(string id)
        {
            var user = db.Users.Find(id);
            ProjectAssignHelper helper = new ProjectAssignHelper();
            var model = new AdminUpdateRolesViewModel();

            model.Id = user.Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.DisplayName = user.DisplayName;
            model.SelectedRoles = helper.ListUserRoles(id).ToArray();
            model.Roles = new MultiSelectList(db.Roles, "Name", "Name", model.SelectedRoles);

            return View(model);
        }

        // POST: User List
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult UserRoles(AdminUpdateRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.Id);
                ProjectAssignHelper helper = new ProjectAssignHelper();

                foreach (var role in db.Roles.Select(r => r.Name).ToList())
                {
                    helper.RemoveUserFromRole(user.Id, role);
                }

                if (model.SelectedRoles != null)
                {
                    foreach (var role in model.SelectedRoles)
                    {
                        helper.AddUserToRole(user.Id, role);
                    }
                }

                return RedirectToAction("UserList", "Admin");
            }
            else
            {
                var user = db.Users.Find(model.Id);
                ProjectAssignHelper helper = new ProjectAssignHelper();
                var returnModel = new AdminUpdateRolesViewModel();

                returnModel.Id = user.Id;
                returnModel.FirstName = user.FirstName;
                returnModel.LastName = user.LastName;
                returnModel.DisplayName = user.DisplayName;
                returnModel.SelectedRoles = helper.ListUserRoles(user.Id).ToArray();
                returnModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", model.SelectedRoles);

                return View(returnModel);
            }
        }
    }
}
