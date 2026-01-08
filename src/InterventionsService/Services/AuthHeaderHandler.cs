namespace InterventionsService.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthHeaderHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var auth = _contextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(auth))
        {
            request.Headers.TryAddWithoutValidation("Authorization", auth);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
