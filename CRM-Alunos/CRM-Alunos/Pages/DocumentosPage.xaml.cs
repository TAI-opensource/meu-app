using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using CRM_Alunos.Data;
using CRM_Alunos.Models;

namespace CRM_Alunos.Pages
{
    public partial class DocumentosPage : UserControl
    {
        private readonly AppDbContext _context = new();
        private List<Documento> _allDocs = new();

        public DocumentosPage()
        {
            InitializeComponent();
            Loaded += DocumentosPage_Loaded;
        }

        private async void DocumentosPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDocs();
        }

        private async System.Threading.Tasks.Task LoadDocs()
        {
            try
            {
                _allDocs = await _context.Documentos
                    .Include(d => d.Aluno)
                    .OrderByDescending(d => d.DataUpload)
                    .ToListAsync();
                DgDocumentos.ItemsSource = _allDocs;
            }
            catch
            {
                DgDocumentos.ItemsSource = null;
            }
        }

        private async void TxtSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var filtered = _allDocs.Where(d =>
                d.Nome.Contains(TxtSearch.Text, System.StringComparison.OrdinalIgnoreCase) ||
                (d.Aluno != null && d.Aluno.Nome.Contains(TxtSearch.Text, System.StringComparison.OrdinalIgnoreCase))
            ).ToList();
            DgDocumentos.ItemsSource = filtered;
        }
    }
}
