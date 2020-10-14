using System.Windows;
using System.Windows.Controls;

namespace FileSwissKnife.CustomControls
{
    public class ProgressBarExt : ProgressBar
    {

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ProgressBarExt), new PropertyMetadata(""));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

    }
}
