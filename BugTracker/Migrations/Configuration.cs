namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "BugTracker.Models.ApplicationDbContext";
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            RoleManager<IdentityRole> roleManager =
                 new RoleManager<IdentityRole>(
                     new RoleStore<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(context));

            var userManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(context));

            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }

            if (!context.Roles.Any(p => p.Name == "Project Manager"))
            {
                var managerRole = new IdentityRole("Project Manager");
                roleManager.Create(managerRole);
            }

            if (!context.Roles.Any(p => p.Name == "Developer"))
            {
                var developerRole = new IdentityRole("Developer");
                roleManager.Create(developerRole);
            }

            if (!context.Roles.Any(p => p.Name == "Submitter"))
            {
                var submitterRole = new IdentityRole("Submitter");
                roleManager.Create(submitterRole);
            }

            ApplicationUser adminUser;

            if (!context.Users.Any(
                p => p.UserName == "admin@mybugtracker.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@mybugtracker.com";
                adminUser.Email = "admin@mybugtracker.com";

                adminUser.EmailConfirmed = true;
                adminUser.DisplayName = "admin";


                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "admin@mybugtracker.com");
            }
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            context.TicketTypes.AddOrUpdate(x => x.Id,
                new Models.Domain.Tickets.TicketType() { Id = 1, Name = "Bug" },
                new Models.Domain.Tickets.TicketType() { Id = 2, Name = "Feature" },
                new Models.Domain.Tickets.TicketType() { Id = 3, Name = "Database" },
                new Models.Domain.Tickets.TicketType() { Id = 4, Name = "Support" });

            context.TicketPriorities.AddOrUpdate(x => x.Id,
                new Models.Domain.Tickets.TicketPriority() { Id = 1, Name = "Low" },
                new Models.Domain.Tickets.TicketPriority() { Id = 2, Name = "Medium" },
                new Models.Domain.Tickets.TicketPriority() { Id = 3, Name = "High" });

            context.TicketStatus.AddOrUpdate(x => x.Id,
                new Models.Domain.Tickets.TicketStatus() { Id = 1, Name = "Opened" },
                new Models.Domain.Tickets.TicketStatus() { Id = 2, Name = "Resolved" },
                new Models.Domain.Tickets.TicketStatus() { Id = 3, Name = "Rejected" });
            context.SaveChanges();
        }
    }
}
