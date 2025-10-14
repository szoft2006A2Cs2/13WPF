using AdminWPF.Models;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
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
        private BookCatalogContext _dbContext;
        private List<PropertyInfo> _dbContextTableProperties;
        public MainWindow()
        {
            InitializeComponent();

            //never closed (not ideal)
            _dbContext = new BookCatalogContext();
            _dbContextTableProperties = this._dbContext.GetType().GetProperties().Where(p => p.PropertyType.Name.Contains("DbSet")).ToList();

            RecordGrid.DataContext = _dbContext;
            lbTableSelect.DataContext = _dbContextTableProperties;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lbTableSelect.ItemsSource = _dbContextTableProperties;    
        }

        private void lbTableSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyInfo item = (PropertyInfo)((ListBox)sender).SelectedItem;
            Type itemType = item.PropertyType.GenericTypeArguments[0]; 
            
            RecordGrid.ItemsSource = (()item.GetValue(this._dbContext)).ToList();
        }
    }
}