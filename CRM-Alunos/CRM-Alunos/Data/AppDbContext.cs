using CRM_Alunos.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM_Alunos.Data;

public class AppDbContext : DbContext
{
    public DbSet<Turma> Turmas { get; set; } = null!;
    public DbSet<Aluno> Alunos { get; set; } = null!;
    public DbSet<Documento> Documentos { get; set; } = null!;
    public DbSet<Pasta> Pastas { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=crm_alunos.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Turma>().HasData(
            new Turma
            {
                Id = 1,
                Nome = "Turma A",
                Descricao = "Primeira turma do ano letivo",
                DataCriacao = new DateTime(2024, 1, 15),
                AnoLetivo = 2024,
                Serie = "1 serie"
            },
            new Turma
            {
                Id = 2,
                Nome = "Turma B",
                Descricao = "Segunda turma do ano letivo",
                DataCriacao = new DateTime(2024, 1, 15),
                AnoLetivo = 2024,
                Serie = "2 serie"
            }
        );

        modelBuilder.Entity<Aluno>().HasData(
            new Aluno
            {
                Id = 1,
                NomeCompleto = "Joao Silva",
                CPF = "123.456.789-00",
                DataNascimento = new DateTime(2005, 3, 10),
                Email = "joao.silva@email.com",
                Telefone = "(11) 99999-1111",
                Endereco = "Rua A, 123",
                Observacoes = "Aluno destacado em matematica",
                DataMatricula = new DateTime(2024, 2, 1),
                Ativo = true,
                TurmaId = 1
            },
            new Aluno
            {
                Id = 2,
                NomeCompleto = "Maria Santos",
                CPF = "987.654.321-00",
                DataNascimento = new DateTime(2005, 7, 22),
                Email = "maria.santos@email.com",
                Telefone = "(11) 99999-2222",
                Endereco = "Rua B, 456",
                Observacoes = "Monitora da turma",
                DataMatricula = new DateTime(2024, 2, 1),
                Ativo = true,
                TurmaId = 1
            },
            new Aluno
            {
                Id = 3,
                NomeCompleto = "Pedro Oliveira",
                CPF = "456.789.123-00",
                DataNascimento = new DateTime(2004, 11, 5),
                Email = "pedro.oliveira@email.com",
                Telefone = "(11) 99999-3333",
                Endereco = "Rua C, 789",
                Observacoes = "Interessado em programacao",
                DataMatricula = new DateTime(2024, 2, 1),
                Ativo = true,
                TurmaId = 2
            },
            new Aluno
            {
                Id = 4,
                NomeCompleto = "Ana Costa",
                CPF = "321.654.987-00",
                DataNascimento = new DateTime(2005, 1, 18),
                Email = "ana.costa@email.com",
                Telefone = "(11) 99999-4444",
                Endereco = "Rua D, 101",
                Observacoes = "Participa do projeto de ciencias",
                DataMatricula = new DateTime(2024, 2, 1),
                Ativo = true,
                TurmaId = 2
            }
        );
    }
}
