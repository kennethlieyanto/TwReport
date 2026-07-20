Can you help me build this project?

What this project does is basically a reporting server that will emails me every scheduled time (scheduled by other scheduler like systemd timer or kubernetes jobs) and sends me e report of how i'm doing "daily, weekly, monthly, and yearly"

Basically it will tells me how many task I've completed on the recurring configuration:
E.g for weekly it will emails me like this:

Kenneth, you've done 13 task this week, here's all the task you've done this week:

- task1
- task2
- task3
- task4
- task5
...

Data is taken from taskwarrior cli by calling `task` cli directly:

monthly: `task rc.context: status:completed -WAITING end:YYYY-MM-DD export`
weekly: `task rc.context: status:completed -WAITING end:YYYY-MM-DD export`
daily: `task rc.context: status:completed -WAITING end:YYYY-MM-DD export` 

where YYYY-MM-DD can be taken with c# as it can access system's date

these will produce json output so C# needs to parse json and present me in a simple mannger like above email format

to get the task count one can calls `task rc.context: status:completed -WAITING end:YYYY-MM-DD count`

i want `rc.context: status;completed -WAITING` part to be configurable via appsettings

TwReport.Core -> contains main logic including calling task cli, parsing, and transform building report
TwReport.Mailer -> simply the ones that emails it

for now we can stub the emailing part by simply console outputting the email report in console instead
