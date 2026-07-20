using TwReport.Core.Models;

namespace TwReport.Core;

public class ReportBuilder
{
    public static string GenerateReport(ReportType type, List<TaskwarriorTask> tasks)
    {
        var periodName = DateHelper.GetPeriodName(type);
        var count = tasks.Count;
        var qualifier = type == ReportType.Daily ? "" : "this ";

        if (count == 0)
        {
            return $"Kenneth, you've completed 0 tasks {qualifier}{periodName}.";
        }

        var header = $"Kenneth, you've completed {count} task{(count > 1 ? "s" : "")} {qualifier}{periodName}:";

        var needsGrouping = type is ReportType.Weekly or ReportType.Quarterly or ReportType.Yearly;

        if (!needsGrouping)
        {
            var taskList = string.Join("\n", tasks.Select(t => $"- {t.Description}"));
            return $"{header}\n\n{taskList}";
        }

        var grouped = GroupTasks(type, tasks);
        var sections = new List<string>();

        foreach (var (label, groupTasks) in grouped)
        {
            var taskLines = string.Join("\n", groupTasks.Select(t => $"  - {t.Description}"));
            sections.Add($"{label} ({groupTasks.Count}):\n{taskLines}");
        }

        return $"{header}\n\n{string.Join("\n\n", sections)}";
    }

    private static List<(string Label, List<TaskwarriorTask> Tasks)> GroupTasks(
        ReportType type, List<TaskwarriorTask> tasks)
    {
        var groups = new List<(string Label, List<TaskwarriorTask> Tasks)>();

        var grouped = tasks
            .Where(t => t.GetEndDate().HasValue)
            .GroupBy(t => GetGroupingKey(type, t.GetEndDate()!.Value))
            .OrderBy(g => g.Key);

        foreach (var group in grouped)
        {
            var sampleDate = group.First().GetEndDate()!.Value;
            var label = DateHelper.GetGroupLabel(type, sampleDate);
            groups.Add((label, group.ToList()));
        }

        return groups;
    }

    private static DateTime GetGroupingKey(ReportType type, DateTime date)
    {
        return type switch
        {
            ReportType.Weekly => date.Date,
            ReportType.Quarterly => new DateTime(date.Year, date.Month, 1),
            ReportType.Yearly => new DateTime(date.Year, ((date.Month - 1) / 3) * 3 + 1, 1),
            _ => date.Date
        };
    }
}
