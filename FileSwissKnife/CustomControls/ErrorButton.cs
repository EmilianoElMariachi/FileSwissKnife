using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace FileSwissKnife.CustomControls
{
    public class ErrorButton : Button
    {
        public static readonly DependencyProperty NoErrorVisibilityProperty = DependencyProperty.Register(
            "NoErrorVisibility", typeof(Visibility), typeof(ErrorButton), new PropertyMetadata(Visibility.Hidden, OnNoErrorVisibilityChanged));

        private static void OnNoErrorVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ErrorButton)d).UpdateVisibility();
        }

        public Visibility NoErrorVisibility
        {
            get => (Visibility)GetValue(NoErrorVisibilityProperty);
            set => SetValue(NoErrorVisibilityProperty, value);
        }

        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            "ErrorMessage", typeof(string), typeof(ErrorButton), new PropertyMetadata(default(string), OnErrorMessageChanged));

        private static void OnErrorMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var errorButton = (ErrorButton)d;
            var errorMessage = (string?)e.NewValue;
            errorButton.UpdateVisibility();
            errorButton.ToolTip = errorMessage;
        }

        public string? ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        public ErrorButton()
        {
            UpdateVisibility();
        }

        protected override void OnClick()
        {
            this.Visibility = NoErrorVisibility;
        }

        private void UpdateVisibility()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            Visibility = (this.ErrorMessage == null) ? NoErrorVisibility : Visibility.Visible;
        }

    }
}
