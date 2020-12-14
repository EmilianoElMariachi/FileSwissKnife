using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using FileSwissKnife.CustomControls;
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

        public event EventHandler? QueryClose;

        public HashedFileViewModel(string fileToHash, string[] hashAlgorithmNames)
        {
            _fileToHash = fileToHash;

            // Recherche le nom de hash le plus long pour ensuite aligner les mots dans la fenêtre de résultat
            _displayLength = hashAlgorithmNames.Aggregate(0, (acc, algorithmName) => algorithmName.Length > acc ? algorithmName.Length : acc);

            _hashes = hashAlgorithmNames.Select(hashAlgorithmName => new Hash(hashAlgorithmName)).ToArray();

            HashOrCancelCommand = new RelayCommand(HashOrCancel);
            CloseCommand = new RelayCommand(Close, CanClose);
            UpdateDisplay();
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
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    UpdateDisplay();
                }

                return;
            }

            try
            {
                ProgressBarVisibility = Visibility.Visible;
                State = PlayStopButtonState.Stop;
                _cancellationTokenSource = new CancellationTokenSource();

                UpdateDisplay();

                var fileHasher = new FileHasher();

                fileHasher.OnProgress += (sender, args) => { this.ProgressBarValue = args.Percent; };

                await fileHasher.ComputeAsync(_fileToHash, _hashes, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                //TODO: implémenter l'affichage de l'erreur
            }
            finally
            {
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

            foreach (var hash in _hashes)
            {
                sb.Append(hash.AlgorithmName + new string(' ', _displayLength - hash.AlgorithmName.Length));
                sb.Append(": ");
                sb.Append(_cancellationTokenSource != null ? (_cancellationTokenSource.IsCancellationRequested ? "Cancelling..." : "in progress...") : hash.ComputedValue); // TODO: à localiser
                sb.Append(Environment.NewLine);
            }

            TextResult = sb.ToString();
        }

        private bool CanClose()
        {
            //TODO: implémenter ici
            return true;
        }

        private void Close()
        {
            NotifyQueryClose();
        }


        private void NotifyQueryClose()
        {
            QueryClose?.Invoke(this, EventArgs.Empty);
        }
    }
}