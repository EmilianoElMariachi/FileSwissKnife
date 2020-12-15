using System.Collections.Specialized;

namespace FileSwissKnife.Views
{
    /// <summary>
    /// Interaction logic for HashView.xaml
    /// </summary>
    public partial class HashView
    {
        public HashView()
        {
            InitializeComponent();
        }

        private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Scroll to the bottom on items added
            if (e.NewItems != null && e.NewItems.Count > 0)
                ScrollViewer.ScrollToEnd();
        }
    }
}
