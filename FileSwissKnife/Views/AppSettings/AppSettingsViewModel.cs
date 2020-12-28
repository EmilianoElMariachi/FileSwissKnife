using FileSwissKnife.Localization;

namespace FileSwissKnife.Views
{
    public class AppSettingsViewModel : TabViewModelBase
    {
        public override string DisplayName => Localizer.Instance.TabNameSettings;

        public override string TechName => "Settings";
    }
}