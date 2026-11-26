# Status do Projeto — v1.3.0 (2026-11-26)

## Estado Atual

Biblioteca de templates reutilizáveis de CI/CD para .NET em três
plataformas — GitHub Actions (workflows reutilizáveis), Azure DevOps
(templates de stage com DEV/HOM/PROD + aprovações) e Jenkins (shared
library) — consumida pelos repositórios do portfólio em vez de
copiar/colar YAML. Inclui um validador próprio (`PipelineGate`, .NET 10)
que valida cada template, roda no CI deste repo (dogfooding, ADR-0004) e
é exposto como reusable workflow `reusable-doccheck.yml`, já adotado por
5 repos do portfólio. Gate de cobertura por módulo (default 80%) e CodeQL
fazem parte do pacote.

Quatro reusable workflows publicados (`.github/workflows/`):
`reusable-dotnet-ci.yml`, `reusable-node-ci.yml`, `reusable-doccheck.yml`
e `reusable-docker-scan.yml`. Versões publicadas: `v1.0.0`, `v1.1.0`,
`v1.2.0`, `v1.3.0` — a tag móvel `v1` acompanha a última release.

## Fases

| Fase | Estado | Observação |
|---|---|---|
| Reusable workflows GitHub Actions (.NET + Node/Astro) | ✅ Concluído | `reusable-dotnet-ci.yml` (v1.1.0 + inputs na v1.2.0), `reusable-node-ci.yml` (v1.2.0) |
| Templates Azure DevOps (build/test-gate/deploy-stage) | ✅ Concluído | `azure-devops/templates/` + sample |
| Jenkins shared library | ✅ Concluído | `jenkins/vars/dotnetPipeline.groovy` |
| Validador PipelineGate com testes | ✅ Concluído | `src/PipelineGate` + `PipelineGate.Tests` (xunit) |
| Dogfooding (compose valida os próprios templates) | ✅ Concluído | ADR-0004, `docker compose up --exit-code-from validate` |
| Gate de docs (reusable-doccheck) | ✅ Concluído | `doccheck-ref` pinado em `v0.5.0` (v1.3.0), adotado por 5 repos listados no README |
| Publicação formal de versões (tagging das workflows) | ✅ Concluído | tags anotadas `v1.0.0`/`v1.1.0`/`v1.2.0`/`v1.3.0` + tag móvel `v1` |
| Documentação padrão do projeto (`.md`) | ✅ Concluído | `AGENTS.md`, `DECISIONS.md`, `MANUAL.md` (v1.3.0) |

## Pendências

- [ ] Ampliar cobertura de validação do PipelineGate conforme convenções novas
- [ ] Publicar PipelineGate como `dotnet tool` no NuGet
- [ ] Módulo Terraform para bootstrap de environments/aprovações no ADO
