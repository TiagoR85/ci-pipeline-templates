using PipelineGate;
using PipelineGate.Validators;
using Xunit;

namespace PipelineGate.Tests;

public class AzureDevOpsValidatorTests
{
    private const string ValidSample = """
        trigger:
          - main
        stages:
          - stage: Build
            jobs:
              - job: BuildTest
                steps:
                  - template: templates/test-with-coverage-gate.yml
                    parameters:
                      testProject: tests/Tests.csproj
          - template: templates/deploy-stage.yml
            parameters:
              environment: DEV
          - template: templates/deploy-stage.yml
            parameters:
              environment: HOM
              dependsOn: Deploy_DEV
          - template: templates/deploy-stage.yml
            parameters:
              environment: PROD
              dependsOn: Deploy_HOM
        """;

    private const string SampleMissingProd = """
        stages:
          - stage: Build
            jobs:
              - job: j
                steps:
                  - script: echo hi
          - template: templates/deploy-stage.yml
            parameters:
              environment: DEV
          - template: templates/deploy-stage.yml
            parameters:
              environment: HOM
        """;

    private const string ValidDeployStage = """
        parameters:
          - name: environment
            type: string
        stages:
          - stage: Deploy_${{ parameters.environment }}
            jobs:
              - deployment: deploy
                environment: ${{ parameters.environment }}
                strategy:
                  runOnce:
                    deploy:
                      steps:
                        - checkout: self
        """;

    private const string DeployWithoutEnvironment = """
        stages:
          - stage: Deploy
            jobs:
              - job: deploy
                steps:
                  - script: echo hi
        """;

    private const string ValidTestTemplate = """
        parameters:
          - name: coverageThreshold
            type: string
            default: '80'
        steps:
          - script: dotnet test t.csproj -p:Threshold=${{ parameters.coverageThreshold }}
            displayName: Test with coverage gate
        """;

    private const string TestTemplateNoGate = """
        steps:
          - script: dotnet test t.csproj
            displayName: Test
        """;

    private const string InvalidYaml = "stages: [oops";

    [Fact]
    public void ValidFiles_ProduceNoFindings()
    {
        var sample = new ValidationReport();
        AzureDevOpsValidator.Validate("s.yml",
            "azure-devops/azure-pipelines.sample.yml", ValidSample, sample);
        Assert.True(sample.IsValid, sample.ToString());

        var deploy = new ValidationReport();
        AzureDevOpsValidator.Validate("d.yml",
            "azure-devops/templates/deploy-stage.yml", ValidDeployStage, deploy);
        Assert.True(deploy.IsValid, deploy.ToString());

        var test = new ValidationReport();
        AzureDevOpsValidator.Validate("t.yml",
            "azure-devops/templates/test-with-coverage-gate.yml",
            ValidTestTemplate, test);
        Assert.True(test.IsValid, test.ToString());

        var build = new ValidationReport();
        AzureDevOpsValidator.Validate("b.yml",
            "azure-devops/templates/build-dotnet.yml",
            "steps:\n  - script: dotnet build", build);
        Assert.True(build.IsValid, build.ToString());
    }

    [Fact]
    public void SampleMissingProd_ReportsAZD002()
    {
        var report = new ValidationReport();
        AzureDevOpsValidator.Validate("s.yml",
            "azure-devops/azure-pipelines.sample.yml", SampleMissingProd, report);
        Assert.Contains(report.Findings, f => f.Rule == "AZD-002");
    }

    [Fact]
    public void DeployWithoutEnvironment_ReportsAZD003()
    {
        var report = new ValidationReport();
        AzureDevOpsValidator.Validate("d.yml",
            "azure-devops/templates/deploy-stage.yml",
            DeployWithoutEnvironment, report);
        Assert.Contains(report.Findings, f => f.Rule == "AZD-003");
    }

    [Fact]
    public void TestTemplateWithoutGate_ReportsAZD004()
    {
        var report = new ValidationReport();
        AzureDevOpsValidator.Validate("t.yml",
            "azure-devops/templates/test-with-coverage-gate.yml",
            TestTemplateNoGate, report);
        Assert.Contains(report.Findings, f => f.Rule == "AZD-004");
    }

    [Fact]
    public void InvalidYaml_ReportsAZD001()
    {
        var report = new ValidationReport();
        AzureDevOpsValidator.Validate("x.yml",
            "azure-devops/azure-pipelines.sample.yml", InvalidYaml, report);
        Assert.Contains(report.Findings, f => f.Rule == "AZD-001");
    }
}
