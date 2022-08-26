using Microsoft.AspNetCore.Mvc.Formatters;
using STrain.Core.Enumerations;

namespace STrain.CQS.NetCore.ErrorHandling
{
    public class ErrorEnumeration : Enumeration
    {
        public string Type { get; }
        public string Title { get; }
        public string Detail { get; }

        public ErrorEnumeration(int id, string name, string type, string title, string detail)
            : base(id, name)
        {
            Type = type;
            Title = title;
            Detail = detail;
        }

        public class NotFound : ErrorEnumeration
        {
            public static NotFound Request => new(0, "Resource", "/errors/resource-not-found", string.Empty, string.Empty);
            public static NotFound Endpoint => new(1, "Endpoint", "/errors/endpoint-not-found", "Endpoint not found.", "Endpoint '{0}' was not found.");

            public NotFound(int id, string name, string type, string title, string detail)
                : base(id, name, type, title, detail)
            {
            }
        }

        public class Authentication : ErrorEnumeration
        {
            public static Authentication Unathorized => new(0, "Unathorized", "/errors/unathorized", "Unathorized request.", "Authentication is required for access '{0}' endpoint.");

            public Authentication(int id, string name, string type, string title, string detail) : base(id, name, type, title, detail)
            {
            }
        }
    }
}
