using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Representa um container de elementos de UI
    /// </summary>
    public class MenuContainer:ItemsControl
    {
        static MenuContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuContainer),
                new FrameworkPropertyMetadata(typeof(MenuContainer))); 
        }

        /// <summary>
        /// Obtém ou define o rótulo do container
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
            typeof(MenuContainer), new FrameworkPropertyMetadata());


        /// <summary>
        /// Obtém ou define o rótulo do container
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
    }
}
