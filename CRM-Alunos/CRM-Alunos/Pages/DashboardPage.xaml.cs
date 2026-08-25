using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.EntityFrameworkCore;
using CRM_Alunos.Data;
using System.Threading.Tasks;

namespace CRM_Alunos.Pages
{
    public sealed partial class DashboardPage : Page
    {
        private readonly AppDbContext _context = new AppDbContext();

        public DashboardPage()
        {
            this.InitializeComponent();
            this.Loaded += DashboardPage_Loaded;
        }

        private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStats();
        }

        private async Task LoadStats()
        {
            try
            {
                var turmaCount = await _context.Turmas.CountAsync();
                var alunoCount = await _context.Alunos.CountAsync();
                var docCount = await _context.Documentos.CountAsync();

                TxtTurmasCount.Text = turmaCount.ToString();
                TxtAlunosCount.Text = alunoCount.ToString();
                TxtDocsCount.Text = docCount.ToString();

                if (docCount > 0)
                {
                    TxtNoActivity.Visibility = Visibility.Collapsed;
                    ActivityList.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                // Database might not exist yet - show zeros
                TxtTurmasCount.Text = "0";
                TxtAlunosCount.Text = "0";
                TxtDocsCount.Text = "0";
            }
        }
    }
}