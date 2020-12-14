using System.Windows;
using System.Windows.Controls;
using FileSwissKnife.ViewModels;

namespace FileSwissKnife.Views
{
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
