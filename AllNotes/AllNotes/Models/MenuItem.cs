using System;
using System.Collections.Generic;
using System.Text;

namespace AllNotes.Models
{
    public class MenuItem
    {
         public string Title { get; set; }
        public Type TargetType { get; set; }
       
        public Action CommandAction { get; set; }
    }
}
