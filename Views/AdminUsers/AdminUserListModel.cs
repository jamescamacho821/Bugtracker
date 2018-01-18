using System;
using jcamacho_bugtracker.Models;
using System.Collections.Generic;

namespace jcamacho_bugtracker.Views.AdminUsers
{
    public class AdminUserListModel
    {
        public ApplicationUser user { get; set; }
        public List<string> Roles { get; set; }
    }
}
