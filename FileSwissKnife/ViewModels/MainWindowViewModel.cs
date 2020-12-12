using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase? _selectedViewModel;

        public ViewModelBase? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public JoinViewModel JoinViewModel { get; } = new JoinViewModel();

        public SplitViewModel SplitViewModel { get; } = new SplitViewModel();

        public HashViewModel HashViewModel { get; } = new HashViewModel();
    }
}
