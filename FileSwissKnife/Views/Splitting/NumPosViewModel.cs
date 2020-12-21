using ElMariachi.FS.Tools.Splitting;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.Views.Splitting
{
    public class NumPosViewModel: ViewModelBase
    {
        private string _numPosText;

        public NumPosViewModel(NumPos numPos)
        {
            Localizer.Instance.LocalizationChanged += OnLocalizationChanged;
            NumPos = numPos;
            Initialize();
        }

        private void Initialize()
        {
            switch (NumPos)
            {
                case NumPos.BeforeBaseName:
                    NumPosText = Localizer.Instance.SplitNumPosBeforeBaseName;
                    break;
                case NumPos.AfterBaseName:
                    NumPosText = Localizer.Instance.SplitNumPosAfterBaseName;
                    break;
                case NumPos.BeforeExt:
                    NumPosText = Localizer.Instance.SplitNumPosBeforeExt;
                    break;
                case NumPos.AfterExt:
                    NumPosText = Localizer.Instance.SplitNumPosAfterExt;
                    break;
                default:
                    NumPosText = "UNKNOWN";
                    break;
            }
        }

        private void OnLocalizationChanged(object sender, LocalizationChangedHandlerArgs args)
        {
            Initialize();
        }

        public NumPos NumPos { get; }

        public string NumPosText
        {
            get => _numPosText;
            set
            {
                _numPosText = value;
                NotifyPropertyChanged();
            }
        }
    }
}