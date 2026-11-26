# Changelog

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/);
versão em [SemVer](https://semver.org/lang/pt-BR/).

## [1.3.0] — 2026-11-26

### Adicionado
- Conjunto padrão de `.md` do projeto: `AGENTS.md` (regras do repo),
  `DECISIONS.md` (índice dos ADRs 0001–0004) e `MANUAL.md` (instalação,
  primeiro uso, comandos, troubleshooting), ao lado de
  `README.md`/`STATUS.md`/`CHANGELOG.md`.
- README: lista nominal dos repos que adotaram o gate de documentação
  (`doccheck`, `youtube-gemini-pipeline`, `interview-assistant`, `portfolio`,
  `vendas-desapego`) e registro do `doccheck` como primeiro consumidor —
  o gate roda verde contra a ferramenta que o gera.

### Alterado
- `reusable-doccheck.yml`: `doccheck-ref` pinado de `v0.3.1` → `v0.4.0` →
  `v0.5.0` (default atual `v0.5.0`), mantendo o gate reprodutível.
- Ferramenta `doccheck` vendorizada em 0.4.0 (`review` passa a levar recortes
  de código) e 0.5.0 (regra `testes` — contagem estática x doc viva, só aviso
  — e teto `max_tokens` 8000 no `review`). O hook de pre-commit segue rodando
  apenas `check`, determinístico e offline.

## [1.2.0] — 2026-10-29

### Adicionado
- `reusable-node-ci.yml` — CI para repos Node/Astro: inputs `node-version`,
  `test-command` (o gate de cobertura vive no comando), `build-command`,
  cache e `working-directory`; `permissions: contents: read`.
- `reusable-doccheck.yml` — gate de documentação (`workflow_call`) para
  qualquer repo: inputs `doccheck-ref`, `path`, `fail-on-findings`
  (`false` = relatório sem quebrar build), `python-version` e `runs-on`.
- Hook de pre-commit `doccheck` (vendorizado em `.githooks/`, só stdlib,
  ~0,3 s): bloqueia commit com doc incoerente — versão do STATUS diferente
  do topo do CHANGELOG, seção vazia, segredo em `.md`, endpoint citado que
  não existe no código. Atualização por `doccheck install`; escape
  consciente `--no-verify`.
- `reusable-dotnet-ci.yml`: inputs `runs-on`, `build-command` e `test-filter`
  (ex.: `TestCategory!=E2E` para testes que exigem segredo externo).

### Alterado
- Ferramenta `doccheck` vendorizada evoluída até 0.3.1: regra `stack`
  (versão .NET citada na doc x `TargetFramework` do `.csproj`), comandos
  `review` e `status` (painel de deriva), e `__pycache__` fora do commit.
- `reusable-doccheck.yml`: `doccheck-ref` default pinado em `v0.3.1`.
- README: exemplos de consumo do Node e do gate de docs, inputs opcionais do
  `.NET` documentados e contagem de consumidores atualizada (18).

## [1.1.0] — 2026-10-13

### Adicionado
- `reusable-dotnet-ci.yml`: input opcional `pre-restore-command` (comando
  antes do restore — ex.: configurar feed privado), sem quebrar consumidores.

### Alterado
- README: nota de transparência sobre o uso de desenvolvimento assistido por
  IA neste projeto de estudo.

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
