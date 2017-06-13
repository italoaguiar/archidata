using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Representa um botão de menu filho de MenuContainer
    /// </summary>
    public class MenuRadioButton:RadioButton
    {
        static MenuRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuRadioButton),
                new FrameworkPropertyMetadata(typeof(MenuRadioButton)));

        }
        
        
        
        /// <summary>
        /// Obtém ou define a orientação do botão de radio.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(LayoutOrientation),
            typeof(MenuRadioButton), new FrameworkPropertyMetadata());

       
        
        
        /// <summary>
        /// Obtém ou define a orientação do botão de radio.
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
        /// Obtém ou define o rótulo do botão de radio.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
            typeof(MenuRadioButton), new FrameworkPropertyMetadata());

        
        
        
        /// <summary>
        /// Obtém ou define o rótulo do botão de radio.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }



        /// <summary>
        /// Obtém ou define o ícone do botão de rádio.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource),
            typeof(MenuRadioButton), new FrameworkPropertyMetadata());

        /// <summary>
        /// Obtém ou define o ícone do botão de rádio.
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
