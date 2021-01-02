using System;
using System.Reflection;
using System.Windows;
using FileSwissKnife.Properties;
using FileSwissKnife.Themes;

// TODO: faire le thème sombre
// TODO: améliorer la routine de détection des fichiers à joindre
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
