using Archidata.Core.Helpers;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;

namespace Archidata.Core.Plugin
{
    /// <summary>
    /// Representa um plugin de interface gráfica
    /// </summary>
    public class UIPlugin : Control, IUIPlugin
    {
        /// <summary>
        /// Inicializa uma nova instância de UIPlugin
        /// </summary>
        public UIPlugin()
        {
            _CopyCommand = new CommandAdapter(parameter => Copy()) { };
            _PasteCommand = new CommandAdapter(parameter => Paste()) { };
            _CutCommand = new CommandAdapter(parameter => Cut()) { };
            _UndoCommand = new CommandAdapter(parameter => Undo()) { };
            _RedoCommand = new CommandAdapter(parameter => Redo()) { };
            _SaveCommand = new CommandAdapter(parameter => Save()) { };
            _PrintCommand = new CommandAdapter(parameter => Print());
            _ExportCommand = new CommandAdapter(parameter => Export());
        }

        private CommandAdapter _CopyCommand;
        private CommandAdapter _PasteCommand;
        private CommandAdapter _CutCommand;
        private CommandAdapter _UndoCommand;
        private CommandAdapter _RedoCommand;
        private CommandAdapter _SaveCommand;
        private CommandAdapter _PrintCommand;
        private CommandAdapter _ExportCommand;

        /// <summary>
        /// Obtém o comando Copiar
        /// </summary>
        public ICommand CopyCommand { get { return _CopyCommand; } }
        /// <summary>
        /// Obtém o comando Colar
        /// </summary>
        public ICommand PasteCommand { get { return _PasteCommand; } }
        /// <summary>
        /// Obtém o comando Recortar
        /// </summary>
        public ICommand CutCommand { get { return _CutCommand; } }
        /// <summary>
        /// Obtém o comando Desfazer
        /// </summary>
        public ICommand UndoCommand { get { return _UndoCommand; } }
        /// <summary>
        /// Obtém o comando Refazer
        /// </summary>
        public ICommand RedoCommand { get { return _RedoCommand; } }
        /// <summary>
        /// Obtém o comando Salvar
        /// </summary>
        public ICommand SaveCommand { get { return _SaveCommand; } }
        /// <summary>
        /// Obtém o comando Imprimir
        /// </summary>
        public ICommand PrintCommand { get { return _PrintCommand; } }
        /// <summary>
        /// Obtém o comando Exportar
        /// </summary>
        public ICommand ExportCommand { get { return _ExportCommand; } }


