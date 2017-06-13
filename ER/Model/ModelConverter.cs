using Archidata.Core.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.Database;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace ER.Model
{
    public class ModelSource : IModelSource
    {
        public DatabaseModel Data
        {
            get { return ConvertBack(); }
            set { Convert(value); }
        }
        public ModelSource(Func<ModelCollection> GetModel, Action<ModelCollection, Size> UpdateModel)
        {
            this.GetModel = GetModel;
            this.UpdateModel = UpdateModel;
        }

        protected Func<ModelCollection> GetModel;
        protected Action<ModelCollection, Size> UpdateModel;

        
        public async void Convert(DatabaseModel model)
        {
            StateService.Disable();

            await Task.Run(async () =>
            {
                ModelCollection mc = new Model.ModelCollection();

                Archidata.Core.AsyncOperationProgress p = new Archidata.Core.AsyncOperationProgress();
                p.Report("Gerando Diagrama Entidade-Relacionamento", 0);



                //cria as entidades do modelo
                for (int i = 0; i < model.Tables.Count; i++)
                {
                    mc.Add(Entity.FromTable(model.Tables[i]));

                    p.Report("Criando Entidade " + model.Tables[i], ((i + 1) * 40)/model.Tables.Count);
                }

                //cria os relacionamentos do modelo
                foreach (var item in model.Tables)
                {
                    for (int i = 0; i < item.Attributes.Count; i++)
                    {
                        //utiliza atributos referenciais para criar relacionamentos
                        if (item.Attributes[i] is ReferencialAttribute)
                        {
                            ReferencialAttribute ra = (ReferencialAttribute)item.Attributes[i];
                            Relationship r = new Model.Relationship();
                            r.Name = string.Format("{0}_{1}", ra.Parent.Name, ra.ReferencedAttribute.Parent.Name);

                            p.Report("Criando Relacionamento " + r.Name, (((i + 1) * 40) / model.Tables.Count) + 40);

                            Connection c1 = new Model.Connection();
                            c1.Target = (Entity)mc.Where(x => x.Name == ra.Parent.Name).First();
                            c1.Cardinality = global::Cardinality.MULTI;

                            Connection c2 = new Model.Connection();
                            c2.Target = (Entity)mc.Where(x => x.Name == ra.ReferencedAttribute.Parent.Name).First();
                            c2.Cardinality = global::Cardinality.ONE;

                            r.Connections.Add(c1);
                            r.Connections.Add(c2);

                            mc.Add(r);
                        }
                    }
                }

                p.Report("Calculando Layout", 85);


                var layout = await (new AutomaticLayout(mc)).DoLayout();

                p.Report("Aplicando Modelo", 90);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateModel(layout.Model, layout.Size);
                });

                p.Report("Pronto", 100);

                p.End();
            });

            StateService.Enable();
        }

        public DatabaseModel ConvertBack()
        {
            DatabaseModel model = new DatabaseModel();

            var m = GetModel();

            var tables = m.Where(x => x is Entity);
            var relationships = m.Where(x => x is Relationship);

            //cria as tabelas
            foreach (Entity table in tables)
            {
                model.Tables.Add(table.ToTable());
            }

            //cria os relacionamentos
            foreach (Relationship rel in relationships)
            {
                switch (GetCardinality(rel))
                {
                    case Cardinality.M_to_M:
                        M2N(ref model, rel);
                        break;
                    case Cardinality.One_to_One:
                        One2One(ref model, rel);
                        break;
                    case Cardinality.One_to_M:
                        One2M(ref model, rel);
                        break;
                }
            }

            return model;
        }

        private void M2N(ref DatabaseModel model, Relationship rel)
        {
            Table t = new Table();

            var conn = rel.Connections.Where(x => x.Cardinality == global::Cardinality.MULTI);

            if (conn.Count() == 1) return;

            foreach (var attr in conn)
            {
                var pk = attr.Target.Attributes.Where(x => x.PrimaryKey).FirstOrDefault();

                t.Name += attr.Target.Name + "_";

                if (pk == null)
                    throw new MissingFieldException("A Entidade " + attr.Target.Name + " não possui chave primária.");


                ReferencialAttribute r = new ReferencialAttribute();
                r.Name = attr.Target.Name + "_Id";
                r.AutoIncrement = new AutoIncrement() { UseAutoIncrement = false };
                r.Type = pk.Type;
                r.Length = pk.Length;
                r.NotNull = true;
                r.Parent = t;
                r.PrimaryKey = true;
                r.ReferencedAttribute = pk;

                t.Attributes.Add(r);
            }
            model.Tables.Add(t);
        }

        private void One2One(ref DatabaseModel model, Relationship rel)
        {
            var conn = rel.Connections.Where(x => x.Cardinality == global::Cardinality.ONE);

            foreach (var attr in conn)
            {
                foreach (var at in conn)
                {
                    if (at != attr)
                    {
                        var pk = at.Target.Attributes.Where(x => x.PrimaryKey).FirstOrDefault();

                        if (pk == null)
                            throw new MissingFieldException("A Entidade " + at.Target.Name + " não possui chave primária.");

                        var parent = model.Tables.Where(x => x.Name == attr.Target.Name).FirstOrDefault();

                        attr.Target.Attributes.Add(new ReferencialAttribute()
                        {
                            AutoIncrement = new AutoIncrement() { UseAutoIncrement = false },
                            Name = at.Target.Name + "_Id",
                            Type = pk.Type,
                            Length = pk.Length,
                            NotNull = true,
                            Parent = parent,
                            PrimaryKey = false,
                            ReferencedAttribute = pk
                        });
                    }
                }
            }
        }

        private void One2M(ref DatabaseModel model, Relationship rel)
        {
            var one = rel.Connections.Where(x => x.Cardinality == global::Cardinality.ONE);
            var multi = rel.Connections.Where(x => x.Cardinality == global::Cardinality.MULTI);

            foreach (var t in multi)
            {
                var table = model.Tables.Where(x => x.Name == t.Target.Name).FirstOrDefault();
                foreach (var at in one)
                {
                    var pk = at.Target.Attributes.Where(x => x.PrimaryKey).FirstOrDefault();

                    if (pk == null)
                        throw new MissingFieldException("A Entidade " + at.Target.Name + " não possui chave primária.");

                    table.Attributes.Add(new ReferencialAttribute()
                    {
                        Name = at.Target.Name + "_Id",
                        Type = pk.Type,
                        Length = pk.Length,
                        AutoIncrement = new AutoIncrement() { UseAutoIncrement = false },
                        NotNull = true,
                        PrimaryKey = false,
                        Parent = table,
                        ReferencedAttribute = pk
                    });
                }
            }
            M2N(ref model, rel);
        }

        private Cardinality GetCardinality(Relationship r)
        {
            Cardinality c = Cardinality.M_to_M;
            foreach (var rel in r.Connections)
            {
                if (rel.Cardinality == global::Cardinality.ONE)
                    c = Cardinality.One_to_One;
            }
            if (c == Cardinality.M_to_M) return c;

            foreach (var rel in r.Connections)
            {
                if (rel.Cardinality == global::Cardinality.MULTI)
                    c = Cardinality.M_to_M;
            }
            if (c == Cardinality.One_to_One) return c;

            return Cardinality.One_to_M;
        }

        private enum Cardinality
        {
            M_to_M,
            One_to_M,
            One_to_One
        }
    }
}
