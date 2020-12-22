using System;
using System.Linq;
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
            Tabs = new TabViewModelBase[] { new JoinViewModel(), new SplitViewModel(), new HashViewModel() };


            var tabToSelect = Tabs.FirstOrDefault(tab => string.Equals(tab.TechName, Settings.Default.ActiveTabTechName, StringComparison.OrdinalIgnoreCase));

            SelectedTab = tabToSelect ?? Tabs[0];
        }

        public TabViewModelBase? SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                if (_selectedTab != null)
                    Settings.Default.ActiveTabTechName = _selectedTab.TechName;
                NotifyPropertyChanged();
            }
        }

        public TabViewModelBase[] Tabs { get; }
    }
}
