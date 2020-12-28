using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ElMariachi.FS.Tools.Hashing;
using FileSwissKnife.CustomControls;
using FileSwissKnife.CustomControls.Error;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils.MVVM;
using FileSwissKnife.Utils.UnitsManagement;

namespace FileSwissKnife.Views.Hashing
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
        private PlayStopButtonState _state;
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

        public ErrorsCollection Errors { get; } = new ErrorsCollection();

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

        private async void HashOrCancel()
        {
            if (_cancellationTokenSource != null)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                _cancellationTokenSource.Cancel();
                UpdateDisplay();
                State = PlayStopButtonState.Cancelling;
                return;
            }

            var errors = Errors;
            try
            {
                errors.Clear();

                _canceled = false;

                State = PlayStopButtonState.Stop;

                UpdateDisplay();

                var fileHasher = new FileHasher();
                ProgressBarValue = 0;
                ProgressBarText = "";
                fileHasher.OnProgress += (sender, args) =>
                {
                    ProgressBarValue = args.Percent;
                    ProgressBarText = $"{args.Percent:F2}%";
                };

                var startDate = DateTime.Now;

                _cancellationTokenSource = new CancellationTokenSource();
                await fileHasher.ComputeAsync(_fileToHash, _hashes, _cancellationTokenSource.Token);

                ProgressBarText = string.Format(Localizer.Instance.OperationFinishedIn, (DateTime.Now - startDate).ToElapsedTime());
            }
            catch (OperationCanceledException)
            {
                _canceled = true;
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
                sb.Append(staticText ?? hash.HexValue);
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