using TechTalk.SpecFlow;

namespace STrain.CQS.Test.Unit.StepDefinitions
{
    [Binding]
    public class ExceptionStepDefinitions
    {
        private readonly ExceptionContext _context;

        public ExceptionStepDefinitions(ExceptionContext context)
        {
            _context = context;
        }

        [Then("NotImplementedException should be thrown")]
        public void ShouldThrowNotImplementedException()
        {
            Assert.IsType<NotImplementedException>(_context.Exception);
        }

        [Then("InvalidOperationException should be thrown")]
        public void ShouldThrowInvalidOperationException()
        {
            Assert.IsType<InvalidOperationException>(_context.Exception);
        }
    }

    public class ExceptionContext
    {
        public Exception? Exception { get; set; }
    }
}
