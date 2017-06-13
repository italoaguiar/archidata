using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Representa um menu flutuante
    /// </summary>
    [ContentProperty("Items")]
    public class FlyoutMenu: ListBox
    {

        Popup popup = new Popup();

        /// <summary>
        /// Cria uma nova instância de FlyoutMenu
        /// </summary>
        public FlyoutMenu()
        {
                        
        }

        /// <summary>
        /// Obtém ou define a posição do Flyout baseado em um elemento de UI
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register("PlacementTarget", typeof(UIElement),
            typeof(Flyout), new FrameworkPropertyMetadata(null));



        /// <summary>
        /// Obtém ou define a posição do Flyout baseado em um elemento de UI
        /// </summary>
        public UIElement PlacementTarget
        {
            get { return (UIElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }


        /// <summary>
        /// Obtém ou define se o flyout é fechado quando for clicado
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool),
            typeof(Flyout), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsOpenChanged)));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FlyoutMenu)d).popup.IsOpen = (bool)e.NewValue;
        }

        /// <summary>
        /// Obtém ou define se o flyout é fechado quando for clicado
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Obtém um contêiner para cada item da coleção de dados
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlyoutMenuItem();
        }

        /// <summary>
        /// Verifica se o contêiner foi sobrescrito
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlyoutMenuItem;
        }
    }

    /// <summary>
    /// Representa um item de FlyoutMenu
    /// </summary>
    public class FlyoutMenuItem : Button
    {
        
    }
}
