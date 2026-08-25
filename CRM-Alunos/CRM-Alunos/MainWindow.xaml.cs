using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CRM_Alunos.Pages;
using System;
using Windows.UI;

namespace CRM_Alunos
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "CRM Alunos - Gestão de Turmas";

            ContentFrame.Navigate(typeof(DashboardPage));
            NavListView.SelectedIndex = 0;

            UpdateNavColors();
        }

        private void NavListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavListView.SelectedItem is ListViewItem item)
            {
                string tag = item.Tag?.ToString() ?? "";
                Type pageType = tag switch
                {
                    "Dashboard" => typeof(DashboardPage),
                    "Turmas" => typeof(TurmasPage),
                    "Alunos" => typeof(AlunosPage),
                    "Documentos" => typeof(DocumentosPage),
                    _ => typeof(DashboardPage)
                };

                if (ContentFrame.CurrentSourcePageType != pageType)
                {
                    ContentFrame.Navigate(pageType);
                }

                UpdateNavColors();
            }
        }

        private void UpdateNavColors()
        {
            var allItems = new[] { NavDashboard, NavTurmas, NavAlunos, NavDocumentos };

            for (int i = 0; i < allItems.Length; i++)
            {
                bool isSelected = NavListView.SelectedIndex == i;
                allItems[i].Background = isSelected
                    ? new SolidColorBrush(Color.FromArgb(40, 129, 140, 248))
                    : new SolidColorBrush(Colors.Transparent);

                if (allItems[i].Content is StackPanel sp && sp.Children.Count > 1)
                {
                    if (sp.Children[0] is FontIcon icon)
                        icon.Foreground = isSelected
                            ? new SolidColorBrush(Color.FromArgb(255, 129, 140, 248))
                            : new SolidColorBrush(Color.FromArgb(255, 148, 163, 184));

                    if (sp.Children[1] is TextBlock text)
                        text.Foreground = isSelected
                            ? new SolidColorBrush(Colors.White)
                            : new SolidColorBrush(Color.FromArgb(255, 203, 213, 225));
                }
            }
        }
    }
}
