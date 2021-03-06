﻿// *********************************************************************
// PLEASE DO NOT REMOVE THIS DISCLAIMER
//
// PropertyGrid - By Jaime Olivares
// July 11, 2011
// Article site: http://www.codeproject.com/KB/grid/PropertyGrid.aspx
// Author site: www.jaimeolivares.com
// License: Code Project Open License (CPOL)
//
// *********************************************************************

using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using System.Xaml;
using System.Xml;

namespace System.Windows.Controls
{
    public enum PropertySort
    {
        NoSort = 0,
        Alphabetical = 1,
        Categorized = 2,
        CategorizedAlphabetical = 3
    };

    /// <summary>
    /// WPF Native PropertyGrid class, uses Workflow Foundation's PropertyInspector
    /// </summary>
    public class PropertyGrid : Grid
    {
        #region Private fields
        private WorkflowDesigner Designer;
        private MethodInfo RefreshMethod;
        private MethodInfo OnSelectionChangedMethod;
        private MethodInfo IsInAlphaViewMethod;
        private TextBlock SelectionTypeLabel;
        private Control PropertyToolBar;
        private Border HelpText;
        private GridSplitter Splitter;
        private double HelpTextHeight = 60;
        #endregion

        #region Public properties
        /// <summary>Get or sets the selected object. Can be null.</summary>
        public object SelectedObject
        {
            get { return GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }
        /// <summary>Get or sets the selected object collection. Returns empty array by default.</summary>
        public object[] SelectedObjects
        {
            get { return GetValue(SelectedObjectsProperty) as object[]; }
            set { SetValue(SelectedObjectsProperty, value); }
        }
        /// <summary>XAML information with PropertyGrid's font and color information</summary>
        /// <seealso>Documentation for WorkflowDesigner.PropertyInspectorFontAndColorData</seealso>
        public string FontAndColorData
        {
            set
            {
                Designer.PropertyInspectorFontAndColorData = value;                
            }
        }
        /// <summary>Shows the description area on the top of the control</summary>
        public bool HelpVisible
        {
            get { return (bool)GetValue(HelpVisibleProperty); }
            set { SetValue(HelpVisibleProperty, value); }
        }
        /// <summary>Shows the tolbar on the top of the control</summary>
        public bool ToolbarVisible
        {
            get { return (bool)GetValue(ToolbarVisibleProperty); }
            set { SetValue(ToolbarVisibleProperty, value); }
        }
        public PropertySort PropertySort
        {
            get { return (PropertySort)GetValue(PropertySortProperty); }
            set { SetValue(PropertySortProperty, value); }
        }
        

        #endregion

        #region Dependency properties registration
        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedObjectPropertyChanged));

        public static readonly DependencyProperty SelectedObjectsProperty =
            DependencyProperty.Register("SelectedObjects", typeof(object[]), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(new object[0], FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedObjectsPropertyChanged, CoerceSelectedObjects));

