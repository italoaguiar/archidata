using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Representa uma lista de temas de UI
    /// </summary>
    public class ThemeList : ListBox
    {
        /// <summary>
        /// Obtém o conteiner associado ao item da lista.
        /// </summary>   
        protected override DependencyObject GetContainerForItemOverride()
        {
            ThemeListItem item = new ThemeListItem();
            item.MouseEnter += (s, e) =>
            {
                ItemMouseEnter?.Invoke(s, e);
            };
            item.MouseLeave += (s, e) =>
            {
                ItemMouseLeave?.Invoke(s, e);
            };
            item.PreviewMouseLeftButtonDown += (s, e) =>
            {
                ItemClick?.Invoke(s, e);
            };
            return item;
        }
        /// <summary>
        /// Dispara um evento quando o mouse está sobre o controle.
        /// </summary>
        public event EventHandler<MouseEventArgs> ItemMouseEnter;
        /// <summary>
        /// Dispara um evento quando o mouse sai do controle.
        /// </summary>
        public event EventHandler<MouseEventArgs> ItemMouseLeave;
        /// <summary>
        /// Dispara um evento quando o mouse clica sobre o controle.
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> ItemClick;


    }
    /// <summary>
    /// Representa um item da lista ThemeList
    /// </summary>
    public class ThemeListItem : ListBoxItem
    {

    }
}
