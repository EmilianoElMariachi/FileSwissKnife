﻿using System.Collections.Generic;
using System.Linq;
using FileSwissKnife.Localization;
using FileSwissKnife.Properties;

namespace FileSwissKnife.Views.AppSettings
{
    public class AppSettingsViewModel : TabViewModelBase
    {
        private Language? _selectedLanguage;

        public AppSettingsViewModel()
        {
            var autoLanguage = new AutoLanguage();
            var languages = new List<Language>
            {
                autoLanguage
            };

            var availableLanguages = Localizer.Instance.AvailableLocalizations.Select(availableLocalization => new AvailableLanguage(availableLocalization)).ToArray();
            languages.AddRange(availableLanguages);

            AvailableLanguages = languages;

            if (Localizer.Instance.Mode == LocalizationMode.Auto)
                SelectedLanguage = autoLanguage;
            else
                SelectedLanguage = availableLanguages.FirstOrDefault(language => language.Localization == Localizer.Instance.Current);
        }

        public override string TabId => "Settings";

        public IEnumerable<Language> AvailableLanguages { get; }

        public Language? SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                if (value != null)
                {
                    switch (value)
                    {
                        case AutoLanguage _:
                            Localizer.Instance.SetLocalizationAuto();
                            break;
                        case AvailableLanguage availableLanguage:
                            Localizer.Instance.SetForcedLocalization(availableLanguage.Localization.CultureName);
                            break;
                    }
                }
                NotifyPropertyChanged();
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
    }
}