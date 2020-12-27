﻿using System.Windows;
using FileSwissKnife.Properties;

// TODO: faire une passe sur toutes les fonctionalités pour voir comment gérer si un répertoire est donné en entrée.
// TODO: faire la page des settings et exposer la taille des buffers, la langue
// TODO: faire une icône pour l'application
// TODO: faire le thème sombre
// TODO: améliorer la routine de détection des fichiers à joindre
// TODO: remettre les barres de progression à 0 quand tout est terminé
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
