using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Representa o Menu Arquivo da aplicação
    /// </summary>
    [TemplatePart(Name = "PART_BTN", Type = typeof(Button))]
    public class FileMenu: ItemsControl
    {
        static FileMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileMenu),
                new FrameworkPropertyMetadata(typeof(FileMenu)));
            
        }

        private Button PART_BTN;

        /// <summary>
        /// Método chamado na aplicação do template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_BTN = GetTemplateChild("PART_BTN") as Button;

            if(PART_BTN != null)
            {
                PART_BTN.Click += (s, a) =>
                {
                    IsOpen = true;
                };
            }            
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsOpen = false;
        }

        /// <summary>
        /// Obtém ou define se o flyout é fechado quando for clicado
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool),
            typeof(FileMenu), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define se o flyout é fechado quando for clicado
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }


        private UIElementCollection _menuItems;

        /// <summary>
        /// Obtém uma instância da coleção de elementos visuais
        /// </summary>
        protected virtual UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return new UIElementCollection(this, logicalParent);
        }
        

        /// <summary>
        /// Obtém ou define os elementos do menu
        /// </summary>        
        public UIElementCollection MenuItems
        {
            get
            {
                if(_menuItems == null)
                    _menuItems = CreateUIElementCollection(null);

                return _menuItems;
            }
            set { _menuItems = value; }
        }


        /// <summary>
        /// Obtém ou define os elementos do menu
        /// </summary>
        public static readonly DependencyProperty RightContainerProperty =
            DependencyProperty.Register("RightContainer", typeof(FrameworkElement),
            typeof(FileMenu), new FrameworkPropertyMetadata(null));



        /// <summary>
        /// Obtém ou define os elementos do menu
        /// </summary>
        public FrameworkElement RightContainer
        {
            get { return (FrameworkElement)GetValue(RightContainerProperty); }
            set { SetValue(RightContainerProperty, value); }
        }

        /// <summary>
        /// Disparado quando o botão é clicado
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            IsOpen = true;
        }

        /// <summary>
        /// Obtém um novo contêiner para cada item
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FileMenuItem();
        }

        /// <summary>
        /// Verifica se um item de contêiner foi sobrescrito
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FileMenuItem;
        }



    }

    /// <summary>
    /// Representa um item de menu
    /// </summary>
    public class FileMenuItem: Button
    {
        static FileMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileMenuItem),
                new FrameworkPropertyMetadata(typeof(FileMenuItem)));

        }

        /// <summary>
        /// Obtém ou define os elementos do menu
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource),
            typeof(FileMenuItem), new FrameworkPropertyMetadata(null));



        /// <summary>
        /// Obtém ou define os elementos do menu
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Obtém ou define os elementos do menu
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
            typeof(FileMenuItem), new FrameworkPropertyMetadata(null));



        /// <summary>
        /// Obtém ou define os elementos do menu
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Chamado quando o item é clicado pelo usuário
        /// </summary>
        protected override void OnClick()
        {
            ParentMenu.IsOpen = false;

            base.OnClick();            
        }

        private FileMenu ParentMenu
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as FileMenu;
            }
        }
    }

}
