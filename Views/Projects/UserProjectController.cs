//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using jcamacho_bugtracker.Models;
//using jcamacho_bugtracker.Helper;

//namespace jcamacho_bugtracker.Views.Projects
////{
////    [Authorize]
////    //[RequireHttps]
////    public class UserProjectController : Controller
////    {
////        private ApplicationDbContext db = new ApplicationDbContext();

////        // GET: Projects/Create
////        [Authorize(Roles = "Administrator, Project Manager")]
////        public ActionResult UserList()
////        {
////            List<AdminProjectListModel> users = new List<AdminProjectListModel>();
////            UserProjectsHelper helper = new UserProjectsHelper();
////            foreach (var user in db.Users.ToList())
////            {
////                var eachUser = new AdminProjectListModel();
////                eachUser.Project = new List<Project>();
////                eachUser.user = user;

////                eachUser.Project = helper.ListUserProjects(user.Id).ToList();

////                users.Add(eachUser);
////            }

////            //ApplicationDbContext context = new ApplicationDbContext();

////            return View(users);
////        }




////        // POST: User List
////        [HttpPost]
////        [Authorize(Roles = "Administrator")]
////        public ActionResult UserProjects(AdminUpdateProjectsViewModel model)
////        {
////            if (ModelState.IsValid)
////            {
////                var project = db.Projects.Find(model.Id);
////                ProjectAssignHelper helper = new ProjectAssignHelper(db);

////                foreach (var user in db.Projects.Select(r => r.ProjectName).ToList())
////                {
////                    helper.RemoveProjectFromUser(.ProjectId, model.Id);
////                }

////                if (model.SelectedProjects != null)
////                {
////                    foreach (var project in model.SelectedProjects)
////                    {
////                        helper.AddProjectToUser(1, model.Id);
////                    }
////                }

////                return RedirectToAction("Index", "Projects");
////            }
////            else
////            {
////                var project = db.Users.Find(model.Id);
////                ProjectAssignHelper helper = new ProjectAssignHelper(db);
////                var returnModel = new AdminUpdateProjectsViewModel();

////                returnModel.Id = project.Id;
////                returnModel.FirstName = project.FirstName;
////                returnModel.LastName = project.LastName;
////                returnModel.DisplayName = project.DisplayName;
////              //  returnModel.SelectedUsers = helper.ListAssignedUsers(project.Id).ToArray();
////                returnModel.Projects = new MultiSelectList(db.Projects, "Name", "Name", model.SelectedProjects);

////                return View(returnModel);
////            }
////        }
////    }
////}



//{
//    [Authorize]
//    //[RequireHttps]
//    public class UserProjectController : Controller
//    {


//        private ApplicationDbContext db = new ApplicationDbContext();

//        // GET: Admin
//       // [Authorize(Roles = "Administrator")]
//        ////public ActionResult UserList(int Id)
//        ////{
//        ////    List<AdminProjectListModel> users = new List<AdminProjectListModel>();
//        ////    Helper.ProjectAssignHelper helper = new Helper.ProjectAssignHelper(db);
            
           
//        ////    foreach (var user in db.Users.ToList())
//        ////    {
//        ////        var eachUser = new AdminProjectListModel();

//        ////      //eachUser.Project = new List<Project>();
//        ////        eachUser.user = user;
//        ////        eachUser.Projects = helper.ListUserProjects(user.Id).ToList();

//        ////        users.Add(eachUser);
//        ////    }
           
//        ////    return View(users);
//        ////}



//        [Authorize(Roles = "Administrator")]
//        public ActionResult UserList()
//        {
//            List<AdminProjectListModel> users = new List<AdminProjectListModel>();
//            Helper.ProjectAssignHelper helper = new Helper.ProjectAssignHelper(db);

//            foreach (var user in db.Users.ToList())
//            {
//                var eachUser = new AdminProjectListModel();

//                eachUser.Projects = new List<Project>();
//                eachUser.user = user;
//                eachUser.Projects = helper.ListUserProjects(user.Id).ToList();

//                users.Add(eachUser);
//            }

//            ApplicationDbContext context = new ApplicationDbContext();

//            var role1 = context.Roles.SingleOrDefault(u => u.Name == "Administrator");
//            var admins = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role1.Id)));

//            var role2 = context.Roles.SingleOrDefault(u => u.Name == "Project Manager");
//            var projectManagers = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role2.Id)));

//            var role3 = context.Roles.SingleOrDefault(u => u.Name == "Developer");
//            var developers = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role3.Id)));

//            var role4 = context.Roles.SingleOrDefault(u => u.Name == "Submitter");
//            var submitters = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role4.Id)));


//            var number1 = admins.Count();
//            var number2 = projectManagers.Count();
//            var number3 = developers.Count();
//            var number4 = submitters.Count();

//            ViewBag.intNumber1 = number1;
//            ViewBag.intNumber2 = number2;
//            ViewBag.intNumber3 = number3;
//            ViewBag.intNumber4 = number4;

//            int[] projectTickets = { 1, 2, 3, 4 };
//            ViewBag.intArray = projectTickets;

//            return View(users);
//        }


//        // GET: User List
//        [Authorize(Roles = "Administrator")]
//        public ActionResult UserProjects(string id)
//        {
//            var user = db.Users.Find(id);
//            Helper.ProjectAssignHelper helper = new Helper.ProjectAssignHelper(db);
//            var model = new AdminUpdateProjectsViewModel();

//            model.Id = user.Id;
//            model.FirstName = user.FirstName;
//            model.LastName = user.LastName;
//            model.DisplayName = user.DisplayName;
//            model.SelectedProjects = helper.ListUserProjects(user.Id).ToArray();
//            model.Projects = new MultiSelectList(db.Projects, "Name", "Name", model.SelectedProjects);

//            return View(model);
//        }

//        // POST: User List
//        [HttpPost]
//        [Authorize(Roles = "Administrator")]
//        public ActionResult UserProjects(AdminUpdateProjectsViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = db.Users.Find(model.Id);
//                Helper.ProjectAssignHelper helper = new Helper.ProjectAssignHelper(db);

//                foreach (var project in db.Projects.Select(r => r.ProjectName).ToList())
//                {
//              //      helper.RemoveUserFromProject(project.Id, user.Id);
//                }

//                if (model.SelectedProjects != null)
//                {
//                    foreach (var project in model.SelectedProjects)
//                    {
//                 //       helper.AddUserToProject(user.Id, project.Id);
//                    }
//                }

//                return RedirectToAction("UserList", "Projects");
//            }
//            else
//            {
//                var user = db.Users.Find(model.Id);
//                Helper.ProjectAssignHelper helper = new Helper.ProjectAssignHelper(db);
//                var returnModel = new AdminUpdateProjectsViewModel();

//                returnModel.Id = user.Id;
//                returnModel.FirstName = user.FirstName;
//                returnModel.LastName = user.LastName;
//                returnModel.DisplayName = user.DisplayName;
//                returnModel.SelectedProjects = helper.ListUserProjects(user.Id).ToArray(); helper.ListUserProjects(user.Id).ToArray();
//                returnModel.Projects = new MultiSelectList(db.Projects, "Name", "Name", model.SelectedProjects);

//                return View(returnModel);
//            }
//        }
//    }
//}
