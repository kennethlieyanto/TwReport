namespace TwReport.Core.Models;

public class ReportConfig
{
    public TaskwarriorConfig Taskwarrior { get; set; } = new();
    public ResendConfig Resend { get; set; } = new();
    public Dictionary<string, string> Subjects { get; set; } = new();
}

public class TaskwarriorConfig
{
    public string Filter { get; set; } = "rc.context: status:completed -WAITING";
}

public class ResendConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string ToEmail { get; set; } = string.Empty;
}
