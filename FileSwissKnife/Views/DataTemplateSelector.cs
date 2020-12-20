using System.Windows;
using System.Windows.Controls;
using FileSwissKnife.Views.Hashing;
using FileSwissKnife.Views.Joining;
using FileSwissKnife.Views.Splitting;

namespace FileSwissKnife.Views
{

    /// <summary>
    /// Select the view to associate with the ViewModel attached to the active Tab
    /// </summary>
    public class TabDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate JoinViewDataTemplate { get; set; }

        public DataTemplate HashViewDataTemplate { get; set; }

        public DataTemplate SplitViewDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is JoinViewModel)
                return JoinViewDataTemplate;

            if (item is HashViewModel)
                return HashViewDataTemplate;

            if (item is SplitViewModel)
                return SplitViewDataTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
