using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Budgeting.Models
{
    public class SingleTimeBudgetEntry
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Category { get; set; }

        [DisplayName("Is Income")]
        [DefaultValue(false)]
        public bool IsIncome { get; set; }

        [DisplayFormat(DataFormatString = "€ {0:n}")]
        [DisplayName("Amount")]
        public decimal MoneyAmount { get; set; }

        [DisplayName("Creditcard payment")]
        public bool FromCreditcard { get; set; }

        [DisplayName("Transfering on")]
        public DateTime? TransferTime { get; set; }

        public string? Description { get; set; }
    }
}
