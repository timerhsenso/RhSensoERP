// src/Infrastructure/Services/SEG/FuncaoService.cs
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.Infrastructure.Services.SEG;

/// <summary>
/// Implementação dos serviços de negócio para Função
/// </summary>
public class FuncaoService : IFuncaoService
{
    private readonly AppDbContext _context;

    public FuncaoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FuncaoDTO>> GetAllAsync()
    {
        return await _context.Funcoes
            .Include(f => f.Sistema)
            .OrderBy(f => f.CdSistema)
            .ThenBy(f => f.CdFuncao)
            .Select(f => new FuncaoDTO
            {
                CdFuncao = f.CdFuncao,
                CdSistema = f.CdSistema,
                DcFuncao = f.DcFuncao,
                DcModulo = f.DcModulo,
                DescricaoModulo = f.DescricaoModulo,
                NomeSistema = f.Sistema.NmSistema
            })
            .ToListAsync();
    }

    public async Task<FuncaoDTO?> GetByCompositeKeyAsync(string cdFuncao, string cdSistema)
    {
        var funcao = await _context.Funcoes
            .Include(f => f.Sistema)
            .FirstOrDefaultAsync(f => f.CdFuncao == cdFuncao && f.CdSistema == cdSistema);

        if (funcao == null) return null;

        return new FuncaoDTO
        {
            CdFuncao = funcao.CdFuncao,
            CdSistema = funcao.CdSistema,
            DcFuncao = funcao.DcFuncao,
            DcModulo = funcao.DcModulo,
            DescricaoModulo = funcao.DescricaoModulo,
            NomeSistema = funcao.Sistema?.NmSistema
        };
    }

    public async Task<IEnumerable<FuncaoDTO>> GetBySistemaAsync(string cdSistema)
    {
        return await _context.Funcoes
            .Include(f => f.Sistema)
            .Where(f => f.CdSistema == cdSistema)
            .OrderBy(f => f.CdFuncao)
            .Select(f => new FuncaoDTO
            {
                CdFuncao = f.CdFuncao,
                CdSistema = f.CdSistema,
                DcFuncao = f.DcFuncao,
                DcModulo = f.DcModulo,
                DescricaoModulo = f.DescricaoModulo,
                NomeSistema = f.Sistema.NmSistema
            })
            .ToListAsync();
    }

    public async Task<FuncaoDTO> CreateAsync(CreateFuncaoDTO createDto)
    {
        // Validar se já existe
        var exists = await _context.Funcoes
            .AnyAsync(f => f.CdFuncao == createDto.CdFuncao && f.CdSistema == createDto.CdSistema);

        if (exists)
            throw new InvalidOperationException($"Função '{createDto.CdFuncao}' já existe no sistema '{createDto.CdSistema}'");

        // Validar se sistema existe
        var sistemaExists = await _context.Sistemas
            .AnyAsync(s => s.CdSistema == createDto.CdSistema);

        if (!sistemaExists)
            throw new InvalidOperationException($"Sistema '{createDto.CdSistema}' não encontrado");

        var funcao = new Funcao
        {
            CdFuncao = createDto.CdFuncao.Trim(),
            CdSistema = createDto.CdSistema.Trim(),
            DcFuncao = createDto.DcFuncao?.Trim(),
            DcModulo = createDto.DcModulo?.Trim(),
            DescricaoModulo = createDto.DescricaoModulo?.Trim()
        };

        _context.Funcoes.Add(funcao);
        await _context.SaveChangesAsync();

        return await GetByCompositeKeyAsync(funcao.CdFuncao, funcao.CdSistema)
            ?? throw new InvalidOperationException("Erro ao recuperar função criada");
    }

    public async Task<FuncaoDTO> UpdateAsync(string cdFuncao, string cdSistema, UpdateFuncaoDTO updateDto)
    {
        var funcao = await _context.Funcoes
            .FirstOrDefaultAsync(f => f.CdFuncao == cdFuncao && f.CdSistema == cdSistema);

        if (funcao == null)
            throw new KeyNotFoundException($"Função '{cdFuncao}' no sistema '{cdSistema}' não encontrada");

        // Atualizar campos permitidos
        funcao.DcFuncao = updateDto.DcFuncao?.Trim();
        funcao.DcModulo = updateDto.DcModulo?.Trim();
        funcao.DescricaoModulo = updateDto.DescricaoModulo?.Trim();

        await _context.SaveChangesAsync();

        return await GetByCompositeKeyAsync(funcao.CdFuncao, funcao.CdSistema)
            ?? throw new InvalidOperationException("Erro ao recuperar função atualizada");
    }

    public async Task<bool> DeleteAsync(string cdFuncao, string cdSistema)
    {
        var funcao = await _context.Funcoes
            .FirstOrDefaultAsync(f => f.CdFuncao == cdFuncao && f.CdSistema == cdSistema);

        if (funcao == null) return false;

        // Validar se há dependências (corrigido para BotoesFuncao sem 's')
        var temBotoes = await _context.BotoesFuncao
            .AnyAsync(b => b.CdFuncao == cdFuncao && b.CdSistema == cdSistema);

        var temGrupos = await _context.GruposFuncoes
            .AnyAsync(g => g.CdFuncao == cdFuncao && g.CdSistema == cdSistema);

        if (temBotoes || temGrupos)
            throw new InvalidOperationException("Não é possível excluir função com dependências (botões ou grupos vinculados)");

        _context.Funcoes.Remove(funcao);
        await _context.SaveChangesAsync();

        return true;
    }
}