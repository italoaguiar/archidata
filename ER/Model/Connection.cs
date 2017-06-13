using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ER.Model
{
    [Serializable]
    public class Connection : INotifyPropertyChanged
    {
        private Cardinality _cardinality;
        private Entity _target;

        /// <summary>
        /// Obtém ou define a cardinalidade de um relacionamento
        /// </summary>
        public Cardinality Cardinality
        {
            get { return _cardinality; }
            set
            {
                _cardinality = value;
                OnPropertyChanged("Cardinality");
            }
        }
        /// <summary>
        /// Obtém ou define a entidade alvo do relacionamento
        /// </summary>
        public Entity Target
        {
            get { return _target; }
            set
            {
                _target = value;
                OnPropertyChanged("Target");
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Notifica a alteração de valor de uma propriedade
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
