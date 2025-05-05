namespace API.Common
{
    public class PagingData
    {
        public dynamic? Data { get; set; }
        public int? PageNum { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public int? TotalItems { get; set; } = 0;
        public int? TotalPages { get; set; } = 0;
        public List<SearchData> ListSearchData { get; set; }
    }

    public class SearchData
    {
        public string? ColSearch { get; set; }
        public string? ValueSearch { get; set; }

    }
}
