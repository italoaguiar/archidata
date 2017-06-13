using Archidata.Core.Plugin;
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
    /// Interaction logic for NewFileDialog.xaml
    /// </summary>
    public partial class NewFileDialog : Window
    {
        public NewFileDialog()
        {
            InitializeComponent();
        }

        public IEnumerable<Archidata.Core.Plugin.FileType> Types { get; set; }

        bool typeSelected = false;
        bool validName = false;

        public FileType SelectedType { get; set; }
        public string FileName { get; set; }

        private void filetype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            typeSelected = filetype.SelectedItem != null;
            SelectedType = filetype.SelectedItem as FileType;
            CheckState();     
        }

        private void CheckState()
        {
            okBtn.IsEnabled = typeSelected && validName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void proj_name_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            validName = ProjectManager.CanCreateProject(proj_name.Text);
            FileName = proj_name.Text;
            CheckState();
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
