using System.Windows;
using FileSwissKnife.Properties;

// TODO: faire le thème sombre
// TODO: améliorer la routine de détection des fichiers à joindre
namespace FileSwissKnife
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Settings.Default.Save();
        }
    }
}
