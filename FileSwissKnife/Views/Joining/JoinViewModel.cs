﻿using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ElMariachi.FS.Tools.Joining;
using FileSwissKnife.CustomControls;
using FileSwissKnife.CustomControls.Error;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.Views.Joining
{
    public class JoinViewModel : TabViewModelBase, IFilesDropped
    {

        private bool _isTaskRunning;
        private double _progressBarValue;
        private CancellationTokenSource? _cancellationTokenSource;
        private string? _progressBarText;
        private readonly FileJoiner _fileJoiner;

        private string _inputFiles = "";
        private string _outputFile = "";
        private PlayStopButtonState _state;

        public JoinViewModel()
        {
            JoinOrCancelCommand = new RelayCommand(JoinOrCancel);

            _fileJoiner = new FileJoiner();

            _fileJoiner.OnProgress += (sender, args) =>
            {
                var percent = args.Percent;
                this.ProgressBarValue = percent;
                this.ProgressBarText = percent.ToString("0.00");
            };
        }


        public override string DisplayName => Localizer.Instance.TabNameJoin;

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

        public bool IsTaskRunning
        {
            get => _isTaskRunning;
            set
            {
                _isTaskRunning = value;
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

        private async void JoinOrCancel()
        {
            if (IsTaskRunning)
            {
                CancelTask();
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

                IsTaskRunning = true;
                State = PlayStopButtonState.Stop;
                _cancellationTokenSource = new CancellationTokenSource();

                var startDateTime = DateTime.Now;

                await _fileJoiner.Run(inputFiles, outputFile, _cancellationTokenSource.Token);

                ProgressBarText = _cancellationTokenSource.IsCancellationRequested ? Localizer.Instance.OperationCanceled : string.Format(Localizer.Instance.OperationFinishedIn, (DateTime.Now - startDateTime));
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
                _cancellationTokenSource = null;
                State = PlayStopButtonState.Play;
                IsTaskRunning = false;
            }
        }

        public void OnFilesDropped(string[] files)
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
                    // TODO: à implémenter
                }
            }
            else
            {
                InputFiles = string.Join(Environment.NewLine, files);
            }
        }

        private void CancelTask()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                ProgressBarText = Localizer.Instance.CancellingJoin;
            }
        }

    }
}
