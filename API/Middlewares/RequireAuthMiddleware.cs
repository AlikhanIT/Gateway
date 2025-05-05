using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace API.Middlewares;

public class RequireAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Dictionary<string, RouteAccessMetadata> _routeAccessMap;

    public RequireAuthMiddleware(RequestDelegate next, Dictionary<string, RouteAccessMetadata> routeAccessMap)
    {
        _next = next;
        _routeAccessMap = routeAccessMap;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var routeId = context.GetEndpoint()?.DisplayName;

        if (routeId != null && _routeAccessMap.TryGetValue(routeId, out var meta) && meta.RequireAuth)
        {
            // Здесь можно внедрить реальную проверку токена, ролей, скопов и т.п.
            // Пока просто возвращаем 403 как демонстрацию
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Forbidden: Auth required");
            return;
        }

        await _next(context);
    }
}

public class RouteAccessMetadata
{
    public bool RequireAuth { get; set; } = false;
    public List<string> Scopes { get; set; } = new();
    public List<string> Roles { get; set; } = new();
}