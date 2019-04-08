using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class UserRoles
    {
        public string Admin { get; set; }
        public string ProjectManager { get; set; }
        public string Developer { get; set; }
        public string Submitter { get; set; }
    }
}