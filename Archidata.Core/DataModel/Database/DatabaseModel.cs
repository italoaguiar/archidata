using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Database
{
    [Serializable]
    public class DatabaseModel
    {
        List<Table> _Tables;
        public List<Table> Tables
        {
            get { return _Tables ?? (_Tables = new List<Table>()); }
            set { _Tables = value; }
        }
        public string DatabaseName { get; set; }

    }
}
