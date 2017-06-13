using Archidata.Core.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Archidata.ViewModel
{
    public class ProjectViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<ItemViewModel> _items;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                if (value != null && value.Count > 0 && SelectedItem == null)
                    SelectedItem = value[0];
            }
        }
        private ItemViewModel selectedItem;
        public ItemViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public ProjectViewModel()
        {
            ItemViewModel.CloseRequested += ItemViewModel_CloseRequested;

        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ItemViewModel_CloseRequested(object sender, EventArgs e)
        {
            if (Items != null)
            {
                ItemViewModel item = (sender as ItemViewModel);
                Close(item);               
            }
        }

        private void Close(ItemViewModel item)
        {
            if (((IUIPlugin)item.Container).SaveCommand.CanExecute(null))
            {
                MessageBoxResult r = MessageBox.Show(
                    "Deseja salvar as alterações em " + item.File.FileName + "?\n\n" +
                    "Se você clicar em 'Não' todo o progresso feito será perdido.",
                    "Archidata",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question
                );

                if (r == MessageBoxResult.Yes)
                {
                    ((IUIPlugin)(item).Container).SaveCommand.Execute(null);
                    Items.Remove(item);
                }
                else if (r == MessageBoxResult.No)
                {
                    Items.Remove(item);
                }
            }
            else
            {
                Items.Remove(item);
            }
        }

        public void CloseAll()
        {
            var items = Items.ToList();
            foreach (var item in items)
            {
                Close(item);
            }
        }
    }

    public class ItemViewModel
    {
        public ProjectFile File { get; set; }
        private UIPlugin _container;
        public FrameworkElement Container
        {
            get
            {
                if(_container == null)
                {
                    _container = (UIPlugin)App.Extensions.GetControlInstanceByExtension(File.FileExtension);

                    if(_container == null)
                    {
                        return _container;
                    }

                    _container.Open(File.FilePath);
                    Binding b = new Binding("SelectedItem") { ElementName = "ThemeMenu" };
                    _container.SetBinding(UIPlugin.ThemeProperty, b);                    
                }
                return _container;
            }
            set { }
        }

        CloseCommand close_cmd = new CloseCommand();

        public CloseCommand CloseCommand
        {
            get { return close_cmd; }
            set { close_cmd = value; }
        }

        public static event EventHandler CloseRequested;

        internal void Close()
        {
            if (CloseRequested != null)
                CloseRequested(this, null);
        }
    }

    public class CloseCommand : ICommand
    {
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
#pragma warning disable CS0067 // The event 'CloseCommand.CanExecuteChanged' is never used
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // The event 'CloseCommand.CanExecuteChanged' is never used
        public void Execute(object parameter)
        {
            var item = parameter as ItemViewModel;
            if (item != null)
            {
                item.Close();
            }
        }
        #endregion
    }
}
