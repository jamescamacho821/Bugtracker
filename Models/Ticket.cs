using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace jcamacho_bugtracker.Models
{
    public class Ticket
    {
        //Constructor
        public Ticket()
        {
            this.IsDeleted = false;
          //  this.TicketStatusId = 1; // default to unassigned.
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketHistories = new HashSet<TicketHistory>();
            this.TicketNotifications = new HashSet<TicketNotification>();

        }

        //Columns
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Title cannot be longer than 50 characters.")]
        public string Title { get; set; }
        public string Body { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset CreatedDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

        //Foreign keys
        public int ProjectId { get; set; }
        public string AuthorId { get; set; }
        public string AssignedUserId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public int TicketTypeId { get; set; }

        
        //Navigation properties
        public virtual Project Project { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
        public virtual TicketPriority TicketPriorities { get; set; }
        public virtual TicketStatus TicketStatuses { get; set; }
        public virtual TicketType TicketTypes { get; set; }

        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }

    }
}
