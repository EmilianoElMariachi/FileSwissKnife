using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.Views.Hashing
{
    public class HashToComputeViewModel : ViewModelBase
    {
        private bool _compute;

        public HashToComputeViewModel(string hashName)
        {
            HashName = hashName;
        }

        public string HashName { get; }

        public bool Compute
        {
            get => _compute;
            set
            {
                _compute = value;
                NotifyPropertyChanged();
            }
        }
    }
}