using System.Web.Mvc;

namespace jcamacho_bugtracker.Models
{
    public class AssignUsersViewModel
    {
        public Project Project { get; set; }
        public Ticket Ticket { get; set; }

        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
    }
}
