namespace Jupeta.Services
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int? PreviousPage
        {
            get
            {
                if (CurrentPage > 1)
                    return CurrentPage - 1;

                return null;
            }
        }

        public int? NextPage
        {
            get
            {
                if (CurrentPage < TotalPages)
                    return CurrentPage + 1;

                return null;
            }
        }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }
        //public bool HasNextPage => Page * PageSize < TotalCount;

        public static PagedList<T> ToPagedList(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = query.Count();
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

    }
}
