using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/octet-stream",
        "application/wasm"
    });
});

var app = builder.Build();
app.UseResponseCompression();
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if ((path.StartsWithSegments("/_framework") && path.Value?.EndsWith(".map", StringComparison.OrdinalIgnoreCase) == true)
        || path.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/.well-known/appspecific"))
    {
        context.Response.StatusCode = StatusCodes.Status204NoContent;
        return;
    }

    await next();
});
app.UseBlazorFrameworkFiles();
var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".wasm"] = "application/wasm";
contentTypeProvider.Mappings[".dll"] = "application/octet-stream";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider
});

app.MapFallbackToFile("index.html");

app.Run();
