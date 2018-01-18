using jcamacho_bugtracker.Models;
using System.Collections.Generic;


namespace jcamacho_bugtracker.Views.Projects
{

    public class AdminProjectListModel
    {
        public ApplicationUser user { get; set; }
        public List<Project> Projects { get; set; }
    }
}
