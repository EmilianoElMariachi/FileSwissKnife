namespace FileSwissKnife.Localization
{
    public interface ILocalization : ILocalizationKeys
    {
        string Name { get; }

        string CultureName { get; }
    }
}