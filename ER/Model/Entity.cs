using DataModel.Database;
using System;
using System.Activities.Presentation.PropertyEditing;
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
    public class Entity: ModelElement
    {
        ObservableCollection<TableAttribute> attributes;


        public Entity()
        {
            attributes = new ObservableCollection<TableAttribute>();
            attributes.CollectionChanged += (s, a) => 
            {
                StateService.NotifyChange();

                foreach(var item in a.NewItems)
                {
                    ((TableAttribute)item).PropertyChanged += (sender, args) =>
                    {
                        StateService.NotifyChange();
                    };
                }
            };
        }

        /// <summary>
        /// Obtém ou define os atributos da entidade
        /// </summary>
        [Editor(typeof(AttributeEditorDialog),typeof(PropertyValueEditor))]
        [DisplayName("Atributos")]
        public ObservableCollection<TableAttribute> Attributes
        {
            get { return attributes; }
            set
            {
                attributes = value;
                OnPropertyChanged("Attributes");
            }
        }

        public static Entity FromTable(Table t)
        {
            Entity e = new Entity();
            e.Name = t.Name;
            for (int j = 0; j < t.Attributes.Count; j++)
            {
                //remove atributos referenciais do modelo físico
                if (!(t.Attributes[j] is ReferencialAttribute))
                {
                    e.Attributes.Add(t.Attributes[j]);
                }
            }
            return e;
        }

        public Table ToTable()
        {
            Table T = new Table();
            T.Name = Name;
            T.Attributes = new DataModel.Collections.AttributeCollection(T);

            foreach (var attr in Attributes)
                T.Attributes.Add(attr);

            return T;
        }
    }
}
