namespace RhSensoERP.Application.SEG.DTOs
{
    public sealed class BotaoFuncaoDto
    {
        public string CdSistema { get; set; } = string.Empty; // PK (parte)
        public string CdFuncao { get; set; } = string.Empty; // PK (parte)
        public string NmBotao { get; set; } = string.Empty; // PK (parte)
        public string DcBotao { get; set; } = string.Empty; // varchar(60)
        public char CdAcao { get; set; }                 // char(1): I/A/E/C
    }

    // Para exclusão em lote (PK composta)
    public sealed class BotaoFuncaoKeyDto
    {
        public string CdSistema { get; set; } = string.Empty;
        public string CdFuncao { get; set; } = string.Empty;
        public string NmBotao { get; set; } = string.Empty;
    }

    // Parâmetros de consulta
    public sealed class BotaoFuncaoQuery
    {
        public string? CdSistema { get; set; }   // filtra por sistema
        public string? CdFuncao { get; set; }   // filtra por função
        public string? Search { get; set; }   // procura em NmBotao/DcBotao
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // Resultado paginado
    public sealed class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public IReadOnlyList<T> Items { get; set; } = [];
    }
}
