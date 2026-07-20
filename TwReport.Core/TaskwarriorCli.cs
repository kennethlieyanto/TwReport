using System.Diagnostics;
using System.Text.Json;
using TwReport.Core.Models;

namespace TwReport.Core;

public class TaskwarriorCli(string filter)
{
    private readonly string _filter = filter;

    public async Task<List<TaskwarriorTask>> GetCompletedTasks(DateTime startDate, DateTime endDate)
    {
        var command = BuildExportCommand(startDate, endDate);
        var output = await ExecuteTaskCommand(command);

        if (string.IsNullOrWhiteSpace(output))
            return [];

        return JsonSerializer.Deserialize<List<TaskwarriorTask>>(output) ?? [];
    }

    public async Task<int> GetCompletedTaskCount(DateTime startDate, DateTime endDate)
    {
        var command = BuildCountCommand(startDate, endDate);
        var output = await ExecuteTaskCommand(command);

        if (string.IsNullOrWhiteSpace(output))
            return 0;

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (int.TryParse(line.Trim(), out var count))
                return count;
        }

        return 0;
    }

    private string BuildExportCommand(DateTime startDate, DateTime endDate)
    {
        var startStr = startDate.ToString("yyyy-MM-dd");
        var endStr = endDate.ToString("yyyy-MM-dd");
        return $"{_filter} end.after:{startStr} end.before:{endStr} export";
    }

    private string BuildCountCommand(DateTime startDate, DateTime endDate)
    {
        var startStr = startDate.ToString("yyyy-MM-dd");
        var endStr = endDate.ToString("yyyy-MM-dd");
        return $"{_filter} end.after:{startStr} end.before:{endStr} count";
    }

    private static async Task<string> ExecuteTaskCommand(string command)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "task",
            Arguments = command,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = psi;
        process.Start();

        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0 && !string.IsNullOrWhiteSpace(stderr))
        {
            throw new InvalidOperationException($"Task command failed: {stderr}");
        }

        return stdout;
    }
}
