using BugTracker.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class ViewTicketViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public string TicketTypeName { get; set; }
        public string TicketPriorityName { get; set; }
        public string TicketStatusName { get; set; }
        public string ProjectName { get; set; }

        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }

        [Required]
        public string CommentBody { get; set; }
        public List<CommentTicketViewModel> Comments { get; set; }

        public HttpPostedFileBase Media { get; set; }

        public string FileUrl { get; set; }
        public List<AttachmentTicketViewModel> Attachments { get; set; }

        public List<ViewHistoryViewModel> Histories { get; set; }

    }
}