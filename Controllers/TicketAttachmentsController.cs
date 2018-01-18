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
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using static jcamacho_bugtracker.ApplicationSignInManager;

namespace jcamacho_bugtracker.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Attachments
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var attachments = db.TicketAttachments.Include(a => a.Author).Include(a => a.Ticket);
            return View(attachments.ToList());
        }

        // GET: Attachments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment attachment = db.TicketAttachments.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        // GET: Attachments/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            return View();
        }

        // POST: Attachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create([Bind(Include = "Id,TicketId,Description,CreatedDate,AuthorId,FileUrl,FileName")] TicketAttachment attachment, HttpPostedFileBase file)
        {
            //if (file != null && file.ContentLength > 0)
            //{
            // Only allow certain file types
            //var ext = Path.GetExtension(file.FileName).ToLower();
            //if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf" && ext != ".doc" && ext != ".docx" && ext != ".xls" && ext != ".xlt" && ext != ".ppt" && ext != ".pps")
            //{
            // Error Message???
            //ModelState.AddModelError("file", "Invalid Format.");
            //    }
            //}
            if (ModelState.IsValid)
            {
                if (attachment != null)
                {
                    // Relative server path
                    var filePath = "/Content/Uploads/";
                    // Path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // File url for relative path
                    attachment.FileUrl = filePath + Path.GetFileName(file.FileName);
                    attachment.FilePath = absPath + Path.GetFileName(file.FileName);                  // Save file
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                    
                }

                


                var history = new TicketHistory
                {
                    ChangedDate = new DateTimeOffset(DateTime.Now),
                    TicketId = attachment.TicketId,
                    UserId = User.Identity.GetUserId(),
                    Property = "A user attached the file: ",
                    NewValue = attachment.FileName
                };
                db.TicketHistories.Add(history);

                await Notify(attachment.TicketId, history);

                attachment.AuthorId = User.Identity.GetUserId();
                attachment.CreatedDate = DateTimeOffset.Now;
                db.TicketAttachments.Add(attachment);
                db.SaveChanges();
                var thisTicket = db.Tickets.Find(attachment.TicketId);
                if (thisTicket != null)
                {
                    return RedirectToAction("Edit", "Tickets", new { id = thisTicket.Id });
                }
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", attachment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", attachment.TicketId);
            return RedirectToAction("Edit", "Tickets", new { id = ViewBag.TicketId });
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
                    Subject = "A ticket that you are assigned to has a new attachment.",
                    Body = ticket.AssignedUser.FirstName + ", </p>" + displayName + " has added the attachment <i>" + history.NewValue + "</i> to ticket <i>" + ticket.Title + "</i>.",
                    IsBodyHtml = true
                });
            }
        }

        // GET: Attachments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment attachment = db.TicketAttachments.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", attachment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", attachment.TicketId);
            return View(attachment);
        }

        // POST: Attachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,Description,CreatedDate,AuthorId,FileUrl,FileName")] TicketAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var history = new TicketHistory
                {
                    ChangedDate = new DateTimeOffset(DateTime.Now),
                    TicketId = attachment.TicketId,
                    UserId = User.Identity.GetUserId(),
                    Property = "A user updated the attached the file: ",
                    NewValue = attachment.FileName
                };
                db.TicketHistories.Add(history);

                //if (attachment != null)
                //{
                //    // Relative server path
                //    var filePath = "/Uploads/";
                //    // Path on physical drive on server
                //    var absPath = Server.MapPath("~" + filePath);
                //    // File url for relative path
                //    attachment.FileUrl = filePath + file.FileName;
                //    // Save file
                //    file.SaveAs(Path.Combine(absPath, file.FileName));
                //}

                attachment.AuthorId = User.Identity.GetUserId();
                attachment.CreatedDate = DateTimeOffset.Now;
                db.TicketAttachments.Add(attachment);
                db.Entry(attachment).State = EntityState.Modified;
                db.SaveChanges();
                var thisTicket = db.Tickets.Find(attachment.TicketId);
                if (thisTicket != null)
                {
                    return RedirectToAction("Edit", "Tickets", new { id = thisTicket.Id });
                }
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", attachment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", attachment.TicketId);
            return View(attachment);
        }

        // GET: Attachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment attachment = db.TicketAttachments.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        // POST: Attachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment attachment = db.TicketAttachments.Find(id);

            var history = new TicketHistory
            {
                ChangedDate = new DateTimeOffset(DateTime.Now),
                TicketId = attachment.TicketId,
                UserId = User.Identity.GetUserId(),
                Property = "A user removed the file: ",
                NewValue = attachment.FileName
            };
            db.TicketHistories.Add(history);

            db.TicketAttachments.Remove(attachment);
            db.SaveChanges();
            var thisTicket = db.Tickets.Find(attachment.TicketId);
            if (thisTicket != null)
            {
                return RedirectToAction("Edit", "Tickets", new { id = thisTicket.Id });
            }
            return View(attachment);
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