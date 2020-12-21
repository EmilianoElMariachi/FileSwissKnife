using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FileSwissKnife.CustomControls;
using FileSwissKnife.Localization;
using FileSwissKnife.Utils.MVVM;
using Microsoft.Win32;

namespace FileSwissKnife.Views.Hashing
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
            var openFileDialog = new OpenFileDialog
            {
                Filter = $"{Localizer.Instance.AllFiles} (*.*)|*.*",
                Multiselect = true,
                Title = Localizer.Instance.BrowseFilesToHashTitle,
            };

            var result = openFileDialog.ShowDialog(Application.Current.MainWindow);
            if (result == null || !result.Value)
                return;

            HashFiles(openFileDialog.FileNames);
        }

        private void HashFiles(IEnumerable<string> filesToHash)
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
}