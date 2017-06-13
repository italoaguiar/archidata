using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Representa um botão de menu filho de MenuContainer
    /// </summary>
    public class MenuButton:Button
    {
        static MenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButton),
                new FrameworkPropertyMetadata(typeof(MenuButton)));
        }



        /// <summary>
        /// Obtém ou define a orientação do botão de menu (Horizontal ou Vertical).
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(LayoutOrientation),
            typeof(MenuButton), new FrameworkPropertyMetadata());




        /// <summary>
        /// Obtém ou define a orientação do botão de menu (Horizontal ou Vertical).
        /// </summary>
        public LayoutOrientation Orientation
        {
            get { return (LayoutOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }



        /// <summary>
        /// Define a orientação do botão de menu (Horizontal ou Vertical).
        /// </summary>
        public enum LayoutOrientation
        {
            /// <summary>
            /// Posição Vertical de Layout
            /// </summary>
            Vertical,
            /// <summary>
            /// Posição Horizontal de Layout
            /// </summary>
            Horizontal
        }



        /// <summary>
        /// Obtém ou define o rótulo do botão.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
            typeof(MenuButton), new FrameworkPropertyMetadata());




        /// <summary>
        /// Obtém ou define o rótulo do botão.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }



        /// <summary>
        /// Obtém ou define o ícone a ser exibido no botão.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource),
            typeof(MenuButton), new FrameworkPropertyMetadata());

        
        
        
        /// <summary>
        /// Obtém ou define o ícone a ser exibido no botão.
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
