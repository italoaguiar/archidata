using DataModel.Database.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DataModel.Database
{
    /// <summary>
    /// Define uma classe de tipos padrões
    /// </summary>
    public class DefaultTypes
    {
        /// <summary>
        /// Obtém o tipo padrão INT
        /// </summary>
        public static readonly DBType INT = new DBType("INT", "11", true);

        /// <summary>
        /// Obtém o tipo padrão SMALLINT
        /// </summary>
        public static readonly DBType SMALLINT = new DBType("SMALLINT","6",true);

        /// <summary>
        /// Obtém o tipo padrão BIGINT
        /// </summary>
        public static readonly DBType BIGINT = new DBType("BIGINT","20",true);

        /// <summary>
        /// Obtém o tipo padrão TINYINT
        /// </summary>
        public static readonly DBType TINYINT = new DBType("TINYINT","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão BIT
        /// </summary>
        public static readonly DBType BIT = new DBType("BIT","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão CHAR
        /// </summary>
        public static readonly DBType CHAR = new DBType("CHAR","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão NCHAR
        /// </summary>
        public static readonly DBType NCHAR = new DBType("NCHAR","15",true);

        /// <summary>
        /// Obtém o tipo padrão VARCHAR
        /// </summary>
        public static readonly DBType VARCHAR = new DBType("VARCHAR","15",true);

        /// <summary>
        /// Obtém o tipo padrão TEXT
        /// </summary>
        public static readonly DBType TEXT = new DBType("TEXT","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão FLOAT
        /// </summary>
        public static readonly DBType FLOAT = new DBType("FLOAT","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão DOUBLE
        /// </summary>
        public static readonly DBType DOUBLE = new DBType("DOUBLE","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão REAL
        /// </summary>
        public static readonly DBType REAL = new DBType("REAL","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão DECIMAL
        /// </summary>
        public static readonly DBType DECIMAL = new DBType("DECIMAL","10,5",true,new DecimalLengthType());

        /// <summary>
        /// Obtém o tipo padrão NUMERIC
        /// </summary>
        public static readonly DBType NUMERIC = new DBType("NUMERIC","10,5",true,new DecimalLengthType());

        /// <summary>
        /// Obtém o tipo padrão MONEY
        /// </summary>
        public static readonly DBType MONEY = new DBType("MONEY","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão SMALLMONEY
        /// </summary>
        public static readonly DBType SMALLMONEY = new DBType("SMALLMONEY","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão TIME
        /// </summary>
        public static readonly DBType TIME = new DBType("TIME","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão DATE
        /// </summary>
        public static readonly DBType DATE = new DBType("DATE","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão DATETIME
        /// </summary>
        public static readonly DBType DATETIME = new DBType("DATETIME","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão TIMESTAMP
        /// </summary>
        public static readonly DBType TIMESTAMP = new DBType("TIMESTAMP","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão BINARY
        /// </summary>
        public static readonly DBType BINARY = new DBType("BINARY", "1", true, new IntegerLengthType() { AllowEmptySize = true });

        /// <summary>
        /// Obtém o tipo padrão IMAGE
        /// </summary>
        public static readonly DBType IMAGE = new DBType("IMAGE","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão XML
        /// </summary>
        public static readonly DBType XML = new DBType("XML","", false, new EmptyLengthType());

        /// <summary>
        /// Obtém o tipo padrão UNIQUEIDENTIFIER
        /// </summary>
        public static readonly DBType UNIQUEIDENTIFIER = new DBType("UNIQUEIDENTIFIER", "", false, new EmptyLengthType());


        /// <summary>
        /// Obtém a lista de tipos padrões
        /// </summary>
        public static IEnumerable<DBType> GetDefaultTypes()
        {
            List<DBType> t = new List<DBType>();

            foreach(var item in typeof(DefaultTypes).GetFields())
            {
                t.Add((DBType)item.GetValue(null));
            }
            return t;
        }
    }
}
