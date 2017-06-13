using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Database
{
    [Serializable]
    public class Table
    {
        Collections.AttributeCollection attributes;

        public Table()
        {
            Attributes = new Collections.AttributeCollection(this);
        }

        /// <summary>
        /// Obtém ou define o nome da Tabela
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Obtém ou define os atributos da entidade
        /// </summary>
        public virtual Collections.AttributeCollection Attributes
        {
            get { return attributes; }
            set
            {
                attributes = value;
            }
        }

    }
}
