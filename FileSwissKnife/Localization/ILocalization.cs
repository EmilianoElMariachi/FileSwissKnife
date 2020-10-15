namespace FileSwissKnife.Localization
{
    public interface ILocalization : ILocalizationKeys
    {
        string CultureName { get; }
    }
}