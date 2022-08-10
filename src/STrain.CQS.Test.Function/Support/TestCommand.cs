namespace STrain.CQS.Test.Function.Support
{
    public class TestCommand : Command
    {
        public TestCommand(Guid? requestId = null)
            : base(requestId)
        {

        }
    }
}
