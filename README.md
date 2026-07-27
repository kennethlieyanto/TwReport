# TwReport
Simple taskwarrior reporter that reports task done on specified schedule.

- Daily
- Weekly
- Monthly
- Quarterly
- Yearly

Currently only supports using resend for email api.

Tested on taskwarrior 3.4.2

## How it works

.NET will simply call `task` command available on path (depends on `task` bin available on path)

Or use docker image which will have the `task` which have taskwarrior `task` bin builtin
