using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSwissKnife.Localization
{
    public class Localizer : ILocalizationKeys
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static Localizer Instance { get; } = new Localizer();


        private readonly Localization_EN _defaultLocalization = new Localization_EN();
        private readonly List<ILocalization> _localizations = new List<ILocalization>();
        private ILocalization _currentLocalization;


        private Localizer()
        {
            _localizations.AddRange(new ILocalization[]
            {
                _defaultLocalization,
                new Localization_FR()
            });

            var initialLocalization = _localizations.FirstOrDefault(localization => localization.CultureName == System.Threading.Thread.CurrentThread.CurrentUICulture.Name);

            _currentLocalization = initialLocalization ?? _defaultLocalization;
        }

        public IEnumerable<ILocalization> AvailableLocalizations => _localizations;

        public ILocalization CurrentLocalization
        {
            get => _currentLocalization;
            set => _currentLocalization = value ?? throw new ArgumentNullException(nameof(CurrentLocalization));
        }

        public string DragMeSomeFilesToJoin => CurrentLocalization.DragMeSomeFilesToJoin;
    }
}
