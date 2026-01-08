using Gateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5005").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseCors("frontend");
app.MapReverseProxy();
app.Run();
