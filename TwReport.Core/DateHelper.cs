using TwReport.Core.Models;

namespace TwReport.Core;

public static class DateHelper
{
    public static (DateTime Start, DateTime End) GetDateRange(ReportType type)
    {
        var today = DateTime.Today;

        return type switch
        {
            ReportType.Daily => (today.AddDays(-1), today),
            ReportType.Weekly => (GetWeekStart(today), today),
            ReportType.Monthly => (new DateTime(today.Year, today.Month, 1), today),
            ReportType.Quarterly => (GetQuarterStart(today), GetQuarterEnd(today).AddDays(1)),
            ReportType.Yearly => (new DateTime(today.Year, 1, 1), new DateTime(today.Year + 1, 1, 1)),
            _ => (today, today)
        };
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff);
    }

    private static DateTime GetQuarterStart(DateTime date)
    {
        var quarter = (date.Month - 1) / 3;
        var startMonth = quarter * 3 + 1;
        return new DateTime(date.Year, startMonth, 1);
    }

    private static DateTime GetQuarterEnd(DateTime date)
    {
        var quarter = (date.Month - 1) / 3;
        var endMonth = quarter * 3 + 3;
        return new DateTime(date.Year, endMonth, DateTime.DaysInMonth(date.Year, endMonth));
    }

    public static string GetPeriodName(ReportType type)
    {
        return type switch
        {
            ReportType.Daily => "yesterday",
            ReportType.Weekly => "week",
            ReportType.Monthly => "month",
            ReportType.Quarterly => "quarter",
            ReportType.Yearly => "year",
            _ => "period"
        };
    }

    public static string GetGroupLabel(ReportType type, DateTime date)
    {
        return type switch
        {
            ReportType.Weekly => date.ToString("ddd, MMM d"),
            ReportType.Quarterly => date.ToString("MMMM"),
            ReportType.Yearly => $"Q{(date.Month - 1) / 3 + 1}",
            _ => string.Empty
        };
    }
}
