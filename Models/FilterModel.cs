using System.Collections.Generic;
using System.Web.Mvc;

namespace jcamacho_bugtracker.Models
{
    public class FilterModel
    {
        public FilterModel()
        {
         //   this.ticketStatusList = new MultiSelectList();
        //    this.ticketPriorityList = new MultiSelectList();
        //    this.projectList = new MultiSelectList();
        //    this.authorList = new MultiSelectList();
        //    this.ticketTypeList = new MultiSelectList();
        //    this.assignedList = new MultiSelectList();
        }
        // Stuff viewbag with filter options
        public MultiSelectList ticketTypeList { get; set; }
        public MultiSelectList ticketStatusList { get; set; }
        public MultiSelectList ticketPriorityList { get; set; }
        public MultiSelectList projectList { get; set; }
        public MultiSelectList authorList { get; set; }
        public MultiSelectList assignedList { get; set; }
    }
}