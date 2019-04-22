using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class EditMembersViewModel
    {
        public int ProjectId { get; set; }
        public List<ApplicationUser> removeUsers { get; set; }
        public List<ApplicationUser> addUsers { get; set; }
    }
}