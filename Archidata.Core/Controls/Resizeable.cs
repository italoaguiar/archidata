using Archidata.Core.Plugin.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Representa um controle de conteiner redimensionável
    /// </summary>
    [TemplatePart(Name = "PART_THUMB_SE", Type = typeof(Thumb))]
    public class Resizeable: ContentControl
    {
        static Resizeable()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Resizeable),
                new FrameworkPropertyMetadata(typeof(Resizeable)));
        }

        internal Thumb PART_THUMB_SE;
        internal ContentControl PART_DRAGDELTA;
        internal Point Origin;
        private bool AreDragging;


        /// <summary>
        /// Define a lógica do controle no carregamento do template XAML
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            PART_THUMB_SE = GetTemplateChild("PART_THUMB_SE") as Thumb;
            PART_DRAGDELTA = GetTemplateChild("PART_DRAGDELTA") as ContentControl;

            if (PART_THUMB_SE == null) return;

            PART_THUMB_SE.DragDelta += PART_THUMB_SE_DragDelta;

            PART_DRAGDELTA.MouseLeftButtonDown += PART_DRAGDELTA_MouseLeftButtonDown;

            EventManager.RegisterClassHandler(typeof(DiagramView), DiagramView.PreviewMouseMoveEvent, new MouseEventHandler(MouseMoveHandler));



            //Mouse.AddPreviewMouseMoveHandler(this, new MouseEventHandler(MouseMoveHandler));
            Mouse.AddPreviewMouseUpHandler(this, new MouseButtonEventHandler(MouseButtonUpHandler));

        }

        private void MouseButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released) AreDragging = false;
        }
        //protected override void OnPreviewMouseMove(MouseEventArgs e)
        //{
        //    base.OnPreviewMouseMove(e);

        //    if (AreDragging && e.LeftButton == MouseButtonState.Pressed && IsSelected)
        //    {
        //        Point Current = e.GetPosition(PART_DRAGDELTA);
        //        double dx = Current.X - Origin.X;
        //        double dy = Current.Y - Origin.Y;


        //        Location = new Point(Location.X + dx, Location.Y + dy);

        //    }
        //}
        void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (AreDragging && e.LeftButton == MouseButtonState.Pressed && IsSelected)
            {
                Point Current = e.GetPosition(PART_DRAGDELTA);
                double dx = Current.X - Origin.X;
                double dy = Current.Y - Origin.Y;


                Location = new Point(Location.X + dx, Location.Y + dy);

            }
        }

        private void PART_DRAGDELTA_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Origin = Mouse.GetPosition(PART_DRAGDELTA);
            AreDragging = true;
        }

        private void PART_THUMB_SE_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ResizeWithCell)
            {
                var w = Math.Max((int)((ActualWidth + e.HorizontalChange)/CellSize.Width) * CellSize.Width, MinWidth);
                var h = Math.Max((int)((ActualHeight + e.VerticalChange)/CellSize.Height) * CellSize.Height, MinHeight);

                Size = new Size((int)w, (int)h);
            }
            else
            {
                Size = new Size(Math.Max(ActualWidth + e.HorizontalChange, MinWidth),
                    Math.Max(ActualHeight + e.VerticalChange, MinHeight));
            }

            this.UpdateLayout();
        }
        

        /// <summary>
        /// Define uma propriedade de dependência para a propriedade IsSelected
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool),
            typeof(Resizeable), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Obtém ou define se o controle está selecionado
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }


        /// <summary>
        /// Obtém ou define se o redimensionamento é restrito a proporção
        /// </summary>
        public bool ResizeWithCell { get; set; }


        /// <summary>
        /// Obtém ou define o tamanho da célula do grid
        /// </summary>
        public Size CellSize { get; set; }




        /// <summary>
        /// Define uma propriedade de dependência para a propriedade Size
        /// </summary>
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size),
            typeof(Resizeable), new FrameworkPropertyMetadata(new Size(0,0), new PropertyChangedCallback(OnSizeChanged)));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Resizeable)d).Width = ((Size)e.NewValue).Width;
            ((Resizeable)d).Height = ((Size)e.NewValue).Height;
        }

        /// <summary>
        /// Obtém ou define o tamanho do controle
        /// </summary>
        public Size Size
        {
            get { return (Size)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }



        /// <summary>
        /// Define uma propriedade de dependência para a propriedade Point
        /// </summary>
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(Point),
            typeof(Resizeable), new FrameworkPropertyMetadata(new Point()));


        /// <summary>
        /// Obtém ou define a coordenada do controle
        /// </summary>
        public Point Location
        {
            get { return (Point)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

    }
}
