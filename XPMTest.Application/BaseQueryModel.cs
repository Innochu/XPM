namespace XPMTest.Application
{
    public class BaseQueryModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class QueryModel : BaseQueryModel
    {
        public string? Filter { get; set; }
        public string? Keyword { get; set; }
    }

}
