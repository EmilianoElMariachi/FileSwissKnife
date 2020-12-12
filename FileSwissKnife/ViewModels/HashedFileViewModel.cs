using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
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
        private CancellationTokenSource _cancellationTokenSource;
        private int _displayLength;

        private class InternalHash : Hash
        {
            public InternalHash(string hashAlgorithmName) : base(hashAlgorithmName)
            {
            }


        }

        public HashedFileViewModel(string fileToHash, string[] hashAlgorithmNames)
        {
            _fileToHash = fileToHash;

            // Recherche le nom de hash le plus long pour ensuite aligner les mots dans la fenêtre de résultat
            _displayLength = hashAlgorithmNames.Aggregate(0, (acc, algorithmName) => algorithmName.Length > acc ? algorithmName.Length : acc);

            _hashes = hashAlgorithmNames.Select(hashAlgorithmName => new Hash(hashAlgorithmName)).ToArray();

            HashOrCancelCommand = new RelayCommand(OnHashOrCancel);
            UpdateDisplay();
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

        private async void OnHashOrCancel()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var fileHasher = new FileHasher();

            fileHasher.OnProgress += (sender, args) =>
            {
                this.ProgressBarValue = args.Percent;
            };

            await fileHasher.ComputeAsync(_cancellationTokenSource.Token, _fileToHash, _hashes);

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var sb = new StringBuilder();
            sb.Append(_fileToHash + Environment.NewLine);

            foreach (var hash in _hashes)
            {
                sb.Append(hash.AlgorithmName + new string(' ', _displayLength - hash.AlgorithmName.Length));
                sb.Append(": ");
                sb.Append(hash.ComputedValue);
                sb.Append(Environment.NewLine);
            }

            TextResult = sb.ToString();
        }
    }
}