using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.Infrastructure.Services.SEG;

/// <summary>
/// Implementação dos serviços de negócio para Sistema
/// </summary>
public class SistemaService : ISistemaService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SistemaService> _logger;

    public SistemaService(AppDbContext context, ILogger<SistemaService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<SistemaDto>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Buscando todos os sistemas");

        return await _context.Sistemas
            .AsNoTracking()
            .OrderBy(s => s.CdSistema)
            .Select(s => new SistemaDto
            {
                CdSistema = s.CdSistema,
                DcSistema = s.DcSistema,
                Ativo = s.Ativo
            })
            .ToListAsync(ct);
    }

    public async Task<SistemaDto?> GetByIdAsync(string cdSistema, CancellationToken ct = default)
    {
        _logger.LogDebug("Buscando sistema {CdSistema}", cdSistema);

        var sistema = await _context.Sistemas
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.CdSistema == cdSistema, ct);

        if (sistema == null)
        {
            _logger.LogWarning("Sistema {CdSistema} não encontrado", cdSistema);
            return null;
        }

        return new SistemaDto
        {
            CdSistema = sistema.CdSistema,
            DcSistema = sistema.DcSistema,
            Ativo = sistema.Ativo
        };
    }

    public async Task<SistemaDto> CreateAsync(SistemaUpsertDto dto, CancellationToken ct = default)
    {
        _logger.LogDebug("Criando sistema {CdSistema}", dto.CdSistema);

        // Validar se já existe
        var exists = await _context.Sistemas
            .AnyAsync(s => s.CdSistema == dto.CdSistema, ct);

        if (exists)
        {
            var msg = $"Já existe um sistema com código '{dto.CdSistema}'";
            _logger.LogWarning(msg);
            throw new InvalidOperationException(msg);
        }

        // Criar novo sistema
        var sistema = new Sistema
        {
            CdSistema = dto.CdSistema.Trim(),
            DcSistema = dto.DcSistema.Trim(),
            Ativo = dto.Ativo
        };

        _context.Sistemas.Add(sistema);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Sistema {CdSistema} criado com sucesso", sistema.CdSistema);

        return new SistemaDto
        {
            CdSistema = sistema.CdSistema,
            DcSistema = sistema.DcSistema,
            Ativo = sistema.Ativo
        };
    }

    public async Task<SistemaDto?> UpdateAsync(string cdSistema, SistemaUpsertDto dto, CancellationToken ct = default)
    {
        _logger.LogDebug("Atualizando sistema {CdSistema}", cdSistema);

        var sistema = await _context.Sistemas
            .FirstOrDefaultAsync(s => s.CdSistema == cdSistema, ct);

        if (sistema == null)
        {
            _logger.LogWarning("Sistema {CdSistema} não encontrado para atualização", cdSistema);
            return null;
        }

        // Atualizar campos
        sistema.DcSistema = dto.DcSistema.Trim();
        sistema.Ativo = dto.Ativo;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Sistema {CdSistema} atualizado com sucesso", cdSistema);

        return new SistemaDto
        {
            CdSistema = sistema.CdSistema,
            DcSistema = sistema.DcSistema,
            Ativo = sistema.Ativo
        };
    }

    public async Task<bool> DeleteAsync(string cdSistema, CancellationToken ct = default)
    {
        _logger.LogDebug("Excluindo sistema {CdSistema}", cdSistema);

        var sistema = await _context.Sistemas
            .FirstOrDefaultAsync(s => s.CdSistema == cdSistema, ct);

        if (sistema == null)
        {
            _logger.LogWarning("Sistema {CdSistema} não encontrado para exclusão", cdSistema);
            return false;
        }

        // Validar se há funções vinculadas
        var temFuncoes = await _context.Funcoes
            .AnyAsync(f => f.CdSistema == cdSistema, ct);

        if (temFuncoes)
        {
            var msg = "Não é possível excluir sistema com funções vinculadas";
            _logger.LogWarning("{Message} - Sistema: {CdSistema}", msg, cdSistema);
            throw new InvalidOperationException(msg);
        }

        _context.Sistemas.Remove(sistema);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Sistema {CdSistema} excluído com sucesso", cdSistema);
        return true;
    }
}