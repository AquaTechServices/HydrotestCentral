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
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Data;
using System.Data.SQLite;
using System.Net.Mail;
//using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using HydrotestCentral.ViewModels;
using HydrotestCentral.Models;
using QBFC13Lib;

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
        public string jobno;
        public double proj_daily_total, proj_addn_chg, proj_job_total, est_days;
        //public QuoteHeaderDataProvider quote_heads;
        //public QuoteItemsDataProvider quote_items;
        private List<TabItem> _tabItems;
        //private List<string> _tabNames;
        //private TabItem _tabAdd;
        public string accounting_string;

        public static MainWindowViewModel main_vm;
        //public static QuoteHeaderDataProvider main_Quoteheader;
        private QuoteHeader quoteHeaderBeingEdited;

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

            //this.AddTabItem();

            // bind tab control
            //tabDynamic.DataContext = _tabItems;

            //tabDynamic.SelectedIndex = 0;
            accounting_string = main_vm.accounting_String;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        /*
        private TabItem AddTabItemByName(string name)
        {
            int count = _tabItems.Count;

            // Create new Tab
            TabItem tab = new TabItem();
            tab.Header = string.Format(name);
            tab.Name = string.Format(name);
            //tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

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
        */

        private void QHeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Make the jobno column non editable
            QHeader.Columns[1].IsReadOnly = true;

            // Get selected Row
            if (QHeader.SelectedItem != null)
            {
                QuoteHeader temp = (QuoteHeader)QHeader.SelectedItem;
                //MessageBox.Show("QHeader Selection Changed...JobNo now: " + temp.jobno);

                // Get selected Row cell base on which the datagrid will be changed
                try
                {
                    this.jobno = temp.jobno;
                    main_vm.Jobno = temp.jobno;
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
                    /*
                    //Change QItems based on Row
                    //main_vm.updateQuoteItemsByJob(this.jobno);

                    //Get the number of Tab Items in the database
                    int tab_count_fromDB = main_vm.getCountOfTabItems(temp.jobno);
                    //MessageBox.Show("DB Tab Count: " + main_vm.getCountOfTabItems(temp.jobno));
                    //Remove all tabs except Day 1 and + Tab
                    if (tab_count_fromDB > 2)
                    {
                        //MessageBox.Show("Tab Matching" + tab_count_fromDB + " | " + (_tabItems.Count - 1));
                        while ((_tabItems.Count - 1) < tab_count_fromDB)
                        {
                            this.AddTabItem();
                        }
                        // bind tab control
                        //tabDynamic.DataContext = _tabItems;
                    }

                    //Change QItems based on Row
                    main_vm.updateQuoteItemsByJob(this.jobno);

                    // Update selected tab child
                    //getTabItemGrid((TabItem)tabDynamic.SelectedItem, tabDynamic.SelectedIndex);
                    */
                }
            }
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public void GetQuoteHeaderData()
        {
            QHeader.ItemsSource = main_vm.quote_headers;
        }

        public void getTabItemGrid(TabItem tab, int tab_index)
        {
            QuoteItemGrid grid = new QuoteItemGrid(jobno, tab_index, main_vm);

            main_vm.Jobno = jobno;
            //MessageBox.Show("Getting Tab Index: " + TabIndex);
            main_vm.updateQuoteItemsByJob_And_Tab(jobno, tab_index);
            grid.QItems.ItemsSource = main_vm.quote_items;
            tab.Content = grid;
            //datagridTest.ItemsSource = main_vm.quote_items;
        }

        public void saveTabItemGrid(string jobno, int tab_index)
        {
            main_vm.saveTabItemGrid(jobno, tab_index);
            Trace.WriteLine("save tab item grid to database");
            main_vm.updateQuoteItemsByJob_And_Tab(jobno, tab_index);
        }

        public void deleteTabItemGrid(TabItem tab, int tab_index)
        {
            main_vm.DeleteQuoteItemGrid(jobno, tab_index);
            Trace.WriteLine(string.Format("tab {0} deleted\n", tab_index + 1));
        }
        /*
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
                    main_vm.selected_tab_index = tabDynamic.SelectedIndex;
                    getTabItemGrid(tab, tabDynamic.SelectedIndex);
                }
            }
        }
        */

        private void Btn_DeleteQuoteHeader_Click(object sender, RoutedEventArgs e)
        {
            //main_Quoteheader.DeleteHeaderItem(jobno);

            //main_vm.DeleteHeaderItem(jobno);
            //main_vm.quote_headers = main_vm.LoadQuoteHeaderData();

            MessageBoxResult result = MessageBox.Show("Are you sure you want to remove the Quote?", "Delete Quote", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    MessageBox.Show("Quote removed successfully");
                    break;
                case MessageBoxResult.No:
                    break;

            }
            if (result == MessageBoxResult.Yes)
            {

                main_vm.DeleteHeaderItem(jobno);
                main_vm.quote_headers = main_vm.LoadQuoteHeaderData();
            }
        }


        private void Btn_SaveQuoteHeader_Click(object sender, RoutedEventArgs e)
        {
            main_vm.UpdateHeaderItem(jobno);
        }

        private void QHeader_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            quoteHeaderBeingEdited = e.Row.Item as QuoteHeader;
        }

        private void QHeader_CurrentCellChanged(object sender, EventArgs e)
        {
            if (quoteHeaderBeingEdited != null)
            {
                //MessageBox.Show(quoteHeaderBeingEdited.jobno + " is now being updated in the database!");
                main_vm.UpdateHeaderItem(quoteHeaderBeingEdited.jobno);
                quoteHeaderBeingEdited = null;
                // MessageBox.Show("Quoted updated successfully!");
            }
        }
        /*
        private void Btn_SaveItems_Click(object sender, RoutedEventArgs e)
        {
            QuoteItemGrid grid = new QuoteItemGrid(jobno, tabDynamic.SelectedIndex, main_vm);

            // Delete all items from QTE_ITEMS where jobno and tab_index match is found
            main_vm.DeleteQuoteItemGrid(jobno, tabDynamic.SelectedIndex);
            saveTabItemGrid(jobno, tabDynamic.SelectedIndex);
            main_vm.updateQuoteItemsByJob_And_Tab(jobno, main_vm.selected_tab_index);

            TabItem tab = (TabItem)tabDynamic.SelectedItem;

            grid.QItems.ItemsSource = main_vm.quote_items;
            tab.Content = grid;
        }

        //private void Btn_DeleteItemRow_Click(object sender, RoutedEventArgs e)
        //{
        //    int row_index = main_vm.selected_row_index;

        //    main_vm.DeleteQuoteItemRow(jobno, tabDynamic.SelectedIndex, row_index);
        //    Trace.WriteLine(string.Format("Item Row {0} Deleted...", row_index));
        //    main_vm.updateQuoteItemsByJob_And_Tab(jobno, main_vm.selected_tab_index);
        //}

        private void Btn_DeleteItemRow_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to remove the Quote Item?", "Delete Quote Item", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    MessageBox.Show("Quote Item removed successfully");
                    break;
                case MessageBoxResult.No:
                    break;
            }

            if (result == MessageBoxResult.Yes)
            {
                QuoteItemGrid grid = new QuoteItemGrid(jobno, tabDynamic.SelectedIndex, main_vm);
                TabItem tab = (TabItem)tabDynamic.SelectedItem;
                //int row_index = main_vm.selected_row_index;
                int row_index = ((HydrotestCentral.Models.QuoteItem)((HydrotestCentral.QuoteItemGrid)tab.Content).QItems.SelectedItem).row_index;

                main_vm.DeleteQuoteItemRow(jobno, tabDynamic.SelectedIndex, row_index);
                Trace.WriteLine(string.Format("Item Row {0} Deleted...", row_index));
                main_vm.updateQuoteItemsByJob_And_Tab(jobno, main_vm.selected_tab_index);

                grid.QItems.ItemsSource = main_vm.quote_items;
                tab.Content = grid;
            }
        }
        */

        private void listViewItem_Selected(object sender, RoutedEventArgs e)
        {
            Quote_MainGrid.Visibility = Visibility.Hidden;
            Dashboard_MainGrid.Visibility = Visibility.Visible;
            Invoice_MainGrid.Visibility = Visibility.Hidden;
            Job_MainGrid.Visibility = Visibility.Hidden;
        }

        private void listViewItem1_Selected(object sender, RoutedEventArgs e)
        {
            Quote_MainGrid.Visibility = Visibility.Visible;
            Dashboard_MainGrid.Visibility = Visibility.Hidden;
            Invoice_MainGrid.Visibility = Visibility.Hidden;
            Job_MainGrid.Visibility = Visibility.Hidden;
        }

        private void listViewItem2_Selected(object sender, RoutedEventArgs e)
        {
            Quote_MainGrid.Visibility = Visibility.Hidden;
            Dashboard_MainGrid.Visibility = Visibility.Hidden;
            Invoice_MainGrid.Visibility = Visibility.Hidden;
            Job_MainGrid.Visibility = Visibility.Visible;
        }

        private void listViewItem3_Selected(object sender, RoutedEventArgs e)
        {
            Quote_MainGrid.Visibility = Visibility.Hidden;
            Dashboard_MainGrid.Visibility = Visibility.Hidden;
            Invoice_MainGrid.Visibility = Visibility.Visible;
            Job_MainGrid.Visibility = Visibility.Hidden;
            Invoice_MainGrid.DataContext = main_vm;
        }
        /*
        private void btn_AddItemRow_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = (TabItem)tabDynamic.SelectedItem;
            //main_vm.ADDQuoteItemsByJob_And_Tab(this.jobno, tabDynamic.SelectedIndex);
            //tab.Content = main_vm.quote_items;

            //QuoteItemGrid grid = new QuoteItemGrid(jobno, tabDynamic.SelectedIndex, main_vm);

            ////MessageBox.Show("Getting Tab Index: " + TabIndex);
            //main_vm.updateQuoteItemsByJob_And_Tab(jobno, tabDynamic.SelectedIndex);
            //grid.QItems.ItemsSource = main_vm.quote_items;
            //tab.Content = grid;

            QuoteItemGrid grid = new QuoteItemGrid(jobno, tabDynamic.SelectedIndex, main_vm);

            //MessageBox.Show("Getting Tab Index: " + TabIndex);
            main_vm.ADDQuoteItemsByJob_And_Tab(jobno, tabDynamic.SelectedIndex);
            grid.QItems.ItemsSource = main_vm.quote_items;
            tab.Content = grid;
        }

        public void UpdateQuoteItems_Row(DataGrid datagrid, int tab_index, int row_index)
        {
            string job = this.jobno;

            try
            {
                connection.Open();
                Trace.WriteLine("Connection String:" + connection.ConnectionString.ToString());
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
                
                cmd.CommandText = string.Format("UPDATE QTE_ITEMS, SET WHERE jobno=(@jobno) AND tab_index=(@tabindex) AND row_index=(@rowindex)");
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
                Trace.WriteLine("Unedited:" + items_dt.Rows.Count.ToString());
                adapter.Update(items_dt);
                Trace.WriteLine("Edited:" + items_dt.Rows.Count.ToString());
                connection.Close();
            }
            catch (Exception Ex)
            {
                System.Windows.MessageBox.Show(Ex.Message);
            }
        }
        */

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
            NewQuoteWindow NQ_Win = new NewQuoteWindow(main_vm);
            NQ_Win.Show();
        }

        private void Btn_Activate_Job(object sender, RoutedEventArgs e)
        {
            bool sessionBegun = false;
            bool connectionOpen = false;
            QBSessionManager sessionManager = null;


            string custName = "";
            string jobName = "";

            // Get selected Row
            if (QHeader.SelectedItem != null)
            {
                QuoteHeader temp = (QuoteHeader)QHeader.SelectedItem;
                //MessageBox.Show("QHeader Selection Changed...JobNo now: " + temp.jobno);

                // Get selected Row cell base on which the datagrid will be changed
                try
                {
                    jobName = temp.jobno;
                    custName = temp.cust;
                    //MessageBox.Show(jobName + " | " + custName);
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
            }
            else
            {
                MessageBox.Show("Please select a job to add job number in Quickbooks!");
            }

            if(MessageBox.Show("Do you want to make " + jobName + " for " + custName + " an ACTIVE job?" + "\n\n-This will:\n\n\t 1) Add the job into Quickbooks\n\n\t", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Try to add the job into Quickbooks
                try
                {
                    //MessageBox.Show("In Try Block: " + jobName + " | " + custName);
                    // Create the session Manager object
                    sessionManager = new QBSessionManager();

                    // Create the message set request object to hold our request
                    //Create the message set request object to hold our request
                    IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 8, 0);
                    requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                    //Connect to QuickBooks and begin a session
                    //sessionManager.OpenConnection(main_vm.accounting_String, "Hydrotest Central");
                    sessionManager.OpenConnection2("", "Hydrotest Central", ENConnectionType.ctLocalQBD);
                    connectionOpen = true;
                    //MessageBox.Show(accounting_string);
                    sessionManager.BeginSession("", ENOpenMode.omDontCare);
                    sessionBegun = true;

                    ICustomerAdd customerAddRq = requestMsgSet.AppendCustomerAddRq();
                    customerAddRq.Name.SetValue(jobName);
                    customerAddRq.ParentRef.FullName.SetValue(custName);

                    customerAddRq.JobStatus.SetValue(ENJobStatus.jsAwarded);

                    //customerAddRq.JobDesc.SetValue("Job Description");

                    //customerAddRq.JobStartDate.SetValue(DateTime.Parse("1/1/2020"));
                    //customerAddRq.JobProjectedEndDate.SetValue(DateTime.Parse("1/30/2020"));
                    //customerAddRq.JobEndDate.SetValue(DateTime.Parse("1/30/2020"));

                    //Send the request and get the response from QuickBooks
                    IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                    IResponse response = responseMsgSet.ResponseList.GetAt(0);
                    ICustomerRet customerRet = (ICustomerRet)response.Detail;


                    if (customerRet.ListID.GetValue() != null)
                    {
                        MessageBox.Show("Job Added into Quickbooks");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
                finally
                {
                    // End the session and close the connection to Quickbooks
                    if (sessionBegun)
                    {
                        sessionManager.EndSession();
                    }
                    if (connectionOpen)
                    {
                        sessionManager.CloseConnection();
                    }
                }
            }
            else
            {
                // Do nothing
            }
        }

        private void Btn_Email(object sender, RoutedEventArgs e)
        {
            try
            {
                SmtpClient server = new SmtpClient("smtp.office365.com");
                server.UseDefaultCredentials = false;
                server.Port = 587;
                server.EnableSsl = true;
                server.Credentials = new System.Net.NetworkCredential("Tyler@hydrotestpros.com", "Hydro#6792", "aquatechhydro.com");
                server.Timeout = 10000;
                server.TargetName = "STARTTLS/smtp.office365.com";

                MailMessage mail = new MailMessage();
                mail.Sender = new MailAddress("Tyler@hydrotesptros.com", "Tyler Trahan");
                mail.From = new MailAddress("Tyler@hydrotestpros.com", "Tyler Trahan");
                mail.To.Add("tyler@hydrotestpros.com");
                mail.Subject = "test out message sending";
                mail.Body = "this is my message body";
                mail.IsBodyHtml = true;


                server.Send(mail);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Btn_print_Click(object sender, RoutedEventArgs e)
        {
            /* DataTable dt = new DataTable();
             int days_count = 0;
             int sheet_count = 0;

             //dt = quote_heads.getQuoteHeaderTableByJob(jobno);
             dt = new DataTable();

             Trace.WriteLine(dt.Rows[0]["jobno"].ToString() + " DataTable Created...");

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
                 Trace.WriteLine("Sheet Count: " + sheet_count.ToString());
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.ToString());
             }

             // Get a Total Count of Days tabs in DataCentral
             days_count = _tabItems.Count - 4;
             Trace.WriteLine("Days Count: " + days_count.ToString());

             // If there are more days in DataCentral than in the worksheet, clone the days tab that many times
             int tabs_to_add = days_count - (sheet_count - 11);
             Trace.WriteLine("Need to add " + tabs_to_add + " days...");

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
                 Trace.WriteLine(dt.Rows[0]["jobno"].ToString() + " saved to Excel in path = " + xl_path + ".xlsx");
             }

             if (checkFilename(pdf_path, ".pdf"))
             {
                 excelWB.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, pdf_path + ".pdf", From: 1, To: (sheet_count - 3));
                 Trace.WriteLine(dt.Rows[0]["jobno"].ToString() + " saved to PDF in path = " + pdf_path + ".pdf");
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
        /*
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
        */

        private void Btn_Load(object sender, RoutedEventArgs e)
        {
            if(txtBx_InvoiceJobno.Text == "" && txtBx_Invno.Text =="")
            {

            }
            else if (txtBx_Invno.Text != main_vm.Invno)
            {
                //Search for and load this Invoice number typed in
                Trace.WriteLine("New InvNo typed in: " + txtBx_Invno.Text);
                main_vm.Invno = txtBx_Invno.Text;
                main_vm.invoice_items = main_vm.LoadInvoiceItemsData(main_vm.Invno);

                //Get JobNo and set main viewmodel to correct jobno
                if (main_vm.invoice_items != null)
                {
                    InvoiceItem first_invItem = main_vm.invoice_items.FirstOrDefault<InvoiceItem>();
                    if (first_invItem != null)
                    {
                        main_vm.Jobno = first_invItem.jobno;
                    }
                }

            }
            else
            {
                main_vm.invoice_headers = main_vm.LoadInvoiceHeaders(txtBx_InvoiceJobno.Text);

                // Need to edit if there is more than one InvNo
                Trace.WriteLine(main_vm.invoice_headers.ToString());

                InvoiceHeader first = main_vm.invoice_headers.FirstOrDefault<InvoiceHeader>();

                if (first != null)
                {
                    Trace.WriteLine("first:" + first.jobno);
                    main_vm.Invno = first.invno;
                    main_vm.invoice_items = main_vm.LoadInvoiceItemsData(main_vm.Invno);
                }

                txtBx_Invno.Text = main_vm.Invno;
            }
        }
            


        private void Btn_exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

    }
}
