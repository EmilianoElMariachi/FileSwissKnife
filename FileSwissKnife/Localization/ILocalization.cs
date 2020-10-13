namespace FileSwissKnife.Localization
{
    public interface ILocalization : ILocalizationKeys
    {
        public string CultureName { get; }
    }
}