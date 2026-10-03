using System.IO;
using YamlDotNet.RepresentationModel;

namespace PipelineGate;

internal static class Yaml
{
    public static YamlMappingNode? Load(
        string content, string path, ValidationReport report, string rule)
    {
        try
        {
            var stream = new YamlStream();
            stream.Load(new StringReader(content));
            if (stream.Documents.Count == 0)
            {
                report.Add(path, rule, "document is empty");
                return null;
            }
            if (stream.Documents[0].RootNode is not YamlMappingNode mapping)
            {
                report.Add(path, rule, "root node must be a mapping");
                return null;
            }
            return mapping;
        }
        catch (Exception ex)
        {
            report.Add(path, rule,
                $"invalid YAML: {ex.Message.Split('\n')[0]}");
            return null;
        }
    }

    public static YamlNode? Child(YamlMappingNode node, params string[] keys)
    {
        foreach (var child in node.Children)
        {
            if (child.Key is YamlScalarNode { Value: { } key }
                && keys.Contains(key, StringComparer.OrdinalIgnoreCase))
                return child.Value;
        }
        return null;
    }

    public static bool HasKeyDeep(YamlNode node, string key)
    {
        if (node is YamlMappingNode map)
        {
            foreach (var child in map.Children)
            {
                if (child.Key is YamlScalarNode { Value: { } k }
                    && string.Equals(k, key, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (HasKeyDeep(child.Value, key))
                    return true;
            }
        }
        else if (node is YamlSequenceNode seq)
        {
            foreach (var item in seq)
                if (HasKeyDeep(item, key))
                    return true;
        }
        return false;
    }
}
