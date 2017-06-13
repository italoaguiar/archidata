using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Archidata
{
    public class EmptyCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return false;
        }

        private void Invoke()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

        public void Execute(object parameter)
        {
            
        }
    }
}
