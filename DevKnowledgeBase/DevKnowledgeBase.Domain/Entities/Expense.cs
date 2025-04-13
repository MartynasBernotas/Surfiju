namespace DevKnowledgeBase.Domain.Entities
{
    public class Expense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TripId { get; set; }
        public Trip Trip { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaidByUserId { get; set; } = string.Empty;
        public User PaidBy { get; set; }
    }
}
