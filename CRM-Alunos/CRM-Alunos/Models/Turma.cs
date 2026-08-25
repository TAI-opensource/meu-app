using System.ComponentModel.DataAnnotations;

namespace CRM_Alunos.Models;

public class Turma
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public int AnoLetivo { get; set; }

    public string? Serie { get; set; }

    public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
}
