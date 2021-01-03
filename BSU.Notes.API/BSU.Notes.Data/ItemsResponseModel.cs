using System.Collections.Generic;

namespace BSU.Notes.Data
{
    public class ItemsResponseModel<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public PaginationModel PaginationInfo { get; set; }
    }
}