        public static readonly DependencyProperty HelpVisibleProperty =
            DependencyProperty.Register("HelpVisible", typeof(bool), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HelpVisiblePropertyChanged));
        public static readonly DependencyProperty ToolbarVisibleProperty =
            DependencyProperty.Register("ToolbarVisible", typeof(bool), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ToolbarVisiblePropertyChanged));
        public static readonly DependencyProperty PropertySortProperty =
            DependencyProperty.Register("PropertySort", typeof(PropertySort), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(PropertySort.CategorizedAlphabetical, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertySortPropertyChanged));
        #endregion

        #region Dependency properties events
        private static object CoerceSelectedObject(DependencyObject d, object value)
        {
            PropertyGrid pg = d as PropertyGrid;

            object[] collection = pg.GetValue(SelectedObjectsProperty) as object[];

            return collection.Length == 0 ? null : value;
        }
        private static object CoerceSelectedObjects(DependencyObject d, object value)
        {
            PropertyGrid pg = d as PropertyGrid;

            object single = pg.GetValue(SelectedObjectsProperty);

            return single == null ? new object[0] : value;
        }

        private static void SelectedObjectPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid pg = source as PropertyGrid;
            pg.CoerceValue(SelectedObjectsProperty);

            if (e.NewValue == null)
            {
                pg.OnSelectionChangedMethod.Invoke(pg.Designer.PropertyInspectorView, new object[] { null });
                pg.SelectionTypeLabel.Text = string.Empty;
            }
            else
            {
                var context = new EditingContext();
                var mtm = new ModelTreeManager(context);
                mtm.Load(e.NewValue);
                Selection selection = Selection.Select(context, mtm.Root);

                pg.OnSelectionChangedMethod.Invoke(pg.Designer.PropertyInspectorView, new object[] { selection });
                pg.SelectionTypeLabel.Text = e.NewValue.GetType().Name;
            }

            pg.ChangeHelpText(string.Empty, string.Empty);
        }
        private static void SelectedObjectsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid pg = source as PropertyGrid;
            pg.CoerceValue(SelectedObjectsProperty);

            object[] collection = e.NewValue as object[];

            if (collection.Length == 0)
            {
                pg.OnSelectionChangedMethod.Invoke(pg.Designer.PropertyInspectorView, new object[] { null });
                pg.SelectionTypeLabel.Text = string.Empty;
            }
            else
            {
                bool same = true;
                Type first = null;

                var context = new EditingContext();
                var mtm = new ModelTreeManager(context);
                Selection selection = null;

                // Accumulates the selection and determines the type to be shown in the top of the PG
                for (int i = 0; i < collection.Length; i++)
                {
                    mtm.Load(collection[i]);
                    if (i == 0)
                    {
                        selection = Selection.Select(context, mtm.Root);
                        first = collection[0].GetType();
                    }
                    else
                    {
                        selection = Selection.Union(context, mtm.Root);
                        if (!collection[i].GetType().Equals(first))
                            same = false;
                    }
                }

                pg.OnSelectionChangedMethod.Invoke(pg.Designer.PropertyInspectorView, new object[] { selection });
                pg.SelectionTypeLabel.Text = same ? first.Name + " <multiple>" : "Object <multiple>";
            }

            pg.ChangeHelpText(string.Empty, string.Empty);
        }
        private static void HelpVisiblePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid pg = source as PropertyGrid;

            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue.Equals(true))
                {
                    pg.RowDefinitions[1].Height = new GridLength(5);
                    pg.RowDefinitions[2].Height = new GridLength(pg.HelpTextHeight);
                }
                else
                {
                    pg.HelpTextHeight = pg.RowDefinitions[2].Height.Value;
                    pg.RowDefinitions[1].Height = new GridLength(0);
                    pg.RowDefinitions[2].Height = new GridLength(0);
                }
            }
        }
        private static void ToolbarVisiblePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid pg = source as PropertyGrid;
            pg.PropertyToolBar.Visibility = e.NewValue.Equals(true) ? Visibility.Visible : Visibility.Collapsed;
        }
        private static void PropertySortPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid pg = source as PropertyGrid;
            PropertySort sort = (PropertySort)e.NewValue;

            bool isAlpha = (sort == PropertySort.Alphabetical || sort == PropertySort.NoSort);
            pg.IsInAlphaViewMethod.Invoke(pg.Designer.PropertyInspectorView, new object[] { isAlpha });
        }
        #endregion

        /// <summary>Default constructor, creates the UIElements including a PropertyInspector</summary>
        public PropertyGrid()
        {
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0) });
            this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0) });

            this.Designer = new WorkflowDesigner();
            TextBlock title = new TextBlock()
            {
                Visibility = Windows.Visibility.Visible,
                TextWrapping = TextWrapping.NoWrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
                FontWeight = FontWeights.Bold
            };
            TextBlock descrip = new TextBlock()
            {
                Visibility = Windows.Visibility.Visible,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            DockPanel dock = new DockPanel()
            {
                Visibility = Windows.Visibility.Visible,
                LastChildFill = true,
                Margin = new Thickness(3, 0, 3, 0)
            };

            title.SetValue(DockPanel.DockProperty, Dock.Top);
            dock.Children.Add(title);
            dock.Children.Add(descrip);
            this.HelpText = new Border()
            {
                Visibility = Windows.Visibility.Visible,
                BorderBrush = SystemColors.ActiveBorderBrush,
                Background = SystemColors.ControlBrush,
                BorderThickness = new Thickness(1),
                Child = dock
            };
            this.Splitter = new GridSplitter()
            {
                Visibility = Windows.Visibility.Visible,
                ResizeDirection = GridResizeDirection.Rows,
                Height = 5,
                HorizontalAlignment = Windows.HorizontalAlignment.Stretch
            };

            var inspector = Designer.PropertyInspectorView;
            inspector.Visibility = Visibility.Visible;



            string rd = @"<ResourceDictionary
                            x:Uid='ResourceDictionary_1' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                            xmlns:sap='clr-namespace:System.Activities.Presentation;assembly=System.Activities.Presentation'
                            xmlns:sapv='clr-namespace:System.Activities.Presentation.View;assembly=System.Activities.Presentation'>
 
                             <SolidColorBrush x:Uid ='SolidColorBrush_01' x:Key='{x:Static sap: WorkflowDesignerColors.PropertyInspectorBorderBrushKey}' Color='White' />
        
                           </ResourceDictionary>";


            StringReader reader  = new StringReader(rd);
            XmlReader xmlReader = XmlReader.Create(reader);
            var fontAndColorDictionary = (ResourceDictionary)(System.Windows.Markup.XamlReader.Load(xmlReader));
            Hashtable hashTable = new Hashtable();
            foreach(var key in fontAndColorDictionary.Keys)
            {
                hashTable.Add(key, fontAndColorDictionary[key]);
            }
            Designer.PropertyInspectorFontAndColorData = XamlServices.Save(hashTable);

            inspector.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);

            this.Splitter.SetValue(Grid.RowProperty, 1);
            this.Splitter.SetValue(Grid.ColumnProperty, 0);

            this.HelpText.SetValue(Grid.RowProperty, 2);
            this.HelpText.SetValue(Grid.ColumnProperty, 0);

            Binding binding = new Binding("Parent.Background");
            title.SetBinding(BackgroundProperty, binding);
            descrip.SetBinding(BackgroundProperty, binding);

            this.Children.Add(inspector);
            this.Children.Add(this.Splitter);
            this.Children.Add(this.HelpText);

            Type inspectorType = inspector.GetType();
            var props = inspectorType.GetProperties(Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Instance |
                Reflection.BindingFlags.DeclaredOnly);

            var methods = inspectorType.GetMethods(Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Instance |
                Reflection.BindingFlags.DeclaredOnly);

            this.RefreshMethod = inspectorType.GetMethod("RefreshPropertyList",
                Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Instance | Reflection.BindingFlags.DeclaredOnly);
            this.IsInAlphaViewMethod = inspectorType.GetMethod("set_IsInAlphaView",
                Reflection.BindingFlags.Public | Reflection.BindingFlags.Instance | Reflection.BindingFlags.DeclaredOnly);
            this.OnSelectionChangedMethod = inspectorType.GetMethod("OnSelectionChanged",
                Reflection.BindingFlags.Public | Reflection.BindingFlags.Instance | Reflection.BindingFlags.DeclaredOnly);
            this.SelectionTypeLabel = inspectorType.GetMethod("get_SelectionTypeLabel",
                Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Instance |
                Reflection.BindingFlags.DeclaredOnly).Invoke(inspector, new object[0]) as TextBlock;
            this.PropertyToolBar = inspectorType.GetMethod("get_PropertyToolBar",
                Reflection.BindingFlags.Public | Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Instance |
                Reflection.BindingFlags.DeclaredOnly).Invoke(inspector, new object[0]) as Control;
            inspectorType.GetEvent("GotFocus").AddEventHandler(this,
                Delegate.CreateDelegate(typeof(RoutedEventHandler), this, "GotFocusHandler", false));

            this.SelectionTypeLabel.Text = string.Empty;

            var p = Designer.PropertyInspectorView;
            var k = p;
        }
        /// <summary>Updates the PropertyGrid's properties</summary>
        public void RefreshPropertyList()
        {
            RefreshMethod.Invoke(Designer.PropertyInspectorView, new object[] { false });
        }

        /// <summary>Traps the change of focused property and updates the help text</summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Points to the source control containing the selected property</param>
        private void GotFocusHandler(object sender, RoutedEventArgs args)
        {
            //if (args.OriginalSource is TextBlock)
            //{
            string title = string.Empty;
            string descrip = string.Empty;
            var theSelectedObjects = this.GetValue(SelectedObjectsProperty) as object[];

            if (theSelectedObjects != null && theSelectedObjects.Length > 0)
            {
                Type first = theSelectedObjects[0].GetType();
                for (int i = 1; i < theSelectedObjects.Length; i++)
                {
                    if (!theSelectedObjects[i].GetType().Equals(first))
                    {
                        ChangeHelpText(title, descrip);
                        return;
                    }
                }

                object data = (args.OriginalSource as FrameworkElement).DataContext;
                PropertyInfo propEntry = data.GetType().GetProperty("PropertyEntry");
                if (propEntry == null)
                {
                    propEntry = data.GetType().GetProperty("ParentProperty");
                }

                if (propEntry != null)
                {
                    object propEntryValue = propEntry.GetValue(data, null);
                    string propName = propEntryValue.GetType().GetProperty("PropertyName").GetValue(propEntryValue, null) as string;
                    title = propEntryValue.GetType().GetProperty("DisplayName").GetValue(propEntryValue, null) as string;
                    PropertyInfo property = theSelectedObjects[0].GetType().GetProperty(propName);
                    object[] attrs = property.GetCustomAttributes(typeof(DescriptionAttribute), true);

                    if (attrs != null && attrs.Length > 0)
                        descrip = (attrs[0] as DescriptionAttribute).Description;
                }
                ChangeHelpText(title, descrip);
            }
            //}
        }
        /// <summary>Changes the text help area contents</summary>
        /// <param name="title">Title in bold</param>
        /// <param name="descrip">Description with ellipsis</param>
        private void ChangeHelpText(string title, string descrip)
        {
            DockPanel dock = this.HelpText.Child as DockPanel;
            (dock.Children[0] as TextBlock).Text = title;
            (dock.Children[1] as TextBlock).Text = descrip;
        }
    }
}
