using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ER
{
    [TemplatePart(Name="PART_COMBOSELECTOR", Type=typeof(ComboBox))]
    public class Connector:Control
    {

        static Connector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Connector),
                new FrameworkPropertyMetadata(typeof(Connector)));
        }

        private const int OFFSET = 15;
        protected ComboBox PART_COMBOSELECTOR;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_COMBOSELECTOR = GetTemplateChild("PART_COMBOSELECTOR") as ComboBox;
            if(PART_COMBOSELECTOR != null)
            {
                PART_COMBOSELECTOR.SelectionChanged += PART_COMBOSELECTOR_SelectionChanged;
            }
            UpdateCardinality();
        }

        private void PART_COMBOSELECTOR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cardinality = ((ComboBox)sender).SelectedIndex == 0? Cardinality.MULTI : Cardinality.ONE;
        }

        private bool _useCardinality;

        public bool UseCardinality
        {
            get { return _useCardinality; }
            set
            {
                _useCardinality = value;
                CardinalityVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static readonly DependencyProperty TargetLocationProperty =
            DependencyProperty.Register("TargetLocation", typeof(Point),
            typeof(Connector), new FrameworkPropertyMetadata(new Point(0,0), new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty OriginLocationProperty =
            DependencyProperty.Register("OriginLocation", typeof(Point),
            typeof(Connector), new FrameworkPropertyMetadata(new Point(0, 0), new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty TargetSizeProperty =
            DependencyProperty.Register("TargetSize", typeof(Size),
            typeof(Connector), new FrameworkPropertyMetadata(new Size(0,0), new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty OriginSizeProperty =
            DependencyProperty.Register("OriginSize", typeof(Size),
            typeof(Connector), new FrameworkPropertyMetadata(new Size(0, 0), new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection),
            typeof(Connector), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty MediumPointProperty =
            DependencyProperty.Register("MediumPoint", typeof(Point),
            typeof(Connector), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty CardinalityProperty =
            DependencyProperty.Register("Cardinality", typeof(Cardinality),
            typeof(Connector), new FrameworkPropertyMetadata(Cardinality.ONE, new PropertyChangedCallback(OnCardinalityChanged)));

        public static readonly DependencyProperty CardinalityVisibilityProperty =
            DependencyProperty.Register("CardinalityVisibility", typeof(Visibility),
            typeof(Connector), new FrameworkPropertyMetadata(Visibility.Visible));


        public Point TargetLocation
        {
            get { return (Point)GetValue(TargetLocationProperty); }
            set { SetValue(TargetLocationProperty, value); }
        }
        public Point OriginLocation
        {
            get { return (Point)GetValue(OriginLocationProperty); }
            set { SetValue(OriginLocationProperty, value); }
        }
        public Size TargetSize
        {
            get { return (Size)GetValue(TargetSizeProperty); }
            set { SetValue(TargetSizeProperty, value); }
        }
        public Size OriginSize
        {
            get { return (Size)GetValue(OriginSizeProperty); }
            set { SetValue(OriginSizeProperty, value); }
        }
        public Cardinality Cardinality
        {
            get { return (Cardinality)GetValue(CardinalityProperty); }
            set { SetValue(CardinalityProperty, value); }
        }
        protected PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
        protected Point MediumPoint
        {
            get { return (Point)GetValue(MediumPointProperty); }
            set { SetValue(MediumPointProperty, value); }
        }

        protected Visibility CardinalityVisibility
        {
            get { return (Visibility)GetValue(CardinalityVisibilityProperty); }
            set { SetValue(CardinalityVisibilityProperty, value); }
        }


        protected Point OriginPoint = new Point(0, 0);
        protected Point OriginMediumPoint = new Point(0, 0);
        protected Point TargetPoint = new Point(0, 0);
        protected Point TargetMediumPoint = new Point(0, 0);


        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Connector)d).UpdatePointCollection();
        }

        private static void OnCardinalityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Connector c = d as Connector;
            c.UpdateCardinality();
        }

        protected virtual void UpdatePointCollection()
        {
            
            CalcVerticalConnection();
            if(OriginMediumPoint.Y > OriginPoint.Y - OriginSize.Height/2 - OFFSET &&
                OriginMediumPoint.Y < OriginPoint.Y + OriginSize.Height / 2 + OFFSET)
                CalcHorizontalConnection();

            PointCollection p = new PointCollection();
            p.Add(OriginPoint);
            p.Add(OriginMediumPoint);
            p.Add(TargetMediumPoint);
            p.Add(TargetPoint);

            Points = p;
        }

        private void CalcVerticalConnection()
        {
            OriginPoint = new Point(OriginLocation.X + OriginSize.Width / 2, OriginLocation.Y + OriginSize.Height / 2);
            double dy = OriginPoint.Y + (TargetPoint.Y - OriginPoint.Y) / 2;
            OriginMediumPoint = new Point(OriginPoint.X, dy);

            TargetPoint = new Point(TargetLocation.X + TargetSize.Width / 2, TargetLocation.Y + TargetSize.Height / 2);
            dy = TargetPoint.Y - (TargetPoint.Y - OriginPoint.Y) / 2;
            TargetMediumPoint = new Point(TargetPoint.X, dy);

            MediumPoint = new Point((OriginMediumPoint.X + TargetMediumPoint.X) / 2, (OriginMediumPoint.Y + TargetMediumPoint.Y) / 2);
        }

        private void CalcHorizontalConnection()
        {
            OriginPoint = new Point(OriginLocation.X + OriginSize.Width / 2, OriginLocation.Y + OriginSize.Height / 2);
            double dx = OriginPoint.X + (TargetPoint.X - OriginPoint.X) / 2;
            OriginMediumPoint = new Point(dx, OriginPoint.Y);

            TargetPoint = new Point(TargetLocation.X + TargetSize.Width / 2, TargetLocation.Y + TargetSize.Height / 2);
            dx = TargetPoint.X - (TargetPoint.X - OriginPoint.X) / 2;
            TargetMediumPoint = new Point(dx, TargetPoint.Y);

            MediumPoint = new Point((OriginMediumPoint.X + TargetMediumPoint.X) / 2, (OriginMediumPoint.Y + TargetMediumPoint.Y) / 2);

            if (TargetPoint.X < TargetMediumPoint.X) CalcMediumConnection();
        }

        private void CalcMediumConnection()
        {
            OriginPoint = new Point(OriginLocation.X + OriginSize.Width / 2, OriginLocation.Y + OriginSize.Height / 2);
            TargetPoint = new Point(TargetLocation.X + TargetSize.Width / 2, TargetLocation.Y + TargetSize.Height / 2);

            double maxHeight = Math.Max(OriginSize.Height, TargetSize.Height);

            if (OriginPoint.Y > TargetPoint.Y)
            {
                OriginMediumPoint = new Point(OriginPoint.X, OriginPoint.Y + maxHeight / 2 + 2 * OFFSET);
                TargetMediumPoint = new Point(TargetPoint.X, OriginPoint.Y + maxHeight / 2 + 2 * OFFSET);
            }
            else
            {
                OriginMediumPoint = new Point(OriginPoint.X, OriginPoint.Y - maxHeight / 2 - 2 * OFFSET);
                TargetMediumPoint = new Point(TargetPoint.X, OriginPoint.Y - maxHeight / 2 - 2 * OFFSET);
            }

            MediumPoint = new Point((OriginMediumPoint.X + TargetMediumPoint.X) / 2, (OriginMediumPoint.Y + TargetMediumPoint.Y) / 2);
        }

        private void UpdateCardinality()
        {
            if (PART_COMBOSELECTOR != null)
                PART_COMBOSELECTOR.SelectedIndex = (int)Cardinality;
        }
    }
}
