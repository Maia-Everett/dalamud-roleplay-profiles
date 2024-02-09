using System.Net;

namespace RoleplayProfiles.Api;

public class ApiErrorResponse
{
    public HttpStatusCode StatusCode { get; set; } = 0;
    public string Message { get; set; } = "";
    public string Error { get; set; } = "";
}
