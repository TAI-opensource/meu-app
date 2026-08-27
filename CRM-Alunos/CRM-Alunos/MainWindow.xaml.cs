using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CRM_Alunos.Pages;

namespace CRM_Alunos
{
    public partial class MainWindow : Window
    {
        private Button[] _navButtons;

        public MainWindow()
        {
            InitializeComponent();

            _navButtons = new[] { BtnDashboard, BtnTurmas, BtnAlunos, BtnDocumentos, BtnConfiguracoes };

            NavigateTo("Dashboard");
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e) => NavigateTo("Dashboard");
        private void BtnTurmas_Click(object sender, RoutedEventArgs e) => NavigateTo("Turmas");
        private void BtnAlunos_Click(object sender, RoutedEventArgs e) => NavigateTo("Alunos");
        private void BtnDocumentos_Click(object sender, RoutedEventArgs e) => NavigateTo("Documentos");
        private void BtnConfiguracoes_Click(object sender, RoutedEventArgs e) => NavigateTo("Configuracoes");

        public void NavigateTo(string tag)
        {
            UserControl page = tag switch
            {
                "Dashboard" => new DashboardPage(),
                "Turmas" => new TurmasPage(),
                "Alunos" => new AlunosPage(),
                "Documentos" => new DocumentosPage(),
                "Configuracoes" => new ConfiguracoesPage(),
                _ => null
            };

            if (page != null)
            {
                ContentFrame.Content = page;
            }

            UpdateActiveButton(tag);
        }

        public void NavigateTo(UserControl page)
        {
            ContentFrame.Content = page;

            string tag = page switch
            {
                DashboardPage => "Dashboard",
                TurmasPage => "Turmas",
                AlunosPage => "Alunos",
                DocumentosPage => "Documentos",
                AlunoDetailPage => "Alunos",
                ConfiguracoesPage => "Configuracoes",
                _ => ""
            };

            UpdateActiveButton(tag);
        }

        private void UpdateActiveButton(string activeTag)
        {
            var activeBrush = new SolidColorBrush(Color.FromArgb(40, 129, 140, 248));
            var inactiveBrush = new SolidColorBrush(Colors.Transparent);
            var activeForeground = new SolidColorBrush(Colors.White);
            var inactiveForeground = new SolidColorBrush(Color.FromArgb(255, 203, 213, 225));
            var activeIconForeground = new SolidColorBrush(Color.FromArgb(255, 129, 140, 248));
            var inactiveIconForeground = new SolidColorBrush(Color.FromArgb(255, 148, 163, 184));

            foreach (var btn in _navButtons)
            {
                bool isActive = btn.Tag?.ToString() == activeTag;
                btn.Background = isActive ? activeBrush : inactiveBrush;

                if (btn.Content is StackPanel sp && sp.Children.Count >= 2)
                {
                    if (sp.Children[0] is System.Windows.Controls.TextBlock icon)
                        icon.Foreground = isActive ? activeIconForeground : inactiveIconForeground;

                    if (sp.Children[1] is System.Windows.Controls.TextBlock text)
                        text.Foreground = isActive ? activeForeground : inactiveForeground;
                }
            }
        }
    }
}
