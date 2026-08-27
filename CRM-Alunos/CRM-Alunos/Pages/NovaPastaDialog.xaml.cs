using System.Windows;

namespace CRM_Alunos.Pages;

public partial class NovaPastaDialog : Window
{
    public string NomePasta => TxtNomePasta.Text.Trim();

    public NovaPastaDialog()
    {
        InitializeComponent();
        TxtNomePasta.Focus();
    }

    private void BtnCriar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNomePasta.Text))
        {
            MessageBox.Show("Informe o nome da pasta.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
