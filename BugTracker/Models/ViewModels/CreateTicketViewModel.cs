using BugTracker.Models.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class CreateTicketViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public string TicketTypeName { get; set; }
        public string TicketPriorityName { get; set; }
        public string ProjectName { get; set; }
    }
}