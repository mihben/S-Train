using STrain.CQS.Test.Function.Drivers;
using STrain.CQS.Test.Function.StepDefinitions;
using STrain.CQS.Test.Unit.Support;

namespace STrain.CQS.Test.Function.Support.Hooks
{
    [Binding]
    public sealed class GenericHooks
    {
        private readonly ScenarioContext _context;

        public GenericHooks(ScenarioContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 0)]
        public void RegistrateDependencies()
        {
            _context.ScenarioContainer.RegisterInstanceAs(new ExceptionContext());

            _context.ScenarioContainer.RegisterTypeAs<SampleHost, SampleHost>();
            _context.ScenarioContainer.RegisterTypeAs<SampleApiDriver, SampleApiDriver>();
        }
    }
}