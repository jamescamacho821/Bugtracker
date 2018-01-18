using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
//using SendGrid;
using System.Web;

namespace jcamacho_bugtracker.Models
{
    public class Contact
    {
        [Required]
        public string Name { get; set; }
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}