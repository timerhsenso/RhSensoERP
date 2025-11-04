// src/Application/SEG/DTOs/FuncaoDTO.cs
namespace RhSensoERP.Shared.Application.SEG.DTOs;

/// <summary>
/// DTO para leitura de Função (GET)
/// </summary>
public class FuncaoDTO
{
    public string CdFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = string.Empty;
    public string? DcFuncao { get; set; }
    public string? DcModulo { get; set; }
    public string? DescricaoModulo { get; set; }

    // Dados do sistema relacionado (opcional)
    public string? NomeSistema { get; set; }
}

/// <summary>
/// DTO para criação de Função (POST)
/// </summary>
public class CreateFuncaoDTO
{
    public string CdFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = string.Empty;
    public string? DcFuncao { get; set; }
    public string? DcModulo { get; set; }
    public string? DescricaoModulo { get; set; }
}

/// <summary>
/// DTO para atualização de Função (PUT)
/// </summary>
public class UpdateFuncaoDTO
{
    public string? DcFuncao { get; set; }
    public string? DcModulo { get; set; }
    public string? DescricaoModulo { get; set; }
}