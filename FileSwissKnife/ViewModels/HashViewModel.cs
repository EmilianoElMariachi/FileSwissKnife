using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FileSwissKnife.CustomControls;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.ViewModels
{
    public class HashViewModel : TabViewModelBase, IFilesDropped
    {
        private readonly ObservableCollection<HashedFileViewModel> _hashedFiles = new ObservableCollection<HashedFileViewModel>();

        public HashViewModel()
        {
            var hashNames = new[] { "SHA1", "MD5", "SHA256", "SHA384", "SHA512" };


            AvailableHashes = hashNames.Select(hashName => new HashToComputeViewModel(hashName)
            {
                Compute = true,
            }).ToArray();

            SelectFilesToHashCommand = new RelayCommand(OnSelectFilesToHash);
        }


        public override string DisplayName => Localizer.Instance.TabNameHash;

        public override string TechName => "Hash";

        public ObservableCollection<HashedFileViewModel> HashedFiles => _hashedFiles;

        public HashToComputeViewModel[] AvailableHashes { get; }

        public ICommand SelectFilesToHashCommand { get; }

        private void OnSelectFilesToHash()
        {
            // TODO: à implémenter


        }

        private void HashFiles(string[] filesToHash)
        {
            var selectedHashes = GetSelectedHashes();

            foreach (var fileToHash in filesToHash)
            {
                var hashedFileViewModel = new HashedFileViewModel(fileToHash, selectedHashes);
                hashedFileViewModel.QueryClose += OnCloseHashedFile;

                hashedFileViewModel.HashOrCancelCommand.Execute(null);
                _hashedFiles.Add(hashedFileViewModel);
            }
        }

        private void OnCloseHashedFile(object? sender, EventArgs e)
        {
            _hashedFiles.Remove((HashedFileViewModel)sender);
        }

        private string[] GetSelectedHashes()
        {
            return AvailableHashes.Where(model => model.Compute).Select(model => model.HashName).ToArray();
        }


        public void OnFilesDropped(string[] files)
        {
            HashFiles(files);
        }

    }

    public class HashToComputeViewModel : ViewModelBase
    {
        private bool _compute;

        public HashToComputeViewModel(string hashName)
        {
            HashName = hashName;
        }

        public string HashName { get; }

        public bool Compute
        {
            get => _compute;
            set
            {
                _compute = value;
                NotifyPropertyChanged();
            }
        }
    }
}