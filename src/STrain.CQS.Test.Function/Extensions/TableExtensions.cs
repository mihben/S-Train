using System.Net;

namespace STrain.CQS.Test.Function.Extensions
{
    internal static class TableExtensions
    {
        public static TEnum GetEnum<TEnum>(this Table dataTable, string header)
            where TEnum : struct, Enum
        {
            return Enum.Parse<TEnum>(dataTable.Rows[0][header]);
        }

        public static T? GetValue<T>(this Table dataTable, string header)
            where T : class, IConvertible
        {
            if (!dataTable.Rows[0].ContainsKey(header)) return null;
            return (T)Convert.ChangeType(dataTable.Rows[0][header], typeof(T));
        }

        public static Problem AsProblem(this Table dataTable, string resource)
        {
            IEnumerable<Problem.Error>? errors = null;
            if (dataTable.Rows[0].TryGetValue("Errors.Property", out var property)) errors = new List<Problem.Error> { new Problem.Error(property, dataTable.GetValue<string>("Errors.Message")!) };

            return new Problem(dataTable.GetValue<string>("Type")!,
                dataTable.GetValue<string>("Title")!, dataTable.GetEnum<HttpStatusCode>("Status"),
                dataTable.GetValue<string>("Detail")!.Replace("{resource}", resource), dataTable.GetValue<string>("Instance")!, errors);
        }
    }

    internal record Problem
    {
        public string Type { get; }
        public string Title { get; }
        public HttpStatusCode Status { get; }
        public string Detail { get; }
        public string Instance { get; }
        public IEnumerable<Error>? Errors { get; }

        public Problem(string type, string title, HttpStatusCode status, string detail, string instance, IEnumerable<Error>? errors)
        {
            Type = type;
            Title = title;
            Status = status;
            Detail = detail;
            Instance = instance;
            Errors = errors;
        }

        public record Error
        {
            public string Property { get; }
            public string Message { get; }

            public Error(string property, string message)
            {
                Property = property;
                Message = message;
            }
        }
    }
}
