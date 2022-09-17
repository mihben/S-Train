using STrain.CQS.Http.RequestSending.Providers.Generic;

namespace STrain.CQS.Test.Unit.NetCore.RequestSending
{
    public class GenericHeaderParameterBinderTest
    {
        private GenericHeaderParameterBinder CreateSUT()
        {
            return new GenericHeaderParameterBinder();
        }
    }
}
