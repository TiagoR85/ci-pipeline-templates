using PipelineGate;
using PipelineGate.Validators;
using Xunit;

namespace PipelineGate.Tests;

public class GitHubActionsValidatorTests
{
    private const string ValidReusable = """
        name: reusable-dotnet-ci
        on:
          workflow_call:
            inputs:
              test-project:
                required: true
                type: string
        permissions:
          contents: read
        jobs:
          test:
            runs-on: ubuntu-latest
            steps:
              - uses: actions/checkout@v7
        """;

    private const string ReusableWithoutCall = """
        name: broken
        on:
          push:
            branches: [main]
        permissions:
          contents: read
        """;

    private const string NoPermissions = """
        name: naive
        on:
          workflow_call:
        """;

    private const string InvalidYaml = "on: [unclosed";

    [Fact]
    public void ValidReusableWorkflow_ProducesNoFindings()
    {
        var report = new ValidationReport();
        GitHubActionsValidator.Validate(
            "w.yml", "reusable-dotnet-ci.yml", ValidReusable, report);
        Assert.True(report.IsValid, report.ToString());
    }

    [Fact]
    public void ReusableWithoutWorkflowCall_ReportsGHA002()
    {
        var report = new ValidationReport();
        GitHubActionsValidator.Validate(
            "w.yml", "reusable-broken.yml", ReusableWithoutCall, report);
        Assert.Contains(report.Findings, f => f.Rule == "GHA-002");
    }

    [Fact]
    public void MissingPermissions_ReportsGHA003()
    {
        var report = new ValidationReport();
        GitHubActionsValidator.Validate(
            "w.yml", "reusable-x.yml", NoPermissions, report);
        Assert.Contains(report.Findings, f => f.Rule == "GHA-003");
    }

    [Fact]
    public void InvalidYaml_ReportsGHA001()
    {
        var report = new ValidationReport();
        GitHubActionsValidator.Validate(
            "w.yml", "ci.yml", InvalidYaml, report);
        Assert.Contains(report.Findings, f => f.Rule == "GHA-001");
    }

    [Fact]
    public void RegularWorkflowWithoutWorkflowCall_IsAllowed()
    {
        var report = new ValidationReport();
        GitHubActionsValidator.Validate(
            "w.yml", "ci.yml", """
                name: ci
                on:
                  push:
                    branches: [main]
                permissions:
                  contents: read
                """, report);
        Assert.True(report.IsValid, report.ToString());
    }
}
