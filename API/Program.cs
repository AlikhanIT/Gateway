using API.Common.Parsers;
using API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Чтение конфигурации ReverseProxy
var proxySection = builder.Configuration.GetSection("ReverseProxy");
var routeSection = proxySection.GetSection("Routes");

var requireAuthRoutes = RouteAuthParser.ExtractAuthRequiredRoutes(routeSection);

builder.Services.AddSingleton(requireAuthRoutes);
builder.Services.AddReverseProxy()
    .LoadFromConfig(proxySection);

var app = builder.Build();

// Middleware
var accessMap = builder.Configuration
    .GetSection("ReverseProxy:Routes")
    .GetChildren()
    .ToDictionary(
        route => route.Key,
        route =>
        {
            var meta = route.GetSection("Metadata");

            return new RouteAccessMetadata
            {
                RequireAuth = meta.GetValue<bool>("RequireAuth"),
                Scopes = meta.GetSection("Scopes").Get<List<string>>() ?? [],
                Roles = meta.GetSection("Roles").Get<List<string>>() ?? []
            };
        });

// 👇 передаём вручную в middleware
app.UseMiddleware<RequireAuthMiddleware>(accessMap);

app.MapReverseProxy();
app.Run();