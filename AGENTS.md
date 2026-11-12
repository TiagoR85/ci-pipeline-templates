# AGENTS — ci-pipeline-templates

Regras deste repo (valem para qualquer agente que trabalhe aqui).

## Entrega

- **TDD**: mudança de convenção no validador sai com teste xunit novo
  (`PipelineGate.Tests`) no mesmo commit.
- **Docs no mesmo commit**: STATUS.md (título com a versão), CHANGELOG.md
  (entrada nova no topo, antigas nunca reescritas), MANUAL.md quando o
  consumo/setup muda, ADR (`docs/adr/`) para decisão de arquitetura.
- **Semver + tag anotada** a cada entrega relevante (`git tag -a vX.Y.Z`).
- **Card no Hub**: `POST http://127.0.0.1:5002/api/tasks`
  (`{"project_id": <id>, "title": "...", "status": "todo"}`), mover para
  `in_progress`/`done` com `PATCH /api/tasks/<id>`.

## Stack (templates de pipeline + .NET)

- **Toda mudança de template passa pelo dogfooding**: `docker compose up
  --build --exit-code-from validate` tem que passar antes do commit.
- Template alterado em YAML ⇒ `PipelineGate` atualizado + teste novo —
  breaking change em workflow consumida por outros repos exige bump de
  versão e nota no README (lista de consumers).
- Consumers fixam `@v1`; nunca reescrever o comportamento de uma tag já
  publicada — nova convenção = workflow nova ou major.
- Dependências .NET centralizadas em `Directory.Packages.props`.
- Segredos nunca em YAML/.env versionado — via variable group (ADO) ou
  credentials (Jenkins).
- Zero leak: nada de chave, token ou caminho pessoal em doc/commit.
