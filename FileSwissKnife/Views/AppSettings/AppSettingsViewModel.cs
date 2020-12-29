using System.Collections.Generic;
using FileSwissKnife.Localization;

namespace FileSwissKnife.Views.AppSettings
{
    public class AppSettingsViewModel : TabViewModelBase
    {
        private ILocalization _selectedLocalization;

        public AppSettingsViewModel()
        {
            SelectedLocalization = Localizer.Instance.Current;
        }

        public override string TechName => "Settings";

        public IEnumerable<ILocalization> AvailableLocalizations => Localizer.Instance.AvailableLocalizations;

        public ILocalization SelectedLocalization
        {
            get => _selectedLocalization;
            set
            {
                _selectedLocalization = value;
                Localizer.Instance.Current = value;
                NotifyPropertyChanged();
            }
        }
    }
}