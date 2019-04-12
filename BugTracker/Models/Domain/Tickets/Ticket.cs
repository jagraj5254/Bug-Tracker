﻿using BugTracker.Models.Domain.Tickets;
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

        public string TicketTypeName { get; set; }
        public virtual TicketType TicketType { get; set; }

        public string TicketPriorityName { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }

        public string TicketStatusName { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }

        //public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        //public string AssignedToId { get; set; }
        public ApplicationUser AssignedTo { get; set; }

        public Ticket()
        {
            DateCreated = DateTime.Now;
        }
    }
}