using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Archidata
{
    public static class ProjectManager
    {
        private const string PROJECTS_FOLDER = "Meus diagramas";

        
        static ProjectManager()
        {
            
            
        }

        public static Project NewProject(string ProjectName)
        {
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var projPath = Path.Combine(docPath, PROJECTS_FOLDER);

            if (!Directory.Exists(projPath))
                Directory.CreateDirectory(projPath);

            var p = Path.Combine(projPath, ProjectName);

            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);

                RecentProjectsManager.AddEntry(p);

                return LoadProject(p);
            }

            return null;
        }


        public static string DefaultFolder
        {
            get
            {
                var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(docPath, PROJECTS_FOLDER);
            }
        }

        public static string SaveProjectFile(string projectname, string filename, string extension)
        {
            if (!extension.StartsWith("."))
                throw new ArgumentException("A extensão deve iniciar com ponto.");

            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var projPath = Path.Combine(docPath, PROJECTS_FOLDER);
            var file = Path.Combine(projPath, projectname, filename + extension);

            var f = File.Create(file);
            f.Close();

            return file;
        }

        public static bool CanCreateProject(string name)
        {
           var p = Path.GetInvalidPathChars();
           var f = Path.GetInvalidFileNameChars();
           if (name.Any((x) => p.Contains(x) || f.Contains(x)))
                return false;

            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var projPath = Path.Combine(docPath, PROJECTS_FOLDER, name);

            return !Directory.Exists(projPath);
        }

        public static Project LoadProject(string projectPath)
        {
            Project p = new Project();
            p.ProjectName = Path.GetFileName(projectPath);
            p.UpdateFiles(projectPath);
            return p;
        }
    }

    public static class RecentProjectsManager
    {
        public static List<string> GetRecentProjects()
        {
            List<string> projects = new List<string>();

            try
            {
                var sf = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var folder = Path.Combine(sf, "Archidata");
                string path = Path.Combine(folder, "projects.rf");
                using (StreamReader sw = new StreamReader(path))
                {
                    while (!sw.EndOfStream)
                    {
                        var path_ = sw.ReadLine();
                        if (Directory.Exists(path_))
                            projects.Add(path_);
                    }
                }
            }
            catch { }

            return projects.Skip(Math.Max(0, projects.Count() - 12)).ToList();
        }

        public static void AddEntry(string projectAdress)
        {
            try
            {
                var sf = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var folder = Path.Combine(sf, "Archidata");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string path = Path.Combine(folder, "projects.rf");

                using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    StreamReader sr = new StreamReader(stream);
                    string content = sr.ReadToEnd();

                    using (StreamWriter sw = new StreamWriter(stream))
                    {
                        var r = projectAdress + "\n" + content;
                        sw.Write(r);
                    }
                }
            }
            catch { }
        }
    }


    public class Project : IDisposable
    {
        ~Project()
        {
            Dispose(false);
        }

        private void OnFolderChanged(object sender, FileSystemEventArgs e)
        {
            UpdateFiles(projectPath);
        }

        FileSystemWatcher w = new FileSystemWatcher();

        ObservableCollection<ProjectFile> files;
        public string ProjectName { get; set; }
        string projectPath;
        public string ProjectPath
        {
            get { return projectPath; }
            set
            {
                projectPath = value;
                w = new FileSystemWatcher(value, "*.*");
                w.IncludeSubdirectories = true;
                w.EnableRaisingEvents = true;
                w.Changed += OnFolderChanged;
                w.Created += OnFolderChanged;
                w.Deleted += OnFolderChanged;
                w.Renamed += OnFolderChanged;
            }
        }
        public ObservableCollection<ProjectFile> Files
        {
            get
            {
                return files ?? (files = new ObservableCollection<ProjectFile>());
            }
            set { files = value; }
        }

        public void UpdateFiles(string projectPath)
        {
            App.Current.Dispatcher.Invoke(() => 
            { 
                ProjectPath = projectPath;                        
                Files.Clear();
                if (Directory.Exists(projectPath))
                {
                    var files = Directory.GetFiles(projectPath);
                    foreach (var f in files)
                    {
                        var a = File.GetAttributes(f);
                        if (a == FileAttributes.Archive)
                        {
                            ProjectFile pf = new ProjectFile(f);
                            var icon = App.Extensions.GetIconByExtension(pf.FileExtension);
                            if(icon == null)
                            {
                                icon = new BitmapImage(new Uri("Images/Icons/Unknown.png",UriKind.Relative));
                            }
                            pf.Icon = icon;
                            Files.Add(pf);
                        }
                    }
                }
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && w != null)
            {
                w.Dispose();
                w = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class ProjectFile
    {
        public ProjectFile(string path)
        {
            FilePath = Path.GetFullPath(path);
            FileName = Path.GetFileName(path);
            FileExtension = Path.GetExtension(path);
        }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public ImageSource Icon { get; set; }
    }
}