        /// <summary>
        /// Obtém ou define a propriedade CanCopy
        /// </summary>
        public static readonly DependencyProperty CanCopyProperty =
            DependencyProperty.Register("CanCopy", typeof(bool),
            typeof(UIPlugin), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define a propriedade CanCopy
        /// </summary>
        public bool CanCopy
        {
            get { return (bool)GetValue(CanCopyProperty); }
            set
            {
                SetValue(CanCopyProperty,value);
                ((CommandAdapter)_CopyCommand).CanExecuteAction = value;
            }
        }

        /// <summary>
        /// Obtém ou define a propriedade CanPaste
        /// </summary>
        public static readonly DependencyProperty CanPasteProperty =
            DependencyProperty.Register("CanPaste", typeof(bool),
            typeof(UIPlugin), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define a propriedade CanPaste
        /// </summary>
        public bool CanPaste
        {
            get { return (bool)GetValue(CanPasteProperty); }
            set
            {
                SetValue(CanPasteProperty, value);
                ((CommandAdapter)_PasteCommand).CanExecuteAction = value;
            }
        }

        /// <summary>
        /// Obtém ou define a propriedade Cut
        /// </summary>
        public static readonly DependencyProperty CanCutProperty =
            DependencyProperty.Register("CanCut", typeof(bool),
            typeof(UIPlugin), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define a propriedade Cut
        /// </summary>
        public bool CanCut
        {
            get { return (bool)GetValue(CanCutProperty); }
            set
            {
                SetValue(CanCutProperty, value);
                ((CommandAdapter)_CutCommand).CanExecuteAction = value;
            }
        }

        /// <summary>
        /// Obtém ou define a propriedade CanUndo
        /// </summary>
        public static  readonly DependencyProperty CanUndoProperty =
            DependencyProperty.Register("CanUndo", typeof(bool),
            typeof(UIPlugin), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define a propriedade CanUndo
        /// </summary>
        public bool CanUndo
        {
            get { return (bool)GetValue(CanUndoProperty); }
            set
            {
                SetValue(CanUndoProperty, value);
                ((CommandAdapter)_UndoCommand).CanExecuteAction = value;
            }
        }

        /// <summary>
        /// Obtém ou define a propriedade CanRedo
        /// </summary>
        public static readonly DependencyProperty CanRedoProperty =
            DependencyProperty.Register("CanRedo", typeof(bool),
            typeof(UIPlugin), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define a propriedade CanRedo
        /// </summary>
        public bool CanRedo
        {
            get { return (bool)GetValue(CanRedoProperty); }
            set
            {
                SetValue(CanRedoProperty, value);
                ((CommandAdapter)_RedoCommand).CanExecuteAction = value;
            }
        }


        /// <summary>
        /// Obtém ou define a propriedade CanSave
        /// </summary>
        public static readonly DependencyProperty CanSaveProperty =
            DependencyProperty.Register("CanSave", typeof(bool),
            typeof(UIPlugin), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define a propriedade CanSave
        /// </summary>
        public bool CanSave
        {
            get { return (bool)GetValue(CanSaveProperty); }
            set
            {
                SetValue(CanSaveProperty, value);
                ((CommandAdapter)_SaveCommand).CanExecuteAction = value;
            }
        }


        /// <summary>
        /// Obtém ou define a propriedade CanRedo
        /// </summary>
        public static readonly DependencyProperty CanPrintProperty =
            DependencyProperty.Register("CanPrint", typeof(bool),
            typeof(UIPlugin), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define a propriedade CanRedo
        /// </summary>
        public bool CanPrint
        {
            get { return (bool)GetValue(CanPrintProperty); }
            set
            {
                SetValue(CanPrintProperty, value);
                ((CommandAdapter)_PrintCommand).CanExecuteAction = value;
            }
        }

        /// <summary>
        /// Obtém ou define a propriedade CanRedo
        /// </summary>
        public static readonly DependencyProperty CanExportProperty =
            DependencyProperty.Register("CanExport", typeof(bool),
            typeof(UIPlugin), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtém ou define a propriedade CanRedo
        /// </summary>
        public bool CanExport
        {
            get { return (bool)GetValue(CanExportProperty); }
            set
            {
                SetValue(CanExportProperty, value);
                ((CommandAdapter)_ExportCommand).CanExecuteAction = value;
            }
        }

        /// <summary>
        /// Obtém ou define o tema visual do plugin
        /// </summary>
        [NonSerialized]
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(Theme),
            typeof(UIPlugin), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnThemeChanged)));

        /// <summary>
        /// Obtém ou define o tema visual do plugin
        /// </summary>
        public Theme Theme
        {
            get { return (Theme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIPlugin)d).OnApplyTheme(e.NewValue as Theme);
        }

        /// <summary>
        /// Obtém ou define o conjunto de elementos do menu de opções da aplicação
        /// </summary>
        public virtual IList MenuControls
        {
            get{ return null; }
            set { }
        }

        /// <summary>
        /// Obtém ou define o Zoom do plugin
        /// </summary>
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double),
            typeof(UIPlugin), new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(ZoomChanged)));

        private static void ZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UIPlugin)d).OnZoomChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Obtém ou define o Zoom do plugin
        /// </summary>
        public virtual double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        /// <summary>
        /// Obtém ou define o conjunto de extensões de arquivo suportadas pelo plugin
        /// </summary>
        public virtual FileType[] FileExtension
        {
            get
            {
                return null;
            }
        }

        public virtual IModelSource Model { get; set; }

        /// <summary>
        /// Executa o comando Copiar
        /// </summary>
        public virtual void Copy() { }
        /// <summary>
        /// Executa o comando Recortar
        /// </summary>
        public virtual void Cut() { }
        /// <summary>
        /// Executa o comando Colar
        /// </summary>
        public virtual void Paste() { }
        /// <summary>
        /// Executa o comando Desfazer
        /// </summary>
        public virtual void Undo() { }
        /// <summary>
        /// Executa o comando Refazer
        /// </summary>
        public virtual void Redo() { }

        /// <summary>
        /// Salva um arquivo de projeto
        /// </summary>
        public virtual void Save(string path) { }

        /// <summary>
        /// Salva um arquivo de projeto
        /// </summary>
        public virtual void Save() { }

        /// <summary>
        /// Cria um novo arquivo
        /// </summary>
        public virtual void CreateFile(string path) { }

        /// <summary>
        /// Imprime um arquivo de projeto
        /// </summary>
        public virtual void Print() { }

        /// <summary>
        /// Exporta um arquivo de projeto
        /// </summary>
        public virtual void Export() { }

        /// <summary>
        /// Abre um arquivo de Projeto
        /// </summary>
        public virtual void Open(string path) { }

        /// <summary>
        /// Notifica uma alteração de Zoom
        /// </summary>
        public virtual void OnZoomChanged(double oldValue, double newValue) { }

        /// <summary>
        /// Notifica uma alteração de tema visual
        /// </summary>
        public virtual void OnApplyTheme(Theme theme) { }
    }
}
