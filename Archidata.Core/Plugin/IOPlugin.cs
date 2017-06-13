using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Archidata.Core.Plugin
{
    /// <summary>
    /// Representa um plugin de entrada e saída de dados
    /// </summary>
    public class IOPlugin : ContentControl, IIOPlugin
    {
        /// <summary>
        /// Obtém ou define o pacote de comandos do aplicativo
        /// </summary>
        public App App { get; set; }

        /// <summary>
        /// Inicializa o pacote de comandos do aplicativo
        /// </summary>
        /// <param name="app">Pacote de comandos do aplicativo</param>
        public void Initialize(App app)
        {
            App = app;
        }

        /// <summary>
        /// Obtém ou define a fonte de dados do plugin
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(IModelSource),
            typeof(IOPlugin), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Obtém ou define a fonte de dados do plugin
        /// </summary>
        public virtual IModelSource Model
        {
            get { return (IModelSource)GetValue(ModelProperty); }
            set
            {
                SetValue(ModelProperty, value);
                OnPropertyChanged("Model");
            }
        }

        /// <summary>
        /// Informa quando uma propriedade é alterada
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Informa quando uma propriedade é alterada
        /// </summary>
        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
