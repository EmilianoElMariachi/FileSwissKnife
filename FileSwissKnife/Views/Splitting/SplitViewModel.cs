﻿using System;
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

            PropertyChanged += OnPropertyChanged;

            UpdateNamingPreview();
        }

        public ICommand BrowseInputFileCommand { get; }

        public ICommand BrowseOutputDirCommand { get; }

        public RelayCommand SplitOrCancelCommand { get; }

        public override string TabId => "Split";

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

            var selectedNumPos = SelectedNumPos;
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
                Title = LocalizationManager.Instance.Current.Keys.BrowseSplitOutputDirTitle
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Settings.Default.SplitLastDir = dialog.FileName;
                OutputDir = dialog.FileName;
            }
        }

        private void BrowseInputFile()
        {
            var openFileDialog = new CommonOpenFileDialog
            {
                Multiselect = false,
                Title = LocalizationManager.Instance.Current.Keys.BrowseFileToSplitTitle,
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
                return;

            InputFile = openFileDialog.FileName;
        }

        private async void SplitOrCancel()
        {
            if (_cancellationTokenSource != null)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                _cancellationTokenSource.Cancel();
                State = PlayStopButtonState.Cancelling;
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
                _fileSizeSplitSizeError.Show(string.Format(LocalizationManager.Instance.Current.Keys.SplitInputFileNotFound, inputFile));
                return;
            }

            var fileInfo = new FileInfo(inputFile);

            if (fileInfo.Length <= splitSizeBytes)
            {
                _fileSizeSplitSizeError.Show(string.Format(LocalizationManager.Instance.Current.Keys.SplitErrorFileSizeLessThanSplitSize, splitSizeBytes, fileInfo.Length));
                return;
            }

            var selectedNumPos = SelectedNumPos;
            if (selectedNumPos == null)
            {
                _noSelectedNumPosError.Show();
                return;
            }

            try
            {
                State = PlayStopButtonState.Stop;

                var fileSplitter = new FileSplitter(Settings.Default.SplitBufferSize);
                ProgressBarValue = 0;
                ProgressBarText = "";
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

                _cancellationTokenSource = new CancellationTokenSource();
                var startDate = DateTime.Now;
                await fileSplitter.Split(inputFile, OutputDir, splitSizeBytes, namingOptions, _cancellationTokenSource.Token);

                ProgressBarText = string.Format(LocalizationManager.Instance.Current.Keys.OperationFinishedIn, (DateTime.Now - startDate).ToElapsedTime());
            }
            catch (OperationCanceledException)
            {
                ProgressBarValue = 0;
                ProgressBarText = LocalizationManager.Instance.Current.Keys.OperationCanceled;
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
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private class NoSelectedUnitErrorViewModel : StaticErrorViewModel
        {
            public NoSelectedUnitErrorViewModel(ErrorsCollection errorsCollection) : base(errorsCollection)
            {
                LocalizationManager.Instance.LocalizationChanged += (sender, args) =>
                {
                    Message = LocalizationManager.Instance.Current.Keys.SplitUnitShouldBeSelected;
                };

                Message = LocalizationManager.Instance.Current.Keys.SplitUnitShouldBeSelected;
            }
        }

        private class NoSelectedNumPosErrorViewModel : StaticErrorViewModel
        {
            public NoSelectedNumPosErrorViewModel(ErrorsCollection errorsCollection) : base(errorsCollection)
            {
                LocalizationManager.Instance.LocalizationChanged += (sender, args) =>
                {
                    Message = LocalizationManager.Instance.Current.Keys.SplitNumPosShouldBeSelected;
                };

                Message = LocalizationManager.Instance.Current.Keys.SplitNumPosShouldBeSelected;
            }
        }

        public void OnFilesDropped(string[] files)
        {
            try
            {
                if (files.Length <= 0)
                    return;
                var file = files[0];
                if (!File.Exists(file))
                    return;

                var dir = Path.GetDirectoryName(file);
                OutputDir = dir;

                InputFile = file;
            }
            catch (Exception ex)
            {
                Errors.Add(new ErrorViewModel
                {
                    Message = ex.Message
                });
            }
        }
    }
}