using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace jcamacho_bugtracker.Models
    {
        public class DashboardModels
        {
            //Tickets
            public IList<Ticket> DashTab1 { get; set; }
            //Alerts
            public IList<TicketNotification> DashTab2 { get; set; }
            //Projects
            public IList<Project> DashTab3 { get; set; }
            //People
            public IList<ApplicationUser> DashTab4 { get; set; }
            public DashboardModels()
            {
                this.DashTab1 = new List<Ticket>();
                this.DashTab2 = new List<TicketNotification>();
                this.DashTab3 = new List<Project>();
                this.DashTab4 = new List<ApplicationUser>();
            }
        }
    }