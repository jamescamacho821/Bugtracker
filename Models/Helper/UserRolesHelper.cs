using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace jcamacho_bugtracker.Models.Helper
{
    public class UserRolesHelper
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        //private UserManager<ApplicationUser> manager =
        //    new UserManager<ApplicationUser>(
        //        new UserStore<ApplicationUser>(
        //            new ApplicationDbContext()));

        public UserRolesHelper(ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            this.roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            this.db = context;
        }

        public bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }

        //public string GetUserInRole(string userId)
        //{
        //    return userManager.GetRoles(userId).First();
        //}

        public IList<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            //var roles = userManager.GetRoles(userId);
            //foreach(var role in roles)
            //{
            //    RemoveUserFromRole(userId, role);
            //}
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var userIDs = roleManager.FindByName(roleName).Users.Select(r => r.UserId);
            return userManager.Users.Where(u => userIDs.Contains(u.Id)).ToList();
            //Select(u => new UserDropDownViewModel { Id = u.Id, Name = u.DisplayName }).
        }

        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var userIDs = System.Web.Security.Roles.GetUsersInRole(roleName);
            return userManager.Users.Where(u => !userIDs.Contains(u.Id)).ToList();
            //Select(u => new UserDropDownViewModel { Id = u.Id, Name = u.DisplayName }).
        }

        //public IList<string> ListUserRoles(string userId)
        //{
        //    return manager.GetRoles(userId);
        //}

        //public bool AddUserToRole(string userId, string roleName)
        //{
        //    var result = manager.AddToRole(userId, roleName);
        //    return result.Succeeded;
        //}

        //public bool RemoveUserFromRole(string userId, string roleName)
        //{
        //    var result = manager.RemoveUserFromRole(string userId, string roleName);
        //    return result.Succeeded;
        //}
    }
}