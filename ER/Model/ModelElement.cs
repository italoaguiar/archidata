using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ER.Model
{
    [Serializable]
    public class ModelElement: INotifyPropertyChanged, ICloneable
    {
        public const double MINHEIGHT = 100;
        public const double MINWIDTH = 160;

        private Size _Size = new Size(MINWIDTH, MINHEIGHT);
        private Point _Location = new Point(0, 0);
        private string _Name;

        [NonSerialized]
        private object _Theme;


        [Browsable(false)]
        public virtual Size Size
        {
            set
            {
                double w = value.Width < MINWIDTH ? MINWIDTH : value.Width;
                double h = value.Height < MINHEIGHT ? MINHEIGHT : value.Height;

                _Size = new Size(w, h);
                OnPropertyChanged("Size");
            }
            get { return _Size; }
        }

        [Browsable(false)]
        public virtual Point Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
                OnPropertyChanged("Location");
            }
        }

        /// <summary>
        /// Obtém ou define o nome do objeto
        /// </summary>
        public virtual string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
                StateService.NotifyChange();
            }
        }

        [Browsable(false)]
        public virtual object Theme
        {
            get { return _Theme; }
            set
            {
                _Theme = value;
                OnPropertyChanged("Theme");
            }
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
