using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using CRM_Alunos.Data;

namespace CRM_Alunos;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public static IServiceProvider? Services { get; private set; }

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
        }

        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Activate();
    }

    public static Window? MainWindow { get; private set; }
    public Window? m_window => MainWindow;

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=crm_alunos.db"));
    }
}
