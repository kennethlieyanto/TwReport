FROM nixos/nix:2.35.1 AS taskwarrior
RUN nix-build '<nixpkgs>' -A taskwarrior3 --no-out-link -o /result
RUN mkdir /nix-store-closure && \
    cp -R $(nix-store -qR /result) /nix-store-closure
RUN mkdir -p /taskwarrior-bin && \
    cp /result/bin/task /taskwarrior-bin/task

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.slnx ./
COPY TwReport.Core/*.csproj TwReport.Core/
COPY TwReport.Mailer/*.csproj TwReport.Mailer/
RUN dotnet restore
COPY . .
RUN dotnet publish TwReport.Mailer -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
COPY --from=taskwarrior /nix-store-closure /nix/store
COPY --from=taskwarrior /taskwarrior-bin/task /usr/local/bin/task
WORKDIR /app
COPY --from=build /app .
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
