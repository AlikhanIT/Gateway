namespace API.Common.Parsers;

public static class RouteAuthParser
{
    public static HashSet<string> ExtractAuthRequiredRoutes(IConfigurationSection routeSection)
    {
        var result = new HashSet<string>();

        foreach (var route in routeSection.GetChildren())
        {
            var metadataSection = route.GetSection("Metadata");
            var routeId = route.Key;

            if (metadataSection.Exists() &&
                metadataSection.GetValue<string>("RequireAuth")?.ToLower() == "true")
            {
                result.Add(routeId);
            }
        }

        return result;
    }
}