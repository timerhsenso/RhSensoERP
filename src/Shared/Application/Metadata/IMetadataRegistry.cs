// =============================================================================
// RHSENSOERP - METADATA REGISTRY INTERFACE
// =============================================================================
// Arquivo: src/Shared/Application/Metadata/IMetadataRegistry.cs
// Descrição: Interface do registry central de metadados
// =============================================================================

namespace RhSensoERP.Shared.Application.Metadata;

/// <summary>
/// Interface do registry central de metadados de entidades.
/// Responsável por armazenar e fornecer acesso aos metadados de todas as entidades.
/// </summary>
public interface IMetadataRegistry
{
    /// <summary>
    /// Busca metadados pelo nome da entidade.
    /// </summary>
    /// <param name="entityName">Nome da entidade (ex: "Banco")</param>
    /// <returns>Metadados ou null se não encontrado</returns>
    EntityMetadata? GetByName(string entityName);

    /// <summary>
    /// Busca metadados pela rota (módulo + entidade).
    /// </summary>
    /// <param name="module">Nome do módulo (ex: "gestaodepessoas")</param>
    /// <param name="entity">Nome da entidade no plural (ex: "bancos")</param>
    /// <returns>Metadados ou null se não encontrado</returns>
    EntityMetadata? GetByRoute(string module, string entity);

    /// <summary>
    /// Retorna todas as entidades de um módulo.
    /// </summary>
    /// <param name="moduleName">Nome do módulo</param>
    /// <returns>Lista de metadados do módulo</returns>
    IEnumerable<EntityMetadata> GetByModule(string moduleName);

    /// <summary>
    /// Retorna todas as entidades registradas.
    /// </summary>
    IEnumerable<EntityMetadata> GetAll();

    /// <summary>
    /// Retorna lista de módulos disponíveis.
    /// </summary>
    IEnumerable<string> GetModules();

    /// <summary>
    /// Registra metadados de uma entidade.
    /// </summary>
    /// <param name="metadata">Metadados a registrar</param>
    void Register(EntityMetadata metadata);

    /// <summary>
    /// Registra múltiplos metadados de uma vez.
    /// </summary>
    /// <param name="metadataList">Lista de metadados a registrar</param>
    void RegisterRange(IEnumerable<EntityMetadata> metadataList);

    /// <summary>
    /// Verifica se uma entidade está registrada.
    /// </summary>
    /// <param name="entityName">Nome da entidade</param>
    bool Exists(string entityName);

    /// <summary>
    /// Remove metadados de uma entidade (para testes).
    /// </summary>
    /// <param name="entityName">Nome da entidade</param>
    bool Remove(string entityName);

    /// <summary>
    /// Limpa todos os registros (para testes).
    /// </summary>
    void Clear();

    /// <summary>
    /// Retorna contagem total de entidades registradas.
    /// </summary>
    int Count { get; }
}