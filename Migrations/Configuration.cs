namespace jcamacho_bugtracker.Migrations
{
    using jcamacho_bugtracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<jcamacho_bugtracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(jcamacho_bugtracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Administrator"))
            {
                roleManager.Create(new IdentityRole { Name = "Administrator" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "jcamacho1964@optonline.net"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcamacho1964@optonline.net",
                    Email = "jcamacho1964@optonline.net",
                    FirstName = "James",
                    LastName = "Camacho",
                    DisplayName = "James Camacho"
                }, "empire111");
                var AdminId = userManager.FindByEmail("jcamacho1964@optonline.net").Id;
                userManager.AddToRole(AdminId, "Administrator");

            }

            if (!context.Users.Any(u => u.Email == "jcamacho0821@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jcamacho0821@gmail.com",
                    Email = "jcamacho0821@gmail.com",
                    FirstName = "James",
                    LastName = "Camacho",
                    DisplayName = "James Camacho"
                }, "empire111");

                var PMId = userManager.FindByEmail("jcamacho0821@gmail.com").Id;
                userManager.AddToRole(PMId, "Project Manager");
            }

            if (!context.Users.Any(u => u.Email == "ewatkins@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ewatkins@coderfoundry.com",
                    Email = "ewatkins@coderfoundry.com",
                    FirstName = "Eric",
                    LastName = "Watkins",
                    DisplayName = "Eric Watkins"
                }, "password");
                var DevId = userManager.FindByEmail("ewatkins@coderfoundry.com").Id;
                userManager.AddToRole(DevId, "Developer");
            }

            if (!context.Users.Any(u => u.Email == "jay_cee50@yahoo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jay_cee50@yahoo.com",
                    Email = "jay_cee50@yahoo.com",
                    FirstName = "James",
                    LastName = "Camacho",
                    DisplayName = "James Camacho"
                }, "password");
                var SubId = userManager.FindByEmail("jay_cee50@yahoo.com").Id;
                userManager.AddToRole(SubId, "Submitter");
            }


            context.TicketTypes.AddOrUpdate(t => t.Id,
                new Models.TicketType() { Id = 1, Name = "Bug" },
                new Models.TicketType() { Id = 2, Name = "Feature Request" },
                new Models.TicketType() { Id = 3, Name = "Change Request" },
                new Models.TicketType() { Id = 4, Name = "General Question" },
                new Models.TicketType() { Id = 5, Name = "Technical Issue" },
                new Models.TicketType() { Id = 6, Name = "UI Issue" },
                new Models.TicketType() { Id = 7, Name = "Cancellation" },
                new Models.TicketType() { Id = 8, Name = "Client-Related Issue" },
                new Models.TicketType() { Id = 9, Name = "Other" }
            );

            context.TicketStatuses.AddOrUpdate(s => s.Id,
                new Models.TicketStatus() { Id = 1, Name = "Unassigned" },
                new Models.TicketStatus() { Id = 2, Name = "Assigned / Active" },
                new Models.TicketStatus() { Id = 3, Name = "Assigned / On-Hold" },
                new Models.TicketStatus() { Id = 4, Name = "Resolved" }
            );

            context.TicketPriorities.AddOrUpdate(p => p.Id,
                new Models.TicketPriority() { Id = 1, Name = "Green" },
                new Models.TicketPriority() { Id = 2, Name = "Yellow" },
                new Models.TicketPriority() { Id = 3, Name = "Orange" },
                new Models.TicketPriority() { Id = 4, Name = "Red" }
            );
        }
    }
}
