// Src/Core/MSO/Entities/MsoAtendAcu.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_atend_acu")]
public class MsoAtendAcu
{
    [Key]
    [Column("setor")]
    [StringLength(20)]
    [Required]
    public string Setor { get; set; } = string.Empty;

    [Column("t01")]
    public int? T01 { get; set; }

    [Column("t02")]
    public int? T02 { get; set; }

    [Column("t03")]
    public int? T03 { get; set; }

    [Column("t04")]
    public int? T04 { get; set; }

    [Column("t05")]
    public int? T05 { get; set; }

    [Column("t06")]
    public int? T06 { get; set; }

    [Column("t07")]
    public int? T07 { get; set; }

    [Column("t08")]
    public int? T08 { get; set; }

    [Column("t09")]
    public int? T09 { get; set; }

    [Column("t10")]
    public int? T10 { get; set; }

    [Column("t11")]
    public int? T11 { get; set; }

    [Column("t12")]
    public int? T12 { get; set; }

    [Column("t13")]
    public int? T13 { get; set; }

    [Column("t14")]
    public int? T14 { get; set; }

    [Column("ano")]
    public int Ano { get; set; } = DateTime.Now.Year;

    [Column("mes")]
    public int? Mes { get; set; }

    [Column("total_atendimentos")]
    public int? TotalAtendimentos { get; set; }

    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Column("data_atualizacao")]
    public DateTime? DataAtualizacao { get; set; }

    [Column("usuario_criacao")]
    [StringLength(50)]
    public string? UsuarioCriacao { get; set; }

    [Column("usuario_atualizacao")]
    [StringLength(50)]
    public string? UsuarioAtualizacao { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    // Propriedades calculadas
    [NotMapped]
    public int TotalTrimestre1 => (T01 ?? 0) + (T02 ?? 0) + (T03 ?? 0);

    [NotMapped]
    public int TotalTrimestre2 => (T04 ?? 0) + (T05 ?? 0) + (T06 ?? 0);

    [NotMapped]
    public int TotalTrimestre3 => (T07 ?? 0) + (T08 ?? 0) + (T09 ?? 0);

    [NotMapped]
    public int TotalTrimestre4 => (T10 ?? 0) + (T11 ?? 0) + (T12 ?? 0);

    [NotMapped]
    public int TotalSemestre1 => TotalTrimestre1 + TotalTrimestre2;

    [NotMapped]
    public int TotalSemestre2 => TotalTrimestre3 + TotalTrimestre4;

    [NotMapped]
    public int TotalAnual => TotalSemestre1 + TotalSemestre2 + (T13 ?? 0) + (T14 ?? 0);

    [NotMapped]
    public decimal MediaMensal => TotalAnual > 0 ? (decimal)TotalAnual / 12 : 0;

    [NotMapped]
    public decimal MediaTrimestral => TotalAnual > 0 ? (decimal)TotalAnual / 4 : 0;

    [NotMapped]
    public string PeriodoDescricao => Mes.HasValue ? $"{GetNomeMes(Mes.Value)}/{Ano}" : Ano.ToString();

    [NotMapped]
    public int MesPico
    {
        get
        {
            var valores = new Dictionary<int, int>
            {
                { 1, T01 ?? 0 }, { 2, T02 ?? 0 }, { 3, T03 ?? 0 }, { 4, T04 ?? 0 },
                { 5, T05 ?? 0 }, { 6, T06 ?? 0 }, { 7, T07 ?? 0 }, { 8, T08 ?? 0 },
                { 9, T09 ?? 0 }, { 10, T10 ?? 0 }, { 11, T11 ?? 0 }, { 12, T12 ?? 0 }
            };

            return valores.OrderByDescending(x => x.Value).First().Key;
        }
    }

    [NotMapped]
    public int MesMenor
    {
        get
        {
            var valores = new Dictionary<int, int>
            {
                { 1, T01 ?? 0 }, { 2, T02 ?? 0 }, { 3, T03 ?? 0 }, { 4, T04 ?? 0 },
                { 5, T05 ?? 0 }, { 6, T06 ?? 0 }, { 7, T07 ?? 0 }, { 8, T08 ?? 0 },
                { 9, T09 ?? 0 }, { 10, T10 ?? 0 }, { 11, T11 ?? 0 }, { 12, T12 ?? 0 }
            };

            return valores.OrderBy(x => x.Value).First().Key;
        }
    }

    [NotMapped]
    public decimal VariacaoPercentual
    {
        get
        {
            var maior = GetValorMes(MesPico);
            var menor = GetValorMes(MesMenor);

            if (menor == 0) return 0;
            return ((decimal)(maior - menor) / menor) * 100;
        }
    }

    // Métodos utilitários
    public int GetValorMes(int mes)
    {
        return mes switch
        {
            1 => T01 ?? 0,
            2 => T02 ?? 0,
            3 => T03 ?? 0,
            4 => T04 ?? 0,
            5 => T05 ?? 0,
            6 => T06 ?? 0,
            7 => T07 ?? 0,
            8 => T08 ?? 0,
            9 => T09 ?? 0,
            10 => T10 ?? 0,
            11 => T11 ?? 0,
            12 => T12 ?? 0,
            13 => T13 ?? 0,
            14 => T14 ?? 0,
            _ => 0
        };
    }

    public void SetValorMes(int mes, int valor, string usuario)
    {
        switch (mes)
        {
            case 1: T01 = valor; break;
            case 2: T02 = valor; break;
            case 3: T03 = valor; break;
            case 4: T04 = valor; break;
            case 5: T05 = valor; break;
            case 6: T06 = valor; break;
            case 7: T07 = valor; break;
            case 8: T08 = valor; break;
            case 9: T09 = valor; break;
            case 10: T10 = valor; break;
            case 11: T11 = valor; break;
            case 12: T12 = valor; break;
            case 13: T13 = valor; break;
            case 14: T14 = valor; break;
            default: throw new ArgumentException($"Mês inválido: {mes}");
        }

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
        AtualizarTotalAtendimentos();
    }

    public void AtualizarTotalAtendimentos()
    {
        TotalAtendimentos = TotalAnual;
    }

    public void IncrementarMes(int mes, int incremento, string usuario)
    {
        var valorAtual = GetValorMes(mes);
        SetValorMes(mes, valorAtual + incremento, usuario);
    }

    public void ZerarMes(int mes, string usuario)
    {
        SetValorMes(mes, 0, usuario);
    }

    public void ZerarAno(string usuario)
    {
        for (int mes = 1; mes <= 14; mes++)
        {
            SetValorMes(mes, 0, usuario);
        }
    }

    public void CopiarDadosDeAno(MsoAtendAcu origem, string usuario)
    {
        if (origem == null) throw new ArgumentNullException(nameof(origem));

        T01 = origem.T01;
        T02 = origem.T02;
        T03 = origem.T03;
        T04 = origem.T04;
        T05 = origem.T05;
        T06 = origem.T06;
        T07 = origem.T07;
        T08 = origem.T08;
        T09 = origem.T09;
        T10 = origem.T10;
        T11 = origem.T11;
        T12 = origem.T12;
        T13 = origem.T13;
        T14 = origem.T14;

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
        AtualizarTotalAtendimentos();
    }

    // Métodos estáticos utilitários
    public static string GetNomeMes(int mes)
    {
        return mes switch
        {
            1 => "Janeiro",
            2 => "Fevereiro",
            3 => "Março",
            4 => "Abril",
            5 => "Maio",
            6 => "Junho",
            7 => "Julho",
            8 => "Agosto",
            9 => "Setembro",
            10 => "Outubro",
            11 => "Novembro",
            12 => "Dezembro",
            13 => "13º Período",
            14 => "14º Período",
            _ => "Inválido"
        };
    }

    public static string GetAbreviacaoMes(int mes)
    {
        return mes switch
        {
            1 => "JAN",
            2 => "FEV",
            3 => "MAR",
            4 => "ABR",
            5 => "MAI",
            6 => "JUN",
            7 => "JUL",
            8 => "AGO",
            9 => "SET",
            10 => "OUT",
            11 => "NOV",
            12 => "DEZ",
            13 => "13º",
            14 => "14º",
            _ => "INV"
        };
    }

    // Validações
    public bool IsAnoValido()
    {
        return Ano >= 2000 && Ano <= DateTime.Now.Year + 5;
    }

    public bool IsMesValido()
    {
        return !Mes.HasValue || (Mes >= 1 && Mes <= 14);
    }

    public bool TemDados()
    {
        return TotalAnual > 0;
    }

    public bool IsSetorValido()
    {
        return !string.IsNullOrWhiteSpace(Setor) && Setor.Length <= 20;
    }

    // Métodos de comparação
    public decimal CompararComAnoAnterior(MsoAtendAcu anoAnterior)
    {
        if (anoAnterior == null || anoAnterior.TotalAnual == 0) return 0;

        return ((decimal)(TotalAnual - anoAnterior.TotalAnual) / anoAnterior.TotalAnual) * 100;
    }

    public Dictionary<int, decimal> CompararMesesComAnoAnterior(MsoAtendAcu anoAnterior)
    {
        var resultado = new Dictionary<int, decimal>();

        if (anoAnterior == null) return resultado;

        for (int mes = 1; mes <= 12; mes++)
        {
            var valorAtual = GetValorMes(mes);
            var valorAnterior = anoAnterior.GetValorMes(mes);

            if (valorAnterior == 0)
                resultado[mes] = valorAtual > 0 ? 100 : 0;
            else
                resultado[mes] = ((decimal)(valorAtual - valorAnterior) / valorAnterior) * 100;
        }

        return resultado;
    }

    // Métodos de exportação/relatório
    public Dictionary<string, object> ToRelatorioData()
    {
        return new Dictionary<string, object>
        {
            ["Setor"] = Setor,
            ["Ano"] = Ano,
            ["Janeiro"] = T01 ?? 0,
            ["Fevereiro"] = T02 ?? 0,
            ["Março"] = T03 ?? 0,
            ["Abril"] = T04 ?? 0,
            ["Maio"] = T05 ?? 0,
            ["Junho"] = T06 ?? 0,
            ["Julho"] = T07 ?? 0,
            ["Agosto"] = T08 ?? 0,
            ["Setembro"] = T09 ?? 0,
            ["Outubro"] = T10 ?? 0,
            ["Novembro"] = T11 ?? 0,
            ["Dezembro"] = T12 ?? 0,
            ["Total_1º_Trimestre"] = TotalTrimestre1,
            ["Total_2º_Trimestre"] = TotalTrimestre2,
            ["Total_3º_Trimestre"] = TotalTrimestre3,
            ["Total_4º_Trimestre"] = TotalTrimestre4,
            ["Total_1º_Semestre"] = TotalSemestre1,
            ["Total_2º_Semestre"] = TotalSemestre2,
            ["Total_Anual"] = TotalAnual,
            ["Média_Mensal"] = MediaMensal,
            ["Mês_Pico"] = GetNomeMes(MesPico),
            ["Mês_Menor"] = GetNomeMes(MesMenor),
            ["Variação_Percentual"] = VariacaoPercentual
        };
    }

    public override string ToString()
    {
        return $"{Setor} - {Ano} (Total: {TotalAnual})";
    }
}