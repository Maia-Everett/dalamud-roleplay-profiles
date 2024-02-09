using System;
using System.Net;

namespace RoleplayProfiles.Api;

public class ApiException(HttpStatusCode statusCode, string message) : ApplicationException(message)
{
    public HttpStatusCode StatusCode { get; init; } = statusCode;
}
