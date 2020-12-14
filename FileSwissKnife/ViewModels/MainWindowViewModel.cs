using System;
using System.Linq;
using System.Windows;
using FileSwissKnife.Properties;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.ViewModels
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

        private void OnAppExit(object sender, ExitEventArgs e)
        {
            var selectedTab = SelectedTab;
            if (selectedTab != null)
            {
                Settings.Default.LastActiveTabTechName = selectedTab.TechName;
                Settings.Default.Save();
            }
        }

        public TabViewModelBase? SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                NotifyPropertyChanged();
            }
        }

        public TabViewModelBase[] Tabs { get; }
    }
}
