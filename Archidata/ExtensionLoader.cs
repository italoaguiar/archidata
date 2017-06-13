using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.Composition.Hosting;
using System.Windows.Media;
using Archidata.Core.Plugin;

namespace Archidata
{
    public class AppExtensions
    {
        public IEnumerable<IUIPlugin> Controls { get; private set; }
        public IEnumerable<IIOPlugin> DBExtensions { get; private set; }

        public delegate void OnProgressChanged(int percent);

        public void Load()
        {
            if (Controls != null) return;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plugins\Graphic\");

            var c = new CompositionLoader<IUIPlugin>();
            Controls = c.Load(path);

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plugins\Database\");

            var c2 = new CompositionLoader<IIOPlugin>();
            DBExtensions = c2.Load(path);

            var k = this;           
        }

        public IUIPlugin GetControlInstanceByExtension(string extension)
        {
            foreach (var i in Controls)
                if (i.FileExtension.Where(p=> p.Extension.Contains(extension)).Count() > 0)
                {
                    var x = (IUIPlugin)Activator.CreateInstance(i.GetType());
                    return x;
                }
            return null;
        } 
        
        public ImageSource GetIconByExtension(string extension)
        {
            foreach (var i in Controls)
                if (i.FileExtension.Where(p => p.Extension.Contains(extension)).Count() > 0)
                {
                    var x = i.FileExtension.First(p => p.Extension == extension).Icon;
                    return x;
                }
            return null;
        }    
        
        public IEnumerable<FileType> GetSupportedLoadedFiles()
        {
            List<FileType> l = new List<FileType>();
            foreach(var i in Controls)
            {
                foreach(var f in i.FileExtension)
                {
                    l.Add(f);
                }
            }
            return l;
        }
         
    }

    

    public class CompositionLoader<T>
    {
        [ImportMany]
        public IEnumerable<T> Items;

        public IEnumerable<T> Load(string path)
        {
            var catalog = new DirectoryCatalog(path);
            using (var container = new CompositionContainer(catalog))
            {
                container.ComposeParts(this);
                return this.Items;
            }
        }
    }
}
