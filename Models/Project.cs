using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace jcamacho_bugtracker.Models
{
    public class Project
    {
        public Project()
        {
            this.IsActive = true;
            this.IsDeleted = false;
            this.Tickets = new HashSet<Ticket>();
            this.Users = new HashSet<ApplicationUser>();
            
        }

        public int Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        [DisplayName("Title")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string ProjectName { get; set; }
        public string Name { get; set; }

        [AllowHtml]
        [Required]
        [DisplayName("Body")]
        public string ProjectDescription { get; set; }
        public string ManagerId { get; set; }

        public bool IsDeleted { get; set; } 
        public bool IsActive { get; set; }

        public virtual ApplicationUser Manager { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; } //Navigation 
        public virtual ICollection<ApplicationUser> Users { get; set; }
        

    }
}

