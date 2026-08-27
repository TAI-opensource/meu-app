using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CRM_Alunos.Data;
using CRM_Alunos.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM_Alunos.Pages
{
    public partial class TurmasPage : UserControl
    {
        private List<Turma> _allTurmas = new();

        public TurmasPage()
        {
            InitializeComponent();
            Loaded += TurmasPage_Loaded;
        }

        private async void TurmasPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTurmas();
        }

        private async System.Threading.Tasks.Task LoadTurmas(string? search = null)
        {
            try
            {
                using var context = new AppDbContext();
                var query = context.Turmas.Include(t => t.Alunos).AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(t => t.Nome.Contains(search));

                _allTurmas = await query.OrderBy(t => t.Nome).ToListAsync();
                DgTurmas.ItemsSource = _allTurmas;
            }
            catch
            {
                _allTurmas = new List<Turma>();
                DgTurmas.ItemsSource = _allTurmas;
            }
        }

        private async void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            await LoadTurmas(TxtSearch.Text);
        }

        private void DgTurmas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgTurmas.SelectedItem is Turma turma)
            {
                var main = Window.GetWindow(this) as MainWindow;
                main?.NavigateTo(new AlunosPage(turma.Id));
            }
        }

        private async void BtnSalvarInline_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var turma = new Turma
            {
                Nome = TxtNome.Text.Trim(),
                Periodo = TxtPeriodo.Text.Trim(),
                Horario = TxtHorario.Text.Trim(),
                Sala = TxtSala.Text.Trim(),
                Status = (CboStatus.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Ativa",
                DataCriacao = DateTime.Now,
                AnoLetivo = DateTime.Now.Year
            };

            using var context = new AppDbContext();
            context.Turmas.Add(turma);
            await context.SaveChangesAsync();

            TxtNome.Text = string.Empty;
            TxtPeriodo.Text = string.Empty;
            TxtHorario.Text = string.Empty;
            TxtSala.Text = string.Empty;
            CboStatus.SelectedIndex = 0;

            await LoadTurmas();
        }

        private async void BtnNovaTurma_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NovaTurmaDialog
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && dialog.Saved)
            {
                await LoadTurmas();
            }
        }
    }
}
