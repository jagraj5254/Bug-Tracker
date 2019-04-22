using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class CommentTicketViewModel
    {
        public virtual ApplicationUser User { get; set; }

        public int Id { get; set; }
        public string Created { get; set; }

        public string CommentBody { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}