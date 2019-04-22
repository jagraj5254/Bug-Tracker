using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Comments
    {
        public int Id { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public string CommentBody { get; set; }

        public virtual ApplicationUser Created { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public Comments()
        {
            DateCreated = DateTime.Now;
        }
    }
}