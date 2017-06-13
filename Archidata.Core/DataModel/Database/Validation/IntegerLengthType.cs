using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Database.Validation
{
    /// <summary>
    /// Define uma classe para validação de tamanhos numéricos
    /// </summary>
    [Serializable]
    public class IntegerLengthType : ITypeLengthValidator
    {
        public bool AllowEmptySize { get; set; }
        /// <summary>
        /// Verifica se um tamanho é válido
        /// </summary>
        /// <param name="value">Representação do tamanho do campo</param>
        /// <returns></returns>
        public bool IsValid(string value)
        {
            if (AllowEmptySize && string.IsNullOrEmpty(value)) return true;

            uint r;
            return uint.TryParse(value, out r);
        }
    }
}
