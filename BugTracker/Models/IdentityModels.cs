using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using BugTracker.Models.Domain;
using BugTracker.Models.Domain.Tickets;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual List<Project> Projects { get; set; }
        public virtual List<Ticket> Tickets { get; set; }
        public virtual List<Comments> Comments { get; set; }

        [InverseProperty(nameof(Ticket.CreatedBy))]
        public virtual List <Ticket> CreatedBy { get; set; }

        [InverseProperty(nameof(Ticket.AssignedTo))]
        public virtual List<Ticket> AssignedTo { get; set; }

        public string DisplayName { get; set; }

        public ApplicationUser()
        {
            Projects = new List<Project>();
            Tickets = new List<Ticket>();
            Comments = new List<Comments>();

            CreatedBy = new List<Ticket>();
            AssignedTo = new List<Ticket>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<TicketStatus> TicketStatus { get; set; }
        public DbSet<TicketPriority> TicketPriorities { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}