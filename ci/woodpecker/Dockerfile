FROM docker:28-cli AS docker-cli

FROM mcr.microsoft.com/dotnet/sdk:10.0

COPY --from=docker-cli /usr/local/bin/docker /usr/local/bin/docker
COPY --from=docker-cli /usr/local/libexec/docker/cli-plugins /usr/local/libexec/docker/cli-plugins

RUN dotnet tool install \
    --tool-path /usr/local/bin \
    Aspire.Cli \
    --version 13.4.6 \
    && docker --version \
    && docker compose version \
    && aspire --version

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
    DOTNET_NOLOGO=true

WORKDIR /workspace
