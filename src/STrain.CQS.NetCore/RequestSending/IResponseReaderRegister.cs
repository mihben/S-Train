using STrain.CQS.NetCore.RequestSending.Parsers;

namespace STrain.CQS.NetCore.RequestSending
{
    public interface IResponseReaderRegister
    {
        IResponseReaderRegister Registrate<TResponseReader>(string mediaType) where TResponseReader : IResponseReader;
    }
}
