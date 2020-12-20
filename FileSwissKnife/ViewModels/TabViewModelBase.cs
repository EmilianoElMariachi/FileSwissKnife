using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.ViewModels
{
    public abstract class TabViewModelBase : ViewModelBase
    {
        public abstract string DisplayName { get; }

        public abstract string TechName { get; }
    }
}