using FileSwissKnife.CustomControls.Error;
using FileSwissKnife.Localization;
using FileSwissKnife.Properties;

namespace FileSwissKnife.Views.Splitting.Validators
{
    internal class SplitSizeValidator
    {
        private string _editedValue;
        private readonly StaticErrorViewModel _error;


        public SplitSizeValidator(ErrorsCollection errors)
        {
            _error = new StaticErrorViewModel(errors);
            _editedValue = Settings.Default.SplitSize.ToString();
            _error.IsFixed = true;
        }

        public bool TryGetSplitSize(out long splitSize)
        {
            if (_error.IsFixed)
            {
                splitSize = Settings.Default.SplitSize;
                return true;
            }
            else
            {
                _error.Show();
                splitSize = default;
                return false;
            }
        }

        public string EditedValue
        {
            get => _editedValue;
            set
            {
                _editedValue = value;
                _error.IsFixed = false;

                if (string.IsNullOrWhiteSpace(_editedValue))
                {
                    _error.Show(LocalizationManager.Instance.Current.Keys.SplitSizeShouldBeDefined);
                    return;
                }

                if (!long.TryParse(_editedValue, out var splitSize) || splitSize <= 0)
                {
                    _error.Show(string.Format(LocalizationManager.Instance.Current.Keys.SplitSizeInvalid, _editedValue));
                    return;
                }

                _error.Hide();

                Settings.Default.SplitSize = splitSize;
                _error.IsFixed = true;
            }
        }

    }
}