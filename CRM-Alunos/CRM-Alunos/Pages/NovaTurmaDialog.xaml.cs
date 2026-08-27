using System.Windows;
using CRM_Alunos.Data;
using CRM_Alunos.Models;

namespace CRM_Alunos.Pages
{
    public partial class NovaTurmaDialog : Window
    {
        public bool Saved { get; private set; }

        public NovaTurmaDialog()
        {
            InitializeComponent();
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var turma = new Turma
            {
                Nome = TxtNome.Text.Trim(),
                Periodo = (CboPeriodo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString(),
                Horario = TxtHorario.Text.Trim(),
                Sala = TxtSala.Text.Trim(),
                Status = "Ativa",
                DataCriacao = DateTime.Now,
                AnoLetivo = DateTime.Now.Year
            };

            using var context = new AppDbContext();
            context.Turmas.Add(turma);
            await context.SaveChangesAsync();

            Saved = true;
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
