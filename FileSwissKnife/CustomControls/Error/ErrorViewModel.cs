using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.CustomControls.Error
{
    public class ErrorViewModel : ViewModelBase
    {
        private string? _message;

        public string? Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

    }
}