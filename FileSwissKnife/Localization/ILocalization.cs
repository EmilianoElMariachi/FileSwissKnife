namespace FileSwissKnife.Localization
{
    public interface ILocalization : ILocalizationKeys
    {
        string DisplayName { get; }

        string CultureName { get; }
    }
}