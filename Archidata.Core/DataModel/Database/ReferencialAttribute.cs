using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Database
{
    [Serializable]
    public class ReferencialAttribute: TableAttribute
    {
        public TableAttribute ReferencedAttribute { get; set; }
        
    }
}
