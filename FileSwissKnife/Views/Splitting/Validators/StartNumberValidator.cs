using FileSwissKnife.CustomControls.Error;
using FileSwissKnife.Localization;
using FileSwissKnife.Properties;

namespace FileSwissKnife.Views.Splitting.Validators
{
    internal class StartNumberValidator
    {
        private string _editedValue;
        private readonly StaticErrorViewModel _error;


        public StartNumberValidator(ErrorsCollection errors)
        {
            _error = new StaticErrorViewModel(errors);
            _editedValue = Settings.Default.SplitStartNumber.ToString();
            _error.IsFixed = true;
        }

        public bool TryGetStartNumber(out long startNumber)
        {
            if (_error.IsFixed)
            {
                startNumber = Settings.Default.SplitStartNumber;
                return true;
            }
            else
            {
                _error.Show();
                startNumber = default;
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
                    _error.Show(LocalizationManager.Instance.Current.Keys.SplitStartNumberShouldBeDefined);
                    return;
                }

                if (!int.TryParse(_editedValue, out var startNumber))
                {
                    _error.Show(string.Format(LocalizationManager.Instance.Current.Keys.SplitStartNumberInvalid, _editedValue));
                    return;
                }

                _error.Hide();

                Settings.Default.SplitStartNumber = startNumber;
                _error.IsFixed = true;
            }
        }

    }
}