using PipelineGate.Validators;

if (args is ["validate", var root] && Directory.Exists(root))
{
    var report = ValidatorRunner.ValidateRepository(root);
    Console.WriteLine(report);
    return report.IsValid ? 0 : 1;
}

Console.Error.WriteLine("usage: pipeline-gate validate <root-dir>");
return 2;
