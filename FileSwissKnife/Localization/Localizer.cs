using System;
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
        private static readonly string _defaultSystemCultureName = Thread.CurrentThread.CurrentUICulture.Name;


        private readonly Localization_EN _defaultLocalization = new Localization_EN();
        private readonly List<ILocalization> _localizations = new List<ILocalization>();
        private ILocalization _current;

        public event LocalizationChangedHandler? LocalizationChanged;

        private Localizer()
        {
            _localizations.AddRange(new ILocalization[]
            {
                _defaultLocalization,
                new Localization_FR()
            });

            _current = GetInitialLocalization();
        }

        private ILocalization GetInitialLocalization()
        {
            var forcedLocalization = _localizations.FirstOrDefault(localization => localization.CultureName == Settings.Default.ForcedLanguage);
            return forcedLocalization ?? GetLocalizationAuto();
        }

        private ILocalization GetLocalizationAuto()
        {
            var systemLocalization = _localizations.FirstOrDefault(localization => localization.CultureName == _defaultSystemCultureName);
            return systemLocalization ?? _defaultLocalization;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public IEnumerable<ILocalization> AvailableLocalizations => _localizations;

        public LocalizationMode Mode => string.IsNullOrWhiteSpace(Settings.Default.ForcedLanguage) ? LocalizationMode.Auto : LocalizationMode.Forced;

        public ILocalization Current
        {
            get => _current;
            private set
            {
                _current = value ?? throw new ArgumentNullException(nameof(Current));
                NotifyLocalizationChanged();
            }
        }

        public void SetForcedLocalization(string cultureName)
        {
            var forcedLocalization = _localizations.FirstOrDefault(localization => localization.CultureName == cultureName);

            if (forcedLocalization != null)
                Current = forcedLocalization;

            Settings.Default.ForcedLanguage = cultureName;
        }

        public void SetLocalizationAuto()
        {
            Current = GetLocalizationAuto();
            Settings.Default.ForcedLanguage = "";
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

        public string DragMeSomeFilesToJoin => Current.DragMeSomeFilesToJoin;

        public string DragMeSomeFilesToHash => Current.DragMeSomeFilesToHash;

        public string DragMeSomeFileToSplit => Current.DragMeSomeFileToSplit;

        public string CancellingHash => Current.CancellingHash;

        public string CancelJoin => Current.CancelJoin;

        public string ConfirmCloseCancelHashTitle => Current.ConfirmCloseCancelHashTitle;

        public string HashInProgress => Current.HashInProgress;

        public string HashCanceled => Current.HashCanceled;

        public string OperationCanceled => Current.OperationCanceled;

        public string OperationFinishedIn => Current.OperationFinishedIn;

        public string OperationError => Current.OperationError;

        public string JoinInputFiles => Current.JoinInputFiles;

        public string SplitInputFile => Current.SplitInputFile;

        public string SplitOutputDir => Current.SplitOutputDir;

        public string SplitSize => Current.SplitSize;

        public string JoinOutputFile => Current.JoinOutputFile;

        public string TabNameJoin => Current.TabNameJoin;

        public string TabNameSplit => Current.TabNameSplit;

        public string TabNameHash => Current.TabNameHash;

        public string OutputFileCantBeUndefined => Current.OutputFileCantBeUndefined;

        public string CanOverrideOutputFile => Current.CanOverrideOutputFile;

        public string YouChooseNotToOverride => Current.YouChooseNotToOverride;

        public string Override => Current.Override;

        public string HashInputFiles => Current.HashInputFiles;

        public string ConfirmCloseCancelHash => Current.ConfirmCloseCancelHash;

        public string UnitB => Current.UnitB;

        public string UnitKB => Current.UnitKB;

        public string UnitKiB => Current.UnitKiB;

        public string UnitMB => Current.UnitMB;

        public string UnitMiB => Current.UnitMiB;

        public string UnitGB => Current.UnitGB;

        public string UnitGiB => Current.UnitGiB;

        public string SplitSizeShouldBeDefined => Current.SplitSizeShouldBeDefined;

        public string SplitSizeInvalid => Current.SplitSizeInvalid;

        public string SplitUnitShouldBeSelected => Current.SplitUnitShouldBeSelected;

        public string BrowseFilesToHashTitle => Current.BrowseFilesToHashTitle;

        public string CopyErrors => Current.CopyErrors;

        public string BrowseSplitOutputDirTitle => Current.BrowseSplitOutputDirTitle;

        public string BrowseFileToSplitTitle => Current.BrowseFileToSplitTitle;

        public string SplitNumPosBeforeBaseName => Current.SplitNumPosBeforeBaseName;

        public string SplitNumPosAfterBaseName => Current.SplitNumPosAfterBaseName;

        public string SplitNumPosBeforeExt => Current.SplitNumPosBeforeExt;

        public string SplitNumPosAfterExt => Current.SplitNumPosAfterExt;

        public string SplitErrorFileSizeLessThanSplitSize => Current.SplitErrorFileSizeLessThanSplitSize;

        public string SplitNumPosShouldBeSelected => Current.SplitNumPosShouldBeSelected;

        public string SplitStartNumberShouldBeDefined => Current.SplitStartNumberShouldBeDefined;

        public string SplitStartNumberInvalid => Current.SplitStartNumberInvalid;

        public string SplitInputFileNotFound => Current.SplitInputFileNotFound;

        public string SplitNamingOptions => Current.SplitNamingOptions;

        public string SplitNamingPreview => Current.SplitNamingPreview;

        public string SplitNamingNumPrefix => Current.SplitNamingNumPrefix;

        public string SplitNamingStartNum => Current.SplitNamingStartNum;

        public string SplitNamingPadWithZeros => Current.SplitNamingPadWithZeros;

        public string SplitNamingNumSuffix => Current.SplitNamingNumSuffix;

        public string SplitNamingNumPos => Current.SplitNamingNumPos;

        public string BrowseJoinOutputFileTitle => Current.BrowseJoinOutputFileTitle;

        public string BrowseJoinInputFilesTitle => Current.BrowseJoinInputFilesTitle;

        public string TabNameSettings => Current.TabNameSettings;

        public string Language => Current.Language;
    }

    public enum LocalizationMode
    {
        Auto,
        Forced
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
