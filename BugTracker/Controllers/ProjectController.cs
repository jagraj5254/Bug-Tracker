using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class ProjectController : Controller
    {
        private ApplicationDbContext DbContext;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;


        public ProjectController()
        {
            DbContext = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(DbContext));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DbContext));
        }

        public ActionResult Index()
        {
            var model = new ViewProjectViewModel();

            var members = DbContext.Users.Count();

            if (User.IsInRole("Admin"))
            {
                var tickets = DbContext.Tickets.Count();
                var projects = DbContext.Projects.Count();

                var openTickets = DbContext.Tickets
               .Where(p => p.TicketStatus.Name == "Open").Count();
                var resolvedTickets = DbContext.Tickets
                    .Where(p => p.TicketStatus.Name == "Resolved").Count();
                var rejectedTickets = DbContext.Tickets
                    .Where(p => p.TicketStatus.Name == "Rejected").Count();

                model.Tickets = tickets;
                model.openTickets = openTickets;
                model.resolvedTickets = resolvedTickets;
                model.rejectedTickets = rejectedTickets;
                model.Projects = projects;
                model.Members = members;
            }
            if (User.IsInRole("Submitter"))
            {
                var userId = User.Identity.GetUserId();

                var userProjects = DbContext.Projects
                    .Where(p => p.Users.Any(m => m.Id == userId)).Count();
                var userTickets = DbContext.Tickets
                    .Where(p => p.CreatedBy.Id == userId)
                    .Select(p => new { p.TicketStatus }).ToList();

                var tickets = userTickets.Count();

                var openTickets = userTickets
              .Where(p => p.TicketStatus.Name == "Open").Count();
                var resolvedTickets = userTickets
                    .Where(p => p.TicketStatus.Name == "Resolved").Count();
                var rejectedTickets = userTickets
                    .Where(p => p.TicketStatus.Name == "Rejected").Count();

                model.Tickets = tickets;
                model.openTickets = openTickets;
                model.resolvedTickets = resolvedTickets;
                model.rejectedTickets = rejectedTickets;
                model.Projects = userProjects;
                model.Members = members;
            }

            if (User.IsInRole("Developer"))
            {
                var userId = User.Identity.GetUserId();

                var userProjects = DbContext.Projects
                    .Where(p => p.Users.Any(m => m.Id == userId)).Count();
                var userTickets = DbContext.Tickets
                    .Where(p => p.AssignedTo.Id == userId)
                    .Select(p => new { p.TicketStatus }).ToList();

                var tickets = userTickets.Count();

                var openTickets = userTickets
              .Where(p => p.TicketStatus.Name == "Open").Count();
                var resolvedTickets = userTickets
                    .Where(p => p.TicketStatus.Name == "Resolved").Count();
                var rejectedTickets = userTickets
                    .Where(p => p.TicketStatus.Name == "Rejected").Count();

                model.Tickets = tickets;
                model.openTickets = openTickets;
                model.resolvedTickets = resolvedTickets;
                model.rejectedTickets = rejectedTickets;
                model.Projects = userProjects;
                model.Members = members;
            }

            return View(model);
        }

        public ActionResult ManageUser()
        {
            DbContext.Users
                .Include(x => x.Roles).ToList();

            var model = DbContext.Users
                                       .Select(p => new UserManagerRoleViewModel
                                       {
                                           UserName = p.Email,
                                           DisplayName = p.DisplayName,
                                           Id = p.Id,
                                       }).ToList();

            foreach (var user in model)
            {
                user.Roles = userManager.GetRoles(user.Id).ToList();
            }


            return View(model);
        }

        public ActionResult ViewProject()
        {
            var model = DbContext.Projects
                            .Select(p => new ViewProjectViewModel
                            {
                                Name = p.Name,
                                Id = p.Id,
                                Created = p.Created,
                                Updated = p.Updated,
                                Members = p.Users.Count,
                                Tickets = p.Ticket.Count
                            }).ToList();
            return View(model);
        }

        public ActionResult MyProjects()
        {
            var userId = User.Identity.GetUserId();

            var model = DbContext
                .Projects
                .Where(p => p.Users.Any(t => t.Id == userId))
                .Select(p => new ViewProjectViewModel
                {
                    Name = p.Name,
                    Id = p.Id,
                    Created = p.Created,
                    Updated = p.Updated,
                    Members = p.Users.Count,
                    Tickets = p.Ticket.Count
                }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int? id, CreateProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Project project;

            if (!id.HasValue)
            {
                project = new Project();
                DbContext.Projects.Add(project);
            }
            else
            {
                project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

                if (project == null)
                {
                    return RedirectToAction(nameof(ProjectController.ViewProject));
                }
            }

            project.Name = model.Name;
            project.Created = DateTime.Now;

            DbContext.SaveChanges();

            return RedirectToAction(nameof(ProjectController.ViewProject));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.ViewProject));
            }

            var project = DbContext.Projects.FirstOrDefault(
                p => p.Id == id);

            if (project == null)
            {
                return RedirectToAction(nameof(ProjectController.ViewProject));
            }

            var model = new CreateProjectViewModel();
            model.Name = project.Name;
            model.Updated = project.Updated;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, CreateProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Project project;

            if (!id.HasValue)
            {
                project = new Project();
                DbContext.Projects.Add(project);
            }
            else
            {
                project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

                if (project == null)
                {
                    return RedirectToAction(nameof(ProjectController.ViewProject));
                }
            }

            project.Name = model.Name;
            project.Updated = DateTime.Now;

            DbContext.SaveChanges();

            return RedirectToAction(nameof(ProjectController.ViewProject));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.ViewProject));
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

            if (project != null)
            {
                DbContext.Projects.Remove(project);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(ProjectController.ViewProject));
        }

        [HttpGet]
        public ActionResult EditRole(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var model = new EditRoleViewModel();

                model.Id = userId;

                var userRoles = userManager.GetRoles(userId);

                if (userRoles.Contains("Admin"))
                {
                    model.Admin = true;
                }
                if (userRoles.Contains("Project Manager"))
                {
                    model.ProjectManager = true;
                }
                if (userRoles.Contains("Developer"))
                {
                    model.Developer = true;
                }
                if (userRoles.Contains("Submitter"))
                {
                    model.Submitter = true;
                }

                return View(model);
            }
            else
            {
                return RedirectToAction(nameof(ProjectController.ManageUser));
            }
        }

        [HttpPost]
        public ActionResult EditRole(EditRoleViewModel model, string userId)
        {
            if (string.IsNullOrEmpty(userId) || !ModelState.IsValid)
            {
                return RedirectToAction(nameof(ProjectController.ManageUser));
            }
            else
            {
                if (model.Admin)
                {
                    userManager.AddToRole(userId, "Admin");
                }
                else
                {
                    userManager.RemoveFromRole(userId, "Admin");
                }

                if (model.ProjectManager)
                {
                    userManager.AddToRole(userId, "Project Manager");
                }
                else
                {
                    userManager.RemoveFromRole(userId, "Project Manager");
                }

                if (model.Developer)
                {
                    userManager.AddToRole(userId, "Developer");
                }
                else
                {
                    userManager.RemoveFromRole(userId, "Developer");
                }

                if (model.Submitter)
                {
                    userManager.AddToRole(userId, "Submitter");
                }
                else
                {
                    userManager.RemoveFromRole(userId, "Submitter");
                }

                return RedirectToAction(nameof(ProjectController.ManageUser));
            }
        }

        public ActionResult EditMembers(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.ViewProject));
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return RedirectToAction(nameof(ProjectController.ViewProject));
            }

            var removeUsers = project.Users;
            var users = DbContext.Users.ToList();
            var addUsers = new List<ApplicationUser>();
            foreach (var user in users)
            {
                if (!removeUsers.Contains(user))
                {
                    addUsers.Add(user);
                }
            }

            var model = new EditMembersViewModel()
            {
                addUsers = addUsers.ToList(),
                removeUsers = removeUsers.ToList(),
                ProjectId = project.Id,
            };

            return View(model);
        }

        public ActionResult AddUser(string id, int? projectId)
        {
            if (string.IsNullOrWhiteSpace(id) || !projectId.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.EditMembers));
            }
            var users = DbContext.Users.FirstOrDefault(u => u.Id == id);
            var project = DbContext.Projects.FirstOrDefault(p => p.Id == projectId);
            project.Users.Add(users);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(ProjectController.EditMembers));
        }

        public ActionResult DeleteUser(string id, int? projectId)
        {
            if (string.IsNullOrWhiteSpace(id) || !projectId.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.EditMembers));
            }
            var users = DbContext.Users.FirstOrDefault(u => u.Id == id);
            var project = DbContext.Projects.FirstOrDefault(p => p.Id == projectId);
            project.Users.Remove(users);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(ProjectController.EditMembers));
        }
    }
}