namespace Budgeting.Models
{
    public class BudgetEntryList
    {
        public int Id { get; set; }
        public BudgetEntry BudgetEntry { get; set; }   
        public BudgetList BudgetList { get; set; }
    }
}
