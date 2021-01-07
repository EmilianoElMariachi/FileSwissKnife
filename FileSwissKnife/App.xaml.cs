using System.Windows;
using FileSwissKnife.Properties;
using FileSwissKnife.Themes;

// TODO: ajouter un bouton pour réinitialiser les settings
namespace FileSwissKnife
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeManager.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Settings.Default.Save();
        }
    }
}
