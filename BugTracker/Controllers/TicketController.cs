using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        // GET: Ticket
        public ActionResult Index()
        {
            var model = DbContext.Tickets
                                        .Select(p => new ViewTicketViewModel
                                        {
                                            Title = p.Title,
                                            Id = p.Id,
                                            Description = p.Description,
                                            DateCreated = p.DateCreated,
                                            TicketTypeName = p.TicketTypeName,
                                            TicketPriorityName = p.TicketPriorityName
                                        }).ToList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
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

            var project = DbContext.Projects
                .Select(p => p.Name)
                .Where()
     
            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.DateCreated = DateTime.Now;
            ticket.TicketTypeName = model.TicketTypeName;
            ticket.TicketPriorityName = model.TicketPriorityName;
            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketController.Index));
        }
    }
}