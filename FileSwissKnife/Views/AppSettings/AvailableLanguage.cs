using System;
using FileSwissKnife.Localization;

namespace FileSwissKnife.Views.AppSettings
{
    public class AvailableLanguage : Language
    {
        public AvailableLanguage(ILocalization localization)
        {
            Localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public override string Name => Localization.DisplayName;

        public ILocalization Localization { get; }
    }
}