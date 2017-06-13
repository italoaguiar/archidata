using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Activities.Presentation.PropertyEditing;
using System.IO;
using System.Windows.Markup;
using Archidata.Core.Helpers;

namespace ER
{
    /// <summary>
    /// Interaction logic for AttributeEditor.xaml
    /// </summary>
    public partial class AttributeEditor : Window
    {
        public AttributeEditor()
        {
            InitializeComponent();

            addCommand = new CommandAdapter((a) =>
            {
                if(this.DataContext != null && DataContext is ObservableCollection<DataModel.Database.TableAttribute>)
                {
                    ((ObservableCollection<DataModel.Database.TableAttribute>)DataContext).Add(new DataModel.Database.TableAttribute());
                }
            });
            addCommand.CanExecuteAction = true;

            removeCommand = new CommandAdapter((a) =>
            {
                if (this.DataContext != null && DataContext is ObservableCollection<DataModel.Database.TableAttribute>)
                {
                    ((ObservableCollection<DataModel.Database.TableAttribute>)DataContext).Remove((DataModel.Database.TableAttribute)a);
                }
            });
            removeCommand.CanExecuteAction = true;

            Types = DataModel.Database.DefaultTypes.GetDefaultTypes().ToList();
            
        }



        CommandAdapter addCommand;
        public ICommand AddCommand
        {
            get { return addCommand; }
        }
        CommandAdapter removeCommand;
        public ICommand RemoveCommand
        {
            get { return removeCommand; }
        }

        public List<DataModel.Database.DBType> Types { get; set; }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var attr = (DataModel.Database.TableAttribute)(sender as ComboBox).Tag;
            var len = (DataModel.Database.DBType)(sender as ComboBox).SelectedItem;

            if(attr.Type == null || attr.Type != len)
            {
                attr.Type = len;
                attr.Length = len.DefaultLength;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }

    public class AttributeEditorDialog: DialogPropertyValueEditor
    {

        public AttributeEditorDialog()
        {
            string template = @"
                <DataTemplate
                    xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                    xmlns:pe='clr-namespace:System.Activities.Presentation.PropertyEditing;assembly=System.Activities.Presentation'>
                    <pe:EditModeSwitchButton TargetEditMode='Dialog' Name='EditButton' Padding='2' Width='100' HorizontalContentAlignment='Center' MaxWidth='400'  
                       HorizontalAlignment='Stretch' >Editar</pe:EditModeSwitchButton>
                </DataTemplate>";

            using (var sr = new MemoryStream(Encoding.UTF8.GetBytes(template)))
            {
                this.InlineEditorTemplate = XamlReader.Load(sr) as DataTemplate;
            }
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            var item = propertyValue.Value as ObservableCollection<DataModel.Database.TableAttribute>;
            AttributeEditor e = new AttributeEditor();
            e.DataContext = item;

            e.ShowDialog();
        }
    }
}
