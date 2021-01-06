using System.Windows;
using FileSwissKnife.Properties;
using FileSwissKnife.Themes;

// TODO: améliorer la routine de détection des fichiers à joindre, régler le problème des .part lors de la reconstitution du nom du fichier de sortie
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
