using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FileSwissKnife.CustomControls;
using FileSwissKnife.Localization;
using FileSwissKnife.Properties;
using FileSwissKnife.Utils.MVVM;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileSwissKnife.Views.Hashing
{
    public class HashViewModel : TabViewModelBase, IFilesDropped
    {
        private readonly ObservableCollection<HashedFileViewModel> _hashedFiles = new ObservableCollection<HashedFileViewModel>();

        public HashViewModel()
        {
            var hashNames = new[] { "SHA1", "MD5", "SHA256", "SHA384", "SHA512" };

            AvailableHashes = hashNames.Select(hashName =>
            {
                var selectedHashesSaved = Settings.Default.SelectedHashes ?? new StringCollection();

                var hashToComputeViewModel = new HashToComputeViewModel(hashName)
                {
                    IsComputed = selectedHashesSaved.Count <= 0 || selectedHashesSaved.Contains(hashName),
                };
                hashToComputeViewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(HashToComputeViewModel.IsComputed))
                    {
                        var selectedHashes = new StringCollection();
                        selectedHashes.AddRange(GetSelectedHashes());
                        Settings.Default.SelectedHashes = selectedHashes;
                    }
                };
                return hashToComputeViewModel;
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
            var openFileDialog = new CommonOpenFileDialog
            {
                Multiselect = true,
                Title = Localizer.Instance.BrowseFilesToHashTitle,
            };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
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
            return AvailableHashes.Where(model => model.IsComputed).Select(model => model.HashName).ToArray();
        }


        public void OnFilesDropped(string[] files)
        {
            HashFiles(files);
        }

    }
}