using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Archidata
{
    /// <summary>
    /// Interaction logic for CanvasSetup.xaml
    /// </summary>
    public partial class CanvasSetup : Window
    {
        public CanvasSetup()
        {
            InitializeComponent();


            List<PaperSize> p = new List<PaperSize>();
            p.Add(new PaperSize() { SizeName = "A4 Retrato (200dpi)", CanvasSize = new Size(1654, 2339) });
            p.Add(new PaperSize() { SizeName = "A4 Paisagem (200dpi)", CanvasSize = new Size(2339, 1654) });
            p.Add(new PaperSize() { SizeName = "A3 Retrato (200dpi)", CanvasSize = new Size(3307, 2338) });
            p.Add(new PaperSize() { SizeName = "A3 Paisagem (200dpi)", CanvasSize = new Size(2338, 3307) });

            paperSizes.ItemsSource = p;
        }

        public static readonly DependencyProperty CanvasSizeProperty =
            DependencyProperty.Register("CanvasSize", typeof(Size),
            typeof(CanvasSetup), new FrameworkPropertyMetadata(new Size(), new PropertyChangedCallback(OnCanvasSizeChanged)));

        private static void OnCanvasSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CanvasSetup)d).CheckSize();
        }

        public Size CanvasSize
        {
            get { return (Size)GetValue(CanvasSizeProperty); }
            set { SetValue(CanvasSizeProperty, value); }
        }

        private void orientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckSize();
        }

        private void CheckSize()
        {
            if (orientation.SelectedIndex == 0)
            {
                if (CanvasSize.Width > CanvasSize.Height)
                {
                    var x = CanvasSize;
                    CanvasSize = new Size(x.Height, x.Width);
                }
            }
            else
            {
                if (CanvasSize.Width < CanvasSize.Height)
                {
                    var x = CanvasSize;
                    CanvasSize = new Size(x.Height, x.Width);
                }
            }
        }

        private void paperSizes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CanvasSize = (paperSizes.SelectedItem as PaperSize).CanvasSize;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    internal class PaperSize
    {
        public string SizeName { get; set; }
        public Size CanvasSize { get; set; }

        public override string ToString()
        {
            return SizeName;
        }
    }

}
