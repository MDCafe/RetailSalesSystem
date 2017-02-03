namespace RetailManagementSystem.Command
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Windows.Input;

  internal class RelayCommand<T> : ICommand
  {
    #region Fields

    readonly Action<T> _execute;
    readonly Predicate<T> _canExecute;
         
    #endregion // Fields

    #region Constructors

    public RelayCommand(Action<T> execute)
      : this(execute, null)
    {
    }

    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");

      _execute = execute;
      _canExecute = canExecute;
    }
    #endregion // Constructors

    #region ICommand Members

    public bool CanExecute(object parameter)
    {
            var param = (T)parameter;
            return _canExecute == null ? true : _canExecute(param);
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
            var param = (T)parameter;
            _execute(param);
    }

    #endregion // ICommand Members
  }
}
