using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CRM_Alunos.Data;
using CRM_Alunos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace CRM_Alunos.Pages
{
    public partial class AlunoDetailPage : UserControl
    {
        private readonly int _alunoId;

        public AlunoDetailPage(int alunoId)
        {
            InitializeComponent();
            _alunoId = alunoId;
            Loaded += AlunoDetailPage_Loaded;
        }

        private async void AlunoDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var context = new AppDbContext();
                var aluno = await context.Alunos
                    .Include(a => a.Turma)
                    .FirstOrDefaultAsync(a => a.Id == _alunoId);

                if (aluno == null) return;

                LblNome.Text = aluno.Nome;
                LblCPF.Text = aluno.CPF ?? "-";
                LblEmail.Text = aluno.Email ?? "-";
                LblTelefone.Text = aluno.Telefone ?? "-";
                LblEndereco.Text = aluno.Endereco ?? "-";
                LblTurma.Text = aluno.Turma?.Nome ?? "-";
                PageTitle.Text = aluno.Nome;

                var documentos = await context.Documentos
                    .Include(d => d.Pasta)
                    .Where(d => d.AlunoId == _alunoId)
                    .OrderBy(d => d.Nome)
                    .ToListAsync();

                DgDocumentos.ItemsSource = documentos;
            }
            catch
            {
                PageTitle.Text = "Erro ao carregar dados";
            }
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            main?.NavigateTo(new AlunosPage());
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funcionalidade de edição será implementada em breve.", "Em desenvolvimento", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Arquivos PDF (*.pdf)|*.pdf",
                Title = "Selecionar Documento"
            };

            if (dialog.ShowDialog() != true) return;

            var fileInfo = new FileInfo(dialog.FileName);
            var pastaId = GetSelectedPastaId();
            var pastaSubfolder = pastaId.HasValue ? pastaId.Value.ToString() : "Geral";
            var targetDir = Path.Combine("Documents", "CRM", _alunoId.ToString(), pastaSubfolder);
            Directory.CreateDirectory(targetDir);

            var destPath = Path.Combine(targetDir, fileInfo.Name);
            File.Copy(dialog.FileName, destPath, true);

            using var context = new AppDbContext();
            var doc = new Documento
            {
                Nome = fileInfo.Name,
                CaminhoArquivo = destPath,
                Tipo = fileInfo.Extension,
                Tamanho = fileInfo.Length,
                DataUpload = DateTime.Now,
                AlunoId = _alunoId,
                PastaId = pastaId
            };
            context.Documentos.Add(doc);
            await context.SaveChangesAsync();

            await ReloadDocumentos();
        }

        private async void BtnNovaPasta_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NovaPastaDialog();
            if (dialog.ShowDialog() != true) return;

            var pastaNome = dialog.NomePasta;
            var localPath = Path.Combine("Documents", "CRM", _alunoId.ToString(), pastaNome);
            Directory.CreateDirectory(localPath);

            using var context = new AppDbContext();
            var pasta = new Pasta
            {
                Nome = pastaNome,
                CaminhoLocal = localPath,
                AlunoId = _alunoId
            };
            context.Pastas.Add(pasta);
            await context.SaveChangesAsync();
        }
        private int? GetSelectedPastaId()
        {
            var selected = DgDocumentos.SelectedItem as Documento;
            return selected?.PastaId;
        }

        private async Task ReloadDocumentos()
        {
            using var context = new AppDbContext();
            var documentos = await context.Documentos
                .Include(d => d.Pasta)
                .Where(d => d.AlunoId == _alunoId)
                .OrderBy(d => d.Nome)
                .ToListAsync();

            DgDocumentos.ItemsSource = documentos;
        }

        private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var selected = DgDocumentos.SelectedItem as Documento;
            if (selected == null)
            {
                MessageBox.Show("Selecione um documento para excluir.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Deseja realmente excluir este documento?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            if (File.Exists(selected.CaminhoArquivo))
                File.Delete(selected.CaminhoArquivo);

            using var context = new AppDbContext();
            var doc = await context.Documentos.FindAsync(selected.Id);
            if (doc != null)
            {
                context.Documentos.Remove(doc);
                await context.SaveChangesAsync();
            }

            await ReloadDocumentos();
        }
    }
}
