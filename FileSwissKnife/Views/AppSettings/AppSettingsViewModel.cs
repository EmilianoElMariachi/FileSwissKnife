using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using FileSwissKnife.Localization;
using FileSwissKnife.Properties;
using FileSwissKnife.Themes;
using FileSwissKnife.Utils;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.Views.AppSettings
{
    public class AppSettingsViewModel : TabViewModelBase
    {
        public AppSettingsViewModel()
        {
            OpenRepositoryCommand = new RelayCommand(OpenRepository);
            ResetDefaultSettingsCommand = new RelayCommand(ResetDefaultSettings);

            Localizer.Instance.LocalizationChanged += OnLocalizationChanged;
        }

        private void OnLocalizationChanged(object sender, LocalizationChangedHandlerArgs args)
        {
            NotifyPropertyChanged(nameof(SelectedLanguage));
        }

        public override string TabId => "Settings";

        public IEnumerable<ILocalization> AvailableLanguages => Localizer.Instance.AvailableLocalizations;

        public ILocalization SelectedLanguage
        {
            get => Localizer.Instance.Current;
            set
            {
                if (value != null) 
                    Localizer.Instance.SetLocalization(value.CultureName);
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<Theme> AvailableThemes => ThemeManager.AvailableThemes;

        public Theme? SelectedTheme
        {
            get => ThemeManager.CurrentTheme;
            set
            {
                NotifyPropertyChanged();
                if (value != null)
                    ThemeManager.SetTheme(value);
            }
        }

        public int JoinBufferSize
        {
            get => Settings.Default.JoinBufferSize;
            set
            {
                Settings.Default.JoinBufferSize = value;
                NotifyPropertyChanged();
            }
        }

        public bool JoinGuessMissingFiles
        {
            get => Settings.Default.JoinGuessMissingFiles;
            set
            {
                Settings.Default.JoinGuessMissingFiles = value;
                NotifyPropertyChanged();
            }
        }

        public int SplitBufferSize
        {
            get => Settings.Default.SplitBufferSize;
            set
            {
                Settings.Default.SplitBufferSize = value;
                NotifyPropertyChanged();
            }
        }

        public int HashBufferSize
        {
            get => Settings.Default.HashBufferSize;
            set
            {
                Settings.Default.HashBufferSize = value;
                NotifyPropertyChanged();
            }
        }

        public string Branding => $"By {AppInfo.Company}";

        public ICommand OpenRepositoryCommand { get; }

        public ICommand ResetDefaultSettingsCommand { get; }

        private void OpenRepository()
        {

            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/EmilianoElMariachi/FileSwissKnife/releases") { UseShellExecute = true });
            }
            catch
            {

            }
        }

        private void ResetDefaultSettings()
        {
            Settings.Default.Reset();
            RefreshBindings();
        }

        private void RefreshBindings()
        {
            var propertyNames = this.GetType()
                .GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name);
            foreach (var propertyName in propertyNames)
            {
                NotifyPropertyChanged(propertyName);
            }
        }
    }
}