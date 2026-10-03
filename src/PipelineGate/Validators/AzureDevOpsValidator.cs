using YamlDotNet.RepresentationModel;

namespace PipelineGate.Validators;

public static class AzureDevOpsValidator
{
    public static void Validate(
        string path, string relativePath, string content, ValidationReport report)
    {
        var root = Yaml.Load(content, path, report, "AZD-001");
        if (root is null)
            return;

        var file = Path.GetFileName(relativePath);

        if (file.Equals("azure-pipelines.sample.yml",
                StringComparison.OrdinalIgnoreCase))
            ValidateSample(path, root, report);

        if (file.Equals("deploy-stage.yml",
                StringComparison.OrdinalIgnoreCase)
            && !Yaml.HasKeyDeep(root, "environment"))
            report.Add(path, "AZD-003",
                "deploy stage template must bind a deployment job to 'environment' (approval hook)");

        if (relativePath.Contains("templates", StringComparison.OrdinalIgnoreCase)
            && file.Contains("test", StringComparison.OrdinalIgnoreCase)
            && (!content.Contains("Threshold=")
                || !content.Contains("coverageThreshold")))
            report.Add(path, "AZD-004",
                "test template must wire the coverage gate (-p:Threshold=${{ parameters.coverageThreshold }})");
    }

    private static void ValidateSample(
        string path, YamlMappingNode root, ValidationReport report)
    {
        if (Yaml.Child(root, "stages") is not YamlSequenceNode stages)
        {
            report.Add(path, "AZD-002", "sample pipeline must declare 'stages'");
            return;
        }

        var identities = new List<string>();
        foreach (var item in stages)
        {
            if (item is not YamlMappingNode mapping)
                continue;
            if (Yaml.Child(mapping, "stage") is YamlScalarNode stage)
                identities.Add(stage.Value ?? string.Empty);
            if (Yaml.Child(mapping, "parameters") is YamlMappingNode parameters
                && Yaml.Child(parameters, "environment") is YamlScalarNode env)
                identities.Add(env.Value ?? string.Empty);
        }

        var joined = string.Join("|", identities);
        foreach (var env in new[] { "DEV", "HOM", "PROD" })
            if (!joined.Contains(env, StringComparison.OrdinalIgnoreCase))
                report.Add(path, "AZD-002",
                    $"sample pipeline is missing the {env} deploy stage");
    }
}
