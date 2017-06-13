using DataModel.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archidata.Core.Plugin
{
    public interface IModelSource
    {
        
        DatabaseModel Data { get; set; }
    }
}
