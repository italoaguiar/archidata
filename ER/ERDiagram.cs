using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Archidata.Core.Plugin;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Archidata.Core.Plugin.Diagram;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Collections;
using Archidata.Core.Controls;
using System.Windows.Shapes;
using System.Reflection;
using ER.Model;
using System.Runtime.Serialization.Formatters.Binary;

namespace ER
{
    [Export(typeof(IUIPlugin))]
    [TemplatePart(Name = "PART_OVERLAY_CANVAS", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_DIAGRAM", Type = typeof(DiagramView))]
    public class ERDiagram: UIPlugin
    {
        static ERDiagram()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ERDiagram),
                new FrameworkPropertyMetadata(typeof(ERDiagram)));
        }

        protected DiagramView PART_DIAGRAM;
        protected Canvas PART_OVERLAY_CANVAS;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_OVERLAY_CANVAS = GetTemplateChild("PART_OVERLAY_CANVAS") as Canvas;
            PART_DIAGRAM = GetTemplateChild("PART_DIAGRAM") as DiagramView;
            PART_DIAGRAM.SelectionChanged += OnSelectionChanged;
            PART_DIAGRAM.ItemDoubleClick += OnItemDoubleClick;
            PART_DIAGRAM.RequestBringIntoView += (s, a) => a.Handled = true;

            CanPrint = true;
            CanExport = true;

            InitializeCommandBar(); 
            Select();

            new StateService(stateManager, () => 
            {
                return ((ModelCollection)DataSource).Clone();
            });

