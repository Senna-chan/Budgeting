namespace Budgeting.Models
{
    public class BudgetListModel
    {
        public BudgetEntry entry { get; set; }
        public Dictionary<string, Dictionary<TimeAmount, decimal>> CombinedPrices { get; set; }
        public List<BudgetEntry> BudgetEntries { get; set; }
        public Dictionary<string, List<BudgetEntry>> BudgetEntriesPerCategory { get; set; }
        public Dictionary<TimeAmount, List<BudgetEntry>> BudgetEntriesPerTime { get; set; }
        public BudgetListModel()
        {
            entry = new BudgetEntry();
            BudgetEntries = new List<BudgetEntry>();
            BudgetEntriesPerCategory = new Dictionary<string, List<BudgetEntry>>();
            BudgetEntriesPerTime = new Dictionary<TimeAmount, List<BudgetEntry>>();
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
