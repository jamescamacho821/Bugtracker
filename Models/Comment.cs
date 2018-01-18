using System;
using System.Web.Mvc;

namespace jcamacho_bugtracker.Models

{
    public class Comment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string AuthorId { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public string UpdatedReason { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ApplicationUser Author { get; set; }
    }
}