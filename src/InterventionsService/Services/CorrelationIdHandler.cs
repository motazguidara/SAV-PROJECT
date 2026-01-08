using InterventionsService.Middleware;

namespace InterventionsService.Services;

public class CorrelationIdHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CorrelationIdHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = _contextAccessor.HttpContext?.Request.Headers[CorrelationIdMiddleware.HeaderName].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, correlationId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
