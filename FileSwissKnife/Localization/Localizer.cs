using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using FileSwissKnife.Properties;

namespace FileSwissKnife.Localization
{
    public class Localizer : ILocalizationKeys, INotifyPropertyChanged
    {

        /// <summary>
        /// Singleton
        /// </summary>
        public static Localizer Instance { get; } = new Localizer();
        private readonly string _defaultSystemCultureName = Thread.CurrentThread.CurrentUICulture.Name;

        private readonly ILocalization _fallbackLocalization;
        private readonly List<ILocalization> _localizations = new List<ILocalization>();
        private ILocalization _current;

        public event LocalizationChangedHandler? LocalizationChanged;

        private Localizer()
        {
            _fallbackLocalization = new AvailableLocalization(new Localization_EN());
            var availableLocalization = new AvailableLocalization(new Localization_FR());

            var availableLocalizations = new List<ILocalization>
            {
                _fallbackLocalization,
                availableLocalization,
            };

            var localizationAuto = BuildLocalizationAuto(availableLocalizations, _fallbackLocalization);
            _localizations.Add(localizationAuto);
            _localizations.AddRange(availableLocalizations);

            _current = InitializeFromSettings();

            Settings.Default.PropertyChanged += OnSettingsChanged;
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Default.SelectedLanguage))
                SetLocalizationInternal(InitializeFromSettings());
        }

        private ILocalization InitializeFromSettings()
        {
            var localization = _localizations.FirstOrDefault(localization => localization.CultureName == Settings.Default.SelectedLanguage);
            return localization ?? _fallbackLocalization;
        }

        private ILocalization BuildLocalizationAuto(IEnumerable<ILocalization> availableLocalizations, ILocalization fallbackLocalization)
        {
            var localizationMatchingSystemCulture = availableLocalizations.FirstOrDefault(localization => localization.CultureName == _defaultSystemCultureName);
            if (localizationMatchingSystemCulture != null)
                return new AutoLocalization(localizationMatchingSystemCulture.Keys);

            return new AutoLocalization(fallbackLocalization.Keys);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public IEnumerable<ILocalization> AvailableLocalizations => _localizations;

        public ILocalization Current => _current;

        public void SetLocalization(string cultureName)
        {
            var localization = _localizations.FirstOrDefault(localization => localization.CultureName == cultureName);

            if (localization != null)
                SetLocalizationInternal(localization);
        }

        private void SetLocalizationInternal(ILocalization localization)
        {
            if (_current.CultureName == localization.CultureName)
                return;

            _current = localization;

            Settings.Default.SelectedLanguage = localization.CultureName;

            NotifyLocalizationChanged();
        }

        private void NotifyLocalizationChanged()
        {
            foreach (var propertyInfo in typeof(ILocalizationKeys).GetProperties())
                NotifyPropertyChanged(propertyInfo.Name);

            LocalizationChanged?.Invoke(this, new LocalizationChangedHandlerArgs(Current));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string DisplayName => Current.DisplayName;

        public string CultureName => Current.CultureName;

        public string DragMeSomeFilesToJoin => Current.Keys.DragMeSomeFilesToJoin;

        public string DragMeSomeFilesToHash => Current.Keys.DragMeSomeFilesToHash;

        public string DragMeSomeFileToSplit => Current.Keys.DragMeSomeFileToSplit;

        public string CancellingHash => Current.Keys.CancellingHash;

        public string CancelJoin => Current.Keys.CancelJoin;

        public string ConfirmCloseCancelHashTitle => Current.Keys.ConfirmCloseCancelHashTitle;

        public string HashInProgress => Current.Keys.HashInProgress;

        public string HashCanceled => Current.Keys.HashCanceled;

        public string OperationCanceled => Current.Keys.OperationCanceled;

        public string OperationFinishedIn => Current.Keys.OperationFinishedIn;

        public string JoinInputFiles => Current.Keys.JoinInputFiles;

        public string SplitInputFile => Current.Keys.SplitInputFile;

        public string SplitOutputDir => Current.Keys.SplitOutputDir;

        public string SplitSize => Current.Keys.SplitSize;

        public string JoinOutputFile => Current.Keys.JoinOutputFile;

        public string TabNameJoin => Current.Keys.TabNameJoin;

        public string TabNameSplit => Current.Keys.TabNameSplit;

        public string TabNameHash => Current.Keys.TabNameHash;

        public string OutputFileCantBeUndefined => Current.Keys.OutputFileCantBeUndefined;

        public string CanOverrideOutputFile => Current.Keys.CanOverrideOutputFile;

        public string YouChooseNotToOverride => Current.Keys.YouChooseNotToOverride;

        public string Override => Current.Keys.Override;

        public string HashInputFiles => Current.Keys.HashInputFiles;

        public string ConfirmCloseCancelHash => Current.Keys.ConfirmCloseCancelHash;

        public string UnitB => Current.Keys.UnitB;

        public string UnitKB => Current.Keys.UnitKB;

        public string UnitKiB => Current.Keys.UnitKiB;

        public string UnitMB => Current.Keys.UnitMB;

        public string UnitMiB => Current.Keys.UnitMiB;

        public string UnitGB => Current.Keys.UnitGB;

        public string UnitGiB => Current.Keys.UnitGiB;

        public string SplitSizeShouldBeDefined => Current.Keys.SplitSizeShouldBeDefined;

        public string SplitSizeInvalid => Current.Keys.SplitSizeInvalid;

        public string SplitUnitShouldBeSelected => Current.Keys.SplitUnitShouldBeSelected;

        public string BrowseFilesToHashTitle => Current.Keys.BrowseFilesToHashTitle;

        public string CopyErrors => Current.Keys.CopyErrors;

        public string BrowseSplitOutputDirTitle => Current.Keys.BrowseSplitOutputDirTitle;

        public string BrowseFileToSplitTitle => Current.Keys.BrowseFileToSplitTitle;

        public string SplitNumPosBeforeBaseName => Current.Keys.SplitNumPosBeforeBaseName;

        public string SplitNumPosAfterBaseName => Current.Keys.SplitNumPosAfterBaseName;

        public string SplitNumPosBeforeExt => Current.Keys.SplitNumPosBeforeExt;

        public string SplitNumPosAfterExt => Current.Keys.SplitNumPosAfterExt;

        public string SplitErrorFileSizeLessThanSplitSize => Current.Keys.SplitErrorFileSizeLessThanSplitSize;

        public string SplitNumPosShouldBeSelected => Current.Keys.SplitNumPosShouldBeSelected;

        public string SplitStartNumberShouldBeDefined => Current.Keys.SplitStartNumberShouldBeDefined;

        public string SplitStartNumberInvalid => Current.Keys.SplitStartNumberInvalid;

        public string SplitInputFileNotFound => Current.Keys.SplitInputFileNotFound;

        public string SplitNamingOptions => Current.Keys.SplitNamingOptions;

        public string SplitNamingPreview => Current.Keys.SplitNamingPreview;

        public string SplitNamingNumPrefix => Current.Keys.SplitNamingNumPrefix;

        public string SplitNamingStartNum => Current.Keys.SplitNamingStartNum;

        public string SplitNamingPadWithZeros => Current.Keys.SplitNamingPadWithZeros;

        public string SplitNamingNumSuffix => Current.Keys.SplitNamingNumSuffix;

        public string SplitNamingNumPos => Current.Keys.SplitNamingNumPos;

        public string BrowseJoinOutputFileTitle => Current.Keys.BrowseJoinOutputFileTitle;

        public string BrowseJoinInputFilesTitle => Current.Keys.BrowseJoinInputFilesTitle;

        public string TabNameSettings => Current.Keys.TabNameSettings;

        public string Language => Current.Keys.Language;

        public string Theme => Current.Keys.Theme;

        public string JoinSettings => Current.Keys.JoinSettings;

        public string JoinBufferSize => Current.Keys.JoinBufferSize;

        public string JoinGuessMissingFiles => Current.Keys.JoinGuessMissingFiles;

        public string SplitSettings => Current.Keys.SplitSettings;

        public string SplitBufferSize => Current.Keys.SplitBufferSize;

        public string HashSettings => Current.Keys.HashSettings;

        public string HashBufferSize => Current.Keys.HashBufferSize;

        public string ResetSettings => Current.Keys.ResetSettings;

    }

    public delegate void LocalizationChangedHandler(object sender, LocalizationChangedHandlerArgs args);

    public class LocalizationChangedHandlerArgs
    {
        public LocalizationChangedHandlerArgs(ILocalization newLocalization)
        {
            NewLocalization = newLocalization;
        }

        public ILocalization NewLocalization { get; }
    }
}
