namespace Dotnet.Homeworks.Mediator;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PipelineOrderAttribute : Attribute
{
    public int Order { get; }

    public PipelineOrderAttribute(int order)
    {
        Order = order;
    }
}
