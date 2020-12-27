using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.CustomControls.Error
{
    public class ErrorButton : Button
    {
        public static readonly DependencyProperty CopyErrorMessageCommandProperty = DependencyProperty.Register(
            "CopyErrorMessageCommand", typeof(ICommand), typeof(ErrorButton), new PropertyMetadata(default(ICommand)));

        public ICommand CopyErrorMessageCommand
        {
            get => (ICommand)GetValue(CopyErrorMessageCommandProperty);
            private set => SetValue(CopyErrorMessageCommandProperty, value);
        }

        public static readonly DependencyProperty NoErrorVisibilityProperty = DependencyProperty.Register(
            "NoErrorVisibility", typeof(Visibility), typeof(ErrorButton), new PropertyMetadata(Visibility.Hidden, OnNoErrorVisibilityChanged));

        private static void OnNoErrorVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ErrorButton)d).UpdateDisplay();
        }

        public Visibility NoErrorVisibility
        {
            get => (Visibility)GetValue(NoErrorVisibilityProperty);
            set => SetValue(NoErrorVisibilityProperty, value);
        }

        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.Register(
            "Errors", typeof(ErrorsCollection), typeof(ErrorButton), new PropertyMetadata(default(ErrorsCollection), OnErrorsDependencyPropertyChanged));

        private static void OnErrorsDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ErrorButton)d).OnErrorsPropertyChanged((ErrorsCollection)e.OldValue, (ErrorsCollection)e.NewValue);
        }

        private void OnErrorsPropertyChanged(ErrorsCollection? oldErrors, ErrorsCollection? newErrors)
        {
            if (oldErrors != null)
                oldErrors.CollectionChanged -= OnErrorsCollectionChanged;

            if (newErrors != null)
                newErrors.CollectionChanged += OnErrorsCollectionChanged;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            Visibility = (Errors == null) ? NoErrorVisibility : Visibility.Visible;

            var errors = Errors;
            if (errors != null && errors.Count > 0)
                Visibility = Visibility.Visible;
            else
                Visibility = NoErrorVisibility;
        }

        private void OnErrorsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateDisplay();
        }

        public ErrorsCollection? Errors
        {
            get => (ErrorsCollection)GetValue(ErrorsProperty);
            set => SetValue(ErrorsProperty, value);
        }

        public ErrorButton()
        {
            UpdateDisplay();
            CopyErrorMessageCommand = new RelayCommand(OnCopyErrors);
        }

        private void OnCopyErrors()
        {
            var errors = Errors;
            string errorsText = errors != null ? string.Join(Environment.NewLine, errors.Select(err => $"‣ {err.Message}")) : "";

            Clipboard.SetText(errorsText);
        }

        protected override void OnClick()
        {
            var errors = Errors;
            errors?.Clear();
            UpdateDisplay();
        }

    }
}
