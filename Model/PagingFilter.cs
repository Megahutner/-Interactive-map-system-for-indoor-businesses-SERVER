using System;
using System.Collections.Generic;
using System.Text;

namespace UWEServer.Model
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int Take { get; set; }
        public int? Skip { get; set; }
        public string? Filter { get; set; }
        public string? Sort { get; set; }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.Take = 0;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
        }
    }

  
}