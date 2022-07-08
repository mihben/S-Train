namespace STrain.CQS.Test.Unit.Support
{
    public class TestCommand : Command
    {
        public TestCommand(Guid? requestId = null)
            : base(requestId)
        {

        }
    }
}
