﻿using System;
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
using MahApps.Metro.Controls;
using System.Data;
using System.Data.SQLite;
//using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using HydrotestCentral.ViewModels;
using HydrotestCentral.Model;

namespace HydrotestCentral
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public SQLiteConnection connection;
        public SQLiteDataAdapter head_dataAdapter, items_dataAdapter;
        public System.Data.DataTable head_dt, items_dt;
        public SQLiteCommandBuilder head_builder, items_builder;
        public string jobno, cust;
        public double proj_daily_total, proj_addn_chg, proj_job_total, est_days;
        //public QuoteHeaderDataProvider quote_heads;
        //public QuoteItemsDataProvider quote_items;
        private List<TabItem> _tabItems;
        private List<string> _tabNames;
        private TabItem _tabAdd;

        private QuoteHeader quoteHeaderBeingEdited;

        public static MainWindowViewModel main_vm;

        public MainWindow()
        {
            InitializeComponent();

            // Set the ViewModel
            main_vm = new MainWindowViewModel();
            DataContext = main_vm;

            // initialize tabItem array
            _tabItems = new List<TabItem>();

            // add a tabItem with + in header 
            TabItem tabAdd = new TabItem();
            tabAdd.Header = "+";
            _tabItems.Add(tabAdd);

            //Find how many day tabs there should be
            this.AddTabItem();

            // bind tab control
            tabDynamic.DataContext = _tabItems;

            tabDynamic.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private TabItem AddTabItemByName(string name)
        {
            int count = _tabItems.Count;

            // Create new Tab
            TabItem tab = new TabItem();
            tab.Header = string.Format(name);
            tab.Name = string.Format(name);
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            // Insert Content Here

            // insert tab item right before the last (+) tab item
            _tabItems.Insert(count - 1, tab);

            return tab;
        }

        private TabItem AddTabItem()
        {
            int count = _tabItems.Count;

            // create new tab item
            TabItem tab = new TabItem();
            tab.Header = string.Format("Day {0}", count);
            tab.Name = string.Format("tab{0}", count - 1);
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            // add controls to tab item, this case I added just a textbox
            getTabItemGrid(tab, count - 1);

            // insert tab item right before the last (+) tab item
            _tabItems.Insert(count - 1, tab);
            return tab;
        }

        private void QHeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get selected Row
            //DataRowView row = (DataRowView) QHeader.SelectedItem;
            //MessageBox.Show(row.Row["jobno"].ToString());
            if (QHeader.SelectedItem != null)
            {
                QuoteHeader temp = (QuoteHeader)QHeader.SelectedItem;
                //MessageBox.Show("QHeader Selection Changed...JobNo now: " + temp.jobno);

                // Get selected Row cell base on which the datagrid will be changed
                try
                {
                    this.jobno = temp.jobno;
                    this.cust = temp.cust;
                    this.est_days = temp.days_est;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }


                //Check if everything is OK
                if (jobno == null || jobno == string.Empty)
                {
                    return;
                }
                else
                {
                    //Change QItems based on Row
                    // OUTDATED ---GetQuoteItemsData(this.jobno);
                    main_vm.updateQuoteItemsByJob(this.jobno);

                    // Update tab child
                    //  MessageBox.Show(this.tabDynamic.GetChildObjects().ToString());
                    getTabItemGrid((TabItem)tabDynamic.SelectedItem, tabDynamic.SelectedIndex);
                }
            }
        }

        public void GetQuoteHeaderData()
        {
            QHeader.ItemsSource = main_vm.quote_headers;
        }

        public int GetNumberOfTabIndex(string jobno)
        {
            //return quote_items.getCountOfTabItems(jobno);

            return 3;
        }

        public void getTabItemGrid(TabItem tab, int tab_index)
        {
            //QuoteItemGrid grid = new QuoteItemGrid(quote_items, this.jobno, tab_index);
            QuoteItemGrid grid = new QuoteItemGrid(main_vm);
            
            //MessageBox.Show("Getting Tab Index: " + TabIndex);
            main_vm.updateQuoteItemsByJob_And_Tab(jobno, tab_index);
            grid.QItems.ItemsSource = main_vm.quote_items;

            tab.Content = grid;
        }

        public void updateTabItemGrid(TabItem tab, int tab_index)
        {
            Console.WriteLine("update tab item grid");
        }

        public void deleteTabItemGrid(TabItem tab, int tab_index)
        {
            main_vm.DeleteQuoteItem(jobno, tab_index);
            Console.WriteLine(string.Format("tab {0} deleted\n", tab_index + 1));
        }

        private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabDynamic.SelectedItem as TabItem;

            if (tab != null && tab.Header != null)
            {
                if (tab.Header.Equals("+"))
                {
                    // clear tab control binding
                    tabDynamic.DataContext = null;

                    // add new tab
                    TabItem newTab = this.AddTabItem();

                    // bind tab control
                    tabDynamic.DataContext = _tabItems;


                    // select newly added tab item
                    tabDynamic.SelectedItem = newTab;
                }
                else
                {
                    //MessageBox.Show("Selected Tab Index: " + tabDynamic.SelectedIndex.ToString());
                    getTabItemGrid(tab, tabDynamic.SelectedIndex);
                }
            }
        }


        private void Btn_DeleteQuoteHeader_Click(object sender, RoutedEventArgs e)
        {
            main_vm.DeleteHeaderItem(jobno);
        }

        private void Btn_SaveQuoteHeader_Click(object sender, RoutedEventArgs e)
        {
            main_vm.UpdateHeaderItem(jobno);
        }

        private void QHeader_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            quoteHeaderBeingEdited = e.Row.Item as QuoteHeader;

            MessageBox.Show(quoteHeaderBeingEdited.jobno + " has changes to be saved!");
        }

        private void QHeader_CurrentCellChanged(object sender, EventArgs e)
        {
            if(quoteHeaderBeingEdited != null)
            {
                MessageBox.Show(quoteHeaderBeingEdited.jobno + " is now being updated in the database!");
                main_vm.UpdateHeaderItem(quoteHeaderBeingEdited.jobno);
                quoteHeaderBeingEdited = null;
            }
        }

        public void UpdateQuoteItems_Row(DataGrid datagrid, int tab_index, int row_index)
            {
            string job = this.jobno;

            try
            {
                connection.Open();
                Console.WriteLine("Connection String:" + connection.ConnectionString.ToString());
                SQLiteCommand cmd = connection.CreateCommand();
                cmd.Parameters.Add(new SQLiteParameter("@jobno", jobno));
                cmd.Parameters.Add(new SQLiteParameter("@tabindex", tab_index));
                cmd.Parameters.Add(new SQLiteParameter("@rowindex", row_index));
                /*
                cmd.Parameters.Add(new SQLiteParameter("@qty", qty));
                cmd.Parameters.Add(new SQLiteParameter("@item", item));
                cmd.Parameters.Add(new SQLiteParameter("@rate", rate));
                cmd.Parameters.Add(new SQLiteParameter("@descr", descr));
                cmd.Parameters.Add(new SQLiteParameter("@group", group));
                cmd.Parameters.Add(new SQLiteParameter("@taxable", taxable));
                cmd.Parameters.Add(new SQLiteParameter("@discountable", discountable));
                cmd.Parameters.Add(new SQLiteParameter("@printable", printable));
                // Calculate line total

                cmd.Parameters.Add(new SQLiteParameter("@line_total", line_total));
                // Calculate tax total

                cmd.Parameters.Add(new SQLiteParameter("@tax_total", tax_total));
                */
                cmd.CommandText = string.Format("UPDATE QTE_ITEMS, SET WHERE jobno=(@jobno) AND tab_index=(@tabindex) AND row_index=(@rowindex)");
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
                Console.WriteLine("Unedited:" + items_dt.Rows.Count.ToString());
                adapter.Update(items_dt);
                Console.WriteLine("Edited:" + items_dt.Rows.Count.ToString());
                connection.Close();
            }
            catch (Exception Ex)
            {
                System.Windows.MessageBox.Show(Ex.Message);
            }
        }

        public void QItems_EditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //e.Row.Item.Text.ToString();
            Console.WriteLine("Row: " + e.Row.GetIndex() + " edited\n");

            try
            {
                //quote_items.UpdateLineTotals();
                //quote_items.saveItemsToDB();
                //UpdateCurrentQuoteDashboard();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        public bool checkFilename(string sourceFolder, string filename)
        {
            if (!string.IsNullOrEmpty(filename) && filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0)
            {
                return true;
            }
            else { return false; }
        }

        private void Btn_NewQuote_Click(object sender, RoutedEventArgs e)
        {
            NewQuoteWindow NQ_Win = new NewQuoteWindow();
            NQ_Win.Show();
        }

        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {
           /* DataTable dt = new DataTable();
            int days_count = 0;
            int sheet_count = 0;

            //dt = quote_heads.getQuoteHeaderTableByJob(jobno);
            dt = new DataTable();

            Console.WriteLine(dt.Rows[0]["jobno"].ToString() + " DataTable Created...");

            var excelApp = new Excel.Application();
            var excelWB = excelApp.ActiveWorkbook;
            string xl_path = txt_path.Text;
            string pdf_path = txt_path.Text;
            string quote_form = string.Format("C:\\Users\\SFWMD\\Aqua-Tech Hydro Services\\IT - Documents\\7.7 Projects\\HydrotestCentral\\BlankQuoteFormV2.xlsx");

            // Make the object visible
            excelApp.Visible = false;

            try
            {
                excelWB = excelApp.Workbooks.Open(quote_form);
                // Get a Total Count of tabs in worksheet
                sheet_count = excelWB.Sheets.Count;
                Console.WriteLine("Sheet Count: " + sheet_count.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            // Get a Total Count of Days tabs in DataCentral
            days_count = _tabItems.Count - 4;
            Console.WriteLine("Days Count: " + days_count.ToString());

            // If there are more days in DataCentral than in the worksheet, clone the days tab that many times
            int tabs_to_add = days_count - (sheet_count - 11);
            Console.WriteLine("Need to add " + tabs_to_add + " days...");

            while (tabs_to_add > 0)
            {
                Excel._Worksheet day1 = excelWB.Sheets["Day 1"];
                // Create new worksheet after day1
                Excel.Worksheet newWS;
                day1.Copy(Type.Missing, day1);
                newWS = excelWB.Sheets[day1.Index + 1];
                newWS.Name = "Test";

                tabs_to_add -= 1;
            }

            Excel._Worksheet coverWS = excelWB.Sheets["Cover"];
            Excel._Worksheet contactWS = excelWB.Sheets["Contact"];
            Excel._Worksheet totalWS = excelWB.Sheets["Total"];
            Excel._Worksheet propWS = excelWB.Sheets["Prop. Accept."];

            if (dt.IsInitialized & !dt.HasErrors)
            {
                #region COVER SHEET
                coverWS.Cells[25, "A"] = dt.Rows[0]["cust"].ToString();
                coverWS.Cells[27, "A"] = dt.Rows[0]["jobtype"].ToString();
                coverWS.Cells[29, "A"] = string.Format("Proposal No: {0}", dt.Rows[0]["jobno"].ToString());
                coverWS.Cells[31, "A"] = string.Format("Proposal Date: {0}", dt.Rows[0]["qt_date"].ToString());
                #endregion
                #region CONTACT SHEET
                contactWS.Cells[8, "A"] = "Sales Representative:   " + dt.Rows[0]["salesman"].ToString();
                contactWS.Cells[11, "A"] = "Contact Number:   " + "(337) 999-1001";
                contactWS.Cells[14, "A"] = "Customer:   " + dt.Rows[0]["cust"].ToString();
                contactWS.Cells[17, "A"] = "Customer Contact:   " + dt.Rows[0]["cust_email"].ToString();
                contactWS.Cells[20, "A"] = "Contact Number:   " + dt.Rows[0]["cust_phone"].ToString();
                contactWS.Cells[23, "A"] = "Job Location:   " + dt.Rows[0]["loc"].ToString();

                //Generate Project Description
                string project = dt.Rows[0]["jobtype"].ToString() + " for " + dt.Rows[0]["endclient"].ToString();
                contactWS.Cells[26, "A"] = "Project:   " + dt.Rows[0]["jobtype"].ToString();

                #endregion
                #region TOTAL SHEET
                //totalWS.Cells[10, "A"] = dt.Rows[0]["jobtype"].ToString();
                #endregion

                #region PROPOSAL SHEET
                propWS.Cells[8, "A"] = string.Format("To Accept Proposal No. {0} please complete, sign, and return this page:", dt.Rows[0]["jobno"].ToString());
                #endregion
            }

            #region PrintFileToXL_and_PDF

            if (checkFilename(xl_path, ".xlsx"))
            {
                excelWB.SaveAs(xl_path + ".xlsx");
                Console.WriteLine(dt.Rows[0]["jobno"].ToString() + " saved to Excel in path = " + xl_path + ".xlsx");
            }

            if (checkFilename(pdf_path, ".pdf"))
            {
                excelWB.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, pdf_path + ".pdf", From: 1, To: (sheet_count - 3));
                Console.WriteLine(dt.Rows[0]["jobno"].ToString() + " saved to PDF in path = " + pdf_path + ".pdf");
            }

            Boolean savechanges = false;

            #endregion

            excelWB.Close(savechanges, Type.Missing, Type.Missing);
            //excelWB.Worksheets.Application.Quit();
            excelWB = null;

            excelApp.Quit();
            excelApp = null;
            GC.Collect();*/
        }

        private void Btn_DeleteTab_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = tabDynamic.Items.Cast<TabItem>().Where(i => i.Name.Equals(tabName)).SingleOrDefault();

            TabItem tab = item as TabItem;

            if (tab != null)
            {
                if (_tabItems.Count < 3)
                {
                    MessageBox.Show("Cannot remove last tab.");
                }
                else if (MessageBox.Show(string.Format("Are you sure you want to remove the tab '{0}'?", tab.Header.ToString()),
                    "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // get selected tab
                    TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

                    deleteTabItemGrid(tab, tabDynamic.SelectedIndex);

                    // clear tab control binding
                    tabDynamic.DataContext = null;

                    _tabItems.Remove(tab);

                    // bind tab control
                    tabDynamic.DataContext = _tabItems;

                    // select previously selected tab. if that is removed then select first tab
                    if (selectedTab == null || selectedTab.Equals(tab))
                    {
                        selectedTab = _tabItems[0];
                    }
                    tabDynamic.SelectedItem = selectedTab;
                }
            }
        }

        private void Btn_exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

    }
}
