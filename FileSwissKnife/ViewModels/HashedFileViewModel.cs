using System;
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
        private readonly string[] _hashAlgorithmNames;
        private string _textResult;
        private string? _progressBarText;
        private double _progressBarValue;
        private CancellationTokenSource _cancellationTokenSource;

        public HashedFileViewModel(string fileToHash, string[] hashAlgorithmNames)
        {
            _fileToHash = fileToHash;
            _hashAlgorithmNames = hashAlgorithmNames;

            HashOrCancelCommand = new RelayCommand(OnHashOrCancel);
            InitDisplay();
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

            //private void HashFiles()
            //{
            //    //TODO: a effacer

            //    var hashAlgorithmNames = new[] { "SHA1", "MD5", "SHA256", "SHA384", "SHA512" };

            //    foreach (var file in FilesToHash.Text.Split(Environment.NewLine))
            //    {
            //        var computeHashes = FileHasher.ComputeHashes(file, hashAlgorithmNames);
            //        for (var i = 0; i < hashAlgorithmNames.Length; i++)
            //        {
            //            var hashName = hashAlgorithmNames[i];
            //            FilesToHash.AppendText(Environment.NewLine + hashName + "=" + computeHashes[i]);
            //        }
            //    }
            //}

            _cancellationTokenSource = new CancellationTokenSource();

            var fileHasher = new FileHasher();
            var hashes = await fileHasher.RunAsync(_cancellationTokenSource.Token, _hashAlgorithmNames, _fileToHash);

            DisplayHashResults(hashes);
        }

        private void InitDisplay()
        {
            //throw new System.NotImplementedException();
        }

        private void DisplayHashResults(string[] hashes)
        {
            var sb = new StringBuilder();
            sb.Append(_fileToHash + Environment.NewLine);

            for (var i = 0; i < _hashAlgorithmNames.Length; i++)
            {
                var hashAlgorithmName = _hashAlgorithmNames[i];
                var hashResult = hashes[i];
                sb.Append(hashAlgorithmName);
                sb.Append(": ");
                sb.Append(hashResult);
                sb.Append(Environment.NewLine);
            }

            TextResult = sb.ToString();
        }
    }
}