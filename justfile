OPENSSL_LIB := `ldd "$(which openssl)" 2>/dev/null | awk '/libssl\.so/{print $3}' | xargs dirname 2>/dev/null`

run:
    dotenv -- dotnet run --project TwReport.Mailer -- --dry-run

run-daily:
    LD_LIBRARY_PATH="{{ OPENSSL_LIB }}" dotenv -- dotnet run --project TwReport.Mailer -- --type daily

run-weekly:
    LD_LIBRARY_PATH="{{ OPENSSL_LIB }}" dotenv -- dotnet run --project TwReport.Mailer -- --type weekly

run-monthly:
    LD_LIBRARY_PATH="{{ OPENSSL_LIB }}" dotenv -- dotnet run --project TwReport.Mailer -- --type monthly

run-quarterly:
    LD_LIBRARY_PATH="{{ OPENSSL_LIB }}" dotenv -- dotnet run --project TwReport.Mailer -- --type quarterly

run-yearly:
    LD_LIBRARY_PATH="{{ OPENSSL_LIB }}" dotenv -- dotnet run --project TwReport.Mailer -- --type yearly

build:
    dotnet build

docker-build:
    docker build -t twreport .

docker-run-daily:
    docker run --rm --env-file .env -v ~/.task:/root/.task -v ~/.config/task/taskrc:/etc/twreport/taskrc:ro -v ~/.config/task/taskrc-secret:/etc/twreport/taskrc-secret:ro twreport --type daily

docker-run-weekly:
    docker run --rm --env-file .env -v ~/.task:/root/.task -v ~/.config/task/taskrc:/etc/twreport/taskrc:ro -v ~/.config/task/taskrc-secret:/etc/twreport/taskrc-secret:ro twreport --type weekly

docker-run-monthly:
    docker run --rm --env-file .env -v ~/.task:/root/.task -v ~/.config/task/taskrc:/etc/twreport/taskrc:ro -v ~/.config/task/taskrc-secret:/etc/twreport/taskrc-secret:ro twreport --type monthly

docker-run-quarterly:
    docker run --rm --env-file .env -v ~/.task:/root/.task -v ~/.config/task/taskrc:/etc/twreport/taskrc:ro -v ~/.config/task/taskrc-secret:/etc/twreport/taskrc-secret:ro twreport --type quarterly

docker-run-yearly:
    docker run --rm --env-file .env -v ~/.task:/root/.task -v ~/.config/task/taskrc:/etc/twreport/taskrc:ro -v ~/.config/task/taskrc-secret:/etc/twreport/taskrc-secret:ro twreport --type yearly
