using System;
using System.Threading.Tasks;

namespace Archidata.Core.Plugin
{
    /// <summary>
    /// Representa um pacote de comandos do aplicativo
    /// </summary>
    public class App
    {
        private Action<string> OpenFile;
        private Func<string, string> SaveFile;

        /// <summary>
        /// Cria uma nova instância de App
        /// </summary>
        /// <param name="save">Comando de salvamento de arquivo de projeto</param>
        /// <param name="open">Comando de abertura de arquivo de projeto</param>
        public App(Func<string,string> save, Action<string> open)
        {
            this.SaveFile = save;
            this.OpenFile = open;
        }
        /// <summary>
        /// Abre um arquivo de projeto
        /// </summary>
        /// <param name="filePath">Caminho para o arquivo</param>
        public async void Open(string filePath)
        {
            await Task.Delay(1000);

            OpenFile?.Invoke(filePath);
        }
        /// <summary>
        /// Salva um arquivo de projeto
        /// </summary>
        /// <param name="fileExtension">Extensão do arquivo</param>
        /// <returns>Caminho do arquivo salvo</returns>
        public string Save(string fileExtension)
        {
            return SaveFile?.Invoke(fileExtension);
        }

        
    }
}
