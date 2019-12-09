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
using HydrotestCentral.Models;
using System.Diagnostics;

namespace HydrotestCentral
{
    /// <summary>
    /// Interaction logic for QuoteItemGrid.xaml
    /// </summary>
    public partial class QuoteItemGrid : UserControl
    {
        public static string jobno;
        public static int tab_index;
        public static MainWindowViewModel main_vm;

        private QuoteItem quoteItemBeingEdited;

        public QuoteItemGrid(string jobno_in, int tab_index_in, MainWindowViewModel vm)
        {
            InitializeComponent();

            jobno = jobno_in;
            tab_index = tab_index_in;

            main_vm = vm;
            this.DataContext = main_vm;

            //MessageBox.Show(vm.ToString());
            //this.QItems.ItemsSource = vm.quote_items;

            foreach (QuoteItem x in vm.quote_items)
            {
                //MessageBox.Show(x.ToString());
                //Trace.WriteLine(x.item.ToString() + " IN VM.QUOTE_ITEMS");
            }
        }

        private void QItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Trace.WriteLine("CELL EDIT - Row: " + e.Row.GetIndex() + " edited\n");
            quoteItemBeingEdited = e.Row.Item as QuoteItem;
        }


        private void QItems_CurrentCellChanged(object sender, EventArgs e)
        {
            if (quoteItemBeingEdited != null)
            {
                //MessageBox.Show(quoteHeaderBeingEdited.jobno + " is now being updated in the database!");
                main_vm.saveTabItemGrid(jobno, tab_index);
                Trace.WriteLine("saved in MainWindowViewModel");
                quoteItemBeingEdited = null;

                //QItems.Items.Refresh();
            }
        }

        private void QItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            main_vm.selected_row_index = QItems.SelectedIndex;
            //main_vm.quoteItemToAdd = (QuoteItem)QItems.SelectedItem;
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

        //private void FieldDataGridChecked(object sender, RoutedEventArgs e)
        //{
        //    foreach (FieldViewModel model in _fields)
        //    {
        //        model.IsChecked = true;
        //    }
        //}

        //private void FieldDataGridUnchecked(object sender, RoutedEventArgs e)
        //{
        //    foreach (FieldViewModel model in _fields)
        //    {
        //        model.IsChecked = false;
        //    }
        //}

    }

}
