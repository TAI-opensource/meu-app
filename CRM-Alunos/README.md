# CRM Alunos - Sistema de Gestão de Turmas e Alunos

Aplicativo desktop WinUI 3 para gestão de turmas, alunos e documentos.

## Descrição

O CRM Alunos é um sistema desktop construído com WinUI 3 e Windows App SDK para auxiliar instituições de ensino na organização de turmas, cadastro de alunos e gerenciamento de documentos acadêmicos. O projeto é totalmente local e gratuito.

## Tech Stack

- **Linguagem**: C# (.NET 8.0)
- **UI Framework**: WinUI 3 (Windows App SDK 1.5.0)
- **Banco de dados**: SQLite
- **ORM**: Entity Framework Core 8.0
- **Arquitetura**: MVVM (CommunityToolkit.Mvvm)
- **Injeção de dependência**: Microsoft.Extensions.DependencyInjection

## Funcionalidades

- **Gestão de Turmas** - Criar, editar e listar turmas
- **Gestão de Alunos** - Cadastro e organização de alunos por turma
- **Sistema de Documentos** - Upload de PDFs, criação e organização de pastas
- **Interface Moderna** - Estilo Twenty CRM com design limpo e intuitivo
- **Totalmente Local** - Sem necessidade de servidor, dados armazenados localmente em SQLite

## Como Buildar

O projeto é buildado automaticamente via **GitHub Actions** sempre que alterações são feitas na pasta `CRM-Alunos/` na branch `main`.

### Build Manual

Para buildar localmente, é necessário:

1. [Visual Studio 2022](https://visualstudio.microsoft.com/) com o workload **Desenvolvimento de Aplicativos da Plataforma Universal Windows** instalado
2. [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
dotnet restore CRM-Alunos/CRM-Alunos/CRM-Alunos.csproj
dotnet build CRM-Alunos/CRM-Alunos/CRM-Alunos.csproj -c Release
```

### Estrutura do Projeto

```
CRM-Alunos/
├── CRM-Alunos.sln              # Solução do Visual Studio
└── CRM-Alunos/
    ├── CRM-Alunos.csproj       # Projeto WinUI 3
    ├── Data/                    # Contexto e configurações do EF Core
    └── Models/                  # Modelos de dados (Turma, Aluno, Documento)
```

## Licença

MIT License - veja o arquivo [LICENSE](../LICENSE) para mais detalhes.
