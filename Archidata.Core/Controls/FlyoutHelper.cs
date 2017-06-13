using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Effects;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Dispões métodos e propriedades para exibição de Flyouts personalizados
    /// </summary>
    public class FlyoutHelper
    {


        /// <summary>
        /// Associa um Flyout a elemento de UI
        /// </summary>
        /// <param name="obj">Elemento pai</param>
        /// <param name="value">Flyout</param>
        public static void SetFlyout(DependencyObject obj, Flyout value)
        {
            obj.SetValue(FlyoutProperty, value);
        }



        /// <summary>
        /// Obtém um Flyout associado a um elemento de UI
        /// </summary>
        /// <param name="obj">Elemento pai</param>
        /// <returns></returns>
        public static Flyout GetFlyout(DependencyObject obj)
        {
            return (Flyout)obj.GetValue(FlyoutProperty);
        }



        /// <summary>
        /// Define a propriedade dependente associada ao Flyout
        /// </summary>
        public static readonly DependencyProperty FlyoutProperty =
            DependencyProperty.RegisterAttached(
                "Flyout",
                typeof(Flyout),
                typeof(FlyoutHelper),
                new PropertyMetadata(null, OnFlyoutContentPropertyChanged));




        private static void OnFlyoutContentPropertyChanged(DependencyObject obj,
             DependencyPropertyChangedEventArgs e)
        {
            var bt = obj as Button;
            if (bt == null)
                return;

            var fly = e.NewValue as Flyout;
            fly.PlacementTarget = bt;

            bt.Click += (s, a) =>
            {
                fly.Open();
            };
        }
    }
    /// <summary>
    /// Exibe elementos gráficos flutuantes
    /// </summary>
    [ContentProperty("Content")]
    public class Flyout : Control
    {
        Popup popup = new Popup();

        /// <summary>
        /// Cria uma nova instância de Flyout
        /// </summary>
        public Flyout()
        {
            Binding b = new Binding("IsOpen");
            b.Source = this;
            b.Mode = BindingMode.TwoWay;
            popup.SetBinding(Popup.IsOpenProperty, b);
        }


        /// <summary>
        /// Abre um Flyout
        /// </summary>
        public void Open()
        {
            popup.PlacementTarget = PlacementTarget == null ? null : PlacementTarget;
            popup.Width = Width;
            popup.Height = Height;            
            popup.Child = Content;
            popup.StaysOpen = false;
            popup.PopupAnimation = PopupAnimation.Fade;
            popup.AllowsTransparency = true;       
            IsOpen = true;

            
            
        }



        /// <summary>
        /// Obtém ou define o conteúdo do Flyout
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement),
            typeof(Flyout), new FrameworkPropertyMetadata(null));



        /// <summary>
        /// Obtém ou define o conteúdo do Flyout
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
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
            typeof(Flyout), new FrameworkPropertyMetadata(false,new PropertyChangedCallback(OnIsOpenChanged)));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Flyout)d).popup.IsOpen = (bool)e.NewValue;
        }

        /// <summary>
        /// Obtém ou define se o flyout é fechado quando for clicado
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
    }
}
