using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using FileSwissKnife.Views.Splitting.Validators;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileSwissKnife.Views.Splitting
{
    public class SplitViewModel : TabViewModelBase, IFilesDropped
    {
        private string _inputFile;
        private Unit? _selectedUnit;
        private readonly NoSelectedUnitErrorViewModel _noSelectedUnitError;

        private readonly ErrorsCollection _errors = new ErrorsCollection();

        private CancellationTokenSource? _cancellationTokenSource;
        private PlayStopButtonState _state;
        private double _progressBarValue;
        private string? _progressBarText;
        private NumPosViewModel? _selectedNumPos;
        private readonly StaticErrorViewModel _fileSizeSplitSizeError;
        private readonly NoSelectedNumPosErrorViewModel _noSelectedNumPosError;
        private string _namePreview;

        private readonly SplitSizeValidator _splitSizeValidator;
        private readonly StartNumberValidator _startNumberValidator;
        private string _outputDir;

        public SplitViewModel()
        {
            _splitSizeValidator = new SplitSizeValidator(_errors);
            _startNumberValidator = new StartNumberValidator(_errors);
            _noSelectedUnitError = new NoSelectedUnitErrorViewModel(_errors);
            _fileSizeSplitSizeError = new StaticErrorViewModel(_errors);
            _noSelectedNumPosError = new NoSelectedNumPosErrorViewModel(_errors);


            SplitOrCancelCommand = new RelayCommand(SplitOrCancel);
            BrowseOutputDirCommand = new RelayCommand(BrowseOutputDir);
            BrowseInputFileCommand = new RelayCommand(BrowseInputFile);

            SplitSizeStr = Settings.Default.SplitSize.ToString();
            SelectedUnit = Units.All.FirstOrDefault(unit => unit.SIUnit == Settings.Default.SplitUnit);

            NumPositions = Enum.GetValues<NumPos>().Select(numPos => new NumPosViewModel(numPos)).ToArray();

            SelectedNumPos = NumPositions.FirstOrDefault(vm => vm.NumPos == Settings.Default.SplitNumPos);

            this.PropertyChanged += OnPropertyChanged;

            UpdateNamingPreview();
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
            get => _splitSizeValidator.EditedValue;
            set
            {
                _splitSizeValidator.EditedValue = value;
                NotifyPropertyChanged();
            }
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
            get => _outputDir;
            set
            {
                _outputDir = value;
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

        public string NumPrefix
        {
            get => Settings.Default.SplitNumPrefix;
            set
            {
                Settings.Default.SplitNumPrefix = value;
                NotifyPropertyChanged();
            }
        }

        public string NumStart
        {
            get => _startNumberValidator.EditedValue;
            set
            {
                _startNumberValidator.EditedValue = value;
                NotifyPropertyChanged();
            }
        }

        public bool PadWithZeros
        {
            get => Settings.Default.SplitPadWithZeros;
            set
            {
                Settings.Default.SplitPadWithZeros = value;
                NotifyPropertyChanged();
            }
        }

        public string NumSuffix
        {
            get => Settings.Default.SplitNumSuffix;
            set
            {
                Settings.Default.SplitNumSuffix = value;
                NotifyPropertyChanged();
            }
        }

        public string NamePreview
        {
            get => _namePreview;
            set
            {
                _namePreview = value;
                NotifyPropertyChanged();
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NumPrefix)
                || e.PropertyName == nameof(NumSuffix)
                || e.PropertyName == nameof(NumStart)
                || e.PropertyName == nameof(SelectedNumPos)
                || e.PropertyName == nameof(PadWithZeros)
                )
                UpdateNamingPreview();
        }

        private void UpdateNamingPreview()
        {
            var fileNameBase = "MyMovie.mp4";

            if (!_startNumberValidator.TryGetStartNumber(out var startNumber))
                return;

            var selectedNumPos = this.SelectedNumPos;
            if (selectedNumPos == null)
            {
                _noSelectedNumPosError.Show();
                return;
            }

            var namingOptions = new NamingOptions
            {
                NumSuffix = NumSuffix,
                NumPrefix = NumPrefix,
                NumPos = selectedNumPos.NumPos,
                StartNumber = startNumber,
                PadWithZeros = PadWithZeros
            };

            var baseName = Path.GetFileNameWithoutExtension(fileNameBase);
            var ext = Path.GetExtension(fileNameBase).Substring(1);

            NamePreview = FileSplitter.BuildFileName(baseName, ext, namingOptions, namingOptions.StartNumber, 3);
        }

        private void BrowseOutputDir()
        {
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = Settings.Default.SplitLastDir,
                IsFolderPicker = true,
                Title = Localizer.Instance.BrowseSplitOutputDirTitle
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Settings.Default.SplitLastDir = dialog.FileName;
                this.OutputDir = dialog.FileName;
            }
        }

        private void BrowseInputFile()
        {
            var openFileDialog = new CommonOpenFileDialog
            {
                Multiselect = false,
                Title = Localizer.Instance.BrowseFileToSplitTitle,
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
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

            if (!_splitSizeValidator.TryGetSplitSize(out var splitSize))
                return;

            if (!_startNumberValidator.TryGetStartNumber(out var startNumber))
                return;

            var splitSizeBytes = selectedUnit.ToNbBytes(splitSize);

            var inputFile = InputFile;

            if (!File.Exists(inputFile))
            {
                _fileSizeSplitSizeError.Show(string.Format(Localizer.Instance.SplitInputFileNotFound, inputFile));
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
                    ProgressBarText = $"{args.Percent:F2}% ({args.Message})";
                };

                var namingOptions = new NamingOptions
                {
                    NumPrefix = NumPrefix,
                    NumPos = selectedNumPos.NumPos,
                    NumSuffix = NumSuffix,
                    StartNumber = startNumber,
                    PadWithZeros = PadWithZeros,
                };

                var startDate = DateTime.Now; 

                await fileSplitter.Split(inputFile, OutputDir, splitSizeBytes, namingOptions, _cancellationTokenSource.Token);

                this.ProgressBarText = string.Format(Localizer.Instance.OperationFinishedIn, (DateTime.Now - startDate).ToElapsedTime());
            }
            catch (OperationCanceledException)
            {
                ProgressBarValue = 0;
                ProgressBarText = Localizer.Instance.OperationCanceled;
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