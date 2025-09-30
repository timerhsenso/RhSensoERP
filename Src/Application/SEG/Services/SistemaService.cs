using AutoMapper;
using FluentValidation;
using RhSensoERP.Application.Common.Interfaces.Repositories.SEG;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Services.Interfaces;
using RhSensoERP.Core.Security.Entities;
using System;

namespace RhSensoERP.Application.SEG.Services
{
    internal sealed class SistemaService : ISistemaService
    {
        private readonly ISistemaRepository _repo;
        private readonly IValidator<SistemaUpsertDto> _validator;
        private readonly IMapper _mapper;

        public SistemaService(
            ISistemaRepository repo,
            IValidator<SistemaUpsertDto> validator,
            IMapper mapper)
        {
            _repo = repo;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SistemaDto>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<SistemaDto>>(list);
        }

        public async Task<SistemaDto?> GetByIdAsync(string cdSistema, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(cdSistema, ct);
            return entity is null ? null : _mapper.Map<SistemaDto>(entity);
        }

        public async Task<SistemaDto> CreateAsync(SistemaUpsertDto dto, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(dto, ct);

            if (await _repo.ExistsAsync(dto.CdSistema, ct))
                throw new InvalidOperationException($"Já existe um Sistema com código '{dto.CdSistema}'.");

            var entity = _mapper.Map<Sistema>(dto);

            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<SistemaDto>(entity);
        }

        public async Task<SistemaDto?> UpdateAsync(string cdSistema, SistemaUpsertDto dto, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(dto, ct);

            var existing = await _repo.GetByIdAsync(cdSistema, ct);
            if (existing is null) return null;

            existing.DcSistema = dto.DcSistema;
            existing.Ativo = dto.Ativo;

            await _repo.UpdateAsync(existing, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<SistemaDto>(existing);
        }

        public async Task<bool> DeleteAsync(string cdSistema, CancellationToken ct = default)
        {
            try
            {
                var removed = await _repo.DeleteAsync(cdSistema, ct);
                if (!removed) return false;

                await _repo.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                // Evita dependência de Microsoft.EntityFrameworkCore no Application.
                throw new InvalidOperationException(
                    $"Não foi possível excluir o Sistema '{cdSistema}'. Verifique relacionamentos.", ex);
            }
        }
    }
}
