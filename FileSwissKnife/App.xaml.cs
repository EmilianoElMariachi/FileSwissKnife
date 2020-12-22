using System.Windows;
using System.Windows.Navigation;
using FileSwissKnife.Properties;

// TODO: faire une passe sur toutes les fonctionalités pour voir comment gérer si un répertoire est donné en entrée.
// TODO: faire la page des settings et exposer la taille des buffers
// TODO: faire une icône pour l'application
// TODO: faire le thème sombre
// TODO: implémenter la règle de nommage du split

namespace FileSwissKnife
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
            Application.Current.Exit += OnAppExit;
        }

        private static void OnAppExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }

    }
}
