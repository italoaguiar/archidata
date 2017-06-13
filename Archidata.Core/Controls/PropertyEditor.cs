using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Archidata.Core.Controls
{
    /// <summary>
    /// Representa um controle editor de propriedades
    /// </summary>
    public class PropertyEditor : Control
    {

        /// <summary>
        /// Obtém ou define o objeto a ser editado
        /// </summary>
        public static readonly DependencyProperty PropertySourceProperty =
            DependencyProperty.Register("PropertySource", typeof(object),
            typeof(PropertyEditor), new FrameworkPropertyMetadata(null,
                new PropertyChangedCallback(OnPropertySourceChanged)));

        private static void OnPropertySourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtém ou define o objeto a ser editado
        /// </summary>
        public object PropertySource
        {
            get;
            set;
        }

        internal ObservableCollection<PropertyModel> Properties { get; set; }

        private void GetProperties(object context)
        {
            Properties.Clear();

            var p = context.GetType().GetRuntimeProperties();

            foreach(PropertyInfo property in p)
            {
                EditorAttribute editor = property.GetCustomAttribute<EditorAttribute>();
                DisplayNameAttribute name = property.GetCustomAttribute<DisplayNameAttribute>();
                BrowsableAttribute browsable = property.GetCustomAttribute<BrowsableAttribute>();

                if (browsable != null)
                    if (!browsable.Browsable)
                        continue;

                PropertyModel m = new PropertyModel();
                m.PropertyName = name == null ? property.Name : name.DisplayName;

                if(editor != null)
                {
                    
                }

            }

        }
        
    }

    internal class PropertyModel
    {
        public string PropertyName { get; set; }
        public UIElement Editor { get; set; }
    }
}
