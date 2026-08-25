using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Alunos.Models;

public class Aluno
{
    public int Id { get; set; }

    [Required]
    public string NomeCompleto { get; set; } = string.Empty;

    public string? CPF { get; set; }

    public DateTime? DataNascimento { get; set; }

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public string? Endereco { get; set; }

    public string? Observacoes { get; set; }

    public string? FotoPath { get; set; }

    public DateTime DataMatricula { get; set; } = DateTime.Now;

    public bool Ativo { get; set; } = true;

    [ForeignKey(nameof(Turma))]
    public int TurmaId { get; set; }

    public Turma Turma { get; set; } = null!;

    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}
