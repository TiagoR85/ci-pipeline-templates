# MANUAL — ci-pipeline-templates

## Visão geral

Receitas reutilizáveis de CI/CD para .NET em GitHub Actions, Azure
DevOps e Jenkins, com fluxo DEV/HOM/PROD (aprovação) e gate de cobertura.
Os repos do portfólio consomem estas workflows/templates em vez de
copiar YAML. Inclui um validador (`PipelineGate`) que checa as convenções.

## Instalação

Pré-requisitos: .NET SDK 10 e Docker (para o validador). Uso como
consumidor não exige instalar nada — só referenciar as workflows.

```bash
git clone https://github.com/TiagoR85/ci-pipeline-templates.git
cd ci-pipeline-templates
```

## Primeiro uso

Validar todos os templates (mesmo gate que o CI roda):

```bash
docker compose up --build --exit-code-from validate
# esperado: OK: all pipeline templates passed validation.
```

Consumir em um workflow GitHub Actions:

```yaml
jobs:
  tests:
    uses: TiagoR85/ci-pipeline-templates/.github/workflows/reusable-dotnet-ci.yml@v1
    with:
      test-project: tests/MyService.Tests/MyService.Tests.csproj
```

Consumir no Azure DevOps (`azure-pipelines.yml`):

```yaml
stages:
  - template: azure-devops/templates/deploy-stage.yml
    parameters:
      environment: PROD
      dependsOn: Deploy_HOM
```

Setup ADO (uma vez): criar environments `DEV`/`HOM`/`PROD` e adicionar
check de **Approval** em `PROD` (e `HOM` se quiser segundo gate).

## Comandos

| Comando | Para que serve | Estado |
|---|---|---|
| `docker compose up --build --exit-code-from validate` | Validar todos os templates (dogfooding) | ✅ |
| `dotnet build` | Compilar `PipelineGate` | ✅ |
| `dotnet test` | Rodar testes xunit do validador | ✅ |

## Configuração (.env)

Copie `.env.example` para `.env` se quiser simular as variáveis de
deploy. Nenhum segredo é necessário para rodar testes ou o validador;
valores nunca vão para o repositório.

| Variável | Obrigatória | Observação |
|---|---|---|
| `SERVICE_URL_HOM` | Não | Consumida pelos pipelines de deploy (ADO variable group / Jenkins credentials) |
| `SERVICE_URL_PROD` | Não | Idem |

## Troubleshooting

| Sintoma | Causa | Correção |
|---|---|---|
| `validate` falha com violação de convenção | Template editado fora do padrão (input renomeado, permissão removida) | Corrigir o template conforme a mensagem do `PipelineGate`; atualizar testes do validador se a convenção mudou |
| Cobertura abaixo do gate | Módulo medido com cobertura < 80% | Excluir sub-projetos não medidos via `Exclude` do coverlet ou aumentar cobertura |
| Stage ADO não pede aprovação | Check de Approval não configurado no Environment | Criar o check em `PROD` (e `HOM`) no portal do ADO |
| Workflow não encontrada (`uses`) | Ref/tag errada | Conferir `@v1` e o nome exato do workflow em `.github/workflows/` |
