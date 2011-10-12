using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ReportWatch.Silverlight
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Predicate<Object> _canExecute = null;
        Action<Object> _executeAction = null;

        public Command(Predicate<Object> canExecute, Action<object> executeAction)
        {
            _canExecute = canExecute;
            _executeAction = executeAction;
        }


        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);

            return true;
        }

        public void UpdateCanExecuteState()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public void Execute(object parameter)
        {
            if (_executeAction != null)
                _executeAction(parameter);

            UpdateCanExecuteState();
        }
    }
}
