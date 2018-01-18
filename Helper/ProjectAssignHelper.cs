using System.Collections.Generic;
using System.Linq;
using jcamacho_bugtracker.Models;

namespace jcamacho_bugtracker.Helper
{
    public class ProjectAssignHelper
    {

       
        private ApplicationDbContext db;
        

        public bool IsOnProject(string userId, int projectId)
        {
            return db.Users.Find(userId).Projects.Any(p => p.Id == projectId);
        }

        public IList<Project> ListUserProjects(string userId)
        {
            var projectList = new List<Project>();
           
            foreach (var proj in db.Projects)
            {
                //if (this.IsProjectOnUser(proj.Id, userId))
                //{
                    projectList.Add(proj);
                //}
            }
           
            return projectList;
        }

        public ProjectAssignHelper(ApplicationDbContext context)
        {
            this.db = context;
        }

        public bool IsProjectOnUser(int projectId, string userId)
        {
            var project = db.Projects.Find(projectId);
            var userCheck = project.Users.Any(p => p.Id == userId);
            return (userCheck);
        }

 
        //public IList<string> ListAssignedUsers(int projectId)
        public ICollection<ApplicationUser> ListAssignedUsers(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            var userList = project.Users.ToList();
            return (userList);
        }

        public bool AddProjectToUser(int projectId, string userId)
        {
            Project project = db.Projects.Find(projectId);
            ApplicationUser user = db.Users.Find(userId);

            project.Users.Add(user);

            try
            {
                var userAdded = db.SaveChanges();

                if (userAdded != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        public bool RemoveProjectFromUser(int projectId, string userId)
        {
            Project project = db.Projects.Find(projectId);
            ApplicationUser user = db.Users.Find(userId);

            var result = project.Users.Remove(user);

            try
            {
                var userRemoved = db.SaveChanges();

                if (userRemoved != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Returns a list of a user's projects
        /// </summary>
        /// <param name="userId">The Id of the specified user</param>
        /// <returns>List of Project objects to which the user is assigned.</returns>
        //public IList<Project> ListUserProjects(string userId)
        //{
        //    var projectList = new List<Project>();
        //    foreach (var proj in db.Projects)
        //    {
        //        if (this.IsOnProject(userId, proj.Id))
        //        {
        //            projectList.Add(proj);
        //        }
        //    }
        //    return projectList;
        //}

        /// <summary>
        /// Returns a list of projects that the user is not a part of
        /// </summary>
        /// <param name="userId">The Id of the specified user</param>
        /// <returns>List of Project objects to which the user is not assigned.</returns>
        public IList<Project> ListUserNOTProjects(string userId)
        {
            var projectList = new List<Project>();
            foreach (var proj in db.Projects)
            {
                if (!this.IsOnProject(userId, proj.Id))
                {
                    projectList.Add(proj);
                }
            }
            return projectList;
        }
    }
}
    
