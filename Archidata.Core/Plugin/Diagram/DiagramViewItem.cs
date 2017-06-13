using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Archidata.Core.Plugin.Diagram
{
    /// <summary>
    /// Define um item do diagrama
    /// </summary>
    public class DiagramViewItem: ListBoxItem
    {
        static DiagramViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramViewItem),
                new FrameworkPropertyMetadata(typeof(DiagramViewItem)));
           
        }

        #region Implementation

        private DiagramView ParentDiagramView
        {
            get
            {
                return ParentSelector as DiagramView;
            }
        }

        internal Selector ParentSelector
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as Selector;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            ParentDiagramView.OnItemDoubleClick(this, e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                return;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            ParentDiagramView.OnItemClick(this, e);
        }

        #endregion

    }
}
