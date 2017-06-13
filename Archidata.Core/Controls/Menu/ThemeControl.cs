using Archidata.Core.Plugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Archidata.Core.Controls
{


    /// <summary>
    /// Representa um controle de seleção de temas visuais
    /// </summary>
    [TemplatePart(Name = "PART_LINEUP", Type =typeof(Button))]
    [TemplatePart(Name = "PART_DOWN", Type = typeof(Button))]
    [TemplatePart(Name = "PART_SCROLLVIEWER", Type =typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_LIST", Type = typeof(ThemeList))]
    [TemplatePart(Name = "PART_POPUPLIST", Type = typeof(ThemeList))]
    public class ThemeControl:Control
    {
        static ThemeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemeControl),
                new FrameworkPropertyMetadata(typeof(ThemeControl)));
        }
        /// <summary>
        /// Construtor do controle de Temas visuais
        /// </summary>
        public ThemeControl()
        {
            List<Theme> themes = new List<Theme>();
            Theme t1 = new Theme()
            {
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1)
            };
            Theme t2 = new Theme()
            {
                Background = new SolidColorBrush(Color.FromRgb(0,115,230)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 75, 150)),
                BorderThickness = new Thickness(1)
            };
            Theme t3 = new Theme()
            {
                Background = new SolidColorBrush(Color.FromRgb(37, 186, 0)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Color.FromRgb(49, 25, 125)),
                BorderThickness = new Thickness(1)
            };

            Theme t4 = new Theme()
            {
                Background = new SolidColorBrush(Color.FromRgb(172, 1, 1)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Color.FromRgb(115, 0, 0)),
                BorderThickness = new Thickness(1)
            };

            Theme t5 = new Theme()
            {
                Background = new SolidColorBrush(Color.FromRgb(244, 146, 6)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 130, 0)),
                BorderThickness = new Thickness(1)
            };

            Theme t6 = new Theme()
            {
                Background = new SolidColorBrush(Color.FromRgb(249, 240, 7)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Color.FromRgb(219, 211, 0)),
                BorderThickness = new Thickness(1)
            };

            Theme t7 = new Theme()
            {
                Background = new SolidColorBrush(Color.FromRgb(168, 13, 226)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Color.FromRgb(117, 0, 184)),
                BorderThickness = new Thickness(1)
            };

            Theme t8 = new Theme()
            {
                Background = new SolidColorBrush(Color.FromRgb(227, 13, 230)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Color.FromRgb(192, 1, 194)),
                BorderThickness = new Thickness(1)
            };

            themes.Add(t1);
            themes.Add(t2);
            themes.Add(t3);
            themes.Add(t4);
            themes.Add(t5);
            themes.Add(t6);
            themes.Add(t7);
            themes.Add(t8);


            this.Items = themes;
        }
        /// <summary>
        /// Elemento gráfico de layout (ScrollViewer) responsável pela rolagem dos temas.
        /// </summary>
        protected ScrollViewer PART_SCROLLVIEWER;
        /// <summary>
        /// Elemento gráfico de layout (Button) responsável pela rolagem dos temas.
        /// </summary>
        protected Button PART_LINEUP;
        /// <summary>
        /// Elemento gráfico de layout (Button) responsável pela rolagem dos temas.
        /// </summary>
        protected Button PART_LINEDOWN;
        /// <summary>
        /// Elemento gráfico de layout (ThemeList:ListBox) responsável pela rolagem dos temas.
        /// </summary>
        protected ThemeList PART_LIST;
        /// <summary>
        /// Elemento gráfico de layout (ThemeList:ListBox) responsável pela rolagem dos temas.
        /// </summary>
        protected ThemeList PART_POPUPLIST;

        /// <summary>
        /// Define e inicializa os elementos de UI necessários para a lógica de negócio.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_SCROLLVIEWER = GetTemplateChild("PART_SCROLLVIEWER") as ScrollViewer;
            PART_LINEUP = GetTemplateChild("PART_LINEUP") as Button;
            PART_LINEDOWN = GetTemplateChild("PART_LINEDOWN") as Button;
            PART_LIST = GetTemplateChild("PART_LIST") as ThemeList;
            PART_POPUPLIST = GetTemplateChild("PART_POPUPLIST") as ThemeList;

            if (PART_LINEUP != null) PART_LINEUP.Click += PART_LINEUP_Click;
            if (PART_LINEDOWN != null) PART_LINEDOWN.Click += PART_LINEDOWN_Click;
            if (PART_LIST != null) PART_LIST.ItemMouseEnter += PART_LIST_ItemMouseEnter;
            if (PART_LIST != null) PART_LIST.ItemMouseLeave += PART_LIST_ItemMouseLeave;
            if (PART_LIST != null) PART_LIST.ItemClick += PART_LIST_ItemClick;
            if (PART_POPUPLIST != null) PART_POPUPLIST.ItemMouseEnter += PART_LIST_ItemMouseEnter;
            if (PART_POPUPLIST != null) PART_POPUPLIST.ItemMouseLeave += PART_LIST_ItemMouseLeave;
            if (PART_POPUPLIST != null) PART_POPUPLIST.ItemClick += PART_LIST_ItemClick;
        }        

        object currentSelection;
        bool isMouseOver = false;
        private void PART_LIST_ItemMouseLeave(object sender, MouseEventArgs e)
        {
            if (AutoPreview && currentSelection != null)
            {
                SelectedItem = currentSelection;
                currentSelection = null;
            }
            isMouseOver = false;
        }

        private async void PART_LIST_ItemMouseEnter(object sender, MouseEventArgs e)
        {
            isMouseOver = true;
            await Task.Delay(500);
            if (AutoPreview && isMouseOver)
            {
                currentSelection = SelectedItem;
                SelectedItem = (sender as ThemeListItem).Content;
            }
        }

        private void PART_LIST_ItemClick(object sender, MouseButtonEventArgs e)
        {
            currentSelection = SelectedItem = (sender as ThemeListItem).Content;
        }

        private void PART_LINEDOWN_Click(object sender, RoutedEventArgs e)
        {
            PART_SCROLLVIEWER.ScrollToVerticalOffset(PART_SCROLLVIEWER.VerticalOffset + 70);
        }

        private void PART_LINEUP_Click(object sender, RoutedEventArgs e)
        {
            PART_SCROLLVIEWER.ScrollToVerticalOffset(PART_SCROLLVIEWER.VerticalOffset - 70);
        }

        /// <summary>
        /// Obtém ou define se o controle pode disparar eventos pré-visualização
        /// Valor Padrão: False
        /// </summary>
        public bool AutoPreview { get; set; }

        #region Dependency Properties

        /// <summary>
        /// Obtém ou define o número de itens a serem exibidos no controle.
        /// </summary>
        public static readonly DependencyProperty ItemsToShowProperty =
            DependencyProperty.Register("ItemsToShow", typeof(int),
            typeof(ThemeControl), new FrameworkPropertyMetadata(4));



        /// <summary>
        /// Obtém ou define o número de itens a serem exibidos no controle.
        /// </summary>
        public int ItemsToShow
        {
            get { return (int)GetValue(ItemsToShowProperty); }
            set { SetValue(ItemsToShowProperty, value); }
        }


        /// <summary>
        /// Obtém ou define uma lista de itens a serem exibidos no controle.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IList),
            typeof(ThemeControl), new FrameworkPropertyMetadata());

        
        
        /// <summary>
        /// Obtém ou define uma lista de itens a serem exibidos no controle.
        /// </summary>
        public IList Items
        {
            get { return (IList)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }



        /// <summary>
        /// Obtém ou define o item selecionado no controle.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object),
            typeof(ThemeControl), new FrameworkPropertyMetadata());

       
        
        /// <summary>
        /// Obtém ou define o item selecionado no controle.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }



        /// <summary>
        /// Obtém ou define o índice do item selecionado no controle.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int?),
            typeof(ThemeControl), new FrameworkPropertyMetadata(null));

        
        
        /// <summary>
        /// Obtém ou define o índice do item selecionado no controle.
        /// </summary>
        public int? SelectedIndex
        {
            get { return (int?)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }



        /// <summary>
        /// Obtém ou define o item de pré-visualização selecionado.
        /// </summary>
        public static readonly DependencyProperty PreviewSelectedItemProperty =
            DependencyProperty.Register("PreviewSelectedItem", typeof(object),
            typeof(ThemeControl), new FrameworkPropertyMetadata());

        /// <summary>
        /// Obtém ou define o item de pré-visualização selecionado.
        /// </summary>
        public object PreviewSelectedItem
        {
            get { return (object)GetValue(PreviewSelectedItemProperty); }
            private set { SetValue(PreviewSelectedItemProperty, value); }
        }
        #endregion
    }
}
