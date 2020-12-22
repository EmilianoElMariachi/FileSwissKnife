using System.Windows;
using FileSwissKnife.Properties;

// TODO: faire une passe sur toutes les fonctionalités pour voir comment gérer si un répertoire est donné en entrée.
// TODO: faire la page des settings et exposer la taille des buffers
// TODO: faire une icône pour l'application
// TODO: faire le thème sombre
// TODO: afficher le % d'avancement dans le calcul des hash
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

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Settings.Default.Save();
        }

    }
}
