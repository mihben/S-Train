using STrain.CQS;

namespace STrain.Sample.Api
{
    public static class Sample
    {
        public record GenericCommand : Command
        {
            public string Value { get; }

            public GenericCommand(string value)
            {
                Value = value;
            }
        }

        public record GenericQuery : Query<string>
        {

        }

        [Get("api/v2/beers")]
        public record ExternalRequest : IRequest
        {
            [QueryParameter]
            public int Size { get; }

            public ExternalRequest(int size)
            {
                Size = size;
            }

            public record Result
            {
                public int Id { get; }
                public Guid Uid { get; }
                public string Brand { get; }
                public string Name { get; }
                public string Style { get; }
                public string Hop { get; }
                public string Yeast { get; }
                public string Malts { get; }
                public string Ibu { get; }
                public string Alcohol { get; }
                public string Blg { get; }

                public Result(int id, Guid uid, string brand, string name, string style, string hop, string yeast, string malts, string ibu, string alcohol, string blg)
                {
                    Id = id;
                    Uid = uid;
                    Brand = brand;
                    Name = name;
                    Style = style;
                    Hop = hop;
                    Yeast = yeast;
                    Malts = malts;
                    Ibu = ibu;
                    Alcohol = alcohol;
                    Blg = blg;
                }
            }
        }
    }

}
