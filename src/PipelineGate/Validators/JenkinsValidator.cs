using System.Text.RegularExpressions;

namespace PipelineGate.Validators;

public static partial class JenkinsValidator
{
    [GeneratedRegex(@"\binput\b")]
    private static partial Regex InputStep();

    public static void Validate(
        string path, string fileName, string content, ValidationReport report)
    {
        if (fileName.StartsWith("Jenkinsfile", StringComparison.OrdinalIgnoreCase))
        {
            if (!content.Contains("pipeline {"))
                report.Add(path, "JEN-001",
                    "Jenkinsfile must use declarative 'pipeline { }'");

            if (!InputStep().IsMatch(content))
                report.Add(path, "JEN-002",
                    "Jenkinsfile must include a manual 'input' approval step");

            if (!content.Contains("@Library("))
                report.Add(path, "JEN-004",
                    "Jenkinsfile must load the shared library via @Library(...)");
        }
        else if (fileName.EndsWith(".groovy", StringComparison.OrdinalIgnoreCase))
        {
            if (!content.Contains("call("))
                report.Add(path, "JEN-003",
                    "shared library script must expose a call(...) entry point");
        }
    }
}
