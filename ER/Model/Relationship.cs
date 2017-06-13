using DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace ER.Model
{
    [Serializable]
    public class Relationship: ModelElement
    {
        private ObservableCollection<Connection> _Connections;

        public Relationship()
        {
            _Connections = new ObservableCollection<Connection>();
            _Connections.CollectionChanged += (s, a) => { StateService.NotifyChange(); };
        }

        public ObservableCollection<Connection> Connections
        {
            get { return _Connections; }
            set
            {
                _Connections = value;
                OnPropertyChanged("Connections");
            }
        }
    }
}
