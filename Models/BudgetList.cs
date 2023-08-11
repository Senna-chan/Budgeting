namespace Budgeting.Models
{
    public class BudgetList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<BudgetEntry>? BudgetEntries { get; set; }
    }
}
