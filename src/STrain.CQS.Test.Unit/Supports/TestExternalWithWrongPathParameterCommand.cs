namespace STrain.CQS.Test.Unit.Supports
{
    [Route("{parameter}")]
    public record TestExternalWithWrongPathParameterCommand : Command
    {
    }
}
