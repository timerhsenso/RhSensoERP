// =============================================================================
// RHSENSOERP WEB - SCREEN KEY HELPER
// =============================================================================
// Arquivo: src/Web/Security/ScreenKey.cs
// Descrição: Utilitário para gerar chaves estáveis de tela
// =============================================================================

namespace RhSensoERP.Web.Security;

/// <summary>
/// Utilitário para gerar ScreenKeys estáveis.
/// A ScreenKey é usada para identificar telas de forma única e persistente.
/// </summary>
public static class ScreenKey
{
    /// <summary>
    /// Gera uma ScreenKey a partir dos dados da tela.
    /// Prioriza chave legado (CdSistema:CdFuncao) se disponível.
    /// </summary>
    /// <param name="cdSistema">Código do sistema (ex: "RHU")</param>
    /// <param name="cdFuncao">Código da função (ex: "RHU_FM_TAUX1")</param>
    /// <param name="area">Área MVC</param>
    /// <param name="controller">Nome do controller</param>
    /// <param name="action">Nome da action</param>
    /// <returns>ScreenKey no formato "CdSistema:CdFuncao" ou "Area:Controller:Action"</returns>
    public static string From(string? cdSistema, string? cdFuncao, string? area, string controller, string action)
    {
        // Prioridade 1: chave do legado (mais estável e ideal para favoritos)
        if (!string.IsNullOrWhiteSpace(cdSistema) && !string.IsNullOrWhiteSpace(cdFuncao))
            return $"{cdSistema}:{cdFuncao}";

        // Prioridade 2: rota lógica
        var a = string.IsNullOrWhiteSpace(area) ? "-" : area.Trim();
        return $"{a}:{controller}:{action}";
    }

    /// <summary>
    /// Gera uma ScreenKey completa com todos os componentes.
    /// Formato: "{CdSistema}:{CdFuncao}:{Area}:{Controller}:{Action}"
    /// </summary>
    public static string FromFull(string? cdSistema, string? cdFuncao, string? area, string controller, string action)
    {
        var sys = cdSistema ?? "-";
        var func = cdFuncao ?? controller;
        var ar = area ?? "-";

        return $"{sys}:{func}:{ar}:{controller}:{action}";
    }
}