namespace Budgeting.Models
{
    public class BudgetListModel
    {
        public BudgetList BudgetList { get; set; }
        public BudgetEntry entry { get; set; }
        public Dictionary<string, Dictionary<TimeAmount, decimal>> CombinedPrices { get; set; }
        public List<BudgetEntry> BudgetEntries { get; set; }
        public BudgetListModel()
        {
            BudgetList = new BudgetList();
            entry = new BudgetEntry();
            BudgetEntries = new List<BudgetEntry>();
            CombinedPrices = new Dictionary<string, Dictionary<TimeAmount, decimal>>
            {
                { "all", entry.CalculateCostsForAllTimes() },
                { "income", entry.CalculateCostsForAllTimes() },
                { "expenses", entry.CalculateCostsForAllTimes() }
            };
        }

        public void AddToCombinedPrices(string entry, Dictionary<TimeAmount, decimal> costsPerTime)
        {
            if(!CombinedPrices.ContainsKey(entry))
            {
                CombinedPrices.Add(entry, costsPerTime);
                return;
            }
            foreach(var timeAndCost in costsPerTime)
            {
                CombinedPrices[entry][timeAndCost.Key] += timeAndCost.Value;
            }
        }
    }
}
