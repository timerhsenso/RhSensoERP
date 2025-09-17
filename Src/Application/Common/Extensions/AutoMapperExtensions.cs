using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Application.Common.Mappings;

namespace RhSensoERP.Application.Common.Extensions;

/// <summary>
/// Extensões para configuração do AutoMapper
/// </summary>
public static class AutoMapperExtensions
{
    /// <summary>
    /// Adiciona e configura o AutoMapper com todos os perfis de mapeamento
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <returns>Coleção de serviços para encadeamento</returns>
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserMappingProfile));
        return services;
    }
}