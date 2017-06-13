using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Specialized;
using System;

namespace Archidata.Core.Plugin.Diagram
{
    /// <summary>
    /// Representa um objeto de diagrama
    /// </summary>
    public class DiagramView : ListBox
    {
        #region Contructors


        static DiagramView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramView),
                new FrameworkPropertyMetadata(typeof(DiagramView)));
        }

        public DiagramView()
        {
            SelectionMode = SelectionMode.Extended;
            
        }

        //private void OnPointerDown(object sender, MouseButtonEventArgs e)
        //{
        //    SelectedItem = null;
        //}

        #endregion

        /// <summary>
        /// Verifica se o tipo do container foi sobrescrito
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is DiagramViewItem);
        }

        /// <summary>
        /// Define um novo tipo para o container
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DiagramViewItem();
        }

        /// <summary>
        /// Notifica quando a coleção de itens é modificada
        /// </summary>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Notifica quando um ou mais itens são selecionados ou deselecionados
        /// </summary>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (!SelectionEnabled)
            {
                SelectedItems.Clear();
                e.Handled = true;
            }
            else
               base.OnSelectionChanged(e);
        }

        /// <summary>
        /// Obtém ou define se os itens podem ser selecionados
        /// </summary>
        public static readonly DependencyProperty CanSelectItemsProperty =
            DependencyProperty.Register("SelectionEnabled", typeof(bool),
            typeof(DiagramView), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Obtém ou define se os itens podem ser selecionados
        /// </summary>
        public bool SelectionEnabled
        {
            get { return (bool)GetValue(CanSelectItemsProperty); }
            set { SetValue(CanSelectItemsProperty, value); }
        }

        /// <summary>
        /// Disparado quando o botão esquerdo do mouse é pressionado
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if(SelectionMode == SelectionMode.Extended || SelectionMode == SelectionMode.Multiple)
            {
                foreach(var i in SelectedItems)
                {
                    var itm = ItemContainerGenerator.ContainerFromItem(i) as DiagramViewItem;
                    if (itm.IsMouseOver)
                        return;
                }
                SelectedItems.Clear();
            }
            else if (SelectedItem != null)
            {
                var item = ItemContainerGenerator.ContainerFromItem(SelectedItem) as DiagramViewItem;
                if (!item.IsMouseOver)
                    SelectedItem = null;
            }            
        }


        public event EventHandler<MouseButtonEventArgs> ItemDoubleClick;
        public event EventHandler<MouseButtonEventArgs> ItemClick;

        internal void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemDoubleClick?.Invoke(sender, e);
        }

        internal void OnItemClick(object sender, MouseButtonEventArgs e)
        {
            ItemClick?.Invoke(sender, e);
        }
    }
}
