using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jcamacho_bugtracker.Models
{
    public class DashboardViewModel
    {
        public List<Project> AllProjects { get; set; }
        public List<Project> RelevantProjects { get; set; }
        public List<Project> IrrelevantProjects { get; set; }
        public List<Ticket> AllTickets { get; set; }
        public List<Ticket> RelevantTickets { get; set; }
        public List<Ticket> IrrelevantTickets { get; set; }
    }
}