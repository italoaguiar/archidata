using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Database.Validation
{
    /// <summary>
    /// Define uma interface para validação do tamanho de tipos de dados
    /// </summary>
    public interface ITypeLengthValidator
    {
        /// <summary>
        /// Verifica se um tamanho de tipo é válido
        /// </summary>
        /// <param name="value">Representação do tipo</param>
        /// <returns>True se válido, False se inválido</returns>
        bool IsValid(string value);
    }
}
