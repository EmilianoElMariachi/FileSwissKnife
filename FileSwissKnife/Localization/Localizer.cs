using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSwissKnife.Localization
{
    public class Localizer : ILocalizationKeys
    {

        private static List<ILocalization> _localizations = new List<ILocalization>();

        static Localizer()
        {

            var localizationInterfaceType = typeof(ILocalization);
            var implTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => localizationInterfaceType.IsAssignableFrom(t) && t != localizationInterfaceType);

            foreach (var implType in implTypes)
            {
                var localization = (ILocalization)implType.GetConstructor(new Type[0]).Invoke(null);
                _localizations.Add(localization);
            }


            foreach (var localization in _localizations)
            {
                if (localization.CultureName == System.Threading.Thread.CurrentThread.CurrentUICulture.Name)
                {
                    CurrentLocalization = localization;
                    break;
                }
            }

        }

        public static ILocalization? CurrentLocalization { get; set; }



        public string DragMeSomeFile => CurrentLocalization?.DragMeSomeFile ?? "Drag me a file here :)";


    }
}
