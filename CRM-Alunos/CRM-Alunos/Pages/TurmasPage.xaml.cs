using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.EntityFrameworkCore;
using CRM_Alunos.Data;
using CRM_Alunos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

namespace CRM_Alunos.Pages
{
    public sealed partial class TurmasPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private List<Turma> _allTurmas = new();

        public TurmasPage()
        {
            this.InitializeComponent();
            this.Loaded += TurmasPage_Loaded;
        }

        private async void TurmasPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTurmas();
        }

        private async Task LoadTurmas()
        {
            try
            {
                _allTurmas = await _context.Turmas.Include(t => t.Alunos).ToListAsync();
                RenderTurmas(_allTurmas);
            }
            catch (Exception ex)
            {
                EmptyState.Visibility = Visibility.Visible;
            }
        }

        private void RenderTurmas(List<Turma> turmas)
        {
            TurmasListView.Items.Clear();
            
            if (turmas.Count == 0)
            {
                EmptyState.Visibility = Visibility.Visible;
                return;
            }
            
            EmptyState.Visibility = Visibility.Collapsed;
            
            foreach (var turma in turmas)
            {
                var grid = new Grid { Padding = new Thickness(20, 12, 20, 12) };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });

                grid.Children.Add(new TextBlock { Text = turma.Nome, FontSize = 14, Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 41, 59)), VerticalAlignment = VerticalAlignment.Center });
                
                var anoText = new TextBlock { Text = turma.AnoLetivo.ToString(), FontSize = 14, Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 85, 105)), VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(anoText, 1);
                grid.Children.Add(anoText);
                
                var serieText = new TextBlock { Text = turma.Serie ?? "-", FontSize = 14, Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 85, 105)), VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(serieText, 2);
                grid.Children.Add(serieText);
                
                var alunosText = new TextBlock { Text = (turma.Alunos?.Count ?? 0).ToString(), FontSize = 14, Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 85, 105)), VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(alunosText, 3);
                grid.Children.Add(alunosText);
                
                var dataText = new TextBlock { Text = turma.DataCriacao.ToString("dd/MM/yyyy"), FontSize = 13, Foreground = new SolidColorBrush(Color.FromArgb(255, 148, 163, 184)), VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(dataText, 4);
                grid.Children.Add(dataText);

                var border = new Border
                {
                    Child = grid,
                    Tag = turma,
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 226, 232, 240)),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(0)
                };
                
                border.PointerPressed += (s, e) =>
                {
                    if (border.Tag is Turma t)
                    {
                        Frame.Navigate(typeof(AlunosPage), t.Id);
                    }
                };

                TurmasListView.Items.Add(border);
            }
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var filtered = _allTurmas.Where(t => 
                    t.Nome.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) ||
                    (t.Serie != null && t.Serie.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase))
                ).ToList();
                RenderTurmas(filtered);
            }
        }

        private void TurmasListView_ItemClick(object sender, ItemClickEventArgs e) { }

        private async void BtnNovaTurma_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Nova Turma",
                PrimaryButtonText = "Criar",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var stack = new StackPanel { Spacing = 12 };
            
            var nomeBox = new TextBox { Header = "Nome da Turma", PlaceholderText = "Ex: 3ano A" };
            var anoBox = new TextBox { Header = "Ano Letivo", PlaceholderText = "2024" };
            var serieBox = new TextBox { Header = "Série", PlaceholderText = "Ex: 3 ano" };
            var descBox = new TextBox { Header = "Descrição", PlaceholderText = "Opcional" };
            
            stack.Children.Add(nomeBox);
            stack.Children.Add(anoBox);
            stack.Children.Add(serieBox);
            stack.Children.Add(descBox);
            dialog.Content = stack;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (int.TryParse(anoBox.Text, out int ano))
                {
                    var turma = new Turma
                    {
                        Nome = nomeBox.Text,
                        AnoLetivo = ano,
                        Serie = serieBox.Text,
                        Descricao = descBox.Text,
                        DataCriacao = DateTime.Now
                    };
                    
                    _context.Turmas.Add(turma);
                    await _context.SaveChangesAsync();
                    await LoadTurmas();
                }
            }
        }
    }
}
