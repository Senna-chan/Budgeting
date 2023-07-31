using System.ComponentModel.DataAnnotations;

namespace Budgeting.Models;

public enum TimeAmount
{
    [Display(Name = "One time")]
    ONCE,
    [Display(Name = "Weekly")]
    WEEK,
    [Display(Name = "4 weeks")]
    FOURWEEK,
    [Display(Name = "Monthly")]
    MONTH,
    [Display(Name = "Quarterly")]
    QUARTERYEAR,
    [Display(Name = "Half year")]
    HALFYEAR,
    [Display(Name = "Yearly")]
    YEAR
}