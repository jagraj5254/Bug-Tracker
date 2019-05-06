using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public virtual List<ApplicationUser> Users { get; set; }
        public virtual List<Ticket> Ticket { get; set; }
        public bool Archive { get; set; }

        public Project()
        {
            Users = new List<ApplicationUser>();
            Created = DateTime.Now;
        }
    }
}