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
            return View();
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
                                Created = p.Created
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
    }
}