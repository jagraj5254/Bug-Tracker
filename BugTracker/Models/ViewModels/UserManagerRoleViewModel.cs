using BugTracker.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BugTracker.Models.ViewModels
{
    public class UserManagerRoleViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }

        public List<string> Roles { get; set; }

        public UserManagerRoleViewModel() {
            Roles = new List<string>();
        }
    }
}