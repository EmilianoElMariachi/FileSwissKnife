using System;
using System.IO;
using System.Linq;
using System.Threading;
using FileSwissKnife.CustomControls;
using FileSwissKnife.Localization;
using FileSwissKnife.Properties;
using FileSwissKnife.Utils;
using FileSwissKnife.Utils.MVVM;
using FileSwissKnife.Utils.UnitsManagement;

namespace FileSwissKnife.ViewModels
{
    public class SplitViewModel : TabViewModelBase, IFilesDropped
    {
        private string _inputFile;
        private string _splitSizeStr;
        private long? _splitSize;
        private Unit? _selectedUnit;
        private string _outputFolder;
        private readonly ErrorViewModel _splitSizeError = new ErrorViewModel(false);
        private readonly NoSelectedUnitErrorViewModel _noSelectedUnitError = new NoSelectedUnitErrorViewModel();

        private CancellationTokenSource? _cancellationTokenSource;

        public SplitViewModel()
        {
            SplitOrCancelCommand = new RelayCommand(SplitOrCancel, CanSplitOrCancel);

            SplitSizeStr = Settings.Default.SplitSize.ToString();
            SelectedUnit = Units.All.FirstOrDefault(unit => unit.SIUnit == Settings.Default.SplitUnit);
        }

        public override string DisplayName => Localizer.Instance.TabNameSplit;

        public override string TechName => "Split";

        public Units Units => Units.All;

        public string InputFile
        {
            get => _inputFile;
            set
            {
                _inputFile = value;
                NotifyPropertyChanged();
            }
        }

        public string SplitSizeStr
        {
            get => _splitSizeStr;
            set
            {
                _splitSizeStr = value;
                _splitSize = null;

                UpdateSplitSize();

                NotifyPropertyChanged();
            }
        }

        private void UpdateSplitSize()
        {
            var errors = Errors;

            _splitSize = null;
            try
            {
                var splitSizeStr = SplitSizeStr;
                if (string.IsNullOrWhiteSpace(splitSizeStr))
                    throw new Exception(Localizer.Instance.SplitSizeShouldBeDefined);

                if (!long.TryParse(splitSizeStr, out var splitSize) || splitSize <= 0)
                    throw new Exception(string.Format(Localizer.Instance.SplitSizeInvalid, splitSizeStr));

                _splitSize = splitSize;
                Settings.Default.SplitSize = splitSize;
                errors.Remove(_splitSizeError);
            }
            catch (Exception ex)
            {
                _splitSizeError.Message = ex.Message;
                errors.Add(_splitSizeError);
            }

            SplitOrCancelCommand.TriggerCanExecuteChanged();
        }

        public Unit? SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;

                if (_selectedUnit == null)
                    Errors.Add(_noSelectedUnitError);
                else
                {
                    Settings.Default.SplitUnit = _selectedUnit.SIUnit;
                    Errors.Remove(_noSelectedUnitError);
                }

                NotifyPropertyChanged();
            }
        }

        public ErrorsCollection Errors { get; } = new ErrorsCollection();

        public string OutputFolder
        {
            get => _outputFolder;
            set
            {
                _outputFolder = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand SplitOrCancelCommand { get; }

        private async void SplitOrCancel()
        {
            var errors = Errors;

            try
            {
                errors.Clear();

                var fileSplitter = new FileSplitter();
                _cancellationTokenSource = new CancellationTokenSource();
                var splitSize = SelectedUnit.ToNbBytes(_splitSize.Value);
                await fileSplitter.Split(InputFile, OutputFolder, splitSize, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                errors.Add(new ErrorViewModel(true)
                {
                    Message = ex.Message
                });
            }
            finally
            {
                _cancellationTokenSource = null;
            }


            //TODO: à implémenter
        }

        private bool CanSplitOrCancel()
        {
            return _splitSize != null;
        }

        private class NoSelectedUnitErrorViewModel : ErrorViewModel
        {
            public NoSelectedUnitErrorViewModel() : base(false)
            {
                Localizer.Instance.LocalizationChanged += (sender, args) =>
                {
                    Message = Localizer.Instance.SplitUnitShouldBeSelected;
                };

                Message = Localizer.Instance.SplitUnitShouldBeSelected;
            }
        }

        public void OnFilesDropped(string[] files)
        {
            if (files.Length <= 0) 
                return;
            var file = files[0];
            if (!File.Exists(file))
                return;

            var dir = Path.GetDirectoryName(file);
            this.OutputFolder = dir;

            InputFile = file;
        }
    }

}