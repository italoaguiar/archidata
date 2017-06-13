using System;
using System.Windows;
using System.Windows.Media;

namespace Archidata.Core.Plugin
{
    /// <summary>
    /// Representa um tema visual para interface gráfica
    /// </summary>    
    public class Theme
    {
        [NonSerialized]
        private Brush _background;
        [NonSerialized]
        private Brush _foreground;
        [NonSerialized]
        private Brush _borderBrush;
        [NonSerialized]
        private Thickness _borderThickness;
        /// <summary>
        /// Obtém ou define o preenchimento do plano de fundo do tema
        /// </summary>                 
        public Brush Background
        {
            get { return _background; }
            set { _background = value; }
        }
        /// <summary>
        /// Obtém ou define preenchimento primário do tema
        /// </summary> 
        public Brush Foreground
        {
            get { return _foreground; }
            set { _foreground = value; }
        }
        /// <summary>
        /// Obtém ou define o preenchimento da borda do tema
        /// </summary> 
        public Brush BorderBrush
        {
            get { return _borderBrush; }
            set { _borderBrush = value; }
        }
        /// <summary>
        /// Obtém ou define a expessura da borda do tema
        /// </summary> 
        public Thickness BorderThickness
        {
            get { return _borderThickness; }
            set { _borderThickness = value; }
        }
    }
}
