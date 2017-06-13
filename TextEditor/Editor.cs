using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Media.Imaging;
using System.Reflection;
using Archidata.Core.Plugin;

namespace TextEditor
{
    [Export(typeof(IUIPlugin))]
    public class Editor : UIPlugin
    {
        static Editor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Editor),
                new FrameworkPropertyMetadata(typeof(Editor)));

        }
        private string[] SQL = new string[]
        {
            "ABORT","BETWEEN","CRASH","DIGITS","ACCEPT","BINARY_INTEGER","CREATE","DISPOSE","ACCESS",
            "BODY","CURRENT","DISTINCT","ADD","BOOLEAN","CURRVAL","DO","ALL","BY","CURSOR","DROP","REMR",
            "ALTER","CASE","DATABASE","ELSE","AND","CHAR","DATA_BASE","ELSIF","ANY","CHAR_BASE","DATE",
            "END","ARRAY","CHECK","DBA","ENTRY","ARRAYLEN","CLOSE","DEBUGOFF","EXCEPTION","AS","CLUSTER",
            "DEBUGON","EXCEPTION_INIT","ASC","CLUSTERS","DECLARE","EXISTS","ASSERT","COLAUTH","DECIMAL",
            "EXIT","ASSIGN","COLUMNS","DEFAULT","FALSE","AT","COMMIT","DEFINITION","FETCH","AUTHORIZATION",
            "COMPRESS","DELAY","FLOAT","AVG","CONNECT","DELETE","FOR","BASE_TABLE","CONSTANT","DELTA",
            "FORM","BEGIN","COUNT","DESC","FROM","FUNCTION","NEW","RELEASE","SUM","GENERIC","NEXTVAL",
            "TABAUTH","GOTO","NOCOMPRESS","RENAME","TABLE","GRANT","NOT","RESOURCE","TABLES","GROUP","WORK",
            "NULL","RETURN","TASK","HAVING","NUMBER","REVERSE","TERMINATE","IDENTIFIED","NUMBER_BASE",
            "THEN","IF","OF","ROLLBACK","TO","IN","ON","ROWID","TRUE","INDEX","OPEN","ROWLABEL","TYPE",
            "OPTION","ROWNUM","UNION","INDICATOR","OR","ROWTYPE","UNIQUE","INSERT","ORDER","RUN","UPDATE",
            "OTHERS","SAVEPOINT","USE","INTERSECT","OUT","SCHEMA", "VALUES","INTO","PACKAGE","SELECT",
            "PARTITION","SEPARATE","VARCHAR2","LEVEL","PCTFREE","SET","VARIANCE","LIKE","POSITIVE","SIZE",
            "LIMITED","PRAGMA","SMALLINT","VIEWS","LOOP","PRIOR","SPACE","WHEN","MAX","PRIVATE","SQL",
            "PROCEDURE","SQLCODE","WHILE","MINUS","PUBLIC","SQLERRM","WITH","MLSLABEL","RAISE","START",
            "MOD","RANGE","STATEMENT","XOR","MODE","REAL","STDDEV","NATURAL","RECORD","SUBTYPE","TRUNCATE",
            "DOUBLE","TINYINT","DATETIME","INTEGER","VARCHAR","IS","WHERE","MIN","VIEW","INDEXES","REVOKE"
        };
        protected ICSharpCode.AvalonEdit.TextEditor PART_EDITOR;



        public override FileType[] FileExtension
        {
            get
            {
                return new FileType[] 
                {
                    new FileType(".sql", "Arquivo SQL", BitmapFrame.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("TextEditor.Icons.SQL.png"))),
                    new FileType(".txt", "Arquivo de Texto", BitmapFrame.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("TextEditor.Icons.TXT.png")))
                };
            }
        }

        public override void Undo()
        {
            PART_EDITOR.Undo();
        }
        public override void Redo()
        {
            PART_EDITOR.Redo();
        }
        public override void Copy()
        {
            PART_EDITOR.Copy();
        }
        public override void Cut()
        {
            PART_EDITOR.Cut();
        }
        public override void Paste()
        {
            PART_EDITOR.Paste();
        }

        public override void OnZoomChanged(double oldValue, double newValue)
        {
            PART_EDITOR.FontSize = 14 * newValue; 
        }

        string text;
        string filePath;
        public override void Open(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    filePath = path;
                    StreamReader sr = File.OpenText(path);
                    text = sr.ReadToEnd();
                }
            }
            catch
            {
                PART_EDITOR.Text = "ARQUIVO INVÁLIDO!";
            }
        }

        public override void CreateFile(string path)
        {
            try
            {
                var f = File.Create(path);
                f.Close();
            }
            catch { }
        }

        public override void Save()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.Write(PART_EDITOR.Text);
                        sw.Flush();
                    }
                    CanSave = false;
                }
            }
            catch
            {

            }
        }

        public override void Save(string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(PART_EDITOR.Text);
                    sw.Flush();
                }
                CanSave = false;
            }
            catch { }
        }

        CompletionWindow w;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_EDITOR = GetTemplateChild("PART_EDITOR") as ICSharpCode.AvalonEdit.TextEditor;
            PART_EDITOR.Loaded += (s, a) =>
            {
                PART_EDITOR.Text = text;
            };
            PART_EDITOR.TextArea.TextEntered += (sender, args) =>
            {
                if(args.Text == " ")
                {
                    w = new CompletionWindow(PART_EDITOR.TextArea);
                    IList<ICompletionData> data = w.CompletionList.CompletionData;
                    foreach(string s in SQL)
                        data.Add(new CompletionData(s));
                    w.Show();
                    w.Closed += delegate {
                        w = null;
                    };
                }
            };

            CanPaste = Clipboard.ContainsText();
            CanCopy = true;
            CanCut = true;

            PART_EDITOR.TextChanged += (s, a) =>
            {
                CanUndo = PART_EDITOR.CanUndo;
                CanRedo = PART_EDITOR.CanRedo;
                CanSave = true;
            };

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TextEditor.sql.xshd"))
            {
                var reader = new System.Xml.XmlTextReader(stream);
                
                PART_EDITOR.SyntaxHighlighting =
                    ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                
            }            
        }
        private BitmapFrame LoadBitmapFromResource(string resourceName)
        {
            Stream stream = Application.GetResourceStream(
                new Uri(string.Format("Controls;component/{0}", resourceName), UriKind.Relative)
            ).Stream;

            return BitmapFrame.Create(stream);
        }
    }
    public class CompletionData : ICompletionData
    {
        public CompletionData(string text)
        {
            this.Text = text;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get { return "Description for " + this.Text; }
        }

        public double Priority
        {
            get
            {
                return 0;
            }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
