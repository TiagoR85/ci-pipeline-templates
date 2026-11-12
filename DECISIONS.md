# Decisões (ADR)

Os ADRs completos vivem em `docs/adr/` (0001–0004, datados 2026-10-02);
abaixo o índice com a essência de cada um.

## #01 — Reusable workflows em vez de composite/marketplace actions

**Data:** 2026-10-02 · **Status:** aceita

**Decisão:** expor CI como workflows reutilizáveis (`uses: ...@v1`) do que
composite actions ou actions de marketplace.

**Contexto:** um único "receituário" versionado consumido por todo o portfólio.

**Alternativas:** composite actions; actions de terceiros.

**Consequência:** repos consumidores dependem da versão fixada aqui; breaking
change aqui quebra o portfólio — exige disciplina de tag.

## #02 — Aprovações via Azure DevOps Environments, não YAML

**Data:** 2026-10-02 · **Status:** aceita

**Decisão:** o check de aprovação dos stages DEV/HOM/PROD vive no
Environment do ADO, não codificado em YAML.

**Contexto:** o pipeline só referencia o nome do environment; a aprovação é
configurada uma vez no ADO.

**Alternativas:** aprovação lógica em YAML (frágil, duplicada por repo).

**Consequência:** setup inicial exige criar DEV/HOM/PROD no ADO (documentado no README).

## #03 — Jenkins como shared library

**Data:** 2026-10-02 · **Status:** aceita

**Decisão:** um `dotnetPipeline.groovy` na biblioteca compartilhada em vez
de Jenkinsfiles copiados por repo.

**Contexto:** Jenkinsfiles copiados sofrem drift entre repos.

**Alternativas:** Jenkinsfile por repo; plugin dedicado.

**Consequência:** consumidores usam `@Library(...)` e o sample de Jenkinsfile.

## #04 — Dogfooding: o pipeline valida a si mesmo

**Data:** 2026-10-02 · **Status:** aceita

**Decisão:** o CI deste repo roda o `PipelineGate` contra os próprios
templates (Docker Compose one-shot com `--exit-code-from validate`).

**Contexto:** templates apodrecem em silêncio (input renomeado, permissão
removida); o validador pega na mudança.

**Alternativas:** revisão manual; validar só nos consumers.

**Consequência:** qualquer mudança de template exige atualizar o validador
e os testes xunit correspondentes no mesmo commit.
