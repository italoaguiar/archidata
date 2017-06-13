using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Archidata.Core.Plugin
{
    /// <summary>
    /// Representa um plugin de entrada e saída de dados
    /// </summary>
    public interface IIOPlugin: INotifyPropertyChanged
    {
        /// <summary>
        /// Inicializa o plugin
        /// </summary>
        /// <param name="app">Pacote de comandos do aplicativo</param>
        void Initialize(App app);
        /// <summary>
        /// Obtém ou define a fonte de dados do plugin
        /// </summary>
        IModelSource Model { get; set; }
    }
}
