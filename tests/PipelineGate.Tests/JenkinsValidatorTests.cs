using PipelineGate;
using PipelineGate.Validators;
using Xunit;

namespace PipelineGate.Tests;

public class JenkinsValidatorTests
{
    private const string ValidJenkinsfile = """
        @Library('ci-pipeline-templates@main') _

        pipeline {
            agent any
            stages {
                stage('Build') {
                    steps { sh 'dotnet build s.slnx' }
                }
                stage('Approve PROD') {
                    steps { input message: 'Deploy to PROD?', ok: 'Deploy' }
                }
            }
        }
        """;

    private const string ValidLibraryScript = """
        def call(Map config = [:]) {
            echo "build ${config.solution}"
        }
        """;

    [Fact]
    public void ValidJenkinsfile_ProducesNoFindings()
    {
        var report = new ValidationReport();
        JenkinsValidator.Validate(
            "jenkins/Jenkinsfile.sample", "Jenkinsfile.sample",
            ValidJenkinsfile, report);
        Assert.True(report.IsValid, report.ToString());
    }

    [Fact]
    public void ValidLibraryScript_ProducesNoFindings()
    {
        var report = new ValidationReport();
        JenkinsValidator.Validate(
            "jenkins/vars/dotnetPipeline.groovy", "dotnetPipeline.groovy",
            ValidLibraryScript, report);
        Assert.True(report.IsValid, report.ToString());
    }

    [Fact]
    public void ScriptedPipeline_ReportsJEN001()
    {
        var report = new ValidationReport();
        JenkinsValidator.Validate("j", "Jenkinsfile.sample",
            "@Library('x') _\nnode { sh 'echo hi' }", report);
        Assert.Contains(report.Findings, f => f.Rule == "JEN-001");
    }

    [Fact]
    public void MissingInput_ReportsJEN002()
    {
        var report = new ValidationReport();
        JenkinsValidator.Validate("j", "Jenkinsfile.sample",
            "@Library('x') _\npipeline { agent any }", report);
        Assert.Contains(report.Findings, f => f.Rule == "JEN-002");
    }

    [Fact]
    public void MissingLibrary_ReportsJEN004()
    {
        var report = new ValidationReport();
        JenkinsValidator.Validate("j", "Jenkinsfile.sample",
            "pipeline { agent any stages { stage('A') { steps { input 'go' } } } }",
            report);
        Assert.Contains(report.Findings, f => f.Rule == "JEN-004");
    }

    [Fact]
    public void LibraryWithoutCall_ReportsJEN003()
    {
        var report = new ValidationReport();
        JenkinsValidator.Validate("g", "dotnetPipeline.groovy",
            "def helper() { echo 'hi' }", report);
        Assert.Contains(report.Findings, f => f.Rule == "JEN-003");
    }
}
