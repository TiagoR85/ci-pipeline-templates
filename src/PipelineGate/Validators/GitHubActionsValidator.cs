using YamlDotNet.RepresentationModel;

namespace PipelineGate.Validators;

public static class GitHubActionsValidator
{
    public static void Validate(
        string path, string fileName, string content, ValidationReport report)
    {
        var root = Yaml.Load(content, path, report, "GHA-001");
        if (root is null)
            return;

        if (fileName.StartsWith("reusable-", StringComparison.OrdinalIgnoreCase))
        {
            // Chave `on` pode ser interpretada como bool (true) em YAML 1.1
            var on = Yaml.Child(root, "on", "true");
            var hasCall = on is YamlMappingNode onMap
                          && Yaml.Child(onMap, "workflow_call") is not null;
            if (!hasCall)
                report.Add(path, "GHA-002",
                    "reusable workflow must declare on.workflow_call");
        }

        if (Yaml.Child(root, "permissions") is null)
            report.Add(path, "GHA-003",
                "workflow must declare top-level 'permissions' (least privilege)");
    }
}
