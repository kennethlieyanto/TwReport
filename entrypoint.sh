#!/bin/sh
cat /etc/twreport/taskrc > /root/.taskrc
echo "" >> /root/.taskrc
cat /etc/twreport/taskrc-secret >> /root/.taskrc

exec dotnet TwReport.Mailer.dll "$@"
