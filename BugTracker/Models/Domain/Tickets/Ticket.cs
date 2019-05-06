using BugTracker.Models.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public virtual TicketType TicketType { get; set; }
        public int TicketTypeId { get; set; }

        public virtual TicketPriority TicketPriority { get; set; }
        public int TicketPriorityId { get; set; }

        public virtual TicketStatus TicketStatus { get; set; }
        public int TicketStatusId { get; set; }

        public virtual Project Project { get; set; }
        public int ProjectId { get; set; }

        public virtual ApplicationUser CreatedBy { get; set; }

        public virtual ApplicationUser AssignedTo { get; set; }

        public virtual List<ApplicationUser> UserNotifications { get; set; }

        public Ticket()
        {
            DateCreated = DateTime.Now;
            UserNotifications = new List<ApplicationUser>();
        }
    }
}