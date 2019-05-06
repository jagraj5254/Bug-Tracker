using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class TicketController : Controller
    {
        private ApplicationDbContext DbContext;

        public TicketController()
        {
            DbContext = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();

            if (User.IsInRole("Submitter") || User.IsInRole("Developer"))
            {
                var model = DbContext.Tickets
                                .Where(p => p.Project.Users.Any(m => m.Id == userID) && p.Project.Archive == false)
                                .Select(p => new ViewTicketViewModel
                                {
                                    Title = p.Title,
                                    Id = p.Id,
                                    Description = p.Description,
                                    DateCreated = p.DateCreated,
                                    DateUpdated = p.DateUpdated,
                                    TicketTypeName = p.TicketType.Name,
                                    TicketPriorityName = p.TicketPriority.Name,
                                    TicketStatusName = p.TicketStatus.Name,
                                    ProjectName = p.Project.Name,
                                    AssignedTo = p.AssignedTo.DisplayName,
                                    CreatedBy = p.CreatedBy.DisplayName
                                }).ToList();
                return View(model);
            }

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                var model = DbContext.Tickets
                                .Where(p => p.Project.Archive == false)
                                .Select(p => new ViewTicketViewModel
                                {
                                    Title = p.Title,
                                    Id = p.Id,
                                    Description = p.Description,
                                    DateCreated = p.DateCreated,
                                    DateUpdated = p.DateUpdated,
                                    TicketTypeName = p.TicketType.Name,
                                    TicketPriorityName = p.TicketPriority.Name,
                                    TicketStatusName = p.TicketStatus.Name,
                                    ProjectName = p.Project.Name,
                                    AssignedTo = p.AssignedTo.DisplayName,
                                    CreatedBy = p.CreatedBy.DisplayName,
                                    UserNotifications = p.UserNotifications
                                }).ToList();
                return View(model);
            }

            return View();
        }

        [Authorize(Roles = "Submitter, Developer")]
        public ActionResult IndexMyTickets()
        {
            string userID = User.Identity.GetUserId();

            if (User.IsInRole("Submitter"))
            {
                var model = DbContext.Tickets
                                .Where(p => p.CreatedBy.Id == userID && p.Project.Archive == false)
                                .Select(p => new ViewTicketViewModel
                                {
                                    Title = p.Title,
                                    Id = p.Id,
                                    Description = p.Description,
                                    DateCreated = p.DateCreated,
                                    DateUpdated = p.DateUpdated,
                                    TicketTypeName = p.TicketType.Name,
                                    TicketPriorityName = p.TicketPriority.Name,
                                    TicketStatusName = p.TicketStatus.Name,
                                    ProjectName = p.Project.Name,
                                    AssignedTo = p.AssignedTo.DisplayName,
                                    CreatedBy = p.CreatedBy.DisplayName
                                }).ToList();
                return View(model);
            }
            if (User.IsInRole("Developer"))
            {
                var model = DbContext.Tickets
                               .Where(p => p.AssignedTo.Id == userID && p.Project.Archive == false)
                               .Select(p => new ViewTicketViewModel
                               {
                                   Title = p.Title,
                                   Id = p.Id,
                                   Description = p.Description,
                                   DateCreated = p.DateCreated,
                                   DateUpdated = p.DateUpdated,
                                   TicketTypeName = p.TicketType.Name,
                                   TicketPriorityName = p.TicketPriority.Name,
                                   TicketStatusName = p.TicketStatus.Name,
                                   ProjectName = p.Project.Name,
                                   AssignedTo = p.AssignedTo.DisplayName,
                                   CreatedBy = p.CreatedBy.DisplayName
                               }).ToList();
                return View(model);
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            var model = new CreateTicketViewModel();

            var userId = User.Identity.GetUserId();

            var projects = DbContext.Projects
                .Where(p => p.Users.Any(m => m.Id == userId && p.Archive == false))
                .Select(p => new { p.Name, p.Id }).ToList();

            var types = DbContext.TicketTypes.ToList();
            var status = DbContext.TicketStatus.ToList();
            var priority = DbContext.TicketPriorities.ToList();

            model.ProjectList = new SelectList(projects, "Name", "Name");
            model.TypeList = new SelectList(types, "Name", "Name");
            model.StatusList = new SelectList(status, "Name", "Name");
            model.PriorityList = new SelectList(priority, "Name", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(int? id, CreateTicketViewModel model)
        {
            var userId = User.Identity.GetUserId();

            if (!ModelState.IsValid)
            {
                var projects = DbContext.Projects
                    .Where(p => p.Users.Any(m => m.Id == userId && p.Archive == false))
                    .Select(p => new { p.Name, p.Id }).ToList();

                var types = DbContext.TicketTypes.ToList();
                var status = DbContext.TicketStatus.ToList();
                var priority = DbContext.TicketPriorities.ToList();

                model.ProjectList = new SelectList(projects, "Name", "Name");
                model.TypeList = new SelectList(types, "Name", "Name");
                model.StatusList = new SelectList(status, "Name", "Name");
                model.PriorityList = new SelectList(priority, "Name", "Name");
                return View(model);
            }

            Ticket ticket;

            if (!id.HasValue)
            {
                ticket = new Ticket();
                DbContext.Tickets.Add(ticket);
            }
            else
            {
                ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id && p.Project.Archive == false);

                if (ticket == null)
                {
                    return RedirectToAction(nameof(TicketController.Index));
                }
            }

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.DateCreated = DateTime.Now;
            ticket.CreatedBy = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            ticket.TicketType = DbContext.TicketTypes.FirstOrDefault(p => p.Name == model.TicketTypeName);
            ticket.TicketPriority = DbContext.TicketPriorities.FirstOrDefault(p => p.Name == model.TicketPriorityName);
            ticket.TicketStatus = DbContext.TicketStatus.FirstOrDefault(p => p.Name == "Open");
            ticket.Project = DbContext.Projects.FirstOrDefault(p => p.Name == model.ProjectName && p.Archive == false);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketController.Index));
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketController.Index));
            }

            var ticket = DbContext.Tickets.FirstOrDefault(
                p => p.Id == id && p.Project.Archive == false);

            if (ticket == null)
            {
                return RedirectToAction(nameof(TicketController.Index));
            }

            var model = new CreateTicketViewModel();
            model.Title = ticket.Title;
            model.Description = ticket.Description;
            model.TicketPriorityName = ticket.TicketPriority.Name;
            model.TicketTypeName = ticket.TicketType.Name;
            model.TicketStatusName = ticket.TicketStatus.Name;
            model.ProjectName = ticket.Project.Name;
            model.DateUpdated = ticket.DateUpdated;

            var userId = User.Identity.GetUserId();

            var projects = DbContext.Projects
                .Where(p => p.Users.Any(m => m.Id == userId) && p.Archive == false)
                .Select(p => new { p.Name, p.Id }).ToList();

            var types = DbContext.TicketTypes.ToList();
            var status = DbContext.TicketStatus.ToList();
            var priority = DbContext.TicketPriorities.ToList();

            model.ProjectList = new SelectList(projects, "Name", "Name");
            model.TypeList = new SelectList(types, "Name", "Name");
            model.StatusList = new SelectList(status, "Name", "Name");
            model.PriorityList = new SelectList(priority, "Name", "Name");

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(int? id, CreateTicketViewModel model, string userId)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Ticket ticket;

            if (!id.HasValue)
            {
                ticket = new Ticket();
                DbContext.Tickets.Add(ticket);
            }
            else
            {
                ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id && p.Project.Archive == false);

                if (ticket == null)
                {
                    return RedirectToAction(nameof(TicketController.IndexMyTickets));
                }
            }

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.DateUpdated = DateTime.Now;
            ticket.TicketType = DbContext.TicketTypes.FirstOrDefault(p => p.Name == model.TicketTypeName);
            ticket.TicketPriority = DbContext.TicketPriorities.FirstOrDefault(p => p.Name == model.TicketPriorityName);
            ticket.TicketStatus = DbContext.TicketStatus.FirstOrDefault(p => p.Name == model.TicketStatusName);
            ticket.Project = DbContext.Projects.FirstOrDefault(p => p.Name == model.ProjectName && p.Archive == false);

            var originalValues = DbContext.Entry(ticket).OriginalValues;
            var currentValues = DbContext.Entry(ticket).CurrentValues;
            foreach (var property in originalValues.PropertyNames)
            {
                if (property != "DateUpdated")
                {
                    var originalValue = originalValues[property].ToString();
                    var currentValue = currentValues[property].ToString();
                    if (originalValue != currentValue)
                    {
                        var history = new History();
                        if (property == "TicketTypeId")
                        {
                            history.Property = "Ticket Type";
                            history.Changed = DateTime.Now;
                            history.TicketId = ticket.Id;
                            var newValue = Convert.ToInt32(currentValue);
                            var oldValue = Convert.ToInt32(originalValue);
                            history.NewValue = DbContext.TicketTypes.FirstOrDefault(p => p.Id == newValue).Name;
                            history.OldValue = DbContext.TicketTypes.FirstOrDefault(p => p.Id == oldValue).Name;
                            history.User = DbContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name);
                        }
                        else if (property == "TicketPriorityId")
                        {
                            history.Property = "Ticket Priority";
                            history.Changed = DateTime.Now;
                            history.TicketId = ticket.Id;
                            var newValue = Convert.ToInt32(currentValue);
                            var oldValue = Convert.ToInt32(originalValue);
                            history.NewValue = DbContext.TicketPriorities.FirstOrDefault(p => p.Id == newValue).Name;
                            history.OldValue = DbContext.TicketPriorities.FirstOrDefault(p => p.Id == oldValue).Name;
                            history.User = DbContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name);
                        }
                        else if (property == "TicketStatusId")
                        {
                            history.Property = "Ticket Status";
                            history.Changed = DateTime.Now;
                            history.TicketId = ticket.Id;
                            var newValue = Convert.ToInt32(currentValue);
                            var oldValue = Convert.ToInt32(originalValue);
                            history.NewValue = DbContext.TicketStatus.FirstOrDefault(p => p.Id == newValue).Name;
                            history.OldValue = DbContext.TicketStatus.FirstOrDefault(p => p.Id == oldValue).Name;
                            history.User = DbContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name);
                        }
                        else if (property == "ProjectId")
                        {
                            history.Property = "Project";
                            history.Changed = DateTime.Now;
                            history.TicketId = ticket.Id;
                            var newValue = Convert.ToInt32(currentValue);
                            var oldValue = Convert.ToInt32(originalValue);
                            history.NewValue = DbContext.Projects.FirstOrDefault(p => p.Id == newValue).Name;
                            history.OldValue = DbContext.Projects.FirstOrDefault(p => p.Id == oldValue).Name;
                            history.User = DbContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name);
                        }
                        else
                        {
                            history.Changed = DateTime.Now;
                            history.NewValue = currentValue;
                            history.OldValue = originalValue;
                            history.Property = property;
                            history.TicketId = ticket.Id;
                            history.User = DbContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name);
                        }

                        DbContext.Histories.Add(history);
                        if (ticket.AssignedTo != null)
                        {
                            EmailNotification.SendNotification(ticket.AssignedTo.Email, $"{ property} was changed", "Notification");
                        }
                        foreach (var user in ticket.UserNotifications)
                        {
                            EmailNotification.SendNotification(user.Email, $"{ property} was changed", "Notification");
                        }
                    }
                }
            }
            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketController.Index));
        }

        [HttpGet]
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(TicketController.Index));


            var ticket = DbContext.Tickets.FirstOrDefault(p =>
            p.Id == id.Value && p.Project.Archive == false);

            if (ticket == null)
                return RedirectToAction(nameof(TicketController.Index));

            var model = new ViewTicketViewModel();
            model.Title = ticket.Title;
            model.Description = ticket.Description;
            model.DateUpdated = ticket.DateUpdated;
            model.Id = ticket.Id;
            model.DateCreated = ticket.DateCreated;
            model.TicketPriorityName = ticket.TicketPriority.Name;
            model.TicketStatusName = ticket.TicketStatus.Name;
            model.TicketTypeName = ticket.TicketType.Name;
            model.Attachments = DbContext.Attachments.Where(p => p.TicketId == ticket.Id).Select(p => new AttachmentTicketViewModel()
            {
                FileUrl = p.FileUrl,
                Id = p.Id,
                Created = p.User.UserName
            }).ToList();
            model.Comments = DbContext.Comments.Where(p => p.TicketId == ticket.Id).Select(p => new CommentTicketViewModel()
            {
                CommentBody = p.CommentBody,
                DateCreated = p.DateCreated,
                Created = p.Created.UserName,
                Id = p.Id,
            }).ToList();

            model.Histories = DbContext.Histories.Where(p => p.TicketId == ticket.Id).Select(p => new ViewHistoryViewModel()
            {
                Changed = p.Changed,
                Property = p.Property,
                OldValue = p.OldValue,
                NewValue = p.NewValue,
                By = p.User.DisplayName
            }).ToList();


            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Details(int? id, ViewTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Attachments attachment;
            attachment = new Attachments();

            attachment.TicketId = model.Id;

            if (model.Media != null)
            {
                if (!Directory.Exists(Constants.MappedUploadFolder))
                {
                    Directory.CreateDirectory(Constants.MappedUploadFolder);
                }

                var fileName = model.Media.FileName;
                var fullPathWithName = Constants.MappedUploadFolder + fileName;

                model.Media.SaveAs(fullPathWithName);

                attachment.FileUrl = Constants.UploadFolder + fileName;
                attachment.User = DbContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name);

            }

            DbContext.Attachments.Add(attachment);
            DbContext.SaveChanges();

            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == attachment.TicketId && p.Project.Archive == false);
            if (ticket.AssignedTo != null)
            {
                EmailNotification.SendNotification(ticket.AssignedTo.Email, "Attachment was added", "Notification");
            }

            foreach (var user in ticket.UserNotifications)
            {
                EmailNotification.SendNotification(user.Email, $"Attachment was changed", "Notification");
            }

            return RedirectToAction(nameof(TicketController.Details));

        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignTicket(int? id)
        {

            var userId = User.Identity.GetUserId();

            var developer = DbContext.Roles.FirstOrDefault(p => p.Name == "Developer");
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id && p.Project.Archive == false);
            var model = DbContext.Users
                                      .Where(p => p.Roles.Any(m => m.RoleId == developer.Id))
                                      .Select(p => new UserManagerRoleViewModel
                                      {
                                          DisplayName = p.DisplayName,
                                          Id = p.Id
                                      }).ToList();

            ViewBag.ticketId = ticket.Id;
            return View(model);
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Assign(int? id, string userId)
        {
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id && p.Project.Archive == false);

            ticket.AssignedTo = user;

            DbContext.SaveChanges();

            EmailNotification.SendNotification(ticket.AssignedTo.Email, "A ticket was assigned", "Notification");

            return RedirectToAction(nameof(TicketController.Index));
        }

        [Authorize]
        [HttpGet]
        public ActionResult Comment()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Comment(int? id, ViewTicketViewModel model)
        {
            return SaveComment(null, model);
        }

        [Authorize]
        public ActionResult SaveComment(int? id, ViewTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", new { id = model.Id });
            }

            Comments comment;

            if (!id.HasValue)
            {
                comment = new Comments();
                DbContext.Comments.Add(comment);
            }
            else
            {
                comment = DbContext.Comments.FirstOrDefault(p => p.Id == id);

                if (comment == null)
                {
                    return RedirectToAction(nameof(TicketController.Details));
                }
            }

            comment.CommentBody = model.CommentBody;
            comment.TicketId = model.Id;
            comment.DateCreated = DateTime.Now;
            comment.Created = DbContext.Users.FirstOrDefault(p => p.UserName == User.Identity.Name);

            DbContext.SaveChanges();

            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == comment.TicketId && p.Project.Archive == false);
            if (ticket.AssignedTo != null)
            {
                EmailNotification.SendNotification(ticket.AssignedTo.Email, "Comment was created", "Notification");
            }

            foreach (var user in ticket.UserNotifications)
            {
                EmailNotification.SendNotification(user.Email, "Comment was created", "Notification");
            }

            return RedirectToAction("Details", new { id = comment.TicketId });
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditComments(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketController.Details));
            }

            var comment = DbContext.Comments.FirstOrDefault(
                p => p.Id == id);

            if (comment == null)
            {
                return RedirectToAction(nameof(TicketController.Details));
            }

            var model = new ViewTicketViewModel();
            model.CommentBody = comment.CommentBody;
            model.Id = comment.Id;
            return View(model);
        }


        [HttpPost]
        [Authorize]
        public ActionResult EditComments(int id, ViewTicketViewModel model)
        {
            var comment = DbContext.Comments.FirstOrDefault(
                 p => p.Id == id);

            comment.CommentBody = model.CommentBody;

            DbContext.SaveChanges();

            return RedirectToAction("Details", new { id = comment.TicketId });
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteComments(int? id, ViewTicketViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketController.Details));
            }

            var comment = DbContext.Comments.FirstOrDefault(p => p.Id == id.Value);

            if (comment != null)
            {
                DbContext.Comments.Remove(comment);
                DbContext.SaveChanges();
            }

            return RedirectToAction("Details", new { id = comment.TicketId });
        }

        [Authorize]
        public ActionResult DeleteAttachments(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketController.Details));
            }

            var attachment = DbContext.Attachments.FirstOrDefault(p => p.Id == id.Value);

            if (attachment != null)
            {
                DbContext.Attachments.Remove(attachment);
                DbContext.SaveChanges();
            }

            return RedirectToAction("Details", new { id = attachment.TicketId });
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult OptIn(int? id)
        {
            var userId = User.Identity.GetUserId();

            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id && p.Project.Archive == false);
            ticket.UserNotifications.Add(user);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketController.Index));
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult OptOut(int? id)
        {
            var userId = User.Identity.GetUserId();

            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id && p.Project.Archive == false);

            ticket.UserNotifications.Remove(user);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketController.Index));
        }
    }
}