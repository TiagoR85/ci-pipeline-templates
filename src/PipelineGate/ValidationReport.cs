namespace PipelineGate;

public sealed record Finding(string File, string Rule, string Message)
{
    public override string ToString() => $"{File}: [{Rule}] {Message}";
}

public sealed class ValidationReport
{
    public List<Finding> Findings { get; } = new();
    public bool IsValid => Findings.Count == 0;

    public void Add(string file, string rule, string message)
        => Findings.Add(new Finding(file, rule, message));

    public override string ToString() => IsValid
        ? "OK: all pipeline templates passed validation."
        : string.Join(Environment.NewLine, Findings);
}
