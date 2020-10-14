using System.Windows;
using System.Windows.Controls;

namespace FileSwissKnife.CustomControls
{
    public class TextBoxExt : TextBox
    {

        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
            "WatermarkText", typeof(string), typeof(TextBoxExt), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty WatermarkVisibilityProperty = DependencyProperty.Register(
            "WatermarkVisibility", typeof(Visibility), typeof(TextBoxExt), new PropertyMetadata(Visibility.Visible));

        public Visibility WatermarkVisibility => (Visibility) GetValue(WatermarkVisibilityProperty);

        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            var watermarkVisibility = string.IsNullOrEmpty(this.Text) ? Visibility.Visible : Visibility.Hidden;

            SetValue(WatermarkVisibilityProperty, watermarkVisibility);
        }
    }
}
