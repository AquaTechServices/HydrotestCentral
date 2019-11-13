using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using HydrotestCentral.ViewModels;
using HydrotestCentral.Model;

namespace HydrotestCentral
{
    /// <summary>
    /// Interaction logic for QuoteItemGrid.xaml
    /// </summary>
    public partial class QuoteItemGrid: UserControl
    {
        public SQLiteConnection connection;
        public SQLiteDataAdapter dataAdapter;
        public string item_placeholder;
        //public QuoteItemsDataProvider quote_items;
        public static string jobno;
        public static int tab_index;
        public MainWindowViewModel other_vm;

        public QuoteItemGrid(MainWindowViewModel vm)
        {
            InitializeComponent();
            
            //this.DataContext = vm;
            this.QItems.ItemsSource = vm.quote_items;

            foreach(QuoteItem x in vm.quote_items)
            {
                Console.WriteLine(x.item.ToString() + " IN VM.QUOTE_ITEMS");
            }
        }

        public QuoteItemGrid(string jobno_in, int tab_index_in)
        {
            InitializeComponent();

            jobno = jobno_in;
            tab_index = tab_index_in;
        }

        public void RefreshGrid()
        {
            this.QItems.Items.Refresh();
        }

        private void UpdateQuoteItemGrid()
        {
            Console.WriteLine("Cell edit\n");
        }

        private void QItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //e.Row.Item.Text.ToString();
            Console.WriteLine("CELL EDIT - Row: " + e.Row.GetIndex() + " edited\n");

            try
            {
                //
                //quote_items.saveItemsToDB();
                //UpdateCurrentQuoteDashboard();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }
    }

    public class DataGridComboBoxColumnWithBindingHack : DataGridComboBoxColumn
    {
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            FrameworkElement element = base.GenerateEditingElement(cell, dataItem);
            CopyItemsSource(element);
            return element;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            FrameworkElement element = base.GenerateElement(cell, dataItem);
            CopyItemsSource(element);
            return element;
        }

        private void CopyItemsSource(FrameworkElement element)
        {
            BindingOperations.SetBinding(element, ComboBox.ItemsSourceProperty,
              BindingOperations.GetBinding(this, ComboBox.ItemsSourceProperty));
        }
    }

    public class Item: INotifyPropertyChanged
    {
        string _itemname;
        string _descr;
        double _rate;

        public Item(string name, string descr, double rate)
        {
            _itemname = name;
            _descr = descr;
            _rate = rate;
        }

        public string Itemname
        {
            get
            {
                return _itemname;
            }
            set
            {
                if(_itemname != value)
                {
                    _itemname = value;
                    NotifyPropertyChanged("Itemname");
                }
            }
        }

        public string Descr
        {
            get
            {
                return _descr;
            }
            set
            {
                if (_descr != value)
                {
                    _descr = value;
                    NotifyPropertyChanged("Descr");
                }
            }
        }

        public double Rate
        {
            get
            {
                return _rate;
            }
            set
            {
                if(_rate != value)
                {
                    _rate = value;
                    NotifyPropertyChanged("Rate");
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
