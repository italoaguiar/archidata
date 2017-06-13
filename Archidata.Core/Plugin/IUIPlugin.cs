using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Archidata.Core.Plugin
{
    /// <summary>
    /// Representa um plugin de interface gráfica
    /// </summary>
    public interface IUIPlugin
    {
        /// <summary>
        /// Salva um arquivo de projeto
        /// </summary>
        /// <param name="path"></param>
        void Save(string path);
        /// <summary>
        /// Abre um arquivo de projeto
        /// </summary>
        /// <param name="path"></param>
        void Open(string path);

        /// <summary>
        /// Cria um novo arquivo
        /// </summary>
        /// <param name="path">Caminho do arquivo</param>
        void CreateFile(string path);

        /// <summary>
        /// Obtém o comando de Desfazer
        /// </summary>
        ICommand UndoCommand { get; }
        /// <summary>
        /// Obtém o comando de Refazer
        /// </summary>
        ICommand RedoCommand { get; }
        /// <summary>
        /// Obtém o comando de Copiar
        /// </summary>
        ICommand CopyCommand { get; }
        /// <summary>
        /// Obtém o comando de Colar
        /// </summary>
        ICommand PasteCommand { get; }
        /// <summary>
        /// Obtém o comando de Recortar
        /// </summary>
        ICommand CutCommand { get; }
        /// <summary>
        /// Obtém o comando de Salvar
        /// </summary>
        ICommand SaveCommand { get; }
        /// <summary>
        /// Obtém o comando de Imprimir
        /// </summary>
        ICommand PrintCommand { get; }
        /// <summary>
        /// Obtém o comando de Exportar
        /// </summary>
        ICommand ExportCommand { get; }

        /// <summary>
        /// Obtém  o conjunto de extesões de arquivo suportadas pelo plugin
        /// </summary>
        FileType[] FileExtension { get; }
        /// <summary>
        /// Obtém ou define o valor da propriedade de Zoom
        /// </summary>
        double Zoom { get; set; }
        /// <summary>
        /// Obtém ou define o tema visual do plugin
        /// </summary>
        Theme Theme { get; set; }

        /// <summary>
        /// Obtém ou define o conjunto de controle do menu de opções do aplicativo
        /// </summary>
        IList MenuControls { get; set; }

        /// <summary>
        /// Obtém ou define um objeto para obtenção da estrutura física do banco de dados
        /// </summary>
        IModelSource Model { get; set; }

    }
}
