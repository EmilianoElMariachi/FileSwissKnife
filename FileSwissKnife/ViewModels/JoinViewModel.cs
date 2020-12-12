using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using FileSwissKnife.CustomControls;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.ViewModels
{
    public class JoinViewModel : ViewModelBase, IFilesDropped
    {

        private bool _isTaskRunning;
        private double _progressBarValue;
        private CancellationTokenSource? _cancellationTokenSource;
        private string? _progressBarText;
        private readonly FileJoiner _fileJoiner;
        private string _joinActionText;
        private Visibility _errorIconVisibility = Visibility.Collapsed;
        private string? _errorMessage;

        private string _inputFiles = "";
        private string _outputFile = "";

        public JoinViewModel()
        {
            JoinCommand = new RelayCommand(OnJoinOrCancelCommand, CanJoinOrCancelCommand);
            HideErrorCommand = new RelayCommand(OnHideError);
            _joinActionText = Localizer.Instance.StartJoin;

            _fileJoiner = new FileJoiner();

            _fileJoiner.OnProgress += (sender, args) =>
            {
                var percent = args.Percent;
                this.ProgressBarValue = percent;
                this.ProgressBarText = percent.ToString("0.00");
                JoinActionText = Localizer.Instance.CancelJoin;
            };
        }

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

        public Visibility ErrorIconVisibility
        {
            get => _errorIconVisibility;
            set
            {
                _errorIconVisibility = value;
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

        public string JoinActionText
        {
            get => _joinActionText;
            set
            {
                _joinActionText = value;
                NotifyPropertyChanged();
            }
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand JoinCommand { get; }

        public ICommand HideErrorCommand { get; }

        private void OnHideError()
        {
            ClearError();
        }

        private bool CanJoinOrCancelCommand()
        {
            return true;
        }

        private async void OnJoinOrCancelCommand()
        {
            if (IsTaskRunning)
            {
                CancelTask();
                return;
            }

            try
            {
                IsTaskRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();
                ClearError();

                var inputFiles = InputFiles.Split(Environment.NewLine);
                var outputFile = OutputFile;
                var startDateTime = DateTime.Now;

                await _fileJoiner.Run(_cancellationTokenSource.Token, inputFiles, outputFile);

                ProgressBarText = _cancellationTokenSource.IsCancellationRequested ? Localizer.Instance.OperationCanceled : string.Format(Localizer.Instance.OperationFinishedIn, (DateTime.Now - startDateTime));
            }
            catch (Exception ex)
            {
                ProgressBarText = Localizer.Instance.OperationError;
                ShowError(ex.Message);
            }
            finally
            {
                _cancellationTokenSource = null;
                JoinActionText = Localizer.Instance.StartJoin;
                IsTaskRunning = false;
            }
        }

        public void OnFilesDropped(string[] files)
        {
            InputFiles = string.Join(Environment.NewLine, files);
        }

        private void CancelTask()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                ProgressBarText = Localizer.Instance.Cancelling;
            }
        }

        private void ShowError(string message)
        {
            ErrorIconVisibility = Visibility.Visible;
            ErrorMessage = message;
        }

        private void ClearError()
        {
            ErrorIconVisibility = Visibility.Collapsed;
            ErrorMessage = "";
        }

    }

}
