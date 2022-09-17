using STrain.CQS.Http.RequestSending.Readers;

namespace STrain.CQS.Http.RequestSending
{
    public interface IResponseReaderRegister
    {
        IResponseReaderRegister Registrate<TResponseReader>(string mediaType) where TResponseReader : IResponseReader;
    }
}
