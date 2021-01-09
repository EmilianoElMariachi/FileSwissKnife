using System;

namespace FileSwissKnife.Localization
{

    public interface ILocalization
    {
        string DisplayName { get; }

        string CultureName { get; }

        ILocalizationKeys Keys { get; }
    }

    public abstract class LocalizationBase : ILocalization
    {
        public abstract string DisplayName { get; }

        public abstract string CultureName { get; }

        public abstract ILocalizationKeys Keys { get; }
    }

    public class AvailableLocalization : LocalizationBase
    {
        private readonly ILocalizationKeys _keys;

        public AvailableLocalization(ILocalizationKeys keys)
        {
            _keys = keys ?? throw new ArgumentNullException(nameof(keys));
        }

        public override ILocalizationKeys Keys => _keys;

        public override string DisplayName => _keys.DisplayName;

        public override string CultureName => _keys.CultureName;
    }

    public class AutoLocalization : LocalizationBase
    {
        public AutoLocalization(ILocalizationKeys keys)
        {
            Keys = keys ?? throw new ArgumentNullException(nameof(keys));
        }

        public override string DisplayName => "Auto";

        public override string CultureName => "Auto";

        public override ILocalizationKeys Keys { get; }
    }

}