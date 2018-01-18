using System.Web.Mvc;
namespace jcamacho_bugtracker.Models
    {
        public class TicketAssignUserViewModel
        {
            public Ticket Ticket { get; set; }

            public SelectList Users { get; set; }
            public string SelectedUser { get; set; }
        }
    }