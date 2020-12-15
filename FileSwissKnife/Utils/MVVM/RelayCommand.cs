using System;
using System.Windows;
using System.Windows.Input;

namespace FileSwissKnife.Utils.MVVM
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null) :
            this(WrapExecuteWithArg(execute), WrapCanExecuteWithArg(canExecute))
        {
        }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }


        private static Action<object?> WrapExecuteWithArg(Action execute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            return o => { execute(); };
        }

        private static Predicate<object?>? WrapCanExecuteWithArg(Func<bool>? canExecute)
        {
            if (canExecute == null)
                return null;

            return o => canExecute();
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public virtual event EventHandler? CanExecuteChanged;

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }


        public virtual void TriggerCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler == null)
                return;

            var fireEventAction = new Action(() => { handler(this, new EventArgs()); });

            var dispatchedForUiThread = false;
            var application = Application.Current;
            var uiDispatcher = application?.Dispatcher;

            if (uiDispatcher != null)
            {
                if (!uiDispatcher.CheckAccess())
                {
                    uiDispatcher.Invoke(fireEventAction);
                    dispatchedForUiThread = true;
                }
            }

            if (!dispatchedForUiThread)
                fireEventAction();
        }
    }
}