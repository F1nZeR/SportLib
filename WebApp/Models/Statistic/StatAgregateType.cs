using System.ComponentModel;

namespace WebApp.Models.Statistic
{
    public enum StatAgregateType
    {
        [Description("Максимум")]
        Max,
        [Description("Минимум")]
        Min,
        [Description("Среднее")]
        Mean,
        [Description("Общее")]
        Total
    }
}