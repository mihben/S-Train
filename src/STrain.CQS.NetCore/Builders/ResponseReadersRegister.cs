using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Parsers;
using STrain.CQS.NetCore.RequestSending.Readers;
using System.Net.Mime;

namespace STrain.CQS.NetCore.Builders
{
    public class ResponseReadersRegister
    {
        public IResponseReaderRegister Register { get; }
        public WebApplicationBuilder Builder { get; }

        public ResponseReadersRegister(IResponseReaderRegister register, WebApplicationBuilder builder)
        {
            Register = register;
            Builder = builder;
        }

        public ResponseReadersRegister UseDefaults()
        {
            UseDefaultTextResponseReader();
            UseDefaultJsonResponseReader();
            return this;
        }

        public ResponseReadersRegister UseDefaultJsonResponseReader() => UseJsonResponseReader<JsonResponseReader>();
        public ResponseReadersRegister UseJsonResponseReader<TResponseReader>()
            where TResponseReader : class, IResponseReader => UseResponseReader<TResponseReader>(MediaTypeNames.Application.Json);

        public ResponseReadersRegister UseDefaultTextResponseReader() => UseTextResponseReader<StringResponseReader>();
        public ResponseReadersRegister UseTextResponseReader<TResponseReader>()
            where TResponseReader : class, IResponseReader => UseResponseReader<TResponseReader>(MediaTypeNames.Text.Plain);

        public ResponseReadersRegister UseResponseReader<TResponseReader>(string mediaType)
            where TResponseReader : class, IResponseReader
        {
            Register.Registrate<TResponseReader>(mediaType);
            Builder.Services.AddResponseReader<TResponseReader>();
            return this;
        }
    }
}
