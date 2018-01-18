using System;
using System.ComponentModel.DataAnnotations;

namespace jcamacho_bugtracker.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string AuthorId { get; set; }
        [Display(Name = "File Path")]
        public string FilePath { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset CreatedDate { get; set; }
        [Display(Name = "File Url")]
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        //Navigation
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}
