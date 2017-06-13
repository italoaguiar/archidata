using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Database
{
    /// <summary>
    /// Representa o Incremento de um atributo de banco de dados
    /// </summary>
    [Serializable]
    public class AutoIncrement
    {
        int initialValue = 1;
        int increment = 1;

        /// <summary>
        /// Obtém ou define usar Auto-Incremento
        /// </summary>
        public bool UseAutoIncrement { get; set; }
        /// <summary>
        /// Obtém ou define o valor inicial do incremento
        /// </summary>
        public int InitialValue
        {
            get { return initialValue; }
            set { initialValue = value; }
        }
        /// <summary>
        /// Obtém ou define o valor incremental
        /// </summary>
        /// <example>Increment = 1 -> {1,2,3,4,5,6}</example>
        /// <example>Increment = 2 -> {2,4,6,8,10,12}</example>
        public int Increment
        {
            get { return increment; }
            set { increment = value; }
        }
    }
}
