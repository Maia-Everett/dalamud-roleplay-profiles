using System;
using System.Net;

public class ApiException : ApplicationException
{
    public HttpStatusCode StatusCode { get; init; }

    public ApiException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
