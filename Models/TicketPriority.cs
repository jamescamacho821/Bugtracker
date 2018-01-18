using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace jcamacho_bugtracker.Models
{

    public class TicketPriority
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public TicketPriority()
        {
            this.Tickets = new HashSet<Ticket>();
        }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}