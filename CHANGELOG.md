# Changelog

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/);
versão em [SemVer](https://semver.org/lang/pt-BR/).

## [1.0.0] — 2026-11-12

### Adicionado
- Reusable workflows de GitHub Actions: `reusable-dotnet-ci.yml` (build +
  testes + gate de cobertura por módulo, default 80%) e
  `reusable-node-ci.yml` (para repos Node/Astro).
- Templates Azure DevOps (`build-dotnet.yml`, `test-with-coverage-gate.yml`,
  `deploy-stage.yml`) com ambientes DEV/HOM/PROD e aprovação por Environment.
- Jenkins shared library (`dotnetPipeline.groovy`) com sample de Jenkinsfile.
- Validador `PipelineGate` (.NET 10 + YamlDotNet) que valida as convenções
  dos três formatos, com suíte xunit e dogfooding via Docker Compose.
- `reusable-doccheck.yml` — gate de documentação consumido por outros
  repos do portfólio (5 registrados em `docs(ci)`).
- ADRs 0001–0004 (`docs/adr/`), CodeQL e CI próprios.
