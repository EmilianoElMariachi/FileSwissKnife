using System.Windows;
using FileSwissKnife.Properties;
using FileSwissKnife.Themes;

// TODO: revoir la watermark en supprimant le TextBoxExt
// TODO: améliorer la routine de détection des fichiers à joindre
// TODO: afficher la version dans la barre des titres
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
