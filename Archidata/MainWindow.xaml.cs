using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Archidata.Core.Helpers;
using Archidata.Core.Plugin;
using System.Linq;
using System.Windows.Controls;

namespace Archidata
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            Initialize();
            this.Loaded += new RoutedEventHandler(win_Loaded);
            this.SourceInitialized += new EventHandler(win_SourceInitialized);

            DataContext = this;

            UpdateRecentProjects();
        }

        private string OnSaveFile(string arg)
        {
            Random rnd = new Random();
            return ProjectManager.SaveProjectFile(CurrentProject.ProjectName, "diagrama_" + rnd.Next(1000,9999), arg);
        }

        private void OnOpenFile(string arg)
        {
            foreach(var proj in CurrentProject.Files)
            {
                if (proj.FilePath == arg)
                    OpenProjectFile(proj);
            }
        }

        public Archidata.ViewModel.ProjectViewModel ViewModel { get; set; }


        ObservableCollection<string> _recentProjects = new ObservableCollection<string>();
        public ObservableCollection<string> RecentProjects
        {
            get {
                return _recentProjects;
            }
            set { _recentProjects = value; }
        }

        private Project _currentProject;
        public Project CurrentProject
        {
            get { return _currentProject; }
            set
            {
                _currentProject = value;
                ViewModel.CloseAll();
                OnPropertyChanged("CurrentProject");
            }
        }

        void win_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

        private void Initialize()
        {
            Archidata.Core.Plugin.App app = new Archidata.Core.Plugin.App(OnSaveFile, OnOpenFile);
            ViewModel = new Archidata.ViewModel.ProjectViewModel();
            ViewModel.Items = new ObservableCollection<Archidata.ViewModel.ItemViewModel>();

            //inicializa as extensões com o valor do plugin
            //de designer selecionado
            foreach (var item in App.Extensions.DBExtensions)
            {
                item.Initialize(app);

                Binding b = new Binding("ViewModel.SelectedItem.Container.Model") { Source = this };
                b.Mode = BindingMode.OneWay;
                ((IOPlugin)item).SetBinding(IOPlugin.ModelProperty, b);
            }

            _commands = new CommandMap();
            _commands.AddCommand("Close", x => { this.Close(); });
            _commands.AddCommand("Minimize", x => { this.WindowState = WindowState.Minimized; });
            _commands.AddCommand("Restore", x => 
            {
                if(this.WindowState == WindowState.Normal)
                    this.WindowState = WindowState.Maximized;
                else this.WindowState = WindowState.Normal;
            });
            _commands.AddCommand("PageSettings", x => { new Popup(new Dialogs.PageSetup()).ShowDialog(); });
            //_commands.AddCommand("Print", x => { p1.Print(); });

            _commands.AddCommand("Test", x => {
                ViewModel.Items.Add(new Archidata.ViewModel.ItemViewModel());
            });
            _commands.AddCommand("OpenProjectFile", OpenProjectFile);

            NewProjectCommand = new CommandAdapter((p) => NewProject()) { CanExecuteAction = true };
            NewFileCommand = new CommandAdapter((p) => NewFile());
            OpenProjectCommand = new CommandAdapter((p) => OpenProject()) { CanExecuteAction = true};
            ConfigurePaperCommand = new CommandAdapter((p) => ConfigurePaper());

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;



            
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((CommandAdapter)ConfigurePaperCommand).CanExecuteAction = false;            

            if (ViewModel.SelectedItem != null && ViewModel.SelectedItem.Container != null)
            {
                var p = ViewModel.SelectedItem.Container.GetType().GetProperty("CanvasSize");
                ((CommandAdapter)ConfigurePaperCommand).CanExecuteAction = p != null;
            }
        }

        public ICommand NewProjectCommand { get; set; }
        public ICommand NewFileCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }
        public ICommand ConfigurePaperCommand { get; set; }

        private void NewProject()
        {
            

            NewProjectDialog d = new NewProjectDialog();
            if(d.ShowDialog() == true)
            {
                CurrentProject = d.Project;
                ((CommandAdapter)NewFileCommand).CanExecuteAction = true;

                UpdateRecentProjects();

                NewFile();
            }
        }

        private void OpenProject()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = ProjectManager.DefaultFolder;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                CurrentProject = ProjectManager.LoadProject(dialog.SelectedPath);
                ((CommandAdapter)NewFileCommand).CanExecuteAction = true;
            }
            
        }
        private async void NewFile()
        {
            NewFileDialog d = new NewFileDialog();
            d.Types = App.Extensions.GetSupportedLoadedFiles();
            if(d.ShowDialog() == true)
            {
                var c = App.Extensions.GetControlInstanceByExtension(d.SelectedType.Extension);
                var path = Path.Combine(CurrentProject.ProjectPath, d.FileName + d.SelectedType.Extension);
                c.Save(path);

                await Task.Delay(100);

                OnOpenFile(path);
                
            }
        }

        private void ConfigurePaper()
        {
            CanvasSetup s = new CanvasSetup();
            Binding b = new Binding("CanvasSize");
            b.Source = ViewModel.SelectedItem.Container;
            b.Mode = BindingMode.TwoWay;
            s.SetBinding(CanvasSetup.CanvasSizeProperty, b);

            s.ShowDialog();
        }

        private void OpenProjectFile(object obj)
        {
            for (int i = 0; i < ViewModel.Items.Count; i++)
            {
                if (ViewModel.Items[i].File.FileName == (obj as ProjectFile).FileName)
                {
                    ViewModel.SelectedItem = ViewModel.Items[i];
                    return;
                }
            }
            var newItem = new Archidata.ViewModel.ItemViewModel() { File = obj as ProjectFile };
            if (newItem.Container != null)
            {
                ViewModel.Items.Add(newItem);
                ViewModel.SelectedItem = ViewModel.Items[ViewModel.Items.Count - 1];
            }
            else
            {
                MessageBox.Show("Não foi encontrado um componente capaz de abrir este arquivo", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            foreach(var item in ViewModel.Items.ToList())
            {
                item.Close();
            }

            if (ViewModel.Items.Count > 0) e.Cancel = true;

            base.OnClosing(e);
        }

        private void UpdateRecentProjects()
        {
            if (RecentProjects == null)
                RecentProjects = new ObservableCollection<string>();

            RecentProjects.Clear();
            var items = RecentProjectsManager.GetRecentProjects();
            foreach (var i in items)
                RecentProjects.Add(i);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void OnRecentProjectsChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListBox).SelectedItem;

            if (item != null)
            {
                CurrentProject = ProjectManager.LoadProject(item.ToString());
                ((CommandAdapter)NewFileCommand).CanExecuteAction = true;
            }

            (sender as ListBox).SelectedItem = null;
        }

        /// <summary>
        /// Get the list of commands
        /// </summary>
        public CommandMap Commands
        {
            get { return _commands; }
        }
        private CommandMap _commands;


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;




        #region Win32 API

        private static System.IntPtr WindowProc(
              System.IntPtr hwnd,
              int msg,
              System.IntPtr wParam,
              System.IntPtr lParam,
              ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:/* WM_GETMINMAXINFO */
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (System.IntPtr)0;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        void win_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }


        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }


        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// 
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion

    }
}
