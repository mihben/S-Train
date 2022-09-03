using STrain.CQS.Test.Function.Drivers;

namespace STrain.CQS.Test.Function.Support.Hooks
{
    [Binding]
    public class GenericHooks
    {
        private readonly ScenarioContext _context;

        public GenericHooks(ScenarioContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = -1)]
        public void Registrate()
        {
            _context.ScenarioContainer.RegisterTypeAs<RequestContext, RequestContext>().InstancePerContext();
            _context.ScenarioContainer.RegisterTypeAs<ApiDriver, ApiDriver>().InstancePerContext();
        }
    }
}
