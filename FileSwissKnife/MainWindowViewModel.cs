using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils;

namespace FileSwissKnife
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isTaskRunning;
        private double _progressBarValue;
        private CancellationTokenSource? _cancellationTokenSource;
        private string? _progressBarText;
        private readonly FileJoiner _fileJoiner;
        private string _filesToJoin = "";
        private string _startActionButtonText;
        private int _selectedModeIndex;
        private Visibility _errorIconVisibility = Visibility.Collapsed;
        private string _errorMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            RunCommand = new RelayCommand(OnRunOrCancelCommand, CanRunCommand);
            HideErrorCommand = new RelayCommand(OnHideError);
            _fileJoiner = new FileJoiner();

            StartActionButtonText = Localizer.Instance.Start;
        }

        public int SelectedModeIndex
        {
            get => _selectedModeIndex;
            set
            {
                _selectedModeIndex = value;
                NotifyPropertyChanged(nameof(SelectedModeIndex));
            }
        }

        private Mode SelectedMode => (Mode)_selectedModeIndex;

        public string? JoinOutputFile
        {
            get => _fileJoiner.OutputFile;
            set
            {
                _fileJoiner.OutputFile = value;
                NotifyPropertyChanged(nameof(JoinOutputFile));
            }
        }

        public string FilesToJoin
        {
            get => _filesToJoin;
            set
            {
                _filesToJoin = value;
                NotifyPropertyChanged(nameof(FilesToJoin));
            }
        }

        public bool IsTaskRunning
        {
            get => _isTaskRunning;
            set
            {
                _isTaskRunning = value;
                NotifyPropertyChanged(nameof(IsTaskRunning));
            }
        }

        public string? ProgressBarText
        {
            get => _progressBarText;
            set
            {
                _progressBarText = value;
                NotifyPropertyChanged(nameof(ProgressBarText));
            }
        }

        public Visibility ErrorIconVisibility
        {
            get => _errorIconVisibility;
            set
            {
                _errorIconVisibility = value;
                NotifyPropertyChanged(nameof(ErrorIconVisibility));
            }
        }


        public double ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                _progressBarValue = value;
                NotifyPropertyChanged(nameof(ProgressBarValue));
            }
        }

        public string StartActionButtonText
        {
            get => _startActionButtonText;
            set
            {
                _startActionButtonText = value;
                NotifyPropertyChanged(nameof(StartActionButtonText));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand RunCommand { get; }

        public ICommand HideErrorCommand { get; }

        private void OnHideError()
        {
            ClearError();
        }

        private bool CanRunCommand()
        {
            return true;
        }

        private void OnRunOrCancelCommand()
        {
            if (IsTaskRunning)
            {
                CancelTask();
                return;
            }


            switch (SelectedMode)
            {
                case Mode.Join:

                    _fileJoiner.InputFiles = FilesToJoin.Split(Environment.NewLine);
                    RunTask(_fileJoiner);
                    break;
                case Mode.Split:
                    break;
                case Mode.Hash:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CancelTask()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                ProgressBarText = Localizer.Instance.Cancelling;
            }
        }

        private async void RunTask(IReportProgressAction reportProgressAction)
        {
            reportProgressAction.OnProgress += (sender, args) =>
            {
                var percent = args.Percent;
                this.ProgressBarValue = percent;
                this.ProgressBarText = percent.ToString("0.00");
                StartActionButtonText = Localizer.Instance.Cancel;
            };


            var startDateTime = DateTime.Now;

            try
            {
                reportProgressAction.CheckPrerequisites();

                IsTaskRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();
                ClearError();
                await Task.Run(() =>
                {
                    reportProgressAction.Run(_cancellationTokenSource.Token);
                });

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
                StartActionButtonText = Localizer.Instance.Start;
                IsTaskRunning = false;
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

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum Mode
    {
        Join = 0,
        Split = 1,
        Hash = 2
    }


}
