using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public interface IThemeable
    {
        /// <summary>
        /// Obtém ou define o tema visual do objeto
        /// </summary>
        object Theme { get; set; }
    }
}
