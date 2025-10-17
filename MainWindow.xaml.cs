using AdminWPF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //bullshit for databinding 
        [ValueConversion(typeof(int), typeof(bool))]
        public class IntToBoolConvert : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (int)value > 0;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool)
                {
                    return (bool)value ? 1 : 0;
                }
                return DependencyProperty.UnsetValue;
            }
        }
        public class NullToBoolConvert : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value != null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                //dont give a flying fuck
                return DependencyProperty.UnsetValue;
            }
        }
        
        public class PInfoWrapper
        {
            public string Name { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }

        private BookCatalogContext _dbContext;
        private List<PInfoWrapper> _dbContextTableProperties;
        public MainWindow()
        {
            InitializeComponent();

            //this should not be used like this but we ball
            _dbContext = new BookCatalogContext();

            _dbContextTableProperties = this._dbContext
                .GetType()
                .GetProperties()
                .Where(p => 
                    p.PropertyType.IsGenericType 
                        && 
                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => new PInfoWrapper{ Name = p.Name, PropertyInfo = p})
                .ToList();

            //binding for the records grid
            BindingOperations.SetBinding(RecordGrid, ListBox.ItemsSourceProperty, new Binding()
            {
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath("."),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

            //binding the delete button's IsEnabled to true when there is at least one selected item, false otherwise
            btn_Delete.DataContext = RecordGrid;
            BindingOperations.SetBinding(btn_Delete, Button.IsEnabledProperty, new Binding()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("SelectedItems.Count"),
                Converter = new IntToBoolConvert(),
            });

            btn_Save.DataContext = RecordGrid;
            BindingOperations.SetBinding(btn_Save, Button.IsEnabledProperty, new Binding()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("DataContext"),
                Converter = new NullToBoolConvert(),
            });


            if (_dbContext.Database.CanConnect() == false)
            {
                MessageBox.Show("Could not connect to database :/");
                Application.Current.Shutdown();
            }

            //real nasty stuff 
            //preloading all tables
            //pls look away 
            foreach (var p in _dbContextTableProperties)
            {
                dynamic dbSet = p.PropertyInfo.GetValue(_dbContext);
                if (dbSet == null) throw new Exception();
                EntityFrameworkQueryableExtensions.Load(dbSet);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbTableSelect.ItemsSource = _dbContextTableProperties;    
            lbTableSelect.DisplayMemberPath = "Name";
        }

        private void lbTableSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecordGrid.Visibility = Visibility.Visible;
            ListBox lb = (ListBox)sender;
            if (lb.SelectedItem is not PInfoWrapper) throw new Exception("SelectedItem must be of type PInfoWrapper");

            var selected = (PInfoWrapper)lb.SelectedItem;
            dynamic dbSet = selected.PropertyInfo.GetValue(_dbContext);
            if (dbSet == null) throw new Exception();

            EntityFrameworkQueryableExtensions.Load(dbSet);
            RecordGrid.DataContext = dbSet.Local.ToObservableCollection();
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            object[] itemsToRemove = new object[RecordGrid.SelectedItems.Count];
            RecordGrid.SelectedItems.CopyTo(itemsToRemove,0);
            foreach (var item in itemsToRemove)
            {
                _dbContext.Remove(item);
            }
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                _dbContext.SaveChanges();
                MessageBox.Show("Saved changes");
                tbLastSave.Text = "Last save\n" + DateTime.Now.ToString(); //10pm, give me a break
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving", MessageBoxButton.OK ,MessageBoxImage.Error);
            }
        }
    }
}