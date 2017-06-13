using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ER.Model
{
    [Serializable]
    public class Specialization: ModelElement
    {
        private Entity _entity;
        private ObservableCollection<Entity> _child;

        public Specialization()
        {
            _child = new ObservableCollection<Entity>();
            _child.CollectionChanged += (s, a) => { StateService.NotifyChange(); };
            Size = new System.Windows.Size(80, 80);
        }
        public Entity Base
        {
            get { return _entity; }
            set
            {
                _entity = value;
                OnPropertyChanged("Base");
                StateService.NotifyChange();
            }
        }

        private Size _Size;
        public override Size Size
        {
            set
            {
                double w = value.Width < 80 ? 80 : value.Width;
                double h = value.Height < 80 ? 80 : value.Height;

                _Size = new Size(w, h);
                OnPropertyChanged("Size");
            }
            get { return _Size; }
        }

        public ObservableCollection<Entity> Child
        {
            get { return _child; }
            set
            {
                _child = value;
                OnPropertyChanged("Child");
                StateService.NotifyChange();
            }
        }

    }
}
