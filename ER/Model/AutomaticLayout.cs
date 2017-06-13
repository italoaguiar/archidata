using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ER.Model
{
    public class AutomaticLayout
    {
        private ModelCollection model;

        public AutomaticLayout(ModelCollection c)
        {
            model = c;
        }

        public async Task<Layout> DoLayout()
        {
            GeometryGraph graph = new GeometryGraph();

            foreach(var item in model)
            {
                graph.Nodes.Add(new Node(
                    CurveFactory.CreateRectangle(item.Size.Width + 100, item.Size.Height + 100, new Microsoft.Msagl.Core.Geometry.Point()), item));
            }

            foreach(Relationship item in model.Where(x=> x is Relationship))
            {
                foreach (var rel in item.Connections)
                {
                    graph.Edges.Add(new Edge(graph.Nodes.First(x => x.UserData == item), graph.Nodes.First(x => x.UserData == rel.Target)));
                }
            }

            var settings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings();

            await Task.Run(() =>
            {
                LayoutHelpers.CalculateLayout(graph, settings, null);
            });
            
            
            foreach(var node in graph.Nodes)
            {                
                ((ModelElement)node.UserData).Location = new System.Windows.Point(node.Center.X, node.Center.Y);
            }

            var layout = new Layout() { Model = model, Size = new Size(graph.Width, graph.Height) };
            return layout;
        }
    }

    public class Layout
    {
        public ModelCollection Model { get; set; }
        public Size Size { get; set; }
    }

}
