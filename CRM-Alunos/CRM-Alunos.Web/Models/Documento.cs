using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Alunos.Web.Models;

public class Documento
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    [Required]
    public string CaminhoArquivo { get; set; } = string.Empty;

    public string? Tipo { get; set; }

    public long Tamanho { get; set; }

    public DateTime DataUpload { get; set; } = DateTime.Now;

    [ForeignKey(nameof(Aluno))]
    public int AlunoId { get; set; }

    public Aluno Aluno { get; set; } = null!;

    [ForeignKey(nameof(Pasta))]
    public int? PastaId { get; set; }

    public Pasta? Pasta { get; set; }
}
