using System;
using System.Collections.Generic;
using System.Text;

namespace AllNotes.Models
{
    public class SubfolderAddedMessage
    {
        public AppFolder ParentFolder { get; set; }
        public AppFolder NewSubfolder { get; set; }

        public SubfolderAddedMessage(AppFolder parent, AppFolder subfolder)
        {
            ParentFolder = parent;
            NewSubfolder = subfolder;
        }
    }
}
