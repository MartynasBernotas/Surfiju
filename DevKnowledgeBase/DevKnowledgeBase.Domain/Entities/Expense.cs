namespace DevKnowledgeBase.Domain.Entities
{
    public class Expense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CampId { get; set; }
        public Camp Camp { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaidByUserId { get; set; } = string.Empty;
        public User PaidBy { get; set; } = null!;
    }
}
