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
    public sealed partial class AlunosPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private List<Aluno> _allAlunos = new();
        private List<Turma> _turmas = new();
        private int? _filterTurmaId;

        public AlunosPage()
        {
            this.InitializeComponent();
            this.Loaded += AlunosPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int turmaId)
            {
                _filterTurmaId = turmaId;
                BtnVoltar.Visibility = Visibility.Visible;
            }
        }

        private async void AlunosPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                _turmas = await _context.Turmas.ToListAsync();
                
                FilterTurma.Items.Clear();
                FilterTurma.Items.Add(new ComboBoxItem { Content = "Todas as turmas", Tag = (int?)null });
                foreach (var t in _turmas)
                {
                    FilterTurma.Items.Add(new ComboBoxItem { Content = t.Nome, Tag = t.Id });
                }
                
                if (_filterTurmaId.HasValue)
                {
                    var turma = _turmas.FirstOrDefault(t => t.Id == _filterTurmaId.Value);
                    if (turma != null)
                    {
                        PageTitle.Text = $"Alunos - {turma.Nome}";
                        PageSubtitle.Text = $"Alunos da turma {turma.Nome}";
                        for (int i = 0; i < FilterTurma.Items.Count; i++)
                        {
                            if (FilterTurma.Items[i] is ComboBoxItem item && item.Tag is int id && id == _filterTurmaId)
                            {
                                FilterTurma.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    FilterTurma.SelectedIndex = 0;
                }
                
                await LoadAlunos();
            }
            catch (Exception)
            {
                EmptyState.Visibility = Visibility.Visible;
            }
        }

        private async Task LoadAlunos()
        {
            try
            {
                IQueryable<Aluno> query = _context.Alunos.Include(a => a.Turma);
                
                if (_filterTurmaId.HasValue)
                    query = query.Where(a => a.TurmaId == _filterTurmaId.Value);
                
                _allAlunos = await query.ToListAsync();
                RenderAlunos(_allAlunos);
            }
            catch
            {
                EmptyState.Visibility = Visibility.Visible;
            }
        }

        private void RenderAlunos(List<Aluno> alunos)
        {
            AlunosListView.Items.Clear();
            
            if (alunos.Count == 0)
            {
                EmptyState.Visibility = Visibility.Visible;
                return;
            }
            
            EmptyState.Visibility = Visibility.Collapsed;
            
            foreach (var aluno in alunos)
            {
                var grid = new Grid { Padding = new Thickness(20, 12, 20, 12) };
                for (int i = 0; i < 6; i++)
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = i == 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(140) });
                grid.ColumnDefinitions[5].Width = new GridLength(100);

                grid.Children.Add(new TextBlock { Text = aluno.NomeCompleto, FontSize = 14, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, Foreground = new SolidColorBrush(Color.FromArgb(255, 30, 41, 59)), VerticalAlignment = VerticalAlignment.Center });
                
                AddCell(grid, 1, aluno.CPF ?? "-");
                AddCell(grid, 2, aluno.Telefone ?? "-");
                AddCell(grid, 3, aluno.Turma?.Nome ?? "-");
                
                var statusBorder = new Border
                {
                    Background = new SolidColorBrush(aluno.Ativo ? Color.FromArgb(255, 220, 252, 231) : Color.FromArgb(255, 254, 226, 226)),
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(10, 4, 10, 4),
                    VerticalAlignment = VerticalAlignment.Center
                };
                statusBorder.Child = new TextBlock
                {
                    Text = aluno.Ativo ? "Ativo" : "Inativo",
                    FontSize = 12,
                    Foreground = new SolidColorBrush(aluno.Ativo ? Color.FromArgb(255, 22, 163, 74) : Color.FromArgb(255, 220, 38, 38))
                };
                Grid.SetColumn(statusBorder, 4);
                grid.Children.Add(statusBorder);
                
                AddCell(grid, 5, aluno.DataMatricula.ToString("dd/MM/yyyy"));

                var border = new Border
                {
                    Child = grid,
                    Tag = aluno,
                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 226, 232, 240)),
                    BorderThickness = new Thickness(0, 0, 0, 1)
                };
                
                border.PointerPressed += (s, e) =>
                {
                    if (border.Tag is Aluno a)
                        Frame.Navigate(typeof(AlunoDetailPage), a.Id);
                };

                AlunosListView.Items.Add(border);
            }
        }

        private void AddCell(Grid grid, int col, string text)
        {
            var tb = new TextBlock { Text = text, FontSize = 13, Foreground = new SolidColorBrush(Color.FromArgb(255, 71, 85, 105)), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(tb, col);
            grid.Children.Add(tb);
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var filtered = _allAlunos.Where(a =>
                    a.NomeCompleto.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) ||
                    (a.CPF != null && a.CPF.Contains(SearchBox.Text)) ||
                    (a.Telefone != null && a.Telefone.Contains(SearchBox.Text))
                ).ToList();
                RenderAlunos(filtered);
            }
        }

        private async void FilterTurma_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterTurma.SelectedItem is ComboBoxItem item && item.Tag is int? turmaId)
            {
                _filterTurmaId = turmaId;
                await LoadAlunos();
            }
        }

        private void BtnVoltar_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void BtnNovoAluno_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Novo Aluno",
                PrimaryButtonText = "Cadastrar",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var stack = new StackPanel { Spacing = 12 };
            
            var nomeBox = new TextBox { Header = "Nome Completo", PlaceholderText = "Nome do aluno" };
            var cpfBox = new TextBox { Header = "CPF", PlaceholderText = "000.000.000-00" };
            var emailBox = new TextBox { Header = "Email", PlaceholderText = "email@exemplo.com" };
            var telBox = new TextBox { Header = "Telefone", PlaceholderText = "(00) 00000-0000" };
            
            var turmaCombo = new ComboBox { Header = "Turma" };
            foreach (var t in _turmas)
                turmaCombo.Items.Add(new ComboBoxItem { Content = t.Nome, Tag = t.Id });
            if (turmaCombo.Items.Count > 0)
                turmaCombo.SelectedIndex = 0;
            
            stack.Children.Add(nomeBox);
            stack.Children.Add(cpfBox);
            stack.Children.Add(emailBox);
            stack.Children.Add(telBox);
            stack.Children.Add(turmaCombo);
            dialog.Content = stack;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                int turmaId = 0;
                if (turmaCombo.SelectedItem is ComboBoxItem selectedTurma && selectedTurma.Tag is int tid)
                    turmaId = tid;

                var aluno = new Aluno
                {
                    NomeCompleto = nomeBox.Text,
                    CPF = cpfBox.Text,
                    Email = emailBox.Text,
                    Telefone = telBox.Text,
                    TurmaId = turmaId,
                    DataMatricula = DateTime.Now,
                    Ativo = true
                };
                
                _context.Alunos.Add(aluno);
                await _context.SaveChangesAsync();
                await LoadAlunos();
            }
        }
    }
}
