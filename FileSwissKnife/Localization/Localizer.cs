using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FileSwissKnife.Localization
{
    public class Localizer : ILocalizationKeys, INotifyPropertyChanged
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static Localizer Instance { get; } = new Localizer();


        private readonly Localization_EN _defaultLocalization = new Localization_EN();
        private readonly List<ILocalization> _localizations = new List<ILocalization>();
        private ILocalization _current;

        public event LocalizationChangedHandler LocalizationChanged;

        private Localizer()
        {
            _localizations.AddRange(new ILocalization[]
            {
                _defaultLocalization,
                new Localization_FR()
            });

            var initialLocalization = _localizations.FirstOrDefault(localization => localization.CultureName == System.Threading.Thread.CurrentThread.CurrentUICulture.Name);

            _current = initialLocalization ?? _defaultLocalization;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<ILocalization> AvailableLocalizations => _localizations;

        public ILocalization Current
        {
            get => _current;
            set
            {
                _current = value ?? throw new ArgumentNullException(nameof(Current));
                NotifyLocalizationChanged();
            }
        }

        private void NotifyLocalizationChanged()
        {
            foreach (var propertyInfo in typeof(ILocalizationKeys).GetProperties())
            {
                NotifyPropertyChanged(propertyInfo.Name);
            }
            LocalizationChanged?.Invoke(this, new LocalizationChangedHandlerArgs(this.Current));
        }
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string DragMeSomeFilesToJoin => Current.DragMeSomeFilesToJoin;

        public string Start => Current.Start;

        public string Cancelling => Current.Cancelling;

        public string Cancel => Current.Cancel;

        public string OperationCanceled => Current.OperationCanceled;

        public string OperationFinishedIn => Current.OperationFinishedIn;

        public string OperationError => Current.OperationError;

        public string JoinInputFiles => Current.JoinInputFiles;

        public string JoinOutputFile => Current.JoinOutputFile;

        public string TabNameJoin => Current.TabNameJoin;

        public string TabNameSplit => Current.TabNameSplit;

        public string TabNameHash => Current.TabNameHash;

        public string HideError => Current.HideError;

        public string OutputFileCantBeUndefined => Current.OutputFileCantBeUndefined;

        public string CanOverrideOutputFile => Current.CanOverrideOutputFile;

        public string YouChooseNotToOverride => Current.YouChooseNotToOverride;

        public string Override => Current.Override;
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
