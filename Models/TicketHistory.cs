using System;
using System.ComponentModel.DataAnnotations;

namespace jcamacho_bugtracker.Models
{
    public class TicketHistory
    {

        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string Property { get; set; }
        [Display(Name = "Old Value")]
        public string OldValue { get; set; }
        [Display(Name = "New Value")]
        public string NewValue { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:M/d/yyyy HH:mm:ss}")]
        public DateTimeOffset? ChangedDate { get; set; }



        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }


    }
}
