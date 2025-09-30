// src/Application/SEG/Interfaces/IFuncaoService.cs
using RhSensoERP.Application.SEG.DTOs;

namespace RhSensoERP.Application.SEG.Interfaces;

/// <summary>
/// Interface para serviços de negócio de Função
/// </summary>
public interface IFuncaoService
{
    Task<IEnumerable<FuncaoDTO>> GetAllAsync();
    Task<FuncaoDTO?> GetByCompositeKeyAsync(string cdFuncao, string cdSistema);
    Task<IEnumerable<FuncaoDTO>> GetBySistemaAsync(string cdSistema);
    Task<FuncaoDTO> CreateAsync(CreateFuncaoDTO createDto);
    Task<FuncaoDTO> UpdateAsync(string cdFuncao, string cdSistema, UpdateFuncaoDTO updateDto);
    Task<bool> DeleteAsync(string cdFuncao, string cdSistema);
}