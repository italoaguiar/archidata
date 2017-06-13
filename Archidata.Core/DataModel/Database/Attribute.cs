using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Database
{
    /// <summary>
    /// Representa um atributo de um elemento do banco de dados
    /// </summary>
    [Serializable]
    public class TableAttribute:INotifyPropertyChanged, ICloneable
    {
        private string name;
        private DBType type;
        private bool notNull;
        private bool primaryKey;
        private string @default;
        private bool isUnique;
        private string length;
        private AutoIncrement autoIncrement;

        /// <summary>
        /// Obtém ou define o nome do atributo
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Obtém ou define o Tipo de dados do atributo
        /// </summary>
        public DBType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        /// <summary>
        /// Obtém ou define se o atributo pode ser nulo ou não
        /// </summary>
        public bool NotNull
        {
            get { return notNull; }
            set
            {
                notNull = value;
                OnPropertyChanged("NotNull");
            }
        }

        /// <summary>
        /// Obtém ou define o valor padrão do atributo
        /// </summary>
        public string Default
        {
            get { return @default; }
            set
            {
                @default = value;
                OnPropertyChanged("Default");
            }
        }

        /// <summary>
        /// Obtém ou define se o atributo é único
        /// </summary>
        public bool IsUnique
        {
            get { return isUnique; }
            set
            {
                isUnique = value;
                OnPropertyChanged("IsUnique");
            }
        }

        /// <summary>
        /// Obtém ou define se o atributo é chave primária
        /// </summary>
        public bool PrimaryKey
        {
            get { return primaryKey; }
            set
            {
                primaryKey = value;
                OnPropertyChanged("PrimaryKey");
            }
        }
        /// <summary>
        /// Obtém ou define o Incremento do atributo
        /// </summary>
        public AutoIncrement AutoIncrement
        {
            get { return autoIncrement; }
            set
            {
                autoIncrement = value;
                OnPropertyChanged("AutoIncrement");
            }
        }

        /// <summary>
        /// Obtém ou define o tamanho do campo
        /// </summary>
        public string Length
        {
            get { return length; }
            set
            {
                if (Type.Validator.IsValid(value))
                {
                    length = value;
                    OnPropertyChanged("Length");
                }
                else
                    throw new ArgumentException(Type.Name + ":" + value);
            }
        }

        /// <summary>
        /// Obtém ou define o Pai do atributo
        /// </summary>
        public Table Parent
        {
            get;
            set;
        }


        /// <summary>
        /// Informa quando uma propriedade é alterada
        /// </summary>
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Obtém uma string que representa o objeto
        /// </summary>
        /// <returns>"Nome:Tipo(Tamanho)"</returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}({2})", Name, Type.Name, Length);
        }
        /// <summary>
        /// Retorna uma cópia do objeto
        /// </summary>
        public object Clone()
        {
            return MemberwiseClone();

        }
    }
}
