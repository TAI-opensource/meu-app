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
    public partial class AlunosPage : UserControl
    {
        private readonly int? _turmaId;
        private List<Aluno> _allAlunos = new();
        private List<Turma> _turmas = new();

        public AlunosPage(int? turmaId = null)
        {
            InitializeComponent();
            _turmaId = turmaId;
            Loaded += AlunosPage_Loaded;
        }

        private async void AlunosPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var context = new AppDbContext();

                _turmas = await context.Turmas.OrderBy(t => t.Nome).ToListAsync();
                CboTurmas.Items.Clear();
                CboTurmas.Items.Add(new Turma { Id = 0, Nome = "Todas" });
                foreach (var t in _turmas)
                    CboTurmas.Items.Add(t);

                CboTurmaEdit.ItemsSource = _turmas;
                if (_turmas.Count > 0)
                    CboTurmaEdit.SelectedIndex = 0;

                if (_turmaId.HasValue)
                {
                    PageTitle.Text = $"Alunos - Turma: {_turmas.FirstOrDefault(t => t.Id == _turmaId)?.Nome ?? ""}";
                    CboTurmas.SelectedItem = _turmas.FirstOrDefault(t => t.Id == _turmaId);
                    CboTurmaEdit.SelectedItem = _turmas.FirstOrDefault(t => t.Id == _turmaId);
                }
                else
                {
                    CboTurmas.SelectedIndex = 0;
                }
            }
            catch
            {
                _turmas = new List<Turma>();
            }

            await LoadAlunos();
        }

        private async System.Threading.Tasks.Task LoadAlunos()
        {
            try
            {
                using var context = new AppDbContext();
                var query = context.Alunos.Include(a => a.Turma).AsQueryable();

                var selectedTurma = CboTurmas.SelectedItem as Turma;
                if (selectedTurma != null && selectedTurma.Id != 0)
                    query = query.Where(a => a.TurmaId == selectedTurma.Id);

                if (!string.IsNullOrWhiteSpace(TxtSearch.Text))
                    query = query.Where(a => a.Nome.Contains(TxtSearch.Text));

                _allAlunos = await query.OrderBy(a => a.Nome).ToListAsync();
                DgAlunos.ItemsSource = _allAlunos;
            }
            catch
            {
                _allAlunos = new List<Aluno>();
                DgAlunos.ItemsSource = _allAlunos;
            }
        }

        private async void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            await LoadAlunos();
        }

        private async void CboTurma_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                await LoadAlunos();
        }

        private void DgAlunos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgAlunos.SelectedItem is Aluno aluno)
            {
                var main = Window.GetWindow(this) as MainWindow;
                main?.NavigateTo(new AlunoDetailPage(aluno.Id));
            }
        }

        private async void BtnNovoAluno_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NovoAlunoDialog
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                await LoadAlunos();
            }
        }
    }
}
