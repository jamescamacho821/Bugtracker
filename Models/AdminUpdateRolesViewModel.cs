using System.Web.Mvc;

namespace jcamacho_bugtracker.Models
{
    public class AdminUpdateRolesViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }
}