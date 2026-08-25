using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Storage;
using CRM_Alunos.Data;
using CRM_Alunos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace CRM_Alunos.Pages
{
    public sealed partial class AlunoDetailPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private Aluno? _aluno;
        private int _alunoId;

        public AlunoDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int alunoId)
            {
                _alunoId = alunoId;
                this.Loaded += AlunoDetailPage_Loaded;
            }
        }

        private async void AlunoDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAluno();
        }

        private async Task LoadAluno()
        {
            try
            {
                _aluno = await _context.Alunos
                    .Include(a => a.Turma)
                    .Include(a => a.Documentos)
                        .ThenInclude(d => d.Pasta)
                    .FirstOrDefaultAsync(a => a.Id == _alunoId);

                if (_aluno == null) return;

                TxtNome.Text = _aluno.NomeCompleto;
                TxtCPF.Text = _aluno.CPF ?? "-";
                TxtEmail.Text = _aluno.Email ?? "-";
                TxtTelefone.Text = _aluno.Telefone ?? "-";
                TxtNascimento.Text = _aluno.DataNascimento?.ToString("dd/MM/yyyy") ?? "-";
                TxtEndereco.Text = _aluno.Endereco ?? "-";
                TxtMatricula.Text = _aluno.DataMatricula.ToString("dd/MM/yyyy");
                TxtObs.Text = _aluno.Observacoes ?? "-";
                TxtTurma.Text = _aluno.Turma?.Nome ?? "";

                if (_aluno.Ativo)
                {
                    StatusBadge.Background = new SolidColorBrush(Color.FromArgb(255, 220, 252, 231));
                    TxtStatus.Text = "Ativo";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 22, 163, 74));
                }
                else
                {
                    StatusBadge.Background = new SolidColorBrush(Color.FromArgb(255, 254, 226, 226));
                    TxtStatus.Text = "Inativo";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 38, 38));
                }

                await LoadDocuments();
            }
            catch (Exception)
            {
                // Handle error
            }
        }

        private void LoadDocuments()
        {
            DocumentsList.Items.Clear();
            FoldersList.Items.Clear();

            if (_aluno?.Documentos == null || _aluno.Documentos.Count == 0)
            {
                EmptyDocs.Visibility = Visibility.Visible;
                return;
            }

            EmptyDocs.Visibility = Visibility.Collapsed;

            // Load folders
            var pastas = _aluno.Documentos
                .Where(d => d.Pasta != null)
                .Select(d => d.Pasta!)
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var pasta in pastas)
            {
                var pastaStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Margin = new Thickness(0, 4, 0, 4) };
                pastaStack.Children.Add(new FontIcon { Glyph = "\uE8B7", FontSize = 16, Foreground = new SolidColorBrush(Color.FromArgb(255, 245, 158, 11)) });
                pastaStack.Children.Add(new TextBlock { Text = pasta.Nome, FontSize = 13, Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 41, 59)), VerticalAlignment = VerticalAlignment.Center });
                
                var pastaBorder = new Border
                {
                    Child = pastaStack,
                    Padding = new Thickness(12, 8, 12, 8),
                    CornerRadius = new CornerRadius(6),
                    Background = new SolidColorBrush(Color.FromArgb(255, 255, 251, 235)),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 253, 230, 138)),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 2, 0, 2)
                };
                FoldersList.Items.Add(pastaBorder);
            }

            // Load documents
            foreach (var doc in _aluno.Documentos.OrderByDescending(d => d.DataUpload))
            {
                var docGrid = new Grid { Padding = new Thickness(12, 10, 12, 10) };
                docGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
                docGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                docGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

                // File icon
                var fileIcon = new FontIcon
                {
                    Glyph = doc.TipoArquivo?.ToUpper() == "PDF" ? "\uE8A5" : "\uE7C3",
                    FontSize = 18,
                    Foreground = new SolidColorBrush(doc.TipoArquivo?.ToUpper() == "PDF" ? Color.FromArgb(255, 239, 68, 68) : Color.FromArgb(255, 71, 85, 105)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                docGrid.Children.Add(fileIcon);

                // File info
                var fileInfo = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                fileInfo.Children.Add(new TextBlock { Text = doc.Nome, FontSize = 13, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 41, 59)) });
                fileInfo.Children.Add(new TextBlock { Text = $"{FormatFileSize(doc.TamanhoArquivo)} • {doc.DataUpload:dd/MM/yyyy}", FontSize = 11, Foreground = new SolidColorBrush(Color.FromArgb(255, 148, 163, 184)) });
                Grid.SetColumn(fileInfo, 1);
                docGrid.Children.Add(fileInfo);

                // Open button
                var openBtn = new Button
                {
                    Content = "Abrir",
                    FontSize = 12,
                    Padding = new Thickness(12, 4, 12, 4),
                    CornerRadius = new CornerRadius(6),
                    Background = new SolidColorBrush(Color.FromArgb(255, 241, 245, 249)),
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 85, 105)),
                    Tag = doc
                };
                openBtn.Click += OpenDoc_Click;
                Grid.SetColumn(openBtn, 2);
                docGrid.Children.Add(openBtn);

                var docBorder = new Border
                {
                    Child = docGrid,
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 226, 232, 240)),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    CornerRadius = new CornerRadius(4)
                };
                DocumentsList.Items.Add(docBorder);
            }
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1048576) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / 1048576.0:F1} MB";
        }

        private async void OpenDoc_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Documento doc)
            {
                try
                {
                    if (File.Exists(doc.CaminhoArquivo))
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri(doc.CaminhoArquivo));
                    }
                }
                catch { }
            }
        }

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".doc");
            picker.FileTypeFilter.Add(".docx");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            
            var hwnd = WindowNative.GetWindowHandle(App.Current.m_window);
            InitializeWithWindow.Initialize(picker, hwnd);
            
            var files = await picker.PickMultipleFilesAsync();
            
            if (files != null && files.Count > 0)
            {
                // Create student folder if it doesn't exist
                string studentFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "CRM_Alunos", _aluno!.NomeCompleto.Replace(" ", "_"));
                Directory.CreateDirectory(studentFolder);

                foreach (var file in files)
                {
                    string destPath = Path.Combine(studentFolder, file.Name);
                    
                    // Copy file
                    using (var sourceStream = await file.OpenReadAsync())
                    using (var destStream = File.Create(destPath))
                    {
                        await sourceStream.CopyToAsync(destStream);
                    }

                    string ext = Path.GetExtension(file.Name).TrimStart('.').ToUpper();
                    
                    var doc = new Documento
                    {
                        Nome = file.Name,
                        CaminhoArquivo = destPath,
                        TipoArquivo = ext,
                        TamanhoArquivo = (long)file.Size,
                        DataUpload = DateTime.Now,
                        AlunoId = _aluno.Id
                    };
                    
                    _context.Documentos.Add(doc);
                }
                
                await _context.SaveChangesAsync();
                await LoadAluno();
            }
        }

        private async void BtnNovaPasta_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Nova Pasta",
                PrimaryButtonText = "Criar",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var textBox = new TextBox { Header = "Nome da Pasta", PlaceholderText = "Ex: Documentos Pessoais" };
            dialog.Content = textBox;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                string studentFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "CRM_Alunos", _aluno!.NomeCompleto.Replace(" ", "_"));
                string pastaPath = Path.Combine(studentFolder, textBox.Text);
                Directory.CreateDirectory(pastaPath);

                var pasta = new Pasta
                {
                    Nome = textBox.Text,
                    CaminhoLocal = pastaPath,
                    AlunoId = _aluno.Id
                };
                
                _context.Pastas.Add(pasta);
                await _context.SaveChangesAsync();
                
                // Refresh
                _aluno = await _context.Alunos
                    .Include(a => a.Turma)
                    .Include(a => a.Documentos).ThenInclude(d => d.Pasta)
                    .FirstOrDefaultAsync(a => a.Id == _alunoId);
                LoadDocuments();
            }
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement edit dialog
        }
    }
}
