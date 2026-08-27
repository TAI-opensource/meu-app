using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Alunos.Web.Models;

public class Pasta
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public string? CaminhoLocal { get; set; }

    [ForeignKey(nameof(Aluno))]
    public int AlunoId { get; set; }

    public Aluno Aluno { get; set; } = null!;

    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}
