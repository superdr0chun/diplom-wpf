using System.Windows;
using DjangoWPFClient.Services;

namespace DjangoWPFClient;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeManager.Init();
    }
}