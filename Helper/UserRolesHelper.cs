using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Web;
using SendGrid.Helpers.Mail;

namespace jcamacho_bugtracker.Models
{
    public class ProjectAssignHelper
    {
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInRole(string userId, string roleName)
        {
            return manager.IsInRole(userId, roleName);
        }
        public void Notify(TicketNotification tn, string action)
        {
            ///// INSERT SENDGRID FOR NOTIFICATIONS
            //db.Entry(tn).Reload();
            string emailSubject = "";
            string emailMessage = "";

            if (action.Equals("Remove"))
            {
                /// removed from ticket
                emailMessage = "You were removed from ";
                try
                {
                    emailMessage += tn.Ticket.Title + ": " + tn.Ticket.Body + ".";
                }
                catch { }
                emailSubject = "Removed from: ";
                try
                {
                    emailSubject += tn.Ticket.Title;
                }
                catch { }
            }
            else if (action.Equals("Add"))
            {
                /// added to ticket
                emailMessage = "You were removed from " + tn.Ticket.Title + ": " + tn.Ticket.Body + ".";
                emailSubject = "Added to: " + tn.Ticket.Title;
            }
            else if (action.Equals("Manage Ticket"))
            {
                /// new ticket created and added to manager's project
                emailMessage = "Project " + tn.Ticket.Project.ProjectName + " has a new ticket, Priority: " + tn.Ticket.TicketPriorities + "\n" +
                    "Ticket: " + tn.Ticket.Title + ".";
                emailSubject = tn.Ticket.Project.ProjectName + " has a new ticket, Priority: " + tn.Ticket.TicketPriorities;
            }

            //SendGrid Login from Web.config
            var MyAddress = ConfigurationManager.AppSettings["ContactEmail"];
            var MyUsername = ConfigurationManager.AppSettings["Username"];
            var MyPassword = ConfigurationManager.AppSettings["Password"];

            //"To" information 
            var toName = db.Users.Find(tn.UserId).DisplayName;
            var toEmail = db.Users.Find(tn.UserId).Email;
            //"From" information
            SendGridMessage mail = new SendGridMessage();
            mail.AddTo(toEmail);
            mail.Subject = emailSubject;
            //      mail.From = new MailAddress("jcamacho1964@optonline.net");
            //      mail.Text = emailMessage;
            var credentials = new NetworkCredential(MyUsername, MyPassword);
            //      var transportWeb = new Web(credentials);
            //    transportWeb.Deliver(mail);
        }

        public IList<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            return manager.RemoveFromRole(userId, roleName).Succeeded;
        }
        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            foreach (var user in manager.Users)
            {
                if (IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }

        [Display(Name = "Users Not In Role")]
        public IList<ApplicationUser> UsersNOTInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            foreach (var user in manager.Users)
            {
                if (!IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }


    ///// <summary>
    ///// PROJECT
    ///// </summary>
    //public class ProjectAssignHelper
    //{
    //    private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
    //    private ApplicationDbContext db = new ApplicationDbContext();

    //    public bool IsOnProject(string userId, int projectId)
    //    {
    //        return db.Users.Find(userId).Projects.Any(p => p.Id == projectId);
    //    }

    //    public IList<Project> ListUserProjects(string userId)
    //    {
    //        var projectList = new List<Project>();
    //        foreach (var proj in db.Projects)
    //        {
    //            if (this.IsOnProject(userId, proj.Id))
    //            {
    //                projectList.Add(proj);
    //            }
    //        }
    //        return projectList;
    //    }

    //    /// <summary>
    //    /// Returns a list of projects that the user is not a part of
    //    /// </summary>
    //    /// <param name="userId">The Id of the specified user</param>
    //    /// <returns>List of Project objects to which the user is not assigned.</returns>
    //    public IList<Project> ListUserNOTProjects(string userId)
    //    {
    //        var projectList = new List<Project>();
    //        foreach (var proj in db.Projects)
    //        {
    //            if (!this.IsOnProject(userId, proj.Id))
    //            {
    //                projectList.Add(proj);
    //            }
    //        }
    //        return projectList;
    //    }

    //    //public bool AddUserToProject(string userId, int projectId)
    //    //{
    //    //    var result = manager.AddToProject(userId, projectId);
    //    //    return result.Succeeded;
    //    //}
}

