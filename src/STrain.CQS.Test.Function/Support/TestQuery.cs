namespace STrain.CQS.Test.Function.Support
{
    public class TestQuery : Query<object>
    {
        public TestQuery(Guid? requestId = null)
            : base(requestId)
        {

        }
    }
}
