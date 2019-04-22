using BugTracker.Models.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace BugTracker.Models.ViewModels
{
    public class CreateTicketViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public string TicketTypeName { get; set; }
        public string TicketPriorityName { get; set; }
        public string TicketStatusName { get; set; }
        public string ProjectName { get; set; }
        public SelectList ProjectList { get; set; }
    }
}