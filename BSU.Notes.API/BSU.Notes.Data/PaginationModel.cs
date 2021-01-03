using System;
using System.Collections.Generic;
using System.Text;

namespace BSU.Notes.Data
{
    public class PaginationModel
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalRecords { get; set; }
    }
}
