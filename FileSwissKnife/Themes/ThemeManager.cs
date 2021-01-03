using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using FileSwissKnife.Properties;

namespace FileSwissKnife.Themes
{
    public static class ThemeManager
    {
        private static readonly Theme[] _availableThemes;

        static ThemeManager()
        {
            _availableThemes = InitializeAvailableThemes().ToArray();
        }

        public static IEnumerable<Theme> AvailableThemes => _availableThemes;

        public static Theme? CurrentTheme { get; private set; }

        public static void SetTheme(Theme theme)
        {
            var app = Application.Current;

            var currentTheme = CurrentTheme;
            if (currentTheme != null)
                app.Resources.MergedDictionaries.Remove(currentTheme.ResourceDictionary);

            app.Resources.MergedDictionaries.Add(theme.ResourceDictionary);

            CurrentTheme = theme;
            Settings.Default.ActiveTheme = theme.Name;
        }

        public static void Initialize()
        {
            // Removes all themes
            foreach (var theme in _availableThemes)
                Application.Current.Resources.MergedDictionaries.Remove(theme.ResourceDictionary);

            var activeTheme = Settings.Default.ActiveTheme;

            var selectedTheme = _availableThemes.FirstOrDefault(theme => theme.Name == activeTheme);
            if (selectedTheme != null)
            {
                SetTheme(selectedTheme);
                return;
            }

            var firstTheme = _availableThemes.FirstOrDefault();
            if (firstTheme != null)
                SetTheme(firstTheme);
        }

        private static IEnumerable<Theme> InitializeAvailableThemes()
        {
            foreach (var resourceDictionary in Application.Current.Resources.MergedDictionaries)
            {
                var resourceUrl = resourceDictionary.Source.OriginalString;
                if (!resourceUrl.StartsWith("Themes", StringComparison.OrdinalIgnoreCase))
                    continue;

                var themeName = Path.GetFileNameWithoutExtension(resourceUrl);

                yield return new Theme(themeName, resourceDictionary);
            }
        }

    }
}
