using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ElMariachi.FS.Tools.Joining;
using FileSwissKnife.CustomControls;
using FileSwissKnife.CustomControls.Error;
using FileSwissKnife.Localization;
using FileSwissKnife.Properties;
using FileSwissKnife.Utils.MVVM;
using FileSwissKnife.Utils.UnitsManagement;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileSwissKnife.Views.Joining
{
    public class JoinViewModel : TabViewModelBase, IFilesDropped
    {

        private double _progressBarValue;
        private CancellationTokenSource? _cancellationTokenSource;
        private string? _progressBarText;

        private string _inputFiles = "";
        private string _outputFile = "";
        private PlayStopButtonState _state;

        public JoinViewModel()
        {
            JoinOrCancelCommand = new RelayCommand(JoinOrCancel);
            BrowseOutputFileCommand = new RelayCommand(BrowseOutputFile);
            BrowseInputFilesCommand = new RelayCommand(BrowseInputFiles);
        }

        public override string TechName => "Join";

        public string OutputFile
        {
            get => _outputFile;
            set
            {
                _outputFile = value;
                NotifyPropertyChanged();
            }
        }

        public string InputFiles
        {
            get => _inputFiles;
            set
            {
                _inputFiles = value;
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

        public ErrorsCollection Errors { get; } = new ErrorsCollection();

        public ICommand JoinOrCancelCommand { get; }

        public PlayStopButtonState State
        {
            get => _state;
            set
            {
                _state = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand BrowseOutputFileCommand { get; }

        public ICommand BrowseInputFilesCommand { get; }

        private async void JoinOrCancel()
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

            try
            {
                var inputFiles = InputFiles.Split(Environment.NewLine);
                var outputFile = OutputFile;

                if (string.IsNullOrEmpty(outputFile))
                    throw new ArgumentException(Localizer.Instance.OutputFileCantBeUndefined);

                if (File.Exists(outputFile))
                {
                    var currentMainWindow = Application.Current.MainWindow;
                    var messageBoxResult = MessageBox.Show(currentMainWindow, string.Format(Localizer.Instance.CanOverrideOutputFile, outputFile), Localizer.Instance.Override, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    if (messageBoxResult != MessageBoxResult.Yes)
                        throw new InvalidOperationException(string.Format(Localizer.Instance.YouChooseNotToOverride, outputFile));
                }

                State = PlayStopButtonState.Stop;

                var fileJoiner = new FileJoiner();
                ProgressBarValue = 0;
                ProgressBarText = "";
                fileJoiner.OnProgress += (sender, args) =>
                {
                    var percent = args.Percent;
                    ProgressBarValue = percent;
                    ProgressBarText = $"{percent:F2}%";
                };

                _cancellationTokenSource = new CancellationTokenSource();
                var startDate = DateTime.Now;
                await fileJoiner.Run(inputFiles, outputFile, _cancellationTokenSource.Token);

                ProgressBarText = string.Format(Localizer.Instance.OperationFinishedIn, (DateTime.Now - startDate).ToElapsedTime());
            }
            catch (OperationCanceledException)
            {
                ProgressBarValue = 0;
                ProgressBarText = Localizer.Instance.OperationCanceled;
            }
            catch (Exception ex)
            {
                ProgressBarText = Localizer.Instance.OperationError;

                errors.Add(new ErrorViewModel
                {
                    Message = ex.Message
                });
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                State = PlayStopButtonState.Play;
            }
        }

        private void BrowseInputFiles()
        {
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = Settings.Default.JoinLastDir,
                Multiselect = true,
                IsFolderPicker = false,
                Title = Localizer.Instance.BrowseJoinInputFilesTitle
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
                return;

            var selectedFiles = dialog.FileNames.ToArray();

            var firstSelectedFile = selectedFiles.FirstOrDefault();
            if (firstSelectedFile != null)
                Settings.Default.JoinLastDir = Path.GetDirectoryName(firstSelectedFile);

            InputFiles = string.Join(Environment.NewLine, selectedFiles);
        }

        private void BrowseOutputFile()
        {
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = Settings.Default.JoinLastDir,
                IsFolderPicker = false,
                Title = Localizer.Instance.BrowseJoinOutputFileTitle
            };
            if (dialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
                return;

            Settings.Default.JoinLastDir = Path.GetDirectoryName(dialog.FileName);
            OutputFile = dialog.FileName;
        }

        public void OnFilesDropped(string[] files)
        {
            try
            {
                if (files.Length == 1)
                {
                    var file = files[0];
                    if (File.Exists(file))
                    {
                        if (FileJoiner.TryGuessFilesToJoin(file, out var inputFiles, out var outputFile))
                        {
                            InputFiles = string.Join(Environment.NewLine, inputFiles);
                            OutputFile = outputFile;
                        }
                        else
                        {
                            InputFiles = string.Join(Environment.NewLine, files);
                        }
                    }
                    else if (Directory.Exists(file))
                    {
                        InputFiles = string.Join(Environment.NewLine, Directory.GetFiles(file));
                        OutputFile = Path.Combine(file, Path.GetFileName(file));
                    }
                }
                else
                {
                    InputFiles = string.Join(Environment.NewLine, files);
                }
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
