using Microsoft.Extensions.Options;
using Resend;
using TwReport.Core;
using TwReport.Core.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<ReportConfig>(
    builder.Configuration.GetSection("ReportConfig"));

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IOptions<ReportConfig>>().Value;
    return new TaskwarriorCli(config.Taskwarrior.Filter);
});

builder.Services.AddSingleton<ReportBuilder>();

builder.Services.AddSingleton(sp =>
{
    var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY");
    if (string.IsNullOrEmpty(apiKey))
        throw new InvalidOperationException("RESEND_API_KEY environment variable is not set.");
    return ResendClient.Create(apiKey);
});

builder.Services.AddSingleton(sp =>
{
    var resend = sp.GetRequiredService<IResend>();
    var config = sp.GetRequiredService<IOptions<ReportConfig>>().Value;
    return new EmailSender(resend, config);
});

var host = builder.Build();

var config = host.Services.GetRequiredService<IOptions<ReportConfig>>().Value;
var taskCli = host.Services.GetRequiredService<TaskwarriorCli>();

var reportTypes = ParseReportTypes(args);
var dryRun = args.Contains("--dry-run");

if (reportTypes.Count == 0 && !dryRun)
{
    Console.Error.WriteLine("Usage: TwReport.Mailer --type <daily|weekly|monthly|quarterly|yearly> [--type ...] [--dry-run]");
    return 1;
}

if (reportTypes.Count == 0)
{
    reportTypes = [ReportType.Daily, ReportType.Weekly, ReportType.Monthly, ReportType.Quarterly, ReportType.Yearly];
}

var allReports = new List<string>();

foreach (var reportType in reportTypes)
{
    var (start, end) = DateHelper.GetDateRange(reportType);
    var tasks = await taskCli.GetCompletedTasks(start, end);
    var report = ReportBuilder.GenerateReport(reportType, tasks);

    Console.WriteLine(report);
    Console.WriteLine();

    allReports.Add(report);
}

if (dryRun)
    return 0;

var emailSender = host.Services.GetRequiredService<EmailSender>();
var combinedReport = string.Join("\n\n", allReports);
var subject = BuildSubject(reportTypes, config.Subjects);

await emailSender.SendReportAsync(subject, combinedReport);

Console.WriteLine("\nEmail sent successfully!");
return 0;

static List<ReportType> ParseReportTypes(string[] args)
{
    var types = new List<ReportType>();

    for (var i = 0; i < args.Length; i++)
    {
        if (args[i] == "--type" && i + 1 < args.Length)
        {
            if (Enum.TryParse<ReportType>(args[++i], ignoreCase: true, out var reportType))
            {
                types.Add(reportType);
            }
            else
            {
                Console.Error.WriteLine($"Unknown report type: {args[i]}");
                Console.Error.WriteLine("Valid types: daily, weekly, monthly, quarterly, yearly");
                return [];
            }
        }
    }

    return types;
}

static string BuildSubject(List<ReportType> reportTypes, Dictionary<string, string> subjects)
{
    var dateStr = DateTime.Now.ToString("yyyy-MM-dd");

    if (reportTypes.Count == 1)
    {
        var key = reportTypes[0].ToString();
        if (subjects.TryGetValue(key, out var template))
            return template.Replace("{date}", dateStr);
    }

    // Fallback for combined reports - try "Combined" or generic
    if (subjects.TryGetValue("Combined", out var combined))
        return combined.Replace("{date}", dateStr);

    return $"TaskWarrior Report - {dateStr}";
}
