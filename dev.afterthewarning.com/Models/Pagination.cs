using System.Collections.Generic;

namespace Models
{
    public class Pagination
    {
        public int pageNo { get; set; } = 1;
        public int itemsPerPage { get; set; } = 20;
        public int totalPages { get; set; } = 0;
        public int totalItems { get; set; } = 0;
        public int itemsToSkip { get; set; } = 0;
    }
}