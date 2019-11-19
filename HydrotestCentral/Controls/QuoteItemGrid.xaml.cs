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
using System.Diagnostics;

namespace HydrotestCentral
{
    /// <summary>
    /// Interaction logic for QuoteItemGrid.xaml
    /// </summary>
    public partial class QuoteItemGrid: UserControl
    {
        public static string jobno;
        public static int tab_index;

        public QuoteItemGrid(string jobno_in, int tab_index_in, MainWindowViewModel vm)
        {
            InitializeComponent();

            jobno = jobno_in;
            tab_index = tab_index_in;

            this.DataContext = vm;
            //MessageBox.Show(vm.ToString());
            //this.QItems.ItemsSource = vm.quote_items;

            foreach(QuoteItem x in vm.quote_items)
            {
                //MessageBox.Show(x.ToString());
                Trace.WriteLine(x.item.ToString() + " IN VM.QUOTE_ITEMS");
            }
        }

        private void QItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //e.Row.Item.Text.ToString();
            Trace.WriteLine("CELL EDIT - Row: " + e.Row.GetIndex() + " edited\n");

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

        private void QItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
    }


    // 11/14/19 - Currently this is not being used
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

}
