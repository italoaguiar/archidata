using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Archidata.Core.Plugin;
using Archidata.Core.Controls;
using DataModel.Collections;
using DataModel.Database;
using System.Threading.Tasks;

namespace MySql
{
    [Export(typeof(IIOPlugin))]
    public class MySqlPlugin : IOPlugin
    {
        public MySqlPlugin()
        {
            var ca = Dispatcher;
            MenuContainer c = new MenuContainer();
            c.Label = "MySQL";

            MenuButton bt1 = new MenuButton();
            bt1.Label = "Gerar SQL";
            bt1.Click += OnGenerateSql;

             
            Binding b = new Binding("Model") { Source= this, Converter = new NullableToBooleanConverter() };
            bt1.SetBinding(MenuButton.IsEnabledProperty, b);


            MenuButton bt2 = new MenuButton();
            bt2.Label = "Obter Diagrama";
            bt2.SetBinding(MenuButton.IsEnabledProperty, b);
            bt2.Click += async (s,a) => { await OnGetDiagram(); };

            c.Items.Add(bt1);
            c.Items.Add(bt2);

            Content = c;
        }
        DatabaseConnection conn;
        string server, database, username, password;
        private async Task OnGetDiagram()
        {
            try
            {
                var r = MessageBox.Show("A operação requisitada irá sobrescrever o diagrama atual. Deseja prosseguir?", "Atenção", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.No) return;

                conn = new DatabaseConnection();
                conn.Server = server;
                conn.Database = database; 
                conn.UserName = username;
                conn.Password = password;

                if (conn.ShowDialog() == true)
                {
                    server = conn.Server;
                    database = conn.Database;
                    username = conn.UserName;
                    password = conn.Password;

                    string cs = string.Format("Server={0};Database={1};Uid={2};Pwd={3};", conn.Server, conn.Database, conn.UserName, conn.Password);

                    ReverseDatabase re = new ReverseDatabase();
                    
                    var result = await Task.Run(async () =>
                    {
                        return await re.Get(cs, conn.Database, new Archidata.Core.AsyncOperationProgress());

                    });

                    Model.Data = result;
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void OnGenerateSql(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = Model.Data;


                SQLGenerator gen = new SQLGenerator();
                string sql = gen.GetSql(model);

                var path = App.Save(".sql");
                using (StreamWriter sr = new StreamWriter(path))
                {
                    sr.Write(sql);
                    System.Diagnostics.Debug.WriteLine("SQL:" + sql);
                }
                App.Open(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
    public class SQLGenerator
    {
        public string GetSql(DatabaseModel model)
        {
            string sql = "";

            //gera as tabelas sql
            sql += "CREATE DATABASE " + model.DatabaseName + ";\n";
            sql += "USE " + model.DatabaseName + ";\n\n";

            foreach (var item in model.Tables)
            {
                sql += "CREATE TABLE `" + item.Name + "`(\n";

                var attr = item.Attributes.Where(x => !(x is ReferencialAttribute)).ToArray();

                for (int i = 0; i < attr.Count(); i++)
                {
                    sql += "\t" + ParseColumn(attr[i]);

                    if (i < attr.Count() - 1) sql += ",\n";
                }

                sql += GetPrimaryKey(item.Attributes);

                sql += "\n);\n\n";
            }

            //altera as tabelas para adicionar os relacionamentos
            foreach (var item in model.Tables)
            {
                for (int i = 0; i < item.Attributes.Count; i++)
                {
                    if (item.Attributes[i] is ReferencialAttribute)
                    {
                        ReferencialAttribute ra = (ReferencialAttribute)item.Attributes[i];

                        var pk = ra.ReferencedAttribute.Parent.Attributes.Where(x => x.PrimaryKey == true).FirstOrDefault();

                        if(pk == null)
                            throw new MissingFieldException("A Tabela " + ra.ReferencedAttribute.Parent.Name + " não possui chave primária.");

                        sql += "ALTER TABLE `" + item.Name.Trim() + "` ADD COLUMN " + ParseColumn(ra) + ";\n";
                        sql += "ALTER TABLE `" + item.Name.Trim() + "` ADD CONSTRAINT `pk_" + ra.ReferencedAttribute.Parent.Name + "` ";
                        sql += "FOREIGN KEY (`" + ra.Name.Trim() + "`) REFERENCES `" + ra.ReferencedAttribute.Parent.Name + "`";
                        sql += "(`" + pk.Name + "`);\n\n";
                    }
                }
            }


            return sql;
        }
        private string GetPrimaryKey(ObservableCollection<DataModel.Database.TableAttribute> e)
        {
            string k = "";
            var p = e.Where(x => x.PrimaryKey).ToArray();
            for (int i = 0; i< p.Count(); i++)
            {
                k += string.Format("`{0}`", p[i].Name);
                if ((i + 1) < p.Count())
                    k += ",";
            }
            if (p.Count() == 0) return string.Empty;

            return string.Format(",\n\tPRIMARY KEY ({0})", k);
        }
        private string ParseColumn(DataModel.Database.TableAttribute t)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("`{0}` ", t.Name));

            string len = t.Type.HaveLength ? string.Format("({0})", t.Length) : string.Empty;

            sb.Append(TypeTable.First(x => x.Item1 == t.Type.Name).Item2 + len);
            
            sb.Append(ParseAttribute(t));
            return sb.ToString();
        }

        private string ParseAttribute(DataModel.Database.TableAttribute a)
        {
            StringBuilder sb = new StringBuilder();
            if(a.NotNull) sb.Append(" NOT NULL");
            if (a.IsUnique) sb.Append(" UNIQUE");
            if (a.Default != null && a.Default == "NULL") sb.Append(" DEFAULT NULL");
            else if (!string.IsNullOrEmpty(a.Default)) sb.Append(string.Format(" DEFAULT \"{0}\"", a.Default));
            if (a.AutoIncrement != null && a.AutoIncrement.UseAutoIncrement) sb.Append(" AUTO_INCREMENT");

            return sb.ToString();
        }

        private Tuple<string, string>[] _typeTable;
        private Tuple<string,string>[] TypeTable
        {
            get
            {
                return _typeTable ?? (_typeTable = GetTypes());
            }
        }

        private Tuple<string,string>[] GetTypes()
        {
            var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("MySql.Types.txt");
            StreamReader sr = new StreamReader(s);
           // sr.
            List<Tuple<string, string>> types = new List<Tuple<string, string>>();

            while (!sr.EndOfStream)
            {
                string[] line = sr.ReadLine().Split('|');
                types.Add(new Tuple<string, string>(line[0], line[1]));
            }

            return types.ToArray();
        }
    }
    public class NullableToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
