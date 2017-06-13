using DataModel.Database.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataModel.Database
{
    /// <summary>
    /// Representa um tipo de dado do banco de dados
    /// </summary>
    [Serializable]
    public class DBType
    {
        /// <summary>
        /// Cria uma nova instância do tipo DBType
        /// </summary>
        protected internal DBType()
        {

        }

        /// <summary>
        /// Cria uma nova instância do tipo DBType
        /// </summary>
        /// <param name="Type">Define o tipo de dado do campo</param>
        /// <param name="Length">Define o tamanho do campo</param>
        protected internal DBType(String Type, string Length)
        {
            this.Name = Type;
            this.DefaultLength = Length;
        }

        /// <summary>
        /// Cria uma nova instância do tipo DBType
        /// </summary>
        /// <param name="Type">Define o tipo de dado do campo</param>
        /// <param name="Length">Define o tamanho do campo</param>
        /// <param name="validator">Define o objeto de validação do tipo</param>
        protected internal DBType(string Type, string Length,ITypeLengthValidator validator)
        {
            this.Name = Type;
            this.DefaultLength = Length;
            this.Validator = validator;
        }

        /// <summary>
        /// Cria uma nova instância do tipo DBType
        /// </summary>
        /// <param name="Type">Define o tipo de dado do campo</param>
        /// <param name="HaveLength">Define se um tipo de dados possui tamanho definido </param>
        protected internal DBType(string Type, bool HaveLength)
        {
            this.Name = Type;
            this.HaveLength = HaveLength;
        }

        /// <summary>
        /// Cria uma nova instância do tipo DBType
        /// </summary>
        /// <param name="Type">Define o tipo de dado do campo</param>
        /// <param name="Length">Define o tamanho do campo</param>
        /// <param name="HaveLength">Define se um tipo de dados possui tamanho definido </param>
        protected internal DBType(string Type, string Length, bool HaveLength)
        {
            this.Name = Type;
            this.DefaultLength = Length;
            this.HaveLength = HaveLength;
        }


        /// <summary>
        /// Cria uma nova instância do tipo DBType
        /// </summary>
        /// <param name="Type">Define o tipo de dado do campo</param>
        /// <param name="Length">Define o tamanho do campo</param>
        /// <param name="HaveLength">Define se um tipo de dados possui tamanho definido </param>
        /// <param name="Validator">Define o objeto de validação do tipo</param>
        protected internal DBType(string Type, string Length, bool HaveLength, ITypeLengthValidator Validator)
        {
            this.Name = Type;
            this.Validator = Validator;
            this.DefaultLength = Length;
            this.HaveLength = HaveLength;            
        }

        /// <summary>
        /// Obtém ou define se um tipo de dados possui tamanho definido
        /// </summary>
        public bool HaveLength { get; set; }
        

        /// <summary>
        /// Obtém ou define o tipo do campo
        /// </summary>
        public string Name { get; set; }

        private ITypeLengthValidator _validator;

        /// <summary>
        /// Obtém ou define um validador de tamanho de campo
        /// </summary>
        [XmlIgnore]
        public ITypeLengthValidator Validator
        {
            get
            {
                return _validator ?? (_validator = new IntegerLengthType());
            }
            set
            {
                _validator = value;
            }
        }

        private string _length;

        /// <summary>
        /// Obtém ou define o tamanho do campo
        /// </summary>
        public string DefaultLength
        {
            get
            {
                return _length;
            }
            set
            {
                if (Validator.IsValid(value)) _length = value;
                else throw new ArgumentException();
            }
        }

        /// <summary>
        /// Obtém uma representação em caracteres do objeto
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}
