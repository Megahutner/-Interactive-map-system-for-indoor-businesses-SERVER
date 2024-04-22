namespace UWEServer.Model
{
    public class PagedResponse<T> : PageResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public bool? SortMode { get; set; }
        public PagedResponse(IQueryable<T> query, PaginationFilter pagination)
        {
            this.PageNumber = pagination.PageNumber;
            this.PageSize = pagination.Take;
            if (pagination.Take > 0)
                this.Data = query.Skip((pagination.PageNumber - 1) * pagination.Take)
                        .Take(pagination.Take).ToList();
            else
            {
                this.Data = query.ToList();
            }
            this.Skip = pagination.Skip;
            this.Take = pagination.Take;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }

    public class DataFilter
    {
        public string file_name { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class DataSort
    {
        public string selector { get; set; }
        public bool desc { get; set; }
    }
}
