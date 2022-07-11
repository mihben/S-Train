using STrain.CQS.Test.Unit.StepDefinitions;

namespace STrain.CQS.Test.Unit.Support.Hooks
{
    [Binding]
    public sealed class GenericHooks
    {
        private readonly ScenarioContext _context;

        public GenericHooks(ScenarioContext context)
        {
            _context = context;
        }

        [BeforeScenario]
        public void RegistrateDependencies()
        {
            _context.ScenarioContainer.RegisterInstanceAs(new ExceptionContext());
        }
    }
}