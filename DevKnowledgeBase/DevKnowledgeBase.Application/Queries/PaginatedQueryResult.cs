namespace DevKnowledgeBase.Application.Queries
{
    public class PaginatedQueryResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public PaginatedQueryResult(List<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
