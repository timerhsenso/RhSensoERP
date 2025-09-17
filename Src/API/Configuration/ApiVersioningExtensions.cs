namespace RhSensoERP.API.Configuration;

public static class ApiVersioningExtensions
{
    public static IEndpointConventionBuilder MapV1(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
        => endpoints.Map(pattern, handler).WithGroupName("v1");
}
