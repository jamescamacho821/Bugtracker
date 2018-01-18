using System.ComponentModel.DataAnnotations;

namespace jcamacho_bugtracker.Models
{
    public class Email
    {
        [Required, Display(Name = "Name")]
        public string FromName { get; set; }

        [Required, Display(Name = "Email"), EmailAddress]
        public string FromEmail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

    }
}