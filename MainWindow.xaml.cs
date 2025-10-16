using AdminWPF.Models;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        struct PInfoWrapper
        {
            public string Name;
            public PropertyInfo PropertyInfo;
        }
        private BookCatalogContext _dbContext;
        private List<PInfoWrapper> _dbContextTableProperties;
        public MainWindow()
        {
            InitializeComponent();

            //never closed (not ideal)
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

            Binding b = new Binding();
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath("."); // took way to long
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b.NotifyOnSourceUpdated = true;
            b.NotifyOnTargetUpdated = true;
            BindingOperations.SetBinding(RecordGrid, ListBox.ItemsSourceProperty, b);
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbTableSelect.ItemsSource = _dbContextTableProperties;    
            lbTableSelect.DisplayMemberPath = "";

            _dbContext.Books.Load();
            _dbContext.Authors.Load();
            RecordGrid.DataContext = _dbContext.Books.Local.ToObservableCollection();
        }

        private void lbTableSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.SelectedItem is not PInfoWrapper) return;

            var selected = (PInfoWrapper)lb.SelectedItem;

            dynamic dbSet = selected.PropertyInfo.GetValue(_dbContext);

            var dbSetType = selected.PropertyInfo.PropertyType.GetGenericArguments()[0];

            Type t = typeof(DbSet<>).MakeGenericType(dbSetType);

            var a = dbSet.GetType();
            //System.Object.ReferenceEquals(this._dbContext.Authors, dbSet) ==> ture ????
            //dbSet.Load(); //tf u mean does not contain definition for Load()

            RecordGrid.DataContext = dbSet.Local.ToObservableCollection();
            
            //RecordGrid.ItemsSource = (()item.GetValue(this._dbContext)).ToList();
        }

        private void RecordGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
        }
        private void RecordGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
        }

        private void RecordGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //_dbContext.Books.Remove(_dbContext.Books.First());
        }
    }
}