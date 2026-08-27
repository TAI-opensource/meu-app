using System.IO;
using System.Windows;
using CRM_Alunos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_Alunos;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public static IServiceProvider? Services { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            MessageBox.Show($"Erro fatal: {ex?.Message}\n\n{ex?.StackTrace}", "CRM Alunos - Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show($"Erro: {args.Exception.Message}\n\n{args.Exception.StackTrace}", "CRM Alunos - Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        try
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crm_alunos.db");
            var connectionString = $"Data Source={dbPath}";

            var services = new ServiceCollection();
            ConfigureServices(services, connectionString);
            _serviceProvider = services.BuildServiceProvider();
            Services = _serviceProvider;

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao inicializar o banco de dados.\n\n" +
                $"Erro: {ex.Message}\n\n" +
                $"Detalhes: {ex.InnerException?.Message ?? "N/A"}",
                "CRM Alunos - Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));
    }
}
