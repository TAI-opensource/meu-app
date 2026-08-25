using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.EntityFrameworkCore;
using CRM_Alunos.Data;
using CRM_Alunos.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

namespace CRM_Alunos.Pages
{
    public sealed partial class DocumentosPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private List<Documento> _allDocs = new();

        public DocumentosPage()
        {
            this.InitializeComponent();
            this.Loaded += DocumentosPage_Loaded;
        }

        private async void DocumentosPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDocs();
        }

        private async Task LoadDocs()
        {
            try
            {
                _allDocs = await _context.Documentos
                    .Include(d => d.Aluno)
                    .OrderByDescending(d => d.DataUpload)
                    .ToListAsync();
                RenderDocs(_allDocs);
            }
            catch
            {
                EmptyState.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
        }

        private void RenderDocs(List<Documento> docs)
        {
            DocsListView.Items.Clear();
            
            if (docs.Count == 0)
            {
                EmptyState.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                return;
            }
            
            EmptyState.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            
            foreach (var doc in docs)
            {
                var grid = new Grid { Padding = new Thickness(20, 12, 20, 12) };
                for (int i = 0; i < 6; i++)
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = i == 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(i <= 1 ? 160 : i <= 3 ? 100 : 120) });

                grid.Children.Add(new TextBlock { Text = doc.Nome, FontSize = 14, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 41, 59)), VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis });
                
                AddCell(grid, 1, doc.Aluno?.NomeCompleto ?? "-");
                AddCell(grid, 2, doc.TipoArquivo ?? "-");
                AddCell(grid, 3, FormatSize(doc.TamanhoArquivo));
                AddCell(grid, 4, doc.DataUpload.ToString("dd/MM/yyyy"));

                var openBtn = new Button
                {
                    Content = "Abrir",
                    FontSize = 12,
                    Padding = new Thickness(12, 4, 12, 4),
                    CornerRadius = new CornerRadius(6),
                    Background = new SolidColorBrush(Color.FromArgb(255, 241, 245, 249)),
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 85, 105)),
                    Tag = doc,
                    VerticalAlignment = VerticalAlignment.Center
                };
                openBtn.Click += OpenBtn_Click;
                Grid.SetColumn(openBtn, 5);
                grid.Children.Add(openBtn);

                var border = new Border
                {
                    Child = grid,
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 226, 232, 240)),
                    BorderThickness = new Thickness(0, 0, 0, 1)
                };
                DocsListView.Items.Add(border);
            }
        }

        private void AddCell(Grid grid, int col, string text)
        {
            var tb = new TextBlock { Text = text, FontSize = 13, Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 85, 105)), VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
            Grid.SetColumn(tb, col);
            grid.Children.Add(tb);
        }

        private string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1048576) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / 1048576.0:F1} MB";
        }

        private async void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Documento doc && File.Exists(doc.CaminhoArquivo))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(doc.CaminhoArquivo));
            }
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var filtered = _allDocs.Where(d =>
                    d.Nome.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) ||
                    (d.Aluno != null && d.Aluno.NomeCompleto.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase))
                ).ToList();
                RenderDocs(filtered);
            }
        }
    }
}
