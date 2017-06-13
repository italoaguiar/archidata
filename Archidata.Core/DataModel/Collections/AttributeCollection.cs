using DataModel;
using DataModel.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Collections
{
    [Serializable]
    public class AttributeCollection : ObservableCollection<Database.TableAttribute>
    {

        public AttributeCollection(Table Parent)
        {
            this.Parent = Parent;
        }

        public Table Parent { get; set; }


        protected override void InsertItem(int index, Database.TableAttribute item)
        {
            item.Parent = this.Parent;
            base.InsertItem(index, item);
        }
    }
}
