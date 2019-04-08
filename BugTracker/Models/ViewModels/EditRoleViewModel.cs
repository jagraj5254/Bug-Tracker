﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        public bool Admin { get; set; }
        public bool ProjectManager { get; set; }
        public bool Developer { get; set; }
        public bool Submitter { get; set; }
    }
}