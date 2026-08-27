using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CRM_Alunos.Data;
using CRM_Alunos.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM_Alunos.Pages
{
    public partial class NovoAlunoDialog : Window
    {
        private List<Turma> _turmas = new();

        public NovoAlunoDialog()
        {
            InitializeComponent();
            Loaded += NovoAlunoDialog_Loaded;
        }

        private async void NovoAlunoDialog_Loaded(object sender, RoutedEventArgs e)
        {
            using var context = new AppDbContext();
            _turmas = await context.Turmas.OrderBy(t => t.Nome).ToListAsync();
            CboTurma.ItemsSource = _turmas;
            if (_turmas.Count > 0)
                CboTurma.SelectedIndex = 0;
            CboStatus.SelectedIndex = 0;
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text))
            {
                MessageBox.Show("O campo Nome e obrigatorio.", "Validacao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CboTurma.SelectedItem is not Turma turma)
            {
                MessageBox.Show("Selecione uma turma.", "Validacao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var statusItem = CboStatus.SelectedItem as System.Windows.Controls.ComboBoxItem;
            var status = statusItem?.Content?.ToString() ?? "Ativo";

            var aluno = new Aluno
            {
                Nome = TxtNome.Text.Trim(),
                CPF = string.IsNullOrWhiteSpace(TxtCPF.Text) ? null : TxtCPF.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim(),
                Telefone = string.IsNullOrWhiteSpace(TxtTelefone.Text) ? null : TxtTelefone.Text.Trim(),
                Endereco = string.IsNullOrWhiteSpace(TxtEndereco.Text) ? null : TxtEndereco.Text.Trim(),
                TurmaId = turma.Id,
                Status = status,
                DataMatricula = System.DateTime.Now
            };

            using (var context = new AppDbContext())
            {
                context.Alunos.Add(aluno);
                await context.SaveChangesAsync();
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
