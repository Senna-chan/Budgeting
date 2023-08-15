using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Budgeting.Models
{
    public class BudgetEntry : CreateUpdateTime
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Category { get; set; }

        [DisplayName("Is Income")]
        [DefaultValue(true)]
        public bool IsIncome { get; set; }

        [DisplayFormat(DataFormatString = "€ {0:n}")]
        [DisplayName("Amount")]
        public decimal MoneyAmount { get; set; }

        [DisplayName("Every X time")]
        public TimeAmount TimeAmount { get; set; }

        [DisplayName("Creditcard payment")]
        public bool FromCreditcard { get; set; }

        [DisplayName("Deposite to payment account")]
        public bool ToSharedAccount { get; set; }

        [DisplayName("Variable Costs")]
        [DefaultValue(false)]
        public bool VariableCosts { get; set; }

        [DisplayName("Transfering on")]
        public DateTime? TransferTime { get; set; }

        public string? Description { get; set; }

        public ICollection<BudgetList>? ListsPartOf { get; set; }

        public decimal CalcuteCostsForSpecificTime(TimeAmount timeAmount)
        {
            decimal pricePerDay = 0;
            decimal result = 0;
            switch (TimeAmount)
            {
                case TimeAmount.ONCE:
                    break;
                case TimeAmount.WEEK:
                    pricePerDay = MoneyAmount / 7;
                    break;
                case TimeAmount.FOURWEEK:
                    pricePerDay = MoneyAmount / (7 * 4);
                    break;
                case TimeAmount.MONTH:
                    pricePerDay = MoneyAmount / (decimal)30.5;
                    break;
                case TimeAmount.QUARTERYEAR:
                    pricePerDay = MoneyAmount / (decimal)(30.5 * 3);
                    break;
                case TimeAmount.HALFYEAR:
                    pricePerDay = MoneyAmount / (decimal)(30.5 * 6);
                    break;
                case TimeAmount.YEAR:
                    pricePerDay = MoneyAmount / 365;
                    break;
            }
            switch (timeAmount)
            {
                case TimeAmount.ONCE:
                    break;
                case TimeAmount.WEEK:
                    result = pricePerDay * 7;
                    break;
                case TimeAmount.FOURWEEK:
                    result = pricePerDay * (7 * 4);
                    break;
                case TimeAmount.MONTH:
                    result = pricePerDay * (decimal)30.5;
                    break;
                case TimeAmount.QUARTERYEAR:
                    result = pricePerDay * (decimal)(30.5 * 3);
                    break;
                case TimeAmount.HALFYEAR:
                    result = pricePerDay * (decimal)(30.5 * 6);
                    break;
                case TimeAmount.YEAR:
                    result = pricePerDay * 365;
                    break;
            }
            return result;
        }
        public Dictionary<TimeAmount, decimal> CalculateCostsForAllTimes()
        {
            Dictionary<TimeAmount, decimal> result = new Dictionary<TimeAmount, decimal>
            {
                { TimeAmount.WEEK, CalcuteCostsForSpecificTime(TimeAmount.WEEK) },
                { TimeAmount.FOURWEEK, CalcuteCostsForSpecificTime(TimeAmount.FOURWEEK) },
                { TimeAmount.MONTH, CalcuteCostsForSpecificTime(TimeAmount.MONTH) },
                { TimeAmount.QUARTERYEAR, CalcuteCostsForSpecificTime(TimeAmount.QUARTERYEAR) },
                { TimeAmount.HALFYEAR, CalcuteCostsForSpecificTime(TimeAmount.HALFYEAR) },
                { TimeAmount.YEAR, CalcuteCostsForSpecificTime(TimeAmount.YEAR) }
            };
            return result;
        }
    }
}