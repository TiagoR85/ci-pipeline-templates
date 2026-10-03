using PipelineGate;
using PipelineGate.Validators;
using Xunit;

namespace PipelineGate.Tests;

public class ValidatorRunnerTests : IDisposable
{
    private readonly string _root =
        Path.Combine(Path.GetTempPath(), "pgate-" + Guid.NewGuid());

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    private void Write(string relative, string content)
    {
        var full = Path.Combine(_root, relative.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        File.WriteAllText(full, content);
    }

    private static string ValidWorkflow => """
        name: w
        on:
          workflow_call:
        permissions:
          contents: read
        jobs: {}
        """;

    private static string ValidSample => """
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
          - template: templates/deploy-stage.yml
            parameters:
              environment: PROD
        """;

    private static string ValidDeploy => """
        stages:
          - stage: Deploy
            jobs:
              - deployment: d
                environment: ${{ parameters.environment }}
                strategy:
                  runOnce:
                    deploy:
                      steps: []
        """;

    private static string ValidTestTemplate => """
        parameters:
          - name: coverageThreshold
            type: string
        steps:
          - script: dotnet test -p:Threshold=${{ parameters.coverageThreshold }}
        """;

    private static string ValidJenkinsfile => """
        @Library('ci-pipeline-templates@main') _
        pipeline {
            agent any
            stages {
                stage('A') { steps { input 'go' } }
            }
        }
        """;

    private static string ValidGroovy => """
        def call(Map config = [:]) { echo 'hi' }
        """;

    [Fact]
    public void RepositoryWithValidTemplates_IsValid()
    {
        Write(".github/workflows/reusable-dotnet-ci.yml", ValidWorkflow);
        Write(".github/workflows/ci.yml", ValidWorkflow);
        Write("azure-devops/azure-pipelines.sample.yml", ValidSample);
        Write("azure-devops/templates/deploy-stage.yml", ValidDeploy);
        Write("azure-devops/templates/test-with-coverage-gate.yml", ValidTestTemplate);
        Write("azure-devops/templates/build-dotnet.yml", "steps:\n  - script: dotnet build");
        Write("jenkins/Jenkinsfile.sample", ValidJenkinsfile);
        Write("jenkins/vars/dotnetPipeline.groovy", ValidGroovy);

        var report = ValidatorRunner.ValidateRepository(_root);

        Assert.True(report.IsValid, report.ToString());
    }

    [Fact]
    public void RepositoryWithBrokenFiles_ReportsFindings()
    {
        Write(".github/workflows/reusable-broken.yml", "name: x\non: push\n");
        Write("jenkins/Jenkinsfile.sample", "node { }");

        var report = ValidatorRunner.ValidateRepository(_root);

        Assert.False(report.IsValid);
        Assert.Contains(report.Findings, f => f.Rule == "GHA-002");
        Assert.Contains(report.Findings, f => f.Rule == "GHA-003");
        Assert.Contains(report.Findings, f => f.Rule == "JEN-001");
        Assert.Contains(report.Findings, f => f.Rule == "JEN-002");
        Assert.Contains(report.Findings, f => f.Rule == "JEN-004");
    }

    [Fact]
    public void EmptyDirectory_IsValid()
    {
        Directory.CreateDirectory(_root);
        var report = ValidatorRunner.ValidateRepository(_root);
        Assert.True(report.IsValid, report.ToString());
    }
}
