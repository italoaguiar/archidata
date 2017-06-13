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
    /// Interaction logic for NewProject.xaml
    /// </summary>
    public partial class NewProjectDialog : Window
    {
        public NewProjectDialog()
        {
            InitializeComponent();

            proj_name.Focus();
        }

        private void proj_name_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            okBtn.IsEnabled = ProjectManager.CanCreateProject(proj_name.Text);
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Project = ProjectManager.NewProject(proj_name.Text);

            if(Project == null)
            {
                MessageBox.Show("Não foi possível criar um novo projeto. Certifique-se que o programa tem permissão para acessar o diretório de documentos.");
            }
            else
            {
                DialogResult = true;
            }
        }

        public Project Project { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
