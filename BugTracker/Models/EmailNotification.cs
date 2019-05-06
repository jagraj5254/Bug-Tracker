using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public static class EmailNotification
    {
        public static void SendNotification(string user, string body, string subject)
        {
            var email = new EmailService();
            email.Send(user, body, subject);
        }
    }
}