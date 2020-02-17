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
using System.Diagnostics;
using System.Data;
using System.Data.SQLite;
using System.Net.Mail;
//using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using HydrotestCentral.ViewModels;
using HydrotestCentral.Models;
using QBFC13Lib;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

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
            loadInvoiceScreen();
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

        public void loadInvoiceScreen()
        {
            if (txtBx_InvoiceJobno.Text == "" && txtBx_Invno.Text == "")
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

        private void Btn_AddInvoiceToQB(object sender, RoutedEventArgs e)
        {
            bool sessionBegun = false;
            bool connectionOpen = false;
            QBSessionManager sessionManager = null;

            Trace.WriteLine("Add Invoice func ran");

            try
            {
                // Create the session Manager object
                sessionManager = new QBSessionManager();
                //sessionManager.OpenConnection(main_vm.accounting_String, "Hydrotest Central");
                sessionManager.OpenConnection2("", "Hydrotest Central", ENConnectionType.ctLocalQBD);
                connectionOpen = true;
                //MessageBox.Show(accounting_string);
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                sessionBegun = true;

                //Create the message set request object to hold our request
                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 8, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                IMsgSetResponse responseMsgSet = null;

                //responseMsgSet = sessionManager.GetErrorRecoveryStatus();

                // ERROR RECOVERY: 
                // All steps are described in QBFC Developers Guide, on pg 41
                // under section titled "Automated Error Recovery"

                // (1) Set the error recovery ID using ErrorRecoveryID function
                //		Value must be in GUID format
                //	You could use c:\Program Files\Microsoft Visual Studio\Common\Tools\GuidGen.exe 
                //	to create a GUID for your unique ID
                string errecid = "{DB6385F9-9122-4F5B-84BB-E925D9C1C232}";
                sessionManager.ErrorRecoveryID.SetValue(errecid);

                // (2) Set EnableErrorRecovery to true to enable error recovery
                sessionManager.EnableErrorRecovery = true;


                // (3) Set SaveAllMsgSetRequestInfo to true so the entire contents of the MsgSetRequest
                //		will be saved to disk. If SaveAllMsgSetRequestInfo is false (default), only the 
                //		newMessageSetID will be saved. 
                sessionManager.SaveAllMsgSetRequestInfo = true;

                // (4) Use IsErrorRecoveryInfo to check whether an unprocessed response exists. 
                //		If IsErrorRecoveryInfo is true:

                if (sessionManager.IsErrorRecoveryInfo())
                {
                    Trace.WriteLine("Error Recovery Processed");
                    //string reqXML;
                    //string resXML;
                    IMsgSetRequest reqMsgSet = null;
                    IMsgSetResponse resMsgSet = null;

                    // a. Get the response status, using GetErrorRecoveryStatus
                    resMsgSet = sessionManager.GetErrorRecoveryStatus();
                    // resXML = resMsgSet.ToXMLString();
                    // MessageBox.Show(resXML);

                    if (resMsgSet.Attributes.MessageSetStatusCode.Equals("600"))
                    {
                        // This case may occur when a transaction has failed after QB processed 
                        // the request but client app didn't get the response and started with 
                        // another company file.
                        MessageBox.Show("The oldMessageSetID does not match any stored IDs, and no newMessageSetID is provided.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9001"))
                    {
                        MessageBox.Show("Invalid checksum. The newMessageSetID specified, matches the currently stored ID, but checksum fails.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9002"))
                    {
                        // Response was not successfully stored or stored properly
                        MessageBox.Show("No stored response was found.");
                    }
                    // 9003 = Not used
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9004"))
                    {
                        // MessageSetID is set with a string of size > 24 char
                        MessageBox.Show("Invalid MessageSetID, greater than 24 character was given.");
                    }
                    else if (resMsgSet.Attributes.MessageSetStatusCode.Equals("9005"))
                    {
                        MessageBox.Show("Unable to store response.");
                    }
                    else
                    {
                        IResponse res = resMsgSet.ResponseList.GetAt(0);
                        int sCode = res.StatusCode;
                        //string sMessage = res.StatusMessage;
                        //string sSeverity = res.StatusSeverity;
                        //MessageBox.Show("StatusCode = " + sCode + "\n" + "StatusMessage = " + sMessage + "\n" + "StatusSeverity = " + sSeverity);

                        if (sCode == 0)
                        {
                            MessageBox.Show("Last request was processed and Invoice was added successfully!");
                        }
                        else if (sCode > 0)
                        {
                            MessageBox.Show("There was a warning but last request was processed successfully!");
                        }
                        else
                        {
                            MessageBox.Show("It seems that there was an error in processing last request");
                            // b. Get the saved request, using GetSavedMsgSetRequest
                            reqMsgSet = sessionManager.GetSavedMsgSetRequest();
                            //reqXML = reqMsgSet.ToXMLString();
                            //MessageBox.Show(reqXML);

                            // c. Process the response, possibly using the saved request
                            resMsgSet = sessionManager.DoRequests(reqMsgSet);
                            IResponse resp = resMsgSet.ResponseList.GetAt(0);
                            int statCode = resp.StatusCode;
                            if (statCode == 0)
                            {
                                string resStr = null;
                                IInvoiceRet invRet = resp.Detail as IInvoiceRet;
                                resStr = resStr + "Following invoice has been successfully submitted to QuickBooks:\n\n\n";
                                if (invRet.TxnNumber != null)
                                    resStr = resStr + "Txn Number = " + Convert.ToString(invRet.TxnNumber.GetValue()) + "\n";
                            } // if (statusCode == 0)
                        } // else (sCode)
                    } // else (MessageSetStatusCode)

                    // d. Clear the response status, using ClearErrorRecovery
                    sessionManager.ClearErrorRecovery();
                    MessageBox.Show("Proceeding with current transaction.");
                }

                // Add the request to the message set request object
                IInvoiceAdd invoiceAdd = requestMsgSet.AppendInvoiceAddRq();

                // ---Set the IInvoiceAdd fields---

                // Customer:Job
                string customer = main_vm.CurrentInvoiceHeader.cust + ":" + main_vm.CurrentInvoiceHeader.jobno;
                if (!customer.Equals(""))
                {
                    invoiceAdd.CustomerRef.FullName.SetValue(customer);
                }

                // Invoice Date
                string invoiceDate = main_vm.CurrentInvoiceHeader.invdate;
                if (!invoiceDate.Equals(""))
                {
                    invoiceAdd.TxnDate.SetValue(Convert.ToDateTime(invoiceDate));
                }

                // Invoice Number
                string invoiceNumber = main_vm.CurrentInvoiceHeader.invno;
                if (!invoiceNumber.Equals(""))
                {
                    invoiceAdd.RefNumber.SetValue(invoiceNumber);
                }

                // Bill Address
                string bAddr1 = main_vm.CurrentInvoiceHeader.cust;
                string bAddr2 = main_vm.CurrentInvoiceHeader.cust_addr1;
                string bAddr3 = main_vm.CurrentInvoiceHeader.cust_addr2;
                string bAddr4 = "";
                string bCity = main_vm.CurrentInvoiceHeader.cust_city;
                string bState = main_vm.CurrentInvoiceHeader.cust_state;
                string bPostal = main_vm.CurrentInvoiceHeader.cust_zip;
                string bCountry = "USA";
                invoiceAdd.BillAddress.Addr1.SetValue(bAddr1);
                invoiceAdd.BillAddress.Addr2.SetValue(bAddr2);
                invoiceAdd.BillAddress.Addr3.SetValue(bAddr3);
                invoiceAdd.BillAddress.Addr4.SetValue(bAddr4);
                invoiceAdd.BillAddress.City.SetValue(bCity);
                invoiceAdd.BillAddress.State.SetValue(bState);
                invoiceAdd.BillAddress.PostalCode.SetValue(bPostal);
                invoiceAdd.BillAddress.Country.SetValue(bCountry);

                // P.O. Number
                string poNumber = main_vm.CurrentInvoiceHeader.po;
                if (!poNumber.Equals(""))
                {
                    invoiceAdd.PONumber.SetValue(poNumber);
                }

                // Terms
                string terms = main_vm.CurrentInvoiceHeader.terms;
                if (terms.IndexOf("Please select one from list") >= 0)
                {
                    terms = "";
                }
                if (!terms.Equals(""))
                {
                    invoiceAdd.TermsRef.FullName.SetValue(terms);
                }

                // Due Date
                string dueDate = main_vm.CurrentInvoiceHeader.duedate;
                if (!dueDate.Equals(""))
                {
                    invoiceAdd.DueDate.SetValue(Convert.ToDateTime(dueDate));
                }

                // Customer Message
                //string customerMsg = "Customer Message";
                //if (!customerMsg.Equals(""))
                //{
                //    invoiceAdd.CustomerMsgRef.FullName.SetValue(customerMsg);
                //}

                // Set the values for the invoice line (main_vm.invoice_items.Count)
                for (int i=0; i<main_vm.invoice_items.Count; i++)
                {
                    // Create the line item for the invoice 
                    int c = 6;

                    if(c == 6)  // full row
                    {
                        string item = main_vm.invoice_items[i].item;
                        string desc = main_vm.invoice_items[i].descr;
                        string rate = main_vm.invoice_items[i].rate.ToString();
                        string qty = main_vm.invoice_items[i].qty.ToString();
                        string amount = main_vm.invoice_items[i].line_total.ToString();
                        //string taxable = "0";

                        if(!item.Equals("") || !desc.Equals(""))
                        {
                            IInvoiceLineAdd invoiceLineAdd = invoiceAdd.ORInvoiceLineAddList.Append().InvoiceLineAdd;

                            invoiceLineAdd.ItemRef.FullName.SetValue(item);
                            invoiceLineAdd.Desc.SetValue(desc);
                            invoiceLineAdd.ORRatePriceLevel.Rate.SetValue(Convert.ToDouble(rate));
                            invoiceLineAdd.Quantity.SetValue(Convert.ToDouble(qty));
                            invoiceLineAdd.Amount.SetValue(Convert.ToDouble(amount));

                            // Currently IsTaxable is not supported in QBD - QuickBooks Desktop Edition
                            /*
                            if (taxable.ToUpper().Equals("Y") || taxable.ToUpper().Equals("N"))
                            {
                                bool isTaxable = false;
                                if (taxable.ToUpper().Equals("Y")) isTaxable=true;
                                    invoiceLineAdd.IsTaxable.SetValue(isTaxable);
                            }
                            */
                        }
                    }
                }

                // If all inputs are in, perform the request and obtain a response from QuickBooks
                if (isAllInputIn())
                {
                    responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                    //MessageBox.Show(responseMsgSet.ToString());

                    // Uncomment the following to view and save the request and response XML
                    string requestXML = requestMsgSet.ToXMLString();
                    MessageBox.Show(requestXML);
                    //SaveXML(requestXML);
                    // string responseXML = responseSet.ToXMLString();
                    // MessageBox.Show(responseXML);
                    // SaveXML(responseXML);

                    IResponse response = responseMsgSet.ResponseList.GetAt(0);
                    int statusCode = response.StatusCode;
                     string statusMessage = response.StatusMessage;
                     string statusSeverity = response.StatusSeverity;
                     MessageBox.Show("Status:\nCode = " + statusCode + "\nMessage = " + statusMessage + "\nSeverity = " + statusSeverity);

                    if (statusCode == 0)
                    {
                        string resString = null;
                        IInvoiceRet invoiceRet = response.Detail as IInvoiceRet;
                        resString = resString + "Following invoice has been successfully submitted to QuickBooks:\n\n\n";
                        if (invoiceRet.TimeCreated != null)
                            resString = resString + "Time Created = " + Convert.ToString(invoiceRet.TimeCreated.GetValue()) + "\n";
                        if (invoiceRet.TxnNumber != null)
                            resString = resString + "Txn Number = " + Convert.ToString(invoiceRet.TxnNumber.GetValue()) + "\n";
                        if (invoiceRet.TxnDate != null)
                            resString = resString + "Txn Date = " + Convert.ToString(invoiceRet.TxnDate.GetValue()) + "\n";
                        if (invoiceRet.RefNumber != null)
                            resString = resString + "Reference Number = " + invoiceRet.RefNumber.GetValue() + "\n";
                        if (invoiceRet.CustomerRef.FullName != null)
                            resString = resString + "Customer FullName = " + invoiceRet.CustomerRef.FullName.GetValue() + "\n";
                        resString = resString + "\nBilling Address:" + "\n";
                        if (invoiceRet.BillAddress.Addr1 != null)
                            resString = resString + "Addr1 = " + invoiceRet.BillAddress.Addr1.GetValue() + "\n";
                        if (invoiceRet.BillAddress.Addr2 != null)
                            resString = resString + "Addr2 = " + invoiceRet.BillAddress.Addr2.GetValue() + "\n";
                        if (invoiceRet.BillAddress.Addr3 != null)
                            resString = resString + "Addr3 = " + invoiceRet.BillAddress.Addr3.GetValue() + "\n";
                        if (invoiceRet.BillAddress.Addr4 != null)
                            resString = resString + "Addr4 = " + invoiceRet.BillAddress.Addr4.GetValue() + "\n";
                        if (invoiceRet.BillAddress.City != null)
                            resString = resString + "City = " + invoiceRet.BillAddress.City.GetValue() + "\n";
                        if (invoiceRet.BillAddress.State != null)
                            resString = resString + "State = " + invoiceRet.BillAddress.State.GetValue() + "\n";
                        if (invoiceRet.BillAddress.PostalCode != null)
                            resString = resString + "Postal Code = " + invoiceRet.BillAddress.PostalCode.GetValue() + "\n";
                        if (invoiceRet.BillAddress.Country != null)
                            resString = resString + "Country = " + invoiceRet.BillAddress.Country.GetValue() + "\n";
                        if (invoiceRet.PONumber != null)
                            resString = resString + "\nPO Number = " + invoiceRet.PONumber.GetValue() + "\n";
                        if (invoiceRet.TermsRef.FullName != null)
                            resString = resString + "Terms = " + invoiceRet.TermsRef.FullName.GetValue() + "\n";
                        if (invoiceRet.DueDate != null)
                            resString = resString + "Due Date = " + Convert.ToString(invoiceRet.DueDate.GetValue()) + "\n";
                        if (invoiceRet.SalesTaxTotal != null)
                            resString = resString + "Sales Tax = " + Convert.ToString(invoiceRet.SalesTaxTotal.GetValue()) + "\n";
                        resString = resString + "\nInvoice Line Items:" + "\n";
                        IORInvoiceLineRetList orInvoiceLineRetList = invoiceRet.ORInvoiceLineRetList;
                        string fullname = "<empty>";
                        string desc = "<empty>";
                        string rate = "<empty>";
                        string quantity = "<empty>";
                        string amount = "<empty>";
                        for (int i = 0; i <= orInvoiceLineRetList.Count - 1; i++)
                        {
                            if (invoiceRet.ORInvoiceLineRetList.GetAt(i).ortype == ENORInvoiceLineRet.orilrInvoiceLineRet)
                            {
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ItemRef.FullName != null)
                                    fullname = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ItemRef.FullName.GetValue();
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Desc != null)
                                    desc = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Desc.GetValue();
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ORRate.Rate != null)
                                    rate = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.ORRate.Rate.GetValue());
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Quantity != null)
                                    quantity = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Quantity.GetValue());
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Amount != null)
                                    amount = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineRet.Amount.GetValue());
                            }
                            else
                            {
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.ItemGroupRef.FullName != null)
                                    fullname = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.ItemGroupRef.FullName.GetValue();
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Desc != null)
                                    desc = invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Desc.GetValue();
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.InvoiceLineRetList.GetAt(i).ORRate.Rate != null)
                                    rate = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.InvoiceLineRetList.GetAt(i).ORRate.Rate.GetValue());
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Quantity != null)
                                    quantity = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.Quantity.GetValue());
                                if (invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.TotalAmount != null)
                                    amount = Convert.ToString(invoiceRet.ORInvoiceLineRetList.GetAt(i).InvoiceLineGroupRet.TotalAmount.GetValue());
                            }
                            resString = resString + "Fullname: " + fullname + "\n";
                            resString = resString + "Description: " + desc + "\n";
                            resString = resString + "Rate: " + rate + "\n";
                            resString = resString + "Quantity: " + quantity + "\n";
                            resString = resString + "Amount: " + amount + "\n\n";
                        }
                        MessageBox.Show(resString);
                    } // if statusCode is zero
                } // if all input is in
                else
                {
                    MessageBox.Show("One or more required input is missing.\n\nPlease check and make sure all of the input have been entered.");
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

        private bool isAllInputIn()
        {
            if ( main_vm.CurrentInvoiceHeader.cust.Equals("") ||
                main_vm.CurrentInvoiceHeader.jobno.Equals("") ||
                main_vm.CurrentInvoiceHeader.cust_addr1.Equals("") ||
                main_vm.CurrentInvoiceHeader.cust_city.Equals("") ||
                main_vm.CurrentInvoiceHeader.cust_state.Equals("") ||
                main_vm.CurrentInvoiceHeader.invno.Equals("") ||
                main_vm.CurrentInvoiceHeader.po.Equals("") ||
                main_vm.CurrentInvoiceHeader.duedate.Equals("") ||
                main_vm.CurrentInvoiceHeader.invdate.Equals("") ||
                main_vm.CurrentInvoiceHeader.terms.Equals("") ||
                main_vm.CurrentInvoiceHeader.inv_total.Equals("")
                )
            {
                MessageBox.Show("Not all fields necessary for Invoice are populated!");
                return false;
            }
            else
            {
                return true;
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

        private void Btn_Load(object sender, RoutedEventArgs e)
        {
            loadInvoiceScreen();
        }

        private void Btn_InvPrev(object sender, RoutedEventArgs e)
        {
            if (main_vm.InvnoCount > 1)
            {
                if(main_vm.CurrentInvIndex > 1)
                {
                    main_vm.CurrentInvIndex = main_vm.CurrentInvIndex - 1;
                    main_vm.CurrentInvoiceHeader = main_vm.invoice_headers[main_vm.CurrentInvIndex];
                    main_vm.Invno = main_vm.CurrentInvoiceHeader.invno;
                    main_vm.invoice_items = main_vm.LoadInvoiceItemsData(main_vm.Invno);
                }
            }
        }

        private void Btn_InvNext(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("InvNext Clicked!");
            if (main_vm.InvnoCount > 1)
            {
                Trace.WriteLine("InvnoCount: " + main_vm.InvnoCount.ToString());
                if(main_vm.CurrentInvIndex < main_vm.InvnoCount)
                {
                    Trace.WriteLine("CurrentInvno: " + main_vm.CurrentInvIndex.ToString());
                    main_vm.CurrentInvoiceHeader = main_vm.invoice_headers[main_vm.CurrentInvIndex];

                    main_vm.CurrentInvIndex = main_vm.CurrentInvIndex + 1;
                    main_vm.Invno = main_vm.CurrentInvoiceHeader.invno;
                    main_vm.invoice_items = main_vm.LoadInvoiceItemsData(main_vm.Invno);
                }
            }
        }
            
        private void Btn_PrintInvoice(object sender, RoutedEventArgs e)
        {

            /* FIRST TEST OF PDFSharp
                // Create the PDF Document
                PdfDocument document = new PdfDocument();
                // Create and empty page
                PdfPage page = document.AddPage();
                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                // Create a font
                XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                // Draw the text
                gfx.DrawString("Hello, World!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormat.Center);
                // Save the document...
                string filename = "C:\\Users\\SFWMD\\Desktop\\HelloWorld.pdf";
                document.Save(filename);
                // ...and start a viewer.
                Process.Start(filename);
            */

            PDFCreator pd = new PDFCreator();

            // Create a MigraDoc document
            Document document = pd.CreateDocument(main_vm.CurrentInvoiceHeader, main_vm.invoice_items);

            //string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;

            renderer.RenderDocument();

            // Save the document...
            string filename = "C:\\Users\\SFWMD\\Desktop\\Test.pdf";
            renderer.PdfDocument.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }

        private void Btn_exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        #region Deprecated Functions
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

        #endregion

    }
}
