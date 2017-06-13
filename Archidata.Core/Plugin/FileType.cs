using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Archidata.Core.Plugin
{
    /// <summary>
    /// Define um tipo de arquivo suportado pelo plugin
    /// </summary>
    public class FileType
    {
        /// <summary>
        /// Cria uma nova intância de FileType
        /// </summary>
        public FileType()
        {

        }

        /// <summary>
        /// Cria uma nova instância de Filetype
        /// </summary>
        /// <param name="Extension">Define a extensão do arquivo</param>
        /// <param name="Icon">Define o ícone do arquivo</param>
        public FileType(string Extension, ImageSource Icon)
        {
            this.Extension = Extension;
            this.Icon = Icon;
        }

        /// <summary>
        /// Cria uma nova instância de Filetype
        /// </summary>
        /// <param name="Extension">Define a extensão do arquivo</param>
        /// <param name="Icon">Define o ícone do arquivo</param>
        /// <param name="Description">Define a descrição do arquivo</param>
        public FileType(string Extension, string Description, ImageSource Icon)
        {
            this.Extension = Extension;
            this.Icon = Icon;
            this.Description = Description;
        }

        /// <summary>
        /// Obtém ou define o icone do arquivo
        /// </summary>
        public ImageSource Icon { get; set; }

        private string _extension;
        /// <summary>
        /// Obtém ou define a extensão do arquivo
        /// </summary>
        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = ValidadeExtension(value);
            }
        }
        
        /// <summary>
        /// Obtém ou define a descrição do tipo de arquivo
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        private string ValidadeExtension(string extension)
        {
            if (extension.Where(p => p == '.').Count() == 1 && extension.StartsWith("."))
                return extension.Trim();
            else throw new ArgumentException();
        }
    }
}
