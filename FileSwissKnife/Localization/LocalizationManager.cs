using System.ComponentModel;
using System.Linq;
using FileSwissKnife.Properties;
using FileSwissKnife.Utils.MVVM.Localization;

namespace FileSwissKnife.Localization
{

    public class LocalizationManager : LocalizationManager<ILocalizationKeys>
    {

        private LocalizationManager()
        {
            InitializeFromSettings();

            Settings.Default.PropertyChanged += OnSettingsChanged;
        }

        private void InitializeFromSettings()
        {
            var localization = AvailableLocalizations.FirstOrDefault(localization => localization.CultureName == Settings.Default.SelectedLanguage);
            if (localization != null)
                Current = localization;
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            InitializeFromSettings();
        }

        protected override void OnLocalizationChanged()
        {
            Settings.Default.SelectedLanguage = Current.CultureName;
        }

        /// <summary>
        /// Singleton
        /// </summary>
        public static LocalizationManager Instance { get; } = new LocalizationManager();

    }

}
