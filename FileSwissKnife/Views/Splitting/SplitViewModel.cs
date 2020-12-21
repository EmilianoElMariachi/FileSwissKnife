using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ElMariachi.FS.Tools.Splitting;
using FileSwissKnife.CustomControls;
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
        private string _outputDir;
        private readonly ErrorViewModel _splitSizeError = new ErrorViewModel(false);
        private readonly NoSelectedUnitErrorViewModel _noSelectedUnitError = new NoSelectedUnitErrorViewModel();

        private CancellationTokenSource? _cancellationTokenSource;
        private PlayStopButtonState _state;
        private double _progressBarValue;
        private string _progressBarText;

        public SplitViewModel()
        {
            SplitOrCancelCommand = new RelayCommand(SplitOrCancel, CanSplitOrCancel);
            BrowseOutputDirCommand = new RelayCommand(BrowseOutputDir);
            BrowseInputFileCommand = new RelayCommand(BrowseInputFile);

            SplitSizeStr = Settings.Default.SplitSize.ToString();
            SelectedUnit = Units.All.FirstOrDefault(unit => unit.SIUnit == Settings.Default.SplitUnit);
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

        private void BrowseOutputDir()
        {
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
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

            try
            {
                var inputFile = InputFile;

                var splitSize = SelectedUnit.ToNbBytes(_splitSize.Value);

                var fileInfo = new FileInfo(inputFile);

                if (fileInfo.Length <= splitSize)
                {
                    this.Errors.Add(new ErrorViewModel(true)
                    {
                        Message = $"Split size «{splitSize}» is greater than file size «{fileInfo.Length}»."
                    });
                    return;
                }


                errors.Clear();
                State = PlayStopButtonState.Stop;
                var fileSplitter = new FileSplitter();
                _cancellationTokenSource = new CancellationTokenSource();
                fileSplitter.OnProgress += (sender, args) =>
                {
                    ProgressBarValue = args.Percent;
                    ProgressBarText = args.Message;
                };

                await fileSplitter.Split(inputFile, OutputDir, splitSize, _cancellationTokenSource.Token);
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
                State = PlayStopButtonState.Play;
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
            this.OutputDir = dir;

            InputFile = file;
        }
    }

}