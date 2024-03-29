﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class ViewProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Members { get; set; }

        public int Projects { get; set; }

        public int Tickets { get; set; }
        public int openTickets { get; set; }
        public int resolvedTickets { get; set; }
        public int rejectedTickets { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}