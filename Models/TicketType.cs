using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace jcamacho_bugtracker.Models
{

    public class TicketType
    {

        public int Id { get; set; }
        public TicketType()
        {
            this.Tickets = new HashSet<Ticket>();
        }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public string Name { get; set; }
    }
}
