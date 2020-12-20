using System;
using System.Linq;
using System.Windows;
using FileSwissKnife.Properties;
using FileSwissKnife.Utils.MVVM;
using FileSwissKnife.Views.Hashing;
using FileSwissKnife.Views.Joining;
using FileSwissKnife.Views.Splitting;

namespace FileSwissKnife.Views
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TabViewModelBase? _selectedTab;


        public MainWindowViewModel()
        {
            Application.Current.Exit += OnAppExit;
            Tabs = new TabViewModelBase[] { new JoinViewModel(), new SplitViewModel(), new HashViewModel() };


            var tabToSelect = Tabs.FirstOrDefault(tab => string.Equals(tab.TechName, Settings.Default.LastActiveTabTechName, StringComparison.OrdinalIgnoreCase));

            SelectedTab = tabToSelect ?? Tabs[0];
        }

        private static void OnAppExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }

        public TabViewModelBase? SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                if (_selectedTab != null)
                    Settings.Default.LastActiveTabTechName = _selectedTab.TechName;
                NotifyPropertyChanged();
            }
        }

        public TabViewModelBase[] Tabs { get; }
    }
}
