using System.ComponentModel.DataAnnotations;

namespace CRM_Alunos.Web.Models;

public class Turma
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public string? Periodo { get; set; }

    public string? Horario { get; set; }

    public string? Sala { get; set; }

    public string? Status { get; set; } = "Ativa";

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public int AnoLetivo { get; set; }

    public string? Serie { get; set; }

    public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
}