            stateManager.StateChanged += (s, a) =>
            {
                this.CanUndo = stateManager.CanUndo;
                this.CanRedo = stateManager.CanRedo;
                this.CanSave = true;
            };

        }

        #region Fields

        DiagramState CurrentDiagramState = DiagramState.Select;
        Type DiagramTypeToAdd;
        Point StartPoint = new Point();
        Line LineConnector = new Line()
        {
            Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
            StrokeDashArray = new DoubleCollection() { 3, 1 },
            StrokeThickness = 5
        };

        #endregion

        #region Commands

        private void InitializeCommandBar()
        {
            MenuContainer addMenu = new MenuContainer();
            addMenu.Label = "Adicionar";

            MenuRadioButton e = new MenuRadioButton();
            e.Label = "Entidade";
            e.Icon = new BitmapImage(new Uri("/ER;component/Icons/Entity.png", UriKind.Relative));
            e.Checked += (s, a) => { DiagramTypeToAdd = typeof(Entity); };
            e.IsChecked = true;

            MenuRadioButton r = new MenuRadioButton();
            r.Label = "Relacionamento";
            r.Icon = new BitmapImage(new Uri("/ER;component/Icons/Relationship.png", UriKind.Relative));
            r.Checked += (s, a) => { DiagramTypeToAdd = typeof(Relationship); };

            MenuRadioButton sp = new MenuRadioButton();
            sp.Label = "Especialização";
            sp.Icon = new BitmapImage(new Uri("/ER;component/Icons/Disconnect.png", UriKind.Relative));
            sp.Checked += (s, a) => { DiagramTypeToAdd = typeof(Specialization); };

            addMenu.Items.Add(e);
            addMenu.Items.Add(r);
            addMenu.Items.Add(sp);


            MenuContainer toolMenu = new MenuContainer();
            toolMenu.Label = "Ferramentas";

            MenuRadioButton sl = new MenuRadioButton();
            sl.Label = "Selecionar";
            sl.Icon = new BitmapImage(new Uri("/ER;component/Icons/Select.png", UriKind.Relative));
            sl.IsChecked = true;
            sl.Checked += (s, a) => Select();

            MenuRadioButton add = new MenuRadioButton();
            add.Label = "Adicionar";
            add.Icon = new BitmapImage(new Uri("/ER;component/Icons/Draw.png", UriKind.Relative));
            add.Orientation = MenuRadioButton.LayoutOrientation.Horizontal;
            add.Checked += (s, a) => Add();

            MenuRadioButton c = new MenuRadioButton();
            c.Label = "Connectar";
            c.Icon = new BitmapImage(new Uri("/ER;component/Icons/Connect.png", UriKind.Relative));
            c.Orientation = MenuRadioButton.LayoutOrientation.Horizontal;
            c.Checked += (s, a) => Connect();

            MenuRadioButton rm = new MenuRadioButton();
            rm.Label = "Remover";
            rm.Icon = new BitmapImage(new Uri("/ER;component/Icons/Delete.png", UriKind.Relative));
            rm.Orientation = MenuRadioButton.LayoutOrientation.Horizontal;
            rm.Checked += (s, a) => Remove();

            MenuRadioButton dc = new MenuRadioButton();
            dc.Label = "Desconnectar";
            dc.Icon = new BitmapImage(new Uri("/ER;component/Icons/Disconnect.png", UriKind.Relative));
            dc.Orientation = MenuRadioButton.LayoutOrientation.Horizontal;
            dc.Checked += (s, a) => Disconnect();            

            toolMenu.Items.Add(sl);
            toolMenu.Items.Add(add);
            toolMenu.Items.Add(c);
            toolMenu.Items.Add(rm);
            toolMenu.Items.Add(dc);            

            MenuControls.Add(addMenu);
            MenuControls.Add(toolMenu);
        }

        private void Select()
        {
            PART_DIAGRAM.SelectionEnabled = true;
            CurrentDiagramState = DiagramState.Select;
            SetCursor(DiagramCursors.Arrow);
        }

        private void Add()
        {
            PART_DIAGRAM.SelectionEnabled = false;
            CurrentDiagramState = DiagramState.Add;
            SetCursor(DiagramCursors.Add);
            ClearSelection();
        }

        private void Remove()
        {
            PART_DIAGRAM.SelectionEnabled = true;
            CurrentDiagramState = DiagramState.Remove;
            SetCursor(DiagramCursors.Remove);
            ClearSelection();
        }

        private void Connect()
        {
            PART_DIAGRAM.SelectionEnabled = false;
            CurrentDiagramState = DiagramState.Connect;
            SetCursor(DiagramCursors.Cross);
            ClearSelection();
            LineConnector.Stroke = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        }

        private void Disconnect()
        {
            PART_DIAGRAM.SelectionEnabled = false;
            CurrentDiagramState = DiagramState.Disconnect;
            SetCursor(DiagramCursors.RedCross);
            ClearSelection();
            LineConnector.Stroke = new SolidColorBrush(Colors.DarkRed);
        }

        #endregion

        #region Interaction
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.Key)
            {
                case Key.Delete:
                    if (PART_DIAGRAM.SelectedItem != null)
                    {
                        var selected = PART_DIAGRAM.SelectedItem as ModelElement;
                        RemoveElement(selected);
                    }
                    break;

                case Key.C:
                    if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        Copy();
                    }
                    break;
                case Key.X:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        Cut();
                    }
                    break;
                case Key.V:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        Paste();
                    }
                    break;
                case Key.Z:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        Undo();
                    }
                    break;
                case Key.R:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        Redo();
                    }
                    break;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            StartPoint = Mouse.GetPosition(PART_DIAGRAM);

            switch (CurrentDiagramState)
            {
                case DiagramState.Select:
                    break;
                case DiagramState.Add:
                    var item = (ModelElement)Activator.CreateInstance(DiagramTypeToAdd);
                    item.Location = e.GetPosition(PART_DIAGRAM);
                    DataSource.Add(item as ModelElement);
                    if (DiagramTypeToAdd == typeof(Entity))
                    {
                        var i = ((Entity)item);
                        i.Name = "Entidade_" + (DataSource.Where(x => x is Entity).Count() + 1);
                        i.Attributes.Add(new DataModel.Database.TableAttribute()
                        {
                            Name = "Id",
                            PrimaryKey = true,
                            Type = DataModel.Database.DefaultTypes.INT,
                            NotNull = true,
                            Length = "11"
                        });
                    }
                    else if (DiagramTypeToAdd == typeof(Relationship))
                    {
                        var i = ((Relationship)item);
                        i.Name = "Relacionamento_" + (DataSource.Where(x => x is Relationship).Count() + 1);
                    }
                    else
                    {
                        var s = (Specialization)item;

                        Entity en = new Entity();
                        en.Location = e.GetPosition(PART_DIAGRAM);
                        en.Name = "Entidade_" + (DataSource.Where(x => x is Entity).Count() + 1);
                        en.Attributes.Add(new DataModel.Database.TableAttribute()
                        {
                            Name = "Id",
                            PrimaryKey = true,
                            Type = DataModel.Database.DefaultTypes.INT,
                            NotNull = true,
                            Length = "11"
                        });

                        s.Base = en;
                        s.Location = new Point(((en.Location.X + (en.Size.Width/2))) - s.Size.Width/2, 
                            en.Location.Y + en.Size.Height + 40);

                        DataSource.Add(en);
                    }
                    
                    break;
                case DiagramState.Connect:
                    LineConnector.X1 = StartPoint.X;
                    LineConnector.Y1 = StartPoint.Y;
                    PART_OVERLAY_CANVAS.Children.Add(LineConnector);
                    break;
                case DiagramState.Disconnect:
                    LineConnector.X1 = StartPoint.X;
                    LineConnector.Y1 = StartPoint.Y;
                    PART_OVERLAY_CANVAS.Children.Add(LineConnector);
                    break;
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            Point EndPosition = Mouse.GetPosition(PART_DIAGRAM);

            switch (CurrentDiagramState)
            {
                case DiagramState.Connect:
                    LineConnector.X2 = EndPosition.X;
                    LineConnector.Y2 = EndPosition.Y;
                    break;
                case DiagramState.Disconnect:
                    LineConnector.X2 = EndPosition.X;
                    LineConnector.Y2 = EndPosition.Y;
                    break;
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            Point EndPoint = Mouse.GetPosition(PART_DIAGRAM);

            var o = GetIntersection(StartPoint);
            var d = GetIntersection(EndPoint);

            try
            {
                PART_OVERLAY_CANVAS.Children.Remove(LineConnector);
            }
            catch { }

            switch (CurrentDiagramState)
            {
                case DiagramState.Connect:
                    if(o != null && d != null)
                    {
                        //Entity <-> Relationship
                        { 
                            var r = o is Relationship ? o : d;
                            var s = o is Relationship ? d : o;

                            if (r is Relationship && s is Entity)
                            {
                                var m = ((Relationship)r).Connections.Where(p => p.Target == s);

                                if (m.Count() == 0)
                                    ((Relationship)r).Connections.Add(new Connection() { Target = (Entity)s });
                            }
                        }
                        //Entity <-> Specialization

                        var a1 = o is Entity ? o : d;
                        var b1 = d is Specialization ? d : o;

                        if (a1 is Entity && b1 is Specialization)
                        {
                            var r1 = ((Specialization)b1).Child.FirstOrDefault(x => x == a1);

                            if (r1 == null)
                                ((Specialization)b1).Child.Add((Entity)a1);
                        }

                        //if (DiagramTypeToAdd == typeof(Specialization))
                        //{
                        //    if(o is Entity && d is Entity)
                        //    {
                        //        Entity a = (Entity)o;
                        //        Entity b = (Entity)d;

                        //        Specialization s;

                        //        var r1 = DataSource.FirstOrDefault(x => x is Specialization && ((Specialization)x).Base == a);
                        //        var r2 = DataSource.FirstOrDefault(x => x is Specialization && ((Specialization)x).Base == b);

                        //        if (r1 != null)
                        //        {
                        //            ((Specialization)r1).Child.Add(b);
                        //        }
                        //        else if (r2 != null)
                        //        {
                        //            ((Specialization)r2).Child.Add(a);
                        //        }
                        //        else
                        //        {

                        //            s = new Specialization();
                        //            s.Location = new Point((a.Location.X + b.Location.X) / 2, (a.Location.Y + b.Location.Y) / 2);
                        //            s.Base = a;
                        //            s.Child.Add(b);

                        //            DataSource.Add(s);
                        //        }
                        //    }
                        //}

                    }
                    break;
                case DiagramState.Disconnect:
                    if (o != null && d != null)
                    {
                        var k = o is Relationship ? o : d;
                        var s = o is Relationship ? d : o;

                        if (k is Relationship && s is Entity)
                        {
                            var m = ((Relationship)k).Connections.Where(p => p.Target == s);

                            if (m.Count() != 0)
                                ((Relationship)k).Connections.Remove(m.FirstOrDefault());
                        }

                        var a1 = o is Entity ? o : d;
                        var b1 = d is Specialization ? d : o;

                        if (a1 is Entity && b1 is Specialization)
                        {
                            if(((Specialization)b1).Child.Any(x=> x == a1))
                                ((Specialization)b1).Child.Remove((Entity)a1);
                        }
                    }
                    break;
                case DiagramState.Remove:
                    var item = PART_DIAGRAM.SelectedItem as ModelElement;
                    RemoveElement(item);
                    break;
            }
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as DiagramViewItem).DataContext;

            if (item is Entity)
            {
                ClearSelection();
                AttributeEditor editor = new ER.AttributeEditor();
                editor.DataContext = (item as Entity).Attributes;
                editor.ShowDialog();
            }
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = PART_DIAGRAM.SelectedItem;
            if(PART_DIAGRAM.SelectedItem != null)
            {
                CanCopy = true;
                CanCut = true;
            }
            else
            {
                CanCopy = false;
                CanCut = false;
            }
        }

        private void ClearSelection()
        {
            PART_DIAGRAM.SelectedItems.Clear();
        }

        private void RemoveElement(ModelElement element)
        {
            if (element is Entity)
            {
                foreach (Relationship r in DataSource.Where(x => x is Relationship))
                {
                    var c = r.Connections.Where(x => x.Target == element).FirstOrDefault();
                    if (c != null)
                        r.Connections.Remove(c);
                }
                foreach (Specialization r in DataSource.Where(x => x is Specialization))
                {
                    var c = r.Child.Where(x => x == element).FirstOrDefault();
                    if (c != null)
                        r.Child.Remove(c);
                }
            }
            
            DataSource.Remove(element);
        }

        private ModelElement GetIntersection(Point mousePosition)
        {
            foreach (ModelElement i in DataSource)
            {
                if (mousePosition.X >= i.Location.X && mousePosition.X <= (i.Size.Width + i.Location.X) &&
                    mousePosition.Y >= i.Location.Y && mousePosition.Y <= (i.Size.Height + i.Location.Y))
                    return i;
            }
            return null;
        }

        private void SetCursor(Cursor cursor)
        {
            Cursor = cursor;
        }

        #endregion

        #region DependencyProperties
        

        private IList _menuControls = new ObservableCollection<FrameworkElement>();

        public override IList MenuControls
        {
            get
            {
                return _menuControls;
            }
            set
            {
                _menuControls = value;
            }
        }

        public static readonly DependencyProperty ShowGridProperty =
            DependencyProperty.Register("ShowGrid", typeof(bool),
            typeof(ERDiagram), new FrameworkPropertyMetadata(true));

        public bool ShowGrid
        {
            get { return (bool)GetValue(ShowGridProperty); }
            set { SetValue(ShowGridProperty, value); }
        }


        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object),
            typeof(ERDiagram), new FrameworkPropertyMetadata(null));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }


        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(ModelCollection),
            typeof(ERDiagram), new FrameworkPropertyMetadata(new ModelCollection(),
                new PropertyChangedCallback(OnDataSourceChanged)));

        /// <summary>
        /// Efetua a conversão da estrutura padrão de dados
        /// para uma estrutura própria do controle
        /// </summary>
        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ERDiagram diagram =  ((ERDiagram)d);
            diagram.CanSave = true;
        }        


        public ModelCollection DataSource
        {
            get { return (ModelCollection)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty CanvasSizeProperty =
            DependencyProperty.Register("CanvasSize", typeof(Size),
            typeof(ERDiagram), new FrameworkPropertyMetadata(new Size(2339, 1654), null));//3508,2480

        public Size CanvasSize
        {
            get { return (Size)GetValue(CanvasSizeProperty); }
            set { SetValue(CanvasSizeProperty, value); }
        }

        public override void OnZoomChanged(double oldValue, double newValue)
        {
            ScaleTransform scaleTransform = new ScaleTransform(newValue, newValue);
            PART_DIAGRAM.LayoutTransform = scaleTransform;
            PART_OVERLAY_CANVAS.LayoutTransform = scaleTransform;
        }

        #endregion

        #region Base Override

        ModelSource _modelConverter;
        public override IModelSource Model
        {
            get
            {
                return _modelConverter ?? (_modelConverter = new ModelSource(
                    () => { return DataSource; },
                    (x,y) => { DataSource = x; CanvasSize = y; }
                ));
            }
        }

        public override FileType[] FileExtension
        {
            get
            {
                return new FileType[] { new FileType
                (
                    ".erd", 
                    "Diagrama Entidade-Relacionamento",
                    BitmapFrame.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("ER.Icons.ERIcon.png"))
                ) };
            }
        }

        public override void Save()
        {
            if(FilePath != null)
            {
                BinaryFormatter serializer = new BinaryFormatter();
                using (Stream writer = new FileStream(FilePath, FileMode.Open))
                {
                    serializer.Serialize(writer, DataSource);
                }
            
                CanSave = false;
            }
        }

        public override void Save(string path)
        {
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                using (Stream writer = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(writer, DataSource);
                }
            }
            catch { }
        }

        public override void CreateFile(string path)
        {
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                using (Stream writer = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(writer, new ModelCollection());
                }
            }
            catch { }
        }
        private string FilePath;
        public override void Open(string path)
        {
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                using (Stream writer = new FileStream(path, FileMode.Open))
                {
                    DataSource = (ModelCollection)serializer.Deserialize(writer);
                }

                // tipos criados dinamicamente devem ser convertidos novamente
                // para os tipos estáticos do núcleo
                var types = DataModel.Database.DefaultTypes.GetDefaultTypes();
                foreach(Entity item in DataSource.Where(x=> x is Entity))
                {
                    foreach(var attr in item.Attributes)
                    {
                        attr.Type = types.First(x => x.Name == attr.Type.Name);
                    }
                }
            }
            catch { }

            FilePath = path;
        }

        public async override void Print()
        {
            PrintDialog pd = new PrintDialog();
            

            bool? result = pd.ShowDialog();

            if (!result.HasValue || !result.Value) return;

            pd.PrintTicket.OutputQuality = System.Printing.OutputQuality.High;
            pd.PrintTicket.PageBorderless = System.Printing.PageBorderless.Borderless;
            pd.PrintTicket.PageMediaSize = new System.Printing.PageMediaSize(System.Printing.PageMediaSizeName.ISOA4);
            pd.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
            pd.PrintTicket.PageResolution = new System.Printing.PageResolution(200, 200);


            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    pd.PrintVisual(PART_DIAGRAM, "ER DIAGRAM");
                });                
            });
        }

        public override void OnApplyTheme(Theme theme)
        {
            foreach(var item in DataSource)
            {
                item.Theme = theme;
            }
        }

        #endregion

        #region Export
        public override void Export()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "PNG (*.png) | *.png | JPG (*.jpg)| *.jpg |Bitmap (*.bmp)| *.bmp| Documento XPS (*.xps)|*.xps";
            if (dlg.ShowDialog() == true)
            {
                switch (dlg.FilterIndex)
                {
                    case 1:
                        RenderPng(dlg.FileName);
                        break;
                    case 2:
                        RenderJpg(dlg.FileName);
                        break;
                    case 3:
                        RenderBmp(dlg.FileName);
                        break;
                    case 4:
                        ExportXPS(dlg.FileName);
                        break;
                        
                }
            }
        }

        private void RenderPng(string filePath)
        {
            ExportBitmap(filePath, new PngBitmapEncoder());
        }

        private void RenderJpg(string filePath)
        {
            ExportBitmap(filePath, new JpegBitmapEncoder());
        }

        private void RenderBmp(string filePath)
        {
            ExportBitmap(filePath, new BmpBitmapEncoder());
        }

        private void ExportBitmap(string filePath, BitmapEncoder encoder)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)PART_DIAGRAM.RenderSize.Width,
            (int)PART_DIAGRAM.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(PART_DIAGRAM);

            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var fs = System.IO.File.OpenWrite(filePath))
            {
                encoder.Save(fs);
            }
        }

        private BitmapFrame CreateUIBitmap(UIElement e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)e.RenderSize.Width,
            (int)e.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(e);

            return BitmapFrame.Create(rtb);
        }

        private void ExportXPS(string filePath)
        {
            if (filePath == null) return;

            Transform transform = PART_DIAGRAM.LayoutTransform;
            PART_DIAGRAM.LayoutTransform = null;

            Size size = new Size(PART_DIAGRAM.Width, PART_DIAGRAM.Height);

            PART_DIAGRAM.Measure(size);
            PART_DIAGRAM.Arrange(new Rect(size));

            Package package = Package.Open(filePath, FileMode.Create);
            XpsDocument doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            writer.Write(PART_DIAGRAM);
            doc.Close();
            package.Close();


            PART_DIAGRAM.LayoutTransform = transform;        
        }

        #endregion

        #region Undo/Redo

        StateManager<ModelCollection> stateManager = new StateManager<ModelCollection>();
        
       

        public override void Undo()
        {
            if (stateManager.CanUndo)
            {
                var u = stateManager.Undo();
                DataSource = u;
            }
        }

        public override void Redo()
        {
            if (CanRedo)
            {
                DataSource = stateManager.Redo();
            }
        }

        #endregion

        #region Copy/Cut/Paste

        private ICloneable ClipboardObject { get; set; }

        public override void Copy()
        {
            if(CanCopy)
            {
                if(PART_DIAGRAM.SelectedItem != null)
                {
                    var item = PART_DIAGRAM.SelectedItem;
                    ClipboardObject = (ICloneable)item;
                    CanPaste = true;
                }
            }
        }

        public override void Cut()
        {
            if (CanCut)
            {
                var item = PART_DIAGRAM.SelectedItem;
                if (item != null)
                {
                    ClipboardObject = (ICloneable)item;
                    RemoveElement(item as ModelElement);
                    CanPaste = true;
                }
            }
        }

        public override void Paste()
        {
            if (CanPaste)
            {
                if(ClipboardObject != null)
                {
                    var item = ClipboardObject.Clone() as ModelElement;
                    var i = item as ModelElement;
                    i.Location = new Point(i.Location.X + 50, i.Location.Y + 50);
                    DataSource.Add(i as ModelElement);
                }
            }
        }

        private void CopyImageToClipboard(UIElement e)
        {
            try
            {
                var bmp = CreateUIBitmap(e);
                Clipboard.SetImage(bmp);

            }catch { }
        }

        #endregion
    }

    internal static class DiagramCursors
    {
        static DiagramCursors()
        {
            Arrow = LoadCursor("/ER;component/Cursors/Arrow.cur");
            Cross = LoadCursor("/ER;component/Cursors/Cross.cur");
            RedCross = LoadCursor("/ER;component/Cursors/RedCross.cur");
            Add = LoadCursor("/ER;component/Cursors/Add.cur");
            Remove = LoadCursor("/ER;component/Cursors/Remove.cur");
        }

        private static Cursor LoadCursor(string resource)
        {
            Cursor c = null;
            try
            {
                c = new Cursor(Application.GetResourceStream(new Uri(resource, UriKind.Relative)).Stream);
            }
            catch { }

            return c;
        }

        public static Cursor Arrow { get; set; }
        public static Cursor Cross { get; set; }
        public static Cursor RedCross { get; set; }
        public static Cursor Add { get; set; }
        public static Cursor Remove { get; set; }
    }

    public enum DiagramState
    {
        Select,
        Add,
        Remove,
        Connect,
        Disconnect
    }

    internal enum CursorIcon
    {
        Arrow,
        Add,
        Delete,
        Cross
    }

    internal class ERTemplateSelector: DataTemplateSelector
    {
        public DataTemplate EntityTemplate { get; set; }
        public DataTemplate RelationshipTemplate { get; set; }

        public DataTemplate SpecializationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Entity)
                return EntityTemplate;
            else if (item is Relationship)
                return RelationshipTemplate;
            else
                return SpecializationTemplate;
        }
    }

    internal class ERContainerStyleSelector : StyleSelector
    {
        public Style EntityStyle { get; set; }
        public Style RelationshipStyle { get; set; }
        public Style SpecializationStyle { get; set; }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is Entity)
                return EntityStyle;
            else if (item is Relationship)
                return RelationshipStyle;
            else
                return SpecializationStyle;
        }
    }
}
