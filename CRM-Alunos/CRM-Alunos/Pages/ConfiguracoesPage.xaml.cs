using System.Windows;
using System.Windows.Controls;
using CRM_Alunos.Services;

namespace CRM_Alunos.Pages
{
    public partial class ConfiguracoesPage : UserControl
    {
        private readonly UpdateService _updateService = new();
        private UpdateInfo? _pendingUpdate;

        public ConfiguracoesPage()
        {
            InitializeComponent();
            Loaded += ConfiguracoesPage_Loaded;
        }

        private void ConfiguracoesPage_Loaded(object sender, RoutedEventArgs e)
        {
            LblVersion.Text = $"Versao atual: {UpdateService.CurrentVersion}";
            LblUpdateStatus.Text = "Clique em 'Verificar Atualizacoes' para buscar novas versoes.";
        }

        private async void BtnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            BtnCheckUpdate.IsEnabled = false;
            BtnCheckUpdate.Content = "Verificando...";
            LblUpdateStatus.Text = "Buscando atualizacoes no GitHub...";

            try
            {
                var update = await _updateService.CheckForUpdateAsync();

                if (update != null)
                {
                    _pendingUpdate = update;
                    LblUpdateStatus.Text = $"Nova versao disponivel: v{update.Version}\n{update.Name}";
                    BtnInstallUpdate.Visibility = Visibility.Visible;
                }
                else
                {
                    LblUpdateStatus.Text = "Voce esta na versao mais recente!";
                    BtnInstallUpdate.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                LblUpdateStatus.Text = "Erro ao verificar atualizacoes. Verifique sua conexao.";
            }

            BtnCheckUpdate.IsEnabled = true;
            BtnCheckUpdate.Content = "Verificar Atualizacoes";
        }

        private async void BtnInstallUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_pendingUpdate == null) return;

            var result = MessageBox.Show(
                $"Deseja instalar a v{_pendingUpdate.Version}?\n\nO aplicativo sera reiniciado.",
                "Confirmar Atualizacao",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            BtnInstallUpdate.IsEnabled = false;
            BtnInstallUpdate.Content = "Instalando...";
            ProgressBorder.Visibility = Visibility.Visible;

            var success = await _updateService.DownloadAndInstallAsync(_pendingUpdate, msg =>
            {
                Dispatcher.Invoke(() => LblProgress.Text = msg);
            });

            if (!success)
            {
                MessageBox.Show("Erro ao instalar atualizacao.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                BtnInstallUpdate.IsEnabled = true;
                BtnInstallUpdate.Content = "Instalar Atualizacao";
                ProgressBorder.Visibility = Visibility.Collapsed;
            }
        }
    }
}
