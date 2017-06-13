using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ER.Model
{
    [Serializable]
    public class ModelCollection : ObservableCollection<ModelElement>, ICloneable
    {
        public virtual object Clone()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            stream.Seek(0, SeekOrigin.Begin);
            ModelCollection result = (ModelCollection)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }
    }
}
