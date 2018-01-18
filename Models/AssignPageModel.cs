using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jcamacho_bugtracker.Models
{
    public class AssignPageModel
    {
        public string DisplayName { get; set; }

        public MultiSelectList UserNotTickets { get; set; }
        public string TicketOwner { get; set; }
        public string[] newTickets { get; set; }

        public MultiSelectList UserNotProjects { get; set; }
        public string Project { get; set; }
        public string[] newProjects { get; set; }

        public MultiSelectList UserNotRoles { get; set; }
        public string Role { get; set; }
        public string[] newRoles { get; set; }

        public MultiSelectList UserTickets { get; set; }
        public string remTicketOwner { get; set; }
        public string[] remTickets { get; set; }

        public MultiSelectList UserProjects { get; set; }
        public string remProject { get; set; }
        public string[] remProjects { get; set; }

        public MultiSelectList UserRoles { get; set; }
        public string remRole { get; set; }
        public string[] remRoles { get; set; }
    }
}