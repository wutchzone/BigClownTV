using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BigClownAppTV.Base.Commands
{
    public class BasicCommand : ICommand
    {
        protected  Action _execute;
        public event EventHandler CanExecuteChanged;

        public BasicCommand(Action execute)
        {
            this._execute = execute;
        }
        protected BasicCommand()
        {
            
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            _execute?.Invoke();
        }
    }
}
