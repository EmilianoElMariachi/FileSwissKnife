using System.Windows;
using System.Windows.Controls;

namespace FileSwissKnife.CustomControls
{
    public class ErrorButton : Button
    {

        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            "ErrorMessage", typeof(string), typeof(ErrorButton), new PropertyMetadata(default(string), OnErrorMessageChanged));

        public ErrorButton()
        {
            this.Visibility = Visibility.Hidden;
        }

        private static void OnErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var errorButton = (ErrorButton)d;
            var errorMessage = (string?)e.NewValue;
            errorButton.Visibility = errorMessage != null ? Visibility.Visible : Visibility.Hidden;
            errorButton.ToolTip = errorMessage;
        }

        public string? ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        protected override void OnClick()
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
