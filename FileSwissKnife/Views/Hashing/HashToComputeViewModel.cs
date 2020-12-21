using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.Views.Hashing
{
    public class HashToComputeViewModel : ViewModelBase
    {
        private bool _isComputed;

        public HashToComputeViewModel(string hashName)
        {
            HashName = hashName;
        }

        public string HashName { get; }

        public bool IsComputed
        {
            get => _isComputed;
            set
            {
                _isComputed = value;
                NotifyPropertyChanged();
            }
        }
    }
}