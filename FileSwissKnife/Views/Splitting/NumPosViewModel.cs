using ElMariachi.FS.Tools.Splitting;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils.MVVM;
using FileSwissKnife.Utils.MVVM.Localization;

namespace FileSwissKnife.Views.Splitting
{
    public class NumPosViewModel: ViewModelBase
    {
        private string _numPosText;

        public NumPosViewModel(NumPos numPos)
        {
            LocalizationManager.Instance.LocalizationChanged += OnLocalizationChanged;
            NumPos = numPos;
            Initialize();
        }

        private void Initialize()
        {
            switch (NumPos)
            {
                case NumPos.BeforeBaseName:
                    NumPosText = LocalizationManager.Instance.Current.Keys.SplitNumPosBeforeBaseName;
                    break;
                case NumPos.AfterBaseName:
                    NumPosText = LocalizationManager.Instance.Current.Keys.SplitNumPosAfterBaseName;
                    break;
                case NumPos.BeforeExt:
                    NumPosText = LocalizationManager.Instance.Current.Keys.SplitNumPosBeforeExt;
                    break;
                case NumPos.AfterExt:
                    NumPosText = LocalizationManager.Instance.Current.Keys.SplitNumPosAfterExt;
                    break;
                default:
                    NumPosText = "UNKNOWN";
                    break;
            }
        }

        private void OnLocalizationChanged(object sender, LocalizationChangedHandlerArgs<ILocalizationKeys> args)
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