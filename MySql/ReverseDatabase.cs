using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Windows;
using DataModel.Collections;
using DataModel.Database;
using Archidata.Core;

namespace MySql
{
    public class ReverseDatabase: IDisposable
    {
        DatabaseModel model = new DatabaseModel();
        public async Task<DatabaseModel> Get(string connectionString, string databaseName, AsyncOperationProgress p)
        {
            try
            {
                //instancia o modelo e atribui o nome
                model = new DatabaseModel();
                model.DatabaseName = databaseName;

                //obtem o nome das tabelas do banco de dados
                var tables = GetTables(connectionString, databaseName);

                p.Report(new OperationReport("Criando Tabelas virtuais", 20));

                //cria as tabelas do banco de dados
                foreach(string name in tables)
                {
                    Table e = new Table();
                    e.Name = name;
                    model.Tables.Add(e);
                }

                int prog = 0;
                //processa os atributos de cada tabela
                foreach (Table table in model.Tables)
                {
                    table.Attributes = await GetAttributes(connectionString, databaseName, table);
                    prog++;

                    p.Report("Obtendo atributos da tabela " + table.Name, 20 + ( prog * 50 / model.Tables.Count));
                }

                ProcessRelationships(connectionString, databaseName,p);

                p.Report("Pronto", 100);
                p.End();

            }
            catch (MySqlException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);

                MessageBox.Show("Não foi possível concluir a operação. Certifique-se de que os dados informados estão " +
                    "corretos e o servidor do banco de dados está ativo." + e.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }


            return model;
        }
        private async Task<AttributeCollection> GetAttributes(string connectionString, string database, Table table)
        {
            AttributeCollection c = new AttributeCollection(table);
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT COLUMN_NAME, COLUMN_TYPE, COLUMN_KEY, COLUMN_DEFAULT, 
                            EXTRA FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME NOT IN
                            (SELECT COLUMN_NAME FROM `information_schema`.`KEY_COLUMN_USAGE` 
                               WHERE TABLE_SCHEMA = @DB AND TABLE_NAME = @TABLE AND 
                               REFERENCED_TABLE_NAME IS NOT NULL)
                            AND TABLE_SCHEMA = @DB AND TABLE_NAME = @TABLE;";

            MySqlCommand command = new MySqlCommand(sql, conn);
            command.Parameters.AddWithValue("@DB", database);
            command.Parameters.AddWithValue("@TABLE", table.Name);

            MySqlDataReader dr = await command.ExecuteReaderAsync();

            while (dr.Read())
            {
                try
                {
                    TableAttribute attr = new TableAttribute();
                    attr.Name = dr["COLUMN_NAME"].ToString();
                    attr.Default = dr["COLUMN_DEFAULT"] != null ? dr["COLUMN_DEFAULT"].ToString() : null;
                    attr.PrimaryKey = dr["COLUMN_KEY"].ToString() == "PRI";
                    attr.AutoIncrement = dr["EXTRA"] != null ? dr["EXTRA"].ToString() == "auto_increment" ?
                        new DataModel.Database.AutoIncrement() { UseAutoIncrement = true } : null : null;
                    var type = ParseDataType(dr["COLUMN_TYPE"]);
                    attr.Type = type.Item2;
                    attr.Length = type.Item1;
                    c.Add(attr);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Erro: {0}; {1}\n", e.Message, e.StackTrace);
                }
            }

            conn.Close();

            return c;
        }
        private List<string> GetTables(string connectionString, string databaseName)
        {
            List<string> s = new List<string>();
            MySqlConnection conn = new MySqlConnection(connectionString);

            conn.Open();

            MySqlCommand command = new MySqlCommand(
                "SELECT `TABLE_NAME` FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=@DB;", 
                conn);

            command.Parameters.AddWithValue("@DB", databaseName);


            MySqlDataReader tables = command.ExecuteReader();

            while (tables.Read())
            {
                string tableName = tables["TABLE_NAME"].ToString();
                s.Add(tableName);
            }
            conn.Close();
            return s;
        }

        private void ProcessRelationships(string connectionString, string databaseName, AsyncOperationProgress p)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);

