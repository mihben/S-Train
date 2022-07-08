namespace STrain.CQS.Test.Unit.Support
{
    public class TestQuery : Query<object>
    {
        public TestQuery(Guid? requestId = null)
            : base(requestId)
        {

        }
    }
}
