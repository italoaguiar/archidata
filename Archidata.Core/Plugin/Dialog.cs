using Archidata.Core.Helpers;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Archidata.Core.Plugin
{
    /// <summary>
    /// Representa uma caixa de diálogo Overlay
    /// </summary>
    public class Dialog : INotifyPropertyChanged
    {
        Visibility visibility = Visibility.Collapsed;
        /// <summary>
        /// Obtém ou define a visibilidade do diálogo
        /// </summary>
        public Visibility Visibility
        {
            get { return visibility; }
            set { visibility = value; onPropertyChanged("Visibility"); }
        }

        Page page;
        /// <summary>
        /// Obtém ou define a página de conteúdo do diálogo
        /// </summary>
        public Page Page
        {
            get { return page; }
            set { page = value; onPropertyChanged("Page"); }
        }

        /// <summary>
        /// Cria uma nova instância de Dialog
        /// </summary>
        protected Dialog()
        {
            closeCommand = new CommandAdapter((o) =>
            {
                Visibility = Visibility.Collapsed;
            });
            closeCommand.CanExecuteAction = true;
        }

        static CommandAdapter closeCommand;

        /// <summary>
        /// Informa quando uma propriedade é alterada
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Obtém ou define o comando Fechar
        /// </summary>
        public static ICommand CloseCommand
        {
            get { return closeCommand; }
        }

        private void onPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private static Dialog _instance;
        /// <summary>
        /// Obtém a instância do Diálogo
        /// </summary>
        public static Dialog Instance
        {
            get { return _instance ?? (_instance = new Dialog()); }
        }
        /// <summary>
        /// Exibe a janela de diálogo
        /// </summary>
        /// <param name="p">Página de conteúdo</param>
        public static void ShowDialog(Page p)
        {
            Dialog.Instance.Page = p;
            Dialog.Instance.Visibility = Visibility.Visible;
        }
    }
}
