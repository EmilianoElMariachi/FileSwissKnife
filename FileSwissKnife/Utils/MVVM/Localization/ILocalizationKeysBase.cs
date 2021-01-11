namespace FileSwissKnife.Utils.MVVM.Localization
{

    /// <summary>
    /// Represents a set of localized keys
    /// </summary>
    public interface ILocalizationKeysBase
    {
        /// <summary>
        /// True if those keys should be used when no better keys are found
        /// </summary>
        bool IsFallback { get; }

        /// <summary>
        /// The localized language name
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// The culture name corresponding to the language of those keys
        /// </summary>
        string CultureName { get; }

        /// <summary>
        /// Localization of «Automatic»
        /// </summary>
        string LanguageAuto { get; }
    }
}