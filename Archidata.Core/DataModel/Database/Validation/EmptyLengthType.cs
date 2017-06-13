using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Database.Validation
{
    /// <summary>
    /// Define uma classe para validação de tamanhos vazios
    /// </summary>
    [Serializable]
    public class EmptyLengthType : ITypeLengthValidator
    {
        /// <summary>
        /// Verifica se um tamanho é válido
        /// </summary>
        /// <param name="value">Representação do tamanho do campo</param>
        /// <returns></returns>
        public bool IsValid(string value)
        {
            return string.IsNullOrEmpty(value) || value.Length == 1;
        }
    }
}
