using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jcamacho_bugtracker.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Configuration;
using static jcamacho_bugtracker.ApplicationSignInManager;

namespace jcamacho_bugtracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.TicketComments.Include(c => c.Author).Include(c => c.Ticket);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = db.TicketComments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create([Bind(Include = "Id,Comment,CreatedDate,UpdatedDate,TicketId,AuthorId")] TicketComment ticketcomment)
        {
          

            if (ModelState.IsValid)
            {
                var history = new TicketHistory
                {
                    ChangedDate = new DateTimeOffset(DateTime.Now),
                    TicketId = ticketcomment.Id,
                    UserId = User.Identity.GetUserId(),
                    Property = "A user made the comment: ",
                    NewValue = ticketcomment.Comment
                };
                db.TicketHistories.Add(history);

                await Notify(ticketcomment.Id, history);

                ticketcomment.TicketId = ticketcomment.Id;
                ticketcomment.AuthorId = User.Identity.GetUserId();
                ticketcomment.CreatedDate = DateTimeOffset.Now;
                ticketcomment.UpdatedDate = DateTimeOffset.Now;
                db.TicketComments.Add(ticketcomment);
                db.SaveChanges();
                var thisTicket = db.Tickets.Find(ticketcomment.TicketId);
                if (thisTicket != null)
                {
                    return RedirectToAction("Edit", "Tickets", new { id = thisTicket.Id });
                }
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticketcomment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketcomment.TicketId);
            return View(ticketcomment);
        }
        
        // POST: Ticket/Notifications
        public async Task Notify(int ticketId, TicketHistory history)
        {
            var db = new ApplicationDbContext();
            var ticket = db.Tickets.Find(ticketId);
            //var ticket = db.Tickets.Include("Project").FirstOrDefault(t => t.Id == ticketId);
            var displayName = db.Users.Find(history.UserId).DisplayName;
            var emailService = new PersonalEmail();

            // If the ticket is already assigned to a user and the user is not being changed
            if (ticket.AssignedUserId != null && history.Property != "Assigned / Active")
            {
                await emailService.SendAsync(new System.Net.Mail.MailMessage(ConfigurationManager.AppSettings["username"], ticket.AssignedUser.UserName)
                {
                    Subject = "A ticket that you are assigned to has a new comment.",
                    Body = ticket.AssignedUser.FirstName + ", </p>" + displayName + " made a comment on ticket <i>" + ticket.Title + ".</i></p> The comment reads: " + history.NewValue,
                    IsBodyHtml = true
                });
            }
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = db.TicketComments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", comment.TicketId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Comment,CreatedDate,UpdatedDate,TicketId,AuthorId")] TicketComment ticketcomment)
        {
            if (ModelState.IsValid)
            {
                var history = new TicketHistory
                {
                    ChangedDate = new DateTimeOffset(DateTime.Now),
                    TicketId = ticketcomment.TicketId,
                    UserId = User.Identity.GetUserId(),
                    Property = "A user updated the comment: ",
                    NewValue = ticketcomment.Comment
                };
                db.TicketHistories.Add(history);
                ticketcomment.UpdatedDate = new DateTimeOffset(DateTime.Now);
                db.Entry(ticketcomment).State = EntityState.Modified;
                db.SaveChanges();
                var thisTicket = db.Tickets.Find(ticketcomment.TicketId);
                if (thisTicket != null)
                {
                    return RedirectToAction("Edit", "Tickets", new { id = thisTicket.Id });
                }
            }
            ViewBag.AuthorUserId = new SelectList(db.Users, "Id", "FirstName", ticketcomment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketcomment.TicketId);
            return View(ticketcomment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = db.TicketComments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment comment = db.TicketComments.Find(id);
            db.TicketComments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}