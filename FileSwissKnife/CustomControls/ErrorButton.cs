using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FileSwissKnife.Utils.MVVM;

namespace FileSwissKnife.CustomControls
{
    public class ErrorButton : Button
    {
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

            Visibility = (this.Errors == null) ? NoErrorVisibility : Visibility.Visible;

            var errors = Errors;
            if (errors != null && errors.Count > 0)
                this.Visibility = Visibility.Visible;
            else
                this.Visibility = NoErrorVisibility;
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
        }

        protected override void OnClick()
        {
            var errors = Errors;
            errors?.CleanDeletable();
            UpdateDisplay();
        }

    }

    public class ErrorsCollection : ObservableCollection<ErrorViewModel>
    {
        public void CleanDeletable()
        {
            var errorsToDelete = new List<ErrorViewModel>();
            foreach (var error in this)
            {
                if (error.Deletable)
                    errorsToDelete.Add(error);
            }

            foreach (var errorToDelete in errorsToDelete)
            {
                this.Remove(errorToDelete);
            }
        }

        protected override void InsertItem(int index, ErrorViewModel item)
        {
            if (this.Contains(item))
                return;
            base.InsertItem(index, item);
        }
    }

    public class ErrorViewModel : ViewModelBase
    {
        private string _message;

        public ErrorViewModel(bool deletable)
        {
            Deletable = deletable;
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public bool Deletable { get; }
    }
}
