﻿using STrain.Core.Enumerations;
using System.Diagnostics.CodeAnalysis;

namespace STrain.CQS.NetCore.ErrorHandling
{
    [ExcludeFromCodeCoverage]
    public class ErrorEnumeration : Enumeration
    {
        public string Type { get; }
        public string Title { get; }
        public string Detail { get; }

        public static ErrorEnumeration Validation => new(400, "Invalid", "/errors/invalid-request", "Invalid request.", "Invalid request. See the errors.");
        public static ErrorEnumeration Unathorized => new(401, "Unathorized", "/errors/unathorized", "Unathorized request.", "Authentication is required for access '{0}' endpoint.");
        public static ErrorEnumeration Forbidden => new(403, "Forbidden", "/errors/forbidden", "Forbidden.", "Specific permission is required for access '{0}' endpoint.");
        public static ErrorEnumeration InternalServerError => new(500, "Internal Server Error", "/errors/internal-server-error", "Internal server error.", "Unexpected error happened. Please, call the support.");

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
    }
}
