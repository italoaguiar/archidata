using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataModel.Database.Validation
{
    /// <summary>
    /// Define uma classe para validação de tamanhos decimais
    /// </summary>
    [Serializable]
    public class DecimalLengthType : ITypeLengthValidator
    {
        /// <summary>
        /// Verifica se um tamanho é válido
        /// </summary>
        /// <param name="value">Representação do tamanho do campo</param>
        /// <returns></returns>
        public bool IsValid(string value)
        {
            Regex r = new Regex(@"(^\d+,\d+$)");
            return r.IsMatch(value);
        }
    }
}
