using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OverallTest.Accessory
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> ExecuteInvoke;
        private readonly Predicate<T> CanExecuteInvoke;
        
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            this.ExecuteInvoke = execute;
            this.CanExecuteInvoke = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            var paramT = (T)(parameter ?? default(T));
            return CanExecuteInvoke == null ? true : CanExecuteInvoke(paramT);
        }
        
        public void Execute(object parameter)
        {
            var paramT = (T)(parameter ?? default(T));
            ExecuteInvoke(paramT);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            : base(execute, canExecute) { }
    }
}
