using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using FileSwissKnife.CustomControls;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.ViewModels
{
    public class HashedFileViewModel : ViewModelBase
    {
        private readonly string _fileToHash;
        private readonly Hash[] _hashes;
        private string _textResult;
        private string? _progressBarText;
        private double _progressBarValue;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly int _displayLength;
        private Visibility _progressBarVisibility = Visibility.Collapsed;
        private PlayStopButtonState _state;
        private string? _errorMessage;
        private bool _canceled = false;

        public event EventHandler? QueryClose;

        public HashedFileViewModel(string fileToHash, string[] hashAlgorithmNames)
        {
            _fileToHash = fileToHash;

            // Recherche le nom de hash le plus long pour ensuite aligner les mots dans la fenêtre de résultat
            _displayLength = hashAlgorithmNames.Aggregate(0, (acc, algorithmName) => algorithmName.Length > acc ? algorithmName.Length : acc);

            _hashes = hashAlgorithmNames.Select(hashAlgorithmName => new Hash(hashAlgorithmName)).ToArray();

            HashOrCancelCommand = new RelayCommand(HashOrCancel);
            CloseCommand = new RelayCommand(Close);
            UpdateDisplay();
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

        public PlayStopButtonState State
        {
            get => _state;
            set
            {
                _state = value;
                NotifyPropertyChanged();
            }
        }

        public string TextResult
        {
            get => _textResult;
            private set
            {
                _textResult = value;
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

        public ICommand HashOrCancelCommand { get; }

        public ICommand CloseCommand { get; }

        public Visibility ProgressBarVisibility
        {
            get => _progressBarVisibility;
            private set
            {
                _progressBarVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private async void HashOrCancel()
        {
            if (_cancellationTokenSource != null)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                _cancellationTokenSource.Cancel();
                UpdateDisplay();
                return;
            }

            try
            {
                _canceled = false;
                _cancellationTokenSource = new CancellationTokenSource();

                ErrorMessage = null;
                ProgressBarVisibility = Visibility.Visible;
                State = PlayStopButtonState.Stop;

                UpdateDisplay();

                var fileHasher = new FileHasher();

                fileHasher.OnProgress += (sender, args) => { this.ProgressBarValue = args.Percent; };

                await fileHasher.ComputeAsync(_fileToHash, _hashes, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                _canceled = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                ProgressBarVisibility = Visibility.Collapsed;
                State = PlayStopButtonState.Play;
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            var sb = new StringBuilder();
            sb.Append(_fileToHash + Environment.NewLine);


            string? staticText;
            if (_canceled)
            {
                staticText = Localizer.Instance.HashCanceled;
            }
            else if (_cancellationTokenSource != null)
            {
                staticText = _cancellationTokenSource.IsCancellationRequested ? Localizer.Instance.CancellingHash : Localizer.Instance.HashInProgress;
            }
            else
            {
                staticText = null; // Here, computation is finished
            }

            foreach (var hash in _hashes)
            {
                sb.Append(hash.AlgorithmName + new string(' ', _displayLength - hash.AlgorithmName.Length));
                sb.Append(": ");
                sb.Append(staticText ?? hash.ComputedValue);
                sb.Append(Environment.NewLine);
            }

            TextResult = sb.ToString();
        }

        private void Close()
        {
            if (_cancellationTokenSource != null)
            {
                var messageBoxResult = MessageBox.Show(Application.Current.MainWindow, Localizer.Instance.ConfirmCloseCancelHash, Localizer.Instance.ConfirmCloseCancelHashTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.No)
                    return;
                _cancellationTokenSource?.Cancel();
            }

            NotifyQueryClose();
        }

        private void NotifyQueryClose()
        {
            QueryClose?.Invoke(this, EventArgs.Empty);
        }
    }
}