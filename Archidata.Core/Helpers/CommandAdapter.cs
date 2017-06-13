using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Archidata.Core.Helpers
{
    /// <summary>
    /// Cria um adaptador para comandos
    /// </summary>
    public class CommandAdapter : ICommand
    {
        /// <summary>
        /// Cria uma nova instância de CommandAdapter
        /// </summary>
        /// <param name="executeAction"></param>
        public CommandAdapter(Action<object> executeAction)
        {
            this.ExecuteAction = executeAction;
        }
        /// <summary>
        /// Ação a ser executada pelo comando
        /// </summary>
        public Action<object> ExecuteAction { get; set; }
        private bool canExecuteAction;

        /// <summary>
        /// Obtém ou define se a ação pode ser executada
        /// </summary>
        public bool CanExecuteAction
        {
            get { return canExecuteAction; }
            set
            {
                canExecuteAction = value;
                CanExecuteChanged?.Invoke(this, null);
            }
        }

        /// <summary>
        /// Notifica uma alteração de estado de execução
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Verifica se a ação pode ser executada
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return canExecuteAction;
        }

        /// <summary>
        /// Executa ação determinada
        /// </summary>
        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }
    }
}
