using FileSwissKnife.Localization;

namespace FileSwissKnife.ViewModels
{
    public class SplitViewModel: TabViewModelBase
    {
        public override string DisplayName => Localizer.Instance.TabNameSplit;

        public override string TechName => "Split";
    }
}