using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ElMariachi.FS.Tools.Splitting;
using FileSwissKnife.CustomControls;
using FileSwissKnife.CustomControls.Error;
using FileSwissKnife.Localization;
using FileSwissKnife.Properties;
using FileSwissKnife.Utils.MVVM;
using FileSwissKnife.Utils.UnitsManagement;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileSwissKnife.Views.Splitting
{
    public class SplitViewModel : TabViewModelBase, IFilesDropped
    {
        private string _inputFile;
        private string _splitSizeStr;
        private long? _splitSize;
        private Unit? _selectedUnit;
        private readonly StaticErrorViewModel _splitSizeUndefinedError;
        private readonly NoSelectedUnitErrorViewModel _noSelectedUnitError;

        private readonly ErrorsCollection _errors = new ErrorsCollection();

        private CancellationTokenSource? _cancellationTokenSource;
        private PlayStopButtonState _state;
        private double _progressBarValue;
        private string? _progressBarText;
        private NumPosViewModel? _selectedNumPos;
        private readonly StaticErrorViewModel _fileSizeSplitSizeError;
        private readonly NoSelectedNumPosErrorViewModel _noSelectedNumPosError;

        public SplitViewModel()
        {
            _noSelectedUnitError = new NoSelectedUnitErrorViewModel(_errors);
            _splitSizeUndefinedError = new StaticErrorViewModel(_errors);
            _fileSizeSplitSizeError = new StaticErrorViewModel(_errors);
            _noSelectedNumPosError = new NoSelectedNumPosErrorViewModel(_errors);


            SplitOrCancelCommand = new RelayCommand(SplitOrCancel);
            BrowseOutputDirCommand = new RelayCommand(BrowseOutputDir);
            BrowseInputFileCommand = new RelayCommand(BrowseInputFile);

            SplitSizeStr = Settings.Default.SplitSize.ToString();
            SelectedUnit = Units.All.FirstOrDefault(unit => unit.SIUnit == Settings.Default.SplitUnit);

            NumPositions = Enum.GetValues<NumPos>().Select(numPos => new NumPosViewModel(numPos)).ToArray();

            SelectedNumPos = NumPositions.FirstOrDefault(vm => vm.NumPos == Settings.Default.SplitNumPos);
        }

        public ICommand BrowseInputFileCommand { get; }

        public ICommand BrowseOutputDirCommand { get; }

        public RelayCommand SplitOrCancelCommand { get; }

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
                _splitSizeUndefinedError.Hide();
            }
            catch (Exception ex)
            {
                _splitSizeUndefinedError.Message = ex.Message;
                _splitSizeUndefinedError.Show();
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
                    _noSelectedUnitError.Show();
                else
                {
                    Settings.Default.SplitUnit = _selectedUnit.SIUnit;
                    _noSelectedUnitError.Hide();
                }

                NotifyPropertyChanged();
            }
        }

        public ErrorsCollection Errors => _errors;

        public NumPosViewModel? SelectedNumPos
        {
            get => _selectedNumPos;
            set
            {
                _selectedNumPos = value;

                if (value != null)
                    Settings.Default.SplitNumPos = value.NumPos;

                NotifyPropertyChanged();
            }
        }

        public string OutputDir
        {
            get => Settings.Default.SplitOutputDir;
            set
            {
                Settings.Default.SplitOutputDir = value;
                NotifyPropertyChanged();
            }
        }

        public PlayStopButtonState State
        {
            get => _state;
            set
            {
                _state = value;
                NotifyPropertyChanged();
            }
        }

        public string? ProgressBarText
        {
            get => _progressBarText;
            set
            {
                _progressBarText = value;
                NotifyPropertyChanged();
            }
        }

        public double ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                _progressBarValue = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<NumPosViewModel> NumPositions { get; }

        private void BrowseOutputDir()
        {
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = Settings.Default.SplitOutputDir,
                IsFolderPicker = true,
                Title = Localizer.Instance.BrowseSplitOutputDirTitle
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                this.OutputDir = dialog.FileName;
        }

        private void BrowseInputFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = $"{Localizer.Instance.AllFiles} (*.*)|*.*",
                Multiselect = false,
                Title = Localizer.Instance.BrowseFileToSplitTitle,
            };

            var result = openFileDialog.ShowDialog(Application.Current.MainWindow);
            if (result == null || !result.Value)
                return;

            InputFile = openFileDialog.FileName;
        }

        private async void SplitOrCancel()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                return;
            }

            var errors = Errors;
            errors.Clear();


            var selectedUnit = SelectedUnit;
            if (selectedUnit == null)
            {
                _noSelectedUnitError.Show();
                return;
            }

            var splitSize = _splitSize;
            if (splitSize == null)
            {
                _splitSizeUndefinedError.Show();
                return;
            }

            var splitSizeBytes = selectedUnit.ToNbBytes(splitSize.Value);

            var inputFile = InputFile;

            if (!File.Exists(inputFile))
            {
                _fileSizeSplitSizeError.Show("Input file not found!");
                return;
            }

            var fileInfo = new FileInfo(inputFile);

            if (fileInfo.Length <= splitSizeBytes)
            {
                _fileSizeSplitSizeError.Show(string.Format(Localizer.Instance.SplitErrorFileSizeLessThanSplitSize, splitSizeBytes, fileInfo.Length));
                return;
            }

            var selectedNumPos = this.SelectedNumPos;
            if (selectedNumPos == null)
            {
                _noSelectedNumPosError.Show();
                return;
            }

            try
            {
                State = PlayStopButtonState.Stop;
                var fileSplitter = new FileSplitter();
                _cancellationTokenSource = new CancellationTokenSource();
                fileSplitter.OnProgress += (sender, args) =>
                {
                    ProgressBarValue = args.Percent;
                    ProgressBarText = args.Message;
                };

                //TODO: implémenter l'édition
                var namingOptions = new NamingOptions
                {
                    NumPrefix = ".part",
                    NumPos = selectedNumPos.NumPos,
                    NumSuffix = "",
                    StartNumber = 1,
                };

                await fileSplitter.Split(inputFile, OutputDir, splitSizeBytes, namingOptions, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                errors.Add(new ErrorViewModel
                {
                    Message = ex.Message
                });
            }
            finally
            {
                State = PlayStopButtonState.Play;
                _cancellationTokenSource = null;
            }
        }

        private class NoSelectedUnitErrorViewModel : StaticErrorViewModel
        {
            public NoSelectedUnitErrorViewModel(ErrorsCollection errorsCollection) : base(errorsCollection)
            {
                Localizer.Instance.LocalizationChanged += (sender, args) =>
                {
                    Message = Localizer.Instance.SplitUnitShouldBeSelected;
                };

                Message = Localizer.Instance.SplitUnitShouldBeSelected;
            }
        }

        private class NoSelectedNumPosErrorViewModel : StaticErrorViewModel
        {
            public NoSelectedNumPosErrorViewModel(ErrorsCollection errorsCollection) : base(errorsCollection)
            {
                Localizer.Instance.LocalizationChanged += (sender, args) =>
                {
                    Message = Localizer.Instance.SplitNumPosShouldBeSelected;
                };

                Message = Localizer.Instance.SplitNumPosShouldBeSelected;
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
            this.OutputDir = dir;

            InputFile = file;
        }
    }
}