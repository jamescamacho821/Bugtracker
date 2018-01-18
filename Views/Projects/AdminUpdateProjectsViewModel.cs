using jcamacho_bugtracker.Models;
using System.Web.Mvc;

namespace jcamacho_bugtracker.Views.Projects
{

    public class AdminUpdateProjectsViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public MultiSelectList Projects { get; set; }
        public Project[] SelectedProjects { get; set; }
    }
}
