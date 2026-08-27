using System.Windows.Controls;
using CRM_Alunos.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM_Alunos.Pages
{
    public partial class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            InitializeComponent();
            Loaded += DashboardPage_Loaded;
        }

        private async void DashboardPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                using var context = new AppDbContext();
                TurmasCount.Text = (await context.Turmas.CountAsync()).ToString();
                AlunosCount.Text = (await context.Alunos.CountAsync()).ToString();
                DocumentosCount.Text = (await context.Documentos.CountAsync()).ToString();
            }
            catch
            {
                TurmasCount.Text = "-";
                AlunosCount.Text = "-";
                DocumentosCount.Text = "-";
            }
        }
    }
}