            conn.Open();
            string sql = @"SELECT DISTINCT T.TABLE_NAME, T.COLUMN_NAME, T.COLUMN_TYPE, T.COLUMN_KEY, 
                            T.COLUMN_DEFAULT, T.EXTRA, R.REFERENCED_TABLE_NAME, R.REFERENCED_COLUMN_NAME 
                            FROM INFORMATION_SCHEMA.COLUMNS T 
                            RIGHT JOIN `information_schema`.`KEY_COLUMN_USAGE` R 
                            ON T.COLUMN_NAME = R.COLUMN_NAME AND T.TABLE_SCHEMA = R.TABLE_SCHEMA
                            WHERE T.TABLE_SCHEMA = @DB AND R.CONSTRAINT_NAME != 'PRIMARY' 
                            AND T.COLUMN_KEY != 'PRI' AND R.REFERENCED_TABLE_NAME is not NULL;";

            MySqlCommand command = new MySqlCommand(sql, conn);
            command.Parameters.AddWithValue("@DB", databaseName);

            MySqlDataReader dr = command.ExecuteReader();

            while (dr.Read())
            {
                ReferencialAttribute attr = new ReferencialAttribute();

                string referencedTableName = dr["REFERENCED_TABLE_NAME"].ToString();
                string referencedColumnName = dr["REFERENCED_COLUMN_NAME"].ToString();

                if (string.IsNullOrEmpty(referencedTableName) || string.IsNullOrEmpty(referencedColumnName))
                    continue;

                p.Report("Relacionamento:" + referencedTableName, 70);

                attr.Name = dr["COLUMN_NAME"].ToString();
                attr.Default = dr["COLUMN_DEFAULT"] != null ? dr["COLUMN_DEFAULT"].ToString() : null;
                attr.PrimaryKey = dr["COLUMN_KEY"].ToString() == "PRI";
                attr.AutoIncrement = dr["EXTRA"] != null ? dr["EXTRA"].ToString() == "auto_increment" ?
                    new DataModel.Database.AutoIncrement() { UseAutoIncrement = true } : null : null;
                var type = ParseDataType(dr["COLUMN_TYPE"]);
                attr.Type = type.Item2;
                attr.Length = type.Item1;

                var referencedTable = model.Tables.FirstOrDefault(
                    r => r.Name == referencedTableName);

                if (referencedTable == null)
                    continue;

                var referencedColumn = referencedTable.Attributes.First(
                    a => a.Name == referencedColumnName);

                attr.ReferencedAttribute = referencedColumn;

                Table table = model.Tables.First(t => t.Name == dr["TABLE_NAME"].ToString());
                table.Attributes.Add(attr);
            }
            conn.Close();
        }

        private Tuple<string,DataModel.Database.DBType> ParseDataType(object value)
        {
            if (value == null) return null;

            var f = value.ToString();
            string lenght = "";

            DBType type = GetDatabaseType("INT");

            try
            {
                if (!f.Contains('(')) type = GetDatabaseType(TypeTable.First(x=> x.Item2.ToUpper() == f.ToUpper()).Item1);
                else
                {
                    var fn = Regex.Match(f, @"^\s*(\w+)\s*\((.*)\)");
                    string functionName = fn.Groups[1].Value;
                    lenght = fn.Groups[2].Value;

                    System.Diagnostics.Debug.WriteLine(functionName);

                    type = GetDatabaseType(TypeTable.First(x => x.Item2 == functionName.ToUpper()).Item1);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
            }

            return new Tuple<string, DataModel.Database.DBType>(lenght, type);
        }
        private Tuple<string, string>[] _typeTable;
        private Tuple<string, string>[] TypeTable
        {
            get
            {
                return _typeTable ?? (_typeTable = GetTypes());
            }
        }

        private Tuple<string, string>[] GetTypes()
        {
            var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("MySql.Types.txt");
            StreamReader sr = new StreamReader(s);

            List<Tuple<string, string>> types = new List<Tuple<string, string>>();

            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split('|');
                types.Add(new Tuple<string, string>(line[0], line[1]));
            }

            return types.ToArray();
        }
        private DBType GetDatabaseType(string type)
        {
            return DefaultTypes.GetDefaultTypes().First(x => x.Name.ToUpper() == type.ToUpper());
        }

        public void Dispose()
        {
            this._typeTable = null;
            this.model = null;
        }
    }
}
