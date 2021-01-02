using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Markup;
using FileSwissKnife.Properties;

namespace FileSwissKnife.Themes
{
    public static class ThemeManager
    {

        static ThemeManager()
        {
            AvailableThemes = ListAvailableThemes().ToArray();
        }

        public static IEnumerable<Theme> AvailableThemes { get; }

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
            var activeTheme = Settings.Default.ActiveTheme;

            var selectedTheme = AvailableThemes.FirstOrDefault(theme => theme.Name == activeTheme);
            if (selectedTheme != null)
            {
                SetTheme(selectedTheme);
                return;
            }

            var firstTheme = AvailableThemes.FirstOrDefault();
            if(firstTheme != null)
                SetTheme(firstTheme);
        }

        private static IEnumerable<Theme> ListAvailableThemes()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            foreach (string manifestResourceName in currentAssembly.GetManifestResourceNames())
            {
                var info = currentAssembly.GetManifestResourceInfo(manifestResourceName);
                if(info == null)
                    continue;

                if (info.ResourceLocation == ResourceLocation.ContainedInAnotherAssembly)
                    continue;

                var resourceStream = currentAssembly.GetManifestResourceStream(manifestResourceName);
                if(resourceStream == null)
                    continue;

                using ResourceReader reader = new ResourceReader(resourceStream);

                foreach (DictionaryEntry entry in reader)
                {
                    var resourceUrl = entry.Key as string;
                    if(resourceUrl == null)
                        continue;

                    if (!resourceUrl.StartsWith("themes/")) 
                        continue;

                    var readStream = entry.Value as Stream;
                    if (readStream == null)
                        continue;

                    var themeName = Path.GetFileNameWithoutExtension(resourceUrl);

                    var loadedObject = XamlReader.Load(new Baml2006Reader(readStream));
                    if (!(loadedObject is ResourceDictionary dictionary))
                        continue;

                    yield return new Theme(themeName, dictionary);
                }
            }
        }

    }
}
