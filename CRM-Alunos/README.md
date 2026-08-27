# CRM Alunos - Sistema de Gestao de Turmas e Alunos

Aplicativo desktop WPF para gestao de turmas, alunos e documentos.

## Descricao

O CRM Alunos e um sistema desktop construido com WPF para auxiliar instituicoes de ensino na organizacao de turmas, cadastro de alunos e gerenciamento de documentos academicos. O projeto e totalmente local e gratuito, utilizando SQLite como banco de dados (arquivo local, sem necessidade de servidor).

## Tech Stack

- **Linguagem**: C# (.NET 8.0)
- **UI Framework**: WPF
- **Banco de dados**: SQLite (arquivo local)
- **ORM**: Entity Framework Core 8.0
- **Arquitetura**: MVVM (CommunityToolkit.Mvvm)

## Funcionalidades

- **Gestao de Turmas** - Criar, editar e listar turmas
- **Gestao de Alunos** - Cadastro e organizacao de alunos por turma
- **Documentos PDF** - Upload e gerenciamento de documentos PDF
- **Sistema de Pastas** - Organizacao de documentos em pastas
- **Interface Moderna** - Design limpo e intuitivo
- **Totalmente Local** - Sem necessidade de servidor, banco de dados em arquivo

## Como Usar

1. Baixe o release mais recente em [Releases](https://github.com/mineblox99los/CRM-Install/releases)
2. Extraia o ZIP em qualquer pasta
3. Execute `CRM-Alunos.exe`
4. Pronto! O banco de dados e criado automaticamente

## Como Buildar

```bash
dotnet restore CRM-Alunos/CRM-Alunos.csproj
dotnet build CRM-Alunos/CRM-Alunos.csproj -c Release
```

## Estrutura do Projeto

```
CRM-Alunos/
├── CRM-Alunos.sln
├── README.md
└── CRM-Alunos/
    ├── CRM-Alunos.csproj
    ├── App.xaml / App.xaml.cs
    ├── MainWindow.xaml / MainWindow.xaml.cs
    ├── Data/
    │   └── AppDbContext.cs
    ├── Models/
    │   ├── Turma.cs
    │   ├── Aluno.cs
    │   ├── Documento.cs
    │   └── Pasta.cs
    └── Pages/
        ├── DashboardPage.xaml
        ├── TurmasPage.xaml
        ├── AlunosPage.xaml
        ├── AlunoDetailPage.xaml
        ├── DocumentosPage.xaml
        ├── NovaTurmaDialog.xaml
        ├── NovoAlunoDialog.xaml
        └── NovaPastaDialog.xaml
```

## GitHub Actions

- **Build**: Build automatizado em cada push
- **Quality**: Formatacao de codigo, deteccao de codigo morto
- **Release**: Release automatico com ZIP para download

## Licenca

MIT License
