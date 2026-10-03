# syntax=docker/dockerfile:1

# Estágio 1: build com SDK completo
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Packages.props .
COPY src/PipelineGate/PipelineGate.csproj src/PipelineGate/
RUN dotnet restore src/PipelineGate/PipelineGate.csproj

COPY src/PipelineGate/ src/PipelineGate/
RUN dotnet publish src/PipelineGate/PipelineGate.csproj \
    -c Release -o /app/publish --no-restore

# Estágio 2: runtime mínimo (console, sem ASP.NET)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# O validador precisa dos próprios templates para conferi-los
COPY .github/ .github/
COPY azure-devops/ azure-devops/
COPY jenkins/ jenkins/

# Usuário não-root built-in (uid 1654) — mesmo padrão do docker-dotnet-template
USER $APP_UID

ENTRYPOINT ["dotnet", "pipeline-gate.dll"]
CMD ["validate", "/app"]
