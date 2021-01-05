using System;
using System.Linq;
using FileSwissKnife.Properties;
using FileSwissKnife.Utils;
using FileSwissKnife.Utils.MVVM;
using FileSwissKnife.Views.AppSettings;
using FileSwissKnife.Views.Hashing;
using FileSwissKnife.Views.Joining;
using FileSwissKnife.Views.Splitting;

namespace FileSwissKnife.Views
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TabViewModelBase? _selectedTab;
        private readonly TabViewModelBase[] _toolsTab;


        public MainWindowViewModel()
        {
            Title = $"{AppInfo.ProductName} v{AppInfo.DisplayVersion}";
            JoinViewModel = new JoinViewModel();
            SplitViewModel = new SplitViewModel();
            HashViewModel = new HashViewModel();
            AppSettingsViewModel = new AppSettingsViewModel();

            // All tools tab except settings
            _toolsTab = new TabViewModelBase[] { JoinViewModel, SplitViewModel, HashViewModel };

            var tabToSelect = _toolsTab.FirstOrDefault(tab => string.Equals(tab.TabId, Settings.Default.ActiveTabId, StringComparison.OrdinalIgnoreCase));

            SelectedTab = tabToSelect ?? _toolsTab[0];
        }

        public string Title { get; }

        public TabViewModelBase? SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                if (_selectedTab != null && _toolsTab.Contains(_selectedTab))
                    Settings.Default.ActiveTabId = _selectedTab.TabId;
                NotifyPropertyChanged();
            }
        }

        public JoinViewModel JoinViewModel { get; }

        public SplitViewModel SplitViewModel { get; }

        public HashViewModel HashViewModel { get; }

        public AppSettingsViewModel AppSettingsViewModel { get; }

    }
}
