namespace PipelineGate.Validators;

public static class ValidatorRunner
{
    public static ValidationReport ValidateRepository(string root)
    {
        var report = new ValidationReport();
        root = Path.GetFullPath(root);

        var workflows = Path.Combine(root, ".github", "workflows");
        if (Directory.Exists(workflows))
            foreach (var file in Directory.GetFiles(workflows, "*.yml"))
                GitHubActionsValidator.Validate(
                    file, Path.GetFileName(file), File.ReadAllText(file), report);

        var ado = Path.Combine(root, "azure-devops");
        if (Directory.Exists(ado))
            foreach (var file in Directory.GetFiles(ado, "*.yml",
                         SearchOption.AllDirectories))
                AzureDevOpsValidator.Validate(
                    file, Path.GetRelativePath(root, file),
                    File.ReadAllText(file), report);

        var jenkins = Path.Combine(root, "jenkins");
        if (Directory.Exists(jenkins))
        {
            foreach (var file in Directory.GetFiles(jenkins, "Jenkinsfile*"))
                JenkinsValidator.Validate(
                    file, Path.GetFileName(file), File.ReadAllText(file), report);

            var vars = Path.Combine(jenkins, "vars");
            if (Directory.Exists(vars))
                foreach (var file in Directory.GetFiles(vars, "*.groovy"))
                    JenkinsValidator.Validate(
                        file, Path.GetFileName(file), File.ReadAllText(file), report);
        }

        return report;
    }
}
