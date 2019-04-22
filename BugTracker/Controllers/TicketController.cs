using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
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

        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();

            if (User.IsInRole("Submitter") || User.IsInRole("Developer"))
            {
                var model = DbContext.Tickets
                                .Where(p => p.Project.Users.Any(m => m.Id == userID))
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

        public ActionResult IndexMyTickets()
        {
            string userID = User.Identity.GetUserId();

            if (User.IsInRole("Submitter"))
            {
                var model = DbContext.Tickets
                                .Where(p => p.CreatedBy.Id == userID)
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
                                .Where(p => p.AssignedTo.Id == userID)
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
        public ActionResult Create()
        {
            var model = new CreateTicketViewModel();

            var userId = User.Identity.GetUserId();

            var projects = DbContext.Projects
                .Where(p => p.Users.Any(m => m.Id == userId))
                .Select(p => new { p.Name, p.Id }).ToList();

            model.ProjectList = new SelectList(projects, "Name", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(int? id, CreateTicketViewModel model)
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
                ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);

                if (ticket == null)
                {
                    return RedirectToAction(nameof(TicketController.Index));
                }
            }
            var userId = User.Identity.GetUserId();

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.DateCreated = DateTime.Now;
            ticket.CreatedBy = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            ticket.TicketType = DbContext.TicketTypes.FirstOrDefault(p => p.Name == model.TicketTypeName);
            ticket.TicketPriority = DbContext.TicketPriorities.FirstOrDefault(p => p.Name == model.TicketPriorityName);
            ticket.TicketStatus = DbContext.TicketStatus.FirstOrDefault(p => p.Name == "Open");
            ticket.Project = DbContext.Projects.FirstOrDefault(p => p.Name == model.ProjectName);

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
                p => p.Id == id);

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
                .Where(p => p.Users.Any(m => m.Id == userId))
                .Select(p => new { p.Name, p.Id }).ToList();

            model.ProjectList = new SelectList(projects, "Name", "Name");
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int? id, CreateTicketViewModel model)
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
                ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);

                if (ticket == null)
                {
                    return RedirectToAction(nameof(TicketController.Index));
                }
            }

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.DateUpdated = DateTime.Now;
            ticket.TicketType = DbContext.TicketTypes.FirstOrDefault(p => p.Name == model.TicketTypeName);
            ticket.TicketPriority = DbContext.TicketPriorities.FirstOrDefault(p => p.Name == model.TicketPriorityName);
            ticket.TicketStatus = DbContext.TicketStatus.FirstOrDefault(p => p.Name == model.TicketStatusName);
            ticket.Project = DbContext.Projects.FirstOrDefault(p => p.Name == model.ProjectName);


            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketController.Index));
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(TicketController.Index));


            var ticket = DbContext.Tickets.FirstOrDefault(p =>
            p.Id == id.Value);

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
                Id = p.Id
            }).ToList();
            model.Comments = DbContext.Comments.Where(p => p.TicketId == ticket.Id).Select(p => new CommentTicketViewModel()
            {
                CommentBody = p.CommentBody,
                DateCreated = p.DateCreated,
                DateUpdated = p.DateUpdated,
                Created = p.Created.DisplayName,
                Id = p.Id,
            }).ToList();



            return View(model);
        }

        [HttpPost]
        public ActionResult Details(int? id, ViewTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            string fileExtension;

            if (model.Media != null)
            {
                fileExtension = Path.GetExtension(model.Media.FileName);

                if (!Constants.AllowedFileExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "File extension is not allowed.");
                    return View();
                }
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
            }

            DbContext.Attachments.Add(attachment);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketController.Details));

        }

        public ActionResult AssignTicket()
        {

            var userId = User.Identity.GetUserId();

            var developer = DbContext.Roles.FirstOrDefault(p => p.Name == "Developer");
            var model = DbContext.Users
                                      .Where(p => p.Roles.Any(m => m.RoleId == developer.Id))
                                      .Select(p => new UserManagerRoleViewModel
                                      {
                                          DisplayName = p.DisplayName
                                      }).ToList();

            return View(model);
        }

        public ActionResult Assign(int? id)
        {
            var userId = User.Identity.GetUserId();

            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);

            ticket.AssignedTo = user;

            return RedirectToAction(nameof(TicketController.Index));
        }

        [HttpGet]
        public ActionResult Comment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Comment(int? id, ViewTicketViewModel model)
        {
            return SaveComment(null, model);
        }


        public ActionResult SaveComment(int? id, ViewTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
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

            var userId = User.Identity.GetUserId();

            comment.CommentBody = model.CommentBody;
            comment.TicketId = model.Id;
            comment.DateCreated = DateTime.Now;
            comment.Created = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            DbContext.SaveChanges();

            return RedirectToAction("Details", new { id = comment.TicketId });
        }
    }
}