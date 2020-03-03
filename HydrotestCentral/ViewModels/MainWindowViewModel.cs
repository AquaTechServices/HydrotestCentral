using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using HydrotestCentral.Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;

namespace HydrotestCentral.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        SQLiteConnection connection;
        SQLiteCommand cmd;
        SQLiteDataAdapter adapter;
        DataSet ds;
        public string connection_String = System.Configuration.ConfigurationManager.ConnectionStrings["connection_String"].ConnectionString;
        public string accounting_String = System.Configuration.ConfigurationManager.ConnectionStrings["accounting_String"].ConnectionString;

        private ObservableCollection<QuoteHeader> quote_header_data = null;
        private ObservableCollection<InvoiceHeader> inv_header_data = null;
        private ObservableCollection<InvoiceItem> inv_item_data = null;
        private ObservableCollection<InvoiceItem> grouped_inv_item_data = null;
        private ObservableCollection<Customer> customer_data = null;
        private ObservableCollection<Job> job_data = null;

        public ObservableCollection<QuoteHeader> quote_headers
        {
            get
            {
                if (quote_header_data != null)
                {
                    return quote_header_data;
                }
                return null;
            }
            set
            {
                if (quote_header_data != value)
                {
                    quote_header_data = value;
                    OnPropertyChanged("quote_headers");
                }
            }
        }
        public ObservableCollection<QuoteItem> quote_items { get; set; }
        public ObservableCollection<InventoryItem> inventory_items { get; set; }
        public ObservableCollection<InvoiceHeader> invoice_headers
        {
            get
            {
                if (inv_header_data != null)
                {
                    return inv_header_data;
                }
                return null;
            }
            set
            {
                if (inv_header_data != value)
                {
                    inv_header_data = value;
                    OnPropertyChanged("invoice_headers");
                }
            }
        }
        public ObservableCollection<InvoiceItem> invoice_items
        {
            get
            {
                if (inv_item_data != null)
                {
                    return inv_item_data;
                }
                return null;
            }
            set
            {
                if (inv_item_data != value)
                {
                    inv_item_data = value;
                    OnPropertyChanged("invoice_items");
                }
            }
        }
        public ObservableCollection<InvoiceItem> grouped_invoice_items
        {
            get
            {
                if (grouped_inv_item_data != null)
                {
                    return grouped_inv_item_data;
                }
                return null;
            }
            set
            {
                if (grouped_inv_item_data != value)
                {
                    grouped_inv_item_data = value;
                    OnPropertyChanged("grouped_inv_item_data");
                }
            }
        }
        public ObservableCollection<Customer> customers
        {
            get
            {
                if (customer_data != null)
                {
                    return customer_data;
                }
                return null;
            }
            set
            {
                if(customer_data != value)
                {
                    customer_data = value;
                    OnPropertyChanged("customers");
                }
            }
        }
        public ObservableCollection<Job> jobs
        {
            get
            {
                if (job_data != null)
                {
                    return job_data;
                }
                return null;
            }
            set
            {
                if (job_data != value)
                {
                    job_data = value;
                    OnPropertyChanged("jobs");
                }
            }
        }

        public int selected_tab_index;
        public int selected_row_index;

        private string _jobno;
        private string _invno;
        private int _currentInvIndex;
        private InvoiceHeader _currentInvoiceHeader;
        private int _invnoCount;

        public string Jobno { get { return _jobno; } set { _jobno = value; OnPropertyChanged("Jobno"); } }
        public string Invno { get { return _invno; } set { _invno = value; OnPropertyChanged("Invno"); } }
        public int CurrentInvIndex { get { return _currentInvIndex; } set { _currentInvIndex = value; OnPropertyChanged("CurrentInvIndex"); } }
        public int InvnoCount { get { return _invnoCount; } set { _invnoCount = value; OnPropertyChanged("InvnoCount"); } }
        public InvoiceHeader CurrentInvoiceHeader { get { return _currentInvoiceHeader; } set { _currentInvoiceHeader = value; OnPropertyChanged("CurrentInvoiceHeader"); } }

        public MainWindowViewModel()
        {
            InitializeComponent();

            quote_headers = new ObservableCollection<QuoteHeader>();
            quote_headers = LoadQuoteHeaderData();
            quote_items = new ObservableCollection<QuoteItem>();
            quote_items = LoadQuoteItemData();
            invoice_headers = new ObservableCollection<InvoiceHeader>();
            invoice_items = new ObservableCollection<InvoiceItem>();

            customers = new ObservableCollection<Customer>();
            customers = LoadCustomers();
            jobs = new ObservableCollection<Job>();

            Jobno = "";
            Invno = "";
            CurrentInvIndex = 0;
            InvnoCount = 0;
        }

        #region Quote Module Functions

        public ObservableCollection<QuoteHeader> LoadQuoteHeaderData()
        {
            var headers = new ObservableCollection<QuoteHeader>();

            try
            {
                connection = new SQLiteConnection(connection_String);
                //MessageBox.Show("Opening Database - " + connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM QTE_HDR ORDER BY jobno");
                adapter = new SQLiteDataAdapter(cmd);

                ds = new DataSet();

                adapter.Fill(ds, "QTE_HDR");


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int cleaned_days = 0;
                    int cleaned_cust_id = 0;
                    double cleaned_value = 0.00;

                    if (Int32.TryParse(dr[3].ToString(), out cleaned_cust_id)) { }
                    if (Int32.TryParse(dr[9].ToString(), out cleaned_days)) { }

                    if (Double.TryParse(dr[19].ToString(), out cleaned_value)) { }

                    //if(!string.IsNullOrEmpty(dr[1].ToString())){ cleaned_qt_date= DateTime.Parse(dr[1].ToString());}

                    //if(!string.IsNullOrEmpty(dr[16].ToString())){ cleaned_est_start_date= DateTime.Parse(dr[16].ToString());}
                    //if(!string.IsNullOrEmpty(dr[17].ToString())){ cleaned_est_stop_date= DateTime.Parse(dr[17].ToString());}

                    headers.Add(new QuoteHeader
                    {
                        quoteno = dr[0].ToString(),
                        jobno = dr[0].ToString(),
                        qt_date = dr[1].ToString(),
                        cust = dr[2].ToString(),
                        cust_id = cleaned_cust_id,
                        cust_contact = dr[4].ToString(),
                        cust_phone = dr[5].ToString(),
                        cust_email = dr[6].ToString(),
                        loc = dr[7].ToString(),
                        salesman = dr[8].ToString(),
                        days_est = cleaned_days,
                        status = dr[10].ToString(),
                        jobtype = dr[11].ToString(),
                        pipe_line_size = dr[12].ToString(),
                        pipe_length = dr[13].ToString(),
                        pressure = dr[14].ToString(),
                        endclient = dr[15].ToString(),
                        supervisor = dr[16].ToString(),
                        est_start_date = dr[17].ToString(), 
                        est_stop_date = dr[18].ToString(),
                        value = cleaned_value
                    });
                    //Trace.WriteLine(dr[0].ToString() + " created in quote_headers");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ds = null;
                adapter.Dispose();
                connection.Close();
                connection.Dispose();
            }

            return headers;
        }

        public ObservableCollection<QuoteItem> LoadQuoteItemData()
        {
            var items = new ObservableCollection<QuoteItem>();

            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM QTE_ITEMS");
                adapter = new SQLiteDataAdapter(cmd);

                ds = new DataSet();

                adapter.Fill(ds, "QTE_ITEM");


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int cleaned_qty = 0;
                    double cleaned_rate = 0.00;
                    int cleaned_grouping = 1;
                    bool cleaned_taxable = false;
                    bool cleaned_discountable = false;
                    bool cleaned_printable = false;
                    double cleaned_line_total = 0.00;
                    double cleaned_tax_total = 0.00;
                    int cleaned_tab_index = 0;
                    int cleaned_row_index = 0;

                    if (Int32.TryParse(dr[0].ToString(), out cleaned_qty)) { }
                    if (Double.TryParse(dr[2].ToString(), out cleaned_rate)) { }
                    if (Int32.TryParse(dr[4].ToString(), out cleaned_grouping)) { }
                    if (Boolean.TryParse(dr[5].ToString(), out cleaned_taxable)) { }
                    if (Boolean.TryParse(dr[6].ToString(), out cleaned_discountable)) { }
                    if (Boolean.TryParse(dr[7].ToString(), out cleaned_printable)) { }
                    if (Double.TryParse(dr[9].ToString(), out cleaned_line_total)) { }
                    if (Double.TryParse(dr[10].ToString(), out cleaned_tax_total)) { }
                    if (Int32.TryParse(dr[11].ToString(), out cleaned_tab_index)) { }
                    if (Int32.TryParse(dr[12].ToString(), out cleaned_row_index)) { }

                    items.Add(new QuoteItem
                    {
                        qty = cleaned_qty,
                        item = dr[1].ToString(),
                        rate = cleaned_rate,
                        descr = dr[3].ToString(),
                        grouping = cleaned_grouping,
                        //taxable = cleaned_taxable,
                        //discountable = cleaned_discountable,
                        //printable = cleaned_printable,
                        taxable = (dr[5].ToString().ToLower() == "1" ? true : false),
                        discountable = (dr[6].ToString().ToLower() == "1" ? true : false),
                        printable = (dr[7].ToString().ToLower() == "1" ? true : false),
                        jobno = dr[8].ToString(),
                        line_total = cleaned_line_total,
                        tax_total = cleaned_tax_total,
                        tab_index = cleaned_tab_index,
                        row_index = cleaned_row_index
                    });
                    //Trace.WriteLine(dr[1].ToString() + " created in quote_items");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ds = null;
                adapter.Dispose();
                connection.Close();
                connection.Dispose();
            }

            return items;
        }

        public int getCountOfTabItems(string incoming_jobno)
        {
            Trace.WriteLine("QuoteItems.Count(): " + quote_items.Count());
            Trace.WriteLine("Linq query: " + quote_items.Where(item => item.jobno == incoming_jobno).Count());
            return quote_items.Where(item => item.jobno == incoming_jobno).Count();
        }

        public void UpdateQuoteHeaderItem(string jobno)
        {
            //Find QuoteHeader for that jobno
            QuoteHeader qh = new QuoteHeader();

            foreach (QuoteHeader header in quote_headers)
            {
                if (header.jobno == jobno)
                {
                    qh = header;
                    break;
                }
            }

            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();

                cmd.Parameters.Add(new SQLiteParameter("@jobno", jobno));
                cmd.Parameters.Add(new SQLiteParameter("@qt_date", qh.qt_date));
                cmd.Parameters.Add(new SQLiteParameter("@cust", qh.cust));
                cmd.Parameters.Add(new SQLiteParameter("@cust_contact", qh.cust_contact));
                cmd.Parameters.Add(new SQLiteParameter("@cust_phone", qh.cust_phone));
                cmd.Parameters.Add(new SQLiteParameter("@cust_email", qh.cust_email));
                cmd.Parameters.Add(new SQLiteParameter("@loc", qh.loc));
                cmd.Parameters.Add(new SQLiteParameter("@salesman", qh.salesman));
                cmd.Parameters.Add(new SQLiteParameter("@days_est", qh.days_est));
                cmd.Parameters.Add(new SQLiteParameter("@status", qh.status));
                cmd.Parameters.Add(new SQLiteParameter("@pipe_line_size", qh.pipe_line_size));
                cmd.Parameters.Add(new SQLiteParameter("@pipe_length", qh.pipe_length));
                cmd.Parameters.Add(new SQLiteParameter("@pressure", qh.pressure));
                cmd.Parameters.Add(new SQLiteParameter("@endclient", qh.endclient));
                cmd.Parameters.Add(new SQLiteParameter("@supervisor", qh.supervisor));
                cmd.Parameters.Add(new SQLiteParameter("@est_start_date", qh.est_start_date));
                cmd.Parameters.Add(new SQLiteParameter("@est_stop_date", qh.est_stop_date));

                cmd.CommandText = String.Format("UPDATE QTE_HDR SET qt_date=(@qt_date), cust=(@cust), cust_contact=(@cust_contact), cust_phone=(@cust_phone),cust_email=(@cust_email), loc=(@loc), salesman=(@salesman), days_est=(@days_est), status=(@status), pipe_line_size=(@pipe_line_size), pipe_length=(@pipe_length), pressure=(@pressure), endclient=(@endclient), supervisor=(@supervisor), est_start_date=(@est_start_date), est_stop_date=(@est_stop_date) WHERE jobno=(@jobno)");
                cmd.ExecuteNonQuery();
                connection.Close();

                quote_headers = LoadQuoteHeaderData();
            }
            catch (Exception Ex)
            {
                System.Windows.MessageBox.Show(Ex.Message);
            }
        }

        public void DeleteQuoteHeaderItem(String jobno)
        {
            try
            {
                //var start_collection = new ObservableCollection<QuoteItem>();
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = String.Format("DELETE FROM QTE_HDR WHERE jobno=\"{0}\"", jobno);
                cmd.ExecuteNonQuery();
                //model_vm.quote_headers = model_vm.LoadQuoteHeaderData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateQuoteItemsByJob(string jobno)
        {
            //MessageBox.Show("updateQuoteItemsByJob called...");
            var start_collection = new ObservableCollection<QuoteItem>();
            var new_collection = new ObservableCollection<QuoteItem>();
            start_collection = LoadQuoteItemData();

            IEnumerable<QuoteItem> items = start_collection.Where(c => c.jobno == jobno);
            Trace.WriteLine("Adding New Collection for Quote Items updated to only show Job: " + jobno);
            foreach (QuoteItem i in items)
            {
                new_collection.Add(i);
                Trace.WriteLine("--->" + i.jobno + " | " + i.item);
            }

            quote_items = new_collection;
        }

        public void updateQuoteItemsByJob_And_Tab(string jobno, int tab_index)
        {
            var start_collection = new ObservableCollection<QuoteItem>();
            var new_collection = new ObservableCollection<QuoteItem>();
            start_collection = LoadQuoteItemData();

            IEnumerable<QuoteItem> items = start_collection.Where(c => c.jobno == jobno && c.tab_index == tab_index);

            foreach (QuoteItem i in items)
            {
                new_collection.Add(i);
            }
            if (jobno != null && items.ToList().Count == 0)
            {
                List<QuoteItem> lst = new List<QuoteItem>();
                new_collection.Add(new QuoteItem
                {
                    item = null,
                    rate = 0,
                    qty = 0,
                    descr = null,
                    grouping = 0,
                    taxable = false,
                    discountable = false,
                    printable = false,
                    jobno = jobno,
                    tab_index = tab_index,

                });
            }
            quote_items = new_collection;
        }

        public void saveTabItemGrid(string jobno, int tab_index)
        {
            //Find QuoteItems for that jobno
            QuoteItem qi = new QuoteItem();

            foreach (QuoteItem item in quote_items)
            {
                if (item.jobno == jobno && item.tab_index == tab_index)
                {
                    qi = item;

                    // Calculate Line Total and Tax Total
                    if (qi.taxable)
                    {
                        qi.tax_total = 0.10 * (qi.qty * qi.rate);
                    }
                    else
                    {
                        qi.tax_total = 0.00;
                    }

                    qi.line_total = (qi.qty * qi.rate) + qi.tax_total;

                    try
                    {
                        connection = new SQLiteConnection(connection_String);
                        connection.Open();
                        cmd = connection.CreateCommand();

                        cmd.Parameters.Add(new SQLiteParameter("@jobno", qi.jobno));
                        cmd.Parameters.Add(new SQLiteParameter("@qty", qi.qty));
                        cmd.Parameters.Add(new SQLiteParameter("@item", qi.item));
                        cmd.Parameters.Add(new SQLiteParameter("@rate", qi.rate));
                        cmd.Parameters.Add(new SQLiteParameter("@descr", qi.descr));
                        cmd.Parameters.Add(new SQLiteParameter("@grouping", qi.grouping));
                        cmd.Parameters.Add(new SQLiteParameter("@taxable", qi.taxable));
                        cmd.Parameters.Add(new SQLiteParameter("@discountable", qi.discountable));
                        cmd.Parameters.Add(new SQLiteParameter("@printable", qi.printable));
                        cmd.Parameters.Add(new SQLiteParameter("@line_total", qi.line_total));
                        cmd.Parameters.Add(new SQLiteParameter("@tax_total", qi.tax_total));
                        cmd.Parameters.Add(new SQLiteParameter("@tab_index", qi.tab_index));
                        cmd.Parameters.Add(new SQLiteParameter("@row_index", qi.row_index));

                        //cmd.CommandText = String.Format("UPDATE QTE_ITEMS SET qty=(@qty), item=(@item), rate=(@rate), descr=(@descr), taxable=(@taxable), discountable=(@discountable), printable=(@printable), grouping=(@grouping), line_total=(@line_total), tax_total=(@tax_total), row_index=(@row_index) WHERE jobno=(@jobno) AND tab_index=(@tab_index)");
                        cmd.CommandText = String.Format("INSERT INTO QTE_ITEMS (qty,item,rate,descr,grouping,taxable,discountable,printable,jobno,line_total,tax_total,tab_index,row_index)VALUES ((@qty),(@item),(@rate),(@descr),(@grouping),(@taxable),(@discountable),(@printable),(@jobno),(@line_total),(@tax_total),(@tab_index),(@row_index))");
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public void DeleteQuoteItemGrid(string jobno, int tab_index)
        {
            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = String.Format("DELETE FROM QTE_ITEMS WHERE jobno=\"{0}\" AND tab_index = {1}", jobno, tab_index);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void DeleteQuoteItemRow(String jobno, int tab_index, int row_index)
        {
            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = String.Format("DELETE FROM QTE_ITEMS WHERE jobno=\"{0}\" AND tab_index = {1} AND row_index = {2}", jobno, tab_index, row_index);
                //cmd.CommandText = String.Format("DELETE FROM QTE_ITEMS WHERE jobno=\"{0}\" AND tab_index = {1}", jobno, tab_index, row_index);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void InsertQuoteItem(QuoteItem qi)
        {
            // Calculate Line Total and Tax Total
            if (qi.taxable)
            {
                qi.tax_total = 0.10 * (qi.qty * qi.rate);
            }
            else
            {
                qi.tax_total = 0.00;
            }

            qi.line_total = (qi.qty * qi.rate) + qi.tax_total;

            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();

                cmd.Parameters.Add(new SQLiteParameter("@jobno", qi.jobno));
                cmd.Parameters.Add(new SQLiteParameter("@qty", qi.qty));
                cmd.Parameters.Add(new SQLiteParameter("@item", qi.item));
                cmd.Parameters.Add(new SQLiteParameter("@rate", qi.rate));
                cmd.Parameters.Add(new SQLiteParameter("@descr", qi.descr));
                cmd.Parameters.Add(new SQLiteParameter("@grouping", qi.grouping));
                cmd.Parameters.Add(new SQLiteParameter("@taxable", qi.taxable));
                cmd.Parameters.Add(new SQLiteParameter("@discountable", qi.discountable));
                cmd.Parameters.Add(new SQLiteParameter("@printable", qi.printable));
                cmd.Parameters.Add(new SQLiteParameter("@line_total", qi.line_total));
                cmd.Parameters.Add(new SQLiteParameter("@tax_total", qi.tax_total));
                cmd.Parameters.Add(new SQLiteParameter("@tab_index", qi.tab_index));
                cmd.Parameters.Add(new SQLiteParameter("@row_index", qi.row_index));

                cmd.CommandText = String.Format("INSERT into QTE_ITEMS VALUES (@qty, @item, @rate, @descr, @grouping, @taxable,@discountable,@printable,@jobno,@line_total,@tax_total,@tab_index,@row_index)");
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public void ADDQuoteItemsByJob_And_Tab(string jobno, int tab_index)
        {

            QuoteItem new_collection = new QuoteItem();

            if (jobno != null)
            {
                new_collection.jobno = jobno;
                new_collection.tab_index = tab_index;
                new_collection.row_index = quote_items.OrderByDescending(x => x.row_index).FirstOrDefault().row_index + 1;
                // row_index = null,
            }
            quote_items.Add(new_collection);
        }

        #endregion

        #region Invoice Module Functions

        public ObservableCollection<InvoiceHeader> LoadInvoiceHeaders(string jobno)
        {
            var invoice_headers = new ObservableCollection<InvoiceHeader>();

            Trace.WriteLine(jobno);

            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM INV_HDR WHERE jobno=\"{0}\"", jobno);
                adapter = new SQLiteDataAdapter(cmd);

                ds = new DataSet();

                adapter.Fill(ds, "INV_HDR");

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int cleaned_cust_id = 0;
                    double cleaned_taxrate = 0.00;
                    double cleaned_subtotal = 0.00;
                    double cleaned_taxtotal = 0.00;
                    double cleaned_invtotal = 0.00;

                    DateTime cleaned_invdate = new DateTime();
                    DateTime cleaned_duedate = new DateTime();

                    string po_string = "";

                    if (Int32.TryParse(dr[6].ToString(), out cleaned_cust_id)) { }

                    if (Double.TryParse(dr[17].ToString(), out cleaned_taxrate)) { }
                    if (Double.TryParse(dr[19].ToString(), out cleaned_subtotal)) { }
                    if (Double.TryParse(dr[20].ToString(), out cleaned_taxtotal)) { }
                    if (Double.TryParse(dr[21].ToString(), out cleaned_invtotal)) { }

                    if (DateTime.TryParse(dr[2].ToString(), out cleaned_invdate)) { }
                    if (DateTime.TryParse(dr[3].ToString(), out cleaned_duedate)) { }

                    if (dr[16].ToString().Equals("")) { po_string = " "; } else { po_string = dr[16].ToString(); }

                    invoice_headers.Add(new InvoiceHeader
                    {
                        jobno = dr[0].ToString(),
                        invno = dr[1].ToString(),
                        invdate = cleaned_invdate,
                        duedate = cleaned_duedate,
                        terms = dr[4].ToString(),
                        cust = dr[5].ToString(),
                        cust_id = cleaned_cust_id,
                        cust_addr1 = dr[7].ToString(),
                        cust_addr2 = dr[8].ToString(),
                        cust_city = dr[9].ToString(),
                        cust_state = dr[10].ToString(),
                        cust_zip = dr[11].ToString(),
                        loc = dr[12].ToString(),
                        salesman = dr[13].ToString(),
                        jobtype = dr[14].ToString(),
                        supervisor = dr[15].ToString(),
                        po = po_string,
                        tax_rate = cleaned_taxrate,
                        tax_descr = dr[18].ToString(),
                        sub_total = cleaned_subtotal,
                        tax_total = cleaned_taxtotal,
                        inv_total = cleaned_invtotal
                    });
                    //Trace.WriteLine(dr[0].ToString() + " created in invoice_headers");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ds = null;
                adapter.Dispose();
                connection.Close();
                connection.Dispose();
            }

            if (invoice_headers.Count > 0)
            {
                CurrentInvIndex = 1;
                CurrentInvoiceHeader = invoice_headers[0];
            }
            else
            {
                CurrentInvIndex = 0;
            }
            InvnoCount = invoice_headers.Count;

            return invoice_headers;
        }

        public ObservableCollection<InvoiceItem> LoadInvoiceItemsData(string invno)
        {
            var items = new ObservableCollection<InvoiceItem>();

            Trace.WriteLine(invno);

            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM INV_ITEMS WHERE invno=\"{0}\" ORDER BY invno", invno);
                adapter = new SQLiteDataAdapter(cmd);

                ds = new DataSet();

                adapter.Fill(ds, "INV_ITEMS");


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int cleaned_row_id = -1;
                    int cleaned_qty = 0;
                    int cleaned_group = 0;

                    double cleaned_rate = 0.00;
                    double cleaned_line_total = 0.00;
                    double cleaned_tax_total = 0.00;

                    DateTime cleaned_invdate = new DateTime();

                    int int_taxable = 0;
                    int int_discountable = 0;
                    int int_printable = 0;
                    bool parsed_taxable = false;
                    bool parsed_discountable = false;
                    bool parsed_printable = false;

                    string descr_string = "";

                    if (Int32.TryParse(dr[0].ToString(), out cleaned_row_id)) { }
                    if (Int32.TryParse(dr[1].ToString(), out cleaned_qty)) { }
                    if (Int32.TryParse(dr[6].ToString(), out cleaned_group)) { }

                    if (Double.TryParse(dr[3].ToString(), out cleaned_rate)) { }
                    if (Double.TryParse(dr[11].ToString(), out cleaned_line_total)) { }
                    if (Double.TryParse(dr[12].ToString(), out cleaned_tax_total)) { }

                    if (DateTime.TryParse(dr[3].ToString(), out cleaned_invdate)) { }

                    if (Int32.TryParse(dr[7].ToString(), out int_taxable)) { }
                    if (int_taxable == 0) { parsed_taxable = false; } else { parsed_taxable = true; }
                    if (Int32.TryParse(dr[8].ToString(), out int_discountable)) { }
                    if (int_discountable == 0) { parsed_discountable = false; } else { parsed_discountable = true; }
                    if (Int32.TryParse(dr[9].ToString(), out int_printable)) { }
                    if (int_printable == 0) { parsed_printable = false; } else { parsed_printable = true; }

                    if (dr[4].ToString().Equals("")) { descr_string = " "; } else { descr_string = dr[4].ToString();
 }

                    items.Add(new InvoiceItem
                    {
                        row_id = cleaned_row_id, 
                        qty = cleaned_qty,
                        item = dr[2].ToString(),
                        rate = cleaned_rate,
                        descr = descr_string,
                        type = dr[5].ToString(),
                        grouping = cleaned_group,
                        taxable = parsed_taxable,
                        discountable = parsed_discountable,
                        printable = parsed_printable,
                        jobno = dr[10].ToString(),
                        line_total = cleaned_line_total,
                        tax_total = cleaned_tax_total,
                        cust = dr[13].ToString(),
                        invno = dr[14].ToString(),
                        invdate = dr[15].ToString()
                    });
                    //Trace.WriteLine(dr[0].ToString() + " created in invoice_headers");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ds = null;
                adapter.Dispose();
                connection.Close();
                connection.Dispose();
            }

            return items;
        }

        public ObservableCollection<InvoiceItem> GetCollapsedInvoiceItemsData(string invno)
        {
            var c_items = new ObservableCollection<InvoiceItem>();
            
            foreach(InvoiceItem ii in invoice_items)
            {
                // Search c_items for the grouping number in this InvoiceItem
                var x = c_items.FirstOrDefault<InvoiceItem>(c => c.grouping == ii.grouping);
                // If the grouping number is not in c_items, add this item
                if(x==null)
                {
                    InvoiceItem c_item = new InvoiceItem();
                    c_item.row_id = -1;
                    c_item.qty = 1;
                    c_item.item = "GROUPING";
                    c_item.jobno = ii.jobno;
                    c_item.type = ii.type;
                    c_item.taxable = ii.taxable;
                    c_item.discountable = false;
                    c_item.printable = true;
                    c_item.cust = ii.cust;
                    c_item.invno = ii.invno;
                    c_item.invdate = ii.invdate;
                    c_item.rate = ii.qty*ii.rate;
                    c_item.line_total = ii.line_total;
                    c_item.tax_total = ii.tax_total;
                    c_item.grouping = ii.grouping;
                    c_item.descr = "Daily Rate  ";

                    c_items.Add(c_item);
                }
                else // If it is, add this total to that grouping 
                {
                    Trace.WriteLine("X was in collapsed items Total:" + x.line_total.ToString());
                    x.rate = (x.qty*x.rate) + (ii.qty*ii.rate);
                    x.tax_total = x.tax_total + ii.tax_total;
                    x.line_total = x.line_total + ii.line_total;
                    Trace.WriteLine("X new total: " + x.line_total.ToString());
                }
                
            }

            return c_items;
        }

        public void CalculateTotals()
        {
            double current_subtotal = 0.00;
            double current_taxtotal = 0.00;
            double current_invtotal = 0.00; ;

            foreach(InvoiceItem ii in invoice_items)
            {
                // Calculate sub_total for this line and add it to subtotal
                ii.line_total = ii.qty * ii.rate;
                //OnPropertyChanged("line_total");
                current_subtotal = current_subtotal + ii.line_total;

                // Calculate tax total
                if (ii.taxable && !CurrentInvoiceHeader.tax_rate.Equals(null))
                {
                    ii.tax_total = ii.qty * ii.rate * CurrentInvoiceHeader.tax_rate;
                    current_taxtotal = current_taxtotal + ii.tax_total;
                }
                else 
                {
                    ii.tax_total = 0.00;
                }

                OnPropertyChanged("invoice_items");
            }

            current_invtotal = current_subtotal + current_taxtotal;

            // Update values to CurrentInvoiceHeader
            CurrentInvoiceHeader.sub_total = current_subtotal;
            CurrentInvoiceHeader.tax_total = current_taxtotal;
            CurrentInvoiceHeader.inv_total = current_invtotal;
            OnPropertyChanged("CurrentInvoiceHeader");

            // Update values to database?
            UpdateCurrentInvoiceHeaderItem_ToDB();
        }

        public void UpdateCurrentInvoiceHeaderItem_ToDB()
        {
            // Current Invoice Header is updated to database

            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();

                cmd.Parameters.Add(new SQLiteParameter("@invno", CurrentInvoiceHeader.invno));
                cmd.Parameters.Add(new SQLiteParameter("@jobno", CurrentInvoiceHeader.jobno));
                cmd.Parameters.Add(new SQLiteParameter("@inv_date", CurrentInvoiceHeader.invdate));
                cmd.Parameters.Add(new SQLiteParameter("@due_date", CurrentInvoiceHeader.duedate));
                cmd.Parameters.Add(new SQLiteParameter("@terms", CurrentInvoiceHeader.terms));
                cmd.Parameters.Add(new SQLiteParameter("@cust", CurrentInvoiceHeader.cust));
                cmd.Parameters.Add(new SQLiteParameter("@cust_id", CurrentInvoiceHeader.cust_id));
                cmd.Parameters.Add(new SQLiteParameter("@cust_addr1", CurrentInvoiceHeader.cust_addr1));
                cmd.Parameters.Add(new SQLiteParameter("@cust_addr2", CurrentInvoiceHeader.cust_addr2));
                cmd.Parameters.Add(new SQLiteParameter("@cust_city", CurrentInvoiceHeader.cust_city));
                cmd.Parameters.Add(new SQLiteParameter("@cust_state", CurrentInvoiceHeader.cust_state));
                cmd.Parameters.Add(new SQLiteParameter("@cust_zip", CurrentInvoiceHeader.cust_zip));
                cmd.Parameters.Add(new SQLiteParameter("@loc", CurrentInvoiceHeader.loc));
                cmd.Parameters.Add(new SQLiteParameter("@salesman", CurrentInvoiceHeader.salesman));
                cmd.Parameters.Add(new SQLiteParameter("@jobtype", CurrentInvoiceHeader.jobtype));
                cmd.Parameters.Add(new SQLiteParameter("@supervisor", CurrentInvoiceHeader.supervisor));
                cmd.Parameters.Add(new SQLiteParameter("@po", CurrentInvoiceHeader.po));
                cmd.Parameters.Add(new SQLiteParameter("@tax_rate", CurrentInvoiceHeader.tax_rate));
                cmd.Parameters.Add(new SQLiteParameter("@tax_descr", CurrentInvoiceHeader.tax_descr));
                cmd.Parameters.Add(new SQLiteParameter("@sub_total", CurrentInvoiceHeader.sub_total));
                cmd.Parameters.Add(new SQLiteParameter("@tax_total", CurrentInvoiceHeader.tax_total));
                cmd.Parameters.Add(new SQLiteParameter("@inv_total", CurrentInvoiceHeader.inv_total));

                cmd.CommandText = String.Format("UPDATE INV_HDR SET jobno=(@jobno), inv_date=(@inv_date), due_date=(@due_date), terms=(@terms), cust=(@cust), cust_id=(@cust_id), cust_addr1=(@cust_addr1),cust_addr1=(@cust_addr1), cust_city=(@cust_city), cust_state=(@cust_state), cust_zip=(@cust_zip), loc=(@loc), salesman=(@salesman), jobtype=(@jobtype), supervisor=(@supervisor), po=(@po), tax_rate=(@tax_rate), tax_descr=(@tax_descr), sub_total=(@sub_total), tax_total=(@tax_total), inv_total=(@inv_total) WHERE invno=(@invno)");
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception Ex)
            {
                System.Windows.MessageBox.Show(Ex.Message);
            }

        }

        public void InsertInvoiceHeader(InvoiceHeader ih)
        {
            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();

                cmd.Parameters.Add(new SQLiteParameter("@invno", ih.invno));
                cmd.Parameters.Add(new SQLiteParameter("@jobno", ih.jobno));
                cmd.Parameters.Add(new SQLiteParameter("@inv_date", ih.invdate));
                cmd.Parameters.Add(new SQLiteParameter("@due_date", ih.duedate));
                cmd.Parameters.Add(new SQLiteParameter("@terms", ih.terms));
                cmd.Parameters.Add(new SQLiteParameter("@cust", ih.cust));
                cmd.Parameters.Add(new SQLiteParameter("@cust_id", ih.cust_id));
                cmd.Parameters.Add(new SQLiteParameter("@cust_addr1", ih.cust_addr1));
                cmd.Parameters.Add(new SQLiteParameter("@cust_addr2", ih.cust_addr2));
                cmd.Parameters.Add(new SQLiteParameter("@cust_city", ih.cust_city));
                cmd.Parameters.Add(new SQLiteParameter("@cust_state", ih.cust_state));
                cmd.Parameters.Add(new SQLiteParameter("@cust_zip", ih.cust_zip));
                cmd.Parameters.Add(new SQLiteParameter("@loc", ih.loc));
                cmd.Parameters.Add(new SQLiteParameter("@salesman", ih.salesman));
                cmd.Parameters.Add(new SQLiteParameter("@jobtype", ih.jobtype));
                cmd.Parameters.Add(new SQLiteParameter("@supervisor", ih.supervisor));
                cmd.Parameters.Add(new SQLiteParameter("@po", ih.po));
                cmd.Parameters.Add(new SQLiteParameter("@tax_rate", ih.tax_rate));
                cmd.Parameters.Add(new SQLiteParameter("@tax_descr", ih.tax_descr));
                cmd.Parameters.Add(new SQLiteParameter("@sub_total", ih.sub_total));
                cmd.Parameters.Add(new SQLiteParameter("@tax_total", ih.tax_total));
                cmd.Parameters.Add(new SQLiteParameter("@inv_total", ih.inv_total));

                cmd.CommandText = String.Format("INSERT INTO INV_HDR (invno, jobno, inv_date, due_date, terms, cust, cust_id, cust_addr1, cust_addr2, cust_city, cust_state, cust_zip, loc, salesman, jobtype, supervisor, po, tax_rate, tax_descr, sub_total, tax_total, inv_total) VALUES (@invno, @jobno, @inv_date, @due_date, @terms, @cust, @cust_id, @cust_addr1,@cust_addr2, @cust_city, @cust_state, @cust_zip, @loc, @salesman, @jobtype, @supervisor, @po, @tax_rate, @tax_descr, @sub_total, @tax_total, @inv_total)");
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception Ex)
            {
                System.Windows.MessageBox.Show(Ex.Message);
            }
        }

        public void DeleteInvoiceHeader(string invno)
        {
            try
            {
                //var start_collection = new ObservableCollection<QuoteItem>();
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = String.Format("DELETE FROM INV_HDR WHERE invno=\"{0}\"", invno);
                cmd.ExecuteNonQuery();


                //Do I need to reload Invoice data?
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void UpdateInvoiceExpandedItems(string invno)
        {
            foreach (InvoiceItem ii in invoice_items)
            {
                //Calculate subtotal, tax_total, and line_total
                CalculateTotals();

                if(ii.row_id.Equals(null) || ii.row_id == -1)
                {
                    try
                    {
                        connection = new SQLiteConnection(connection_String);
                        connection.Open();
                        cmd = connection.CreateCommand();

                        cmd.Parameters.Add(new SQLiteParameter("@qty", ii.qty));
                        cmd.Parameters.Add(new SQLiteParameter("@item", ii.item));
                        cmd.Parameters.Add(new SQLiteParameter("@rate", ii.rate));
                        cmd.Parameters.Add(new SQLiteParameter("@descr", ii.descr));
                        cmd.Parameters.Add(new SQLiteParameter("@type", ii.type));
                        cmd.Parameters.Add(new SQLiteParameter("@grouping", ii.grouping));
                        cmd.Parameters.Add(new SQLiteParameter("@taxable", ii.taxable));
                        cmd.Parameters.Add(new SQLiteParameter("@discountable", ii.discountable));
                        cmd.Parameters.Add(new SQLiteParameter("@printable", ii.printable));
                        cmd.Parameters.Add(new SQLiteParameter("@jobno", ii.jobno));
                        cmd.Parameters.Add(new SQLiteParameter("@line_total", ii.line_total));
                        cmd.Parameters.Add(new SQLiteParameter("@tax_total", ii.tax_total));
                        cmd.Parameters.Add(new SQLiteParameter("@cust", ii.cust));
                        cmd.Parameters.Add(new SQLiteParameter("@invdate", ii.invdate));

                        cmd.CommandText = String.Format("INSERT INTO INV_ITEMS (qty,item,rate,descr,grouping,taxable,discountable,printable,jobno,line_total,tax_total,cust,invdate)VALUES ((@qty),(@item),(@rate),(@descr),(@grouping),(@taxable),(@discountable),(@printable),(@jobno),(@line_total),(@tax_total),(@cust),(@invdate))");
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        connection = new SQLiteConnection(connection_String);
                        connection.Open();
                        cmd = connection.CreateCommand();

                        cmd.Parameters.Add(new SQLiteParameter("@row_id", ii.row_id));
                        cmd.Parameters.Add(new SQLiteParameter("@qty", ii.qty));
                        cmd.Parameters.Add(new SQLiteParameter("@item", ii.item));
                        cmd.Parameters.Add(new SQLiteParameter("@rate", ii.rate));
                        cmd.Parameters.Add(new SQLiteParameter("@descr", ii.descr));
                        cmd.Parameters.Add(new SQLiteParameter("@type", ii.type));
                        cmd.Parameters.Add(new SQLiteParameter("@grouping", ii.grouping));
                        cmd.Parameters.Add(new SQLiteParameter("@taxable", ii.taxable));
                        cmd.Parameters.Add(new SQLiteParameter("@discountable", ii.discountable));
                        cmd.Parameters.Add(new SQLiteParameter("@printable", ii.printable));
                        cmd.Parameters.Add(new SQLiteParameter("@jobno", ii.jobno));
                        cmd.Parameters.Add(new SQLiteParameter("@line_total", ii.line_total));
                        cmd.Parameters.Add(new SQLiteParameter("@tax_total", ii.tax_total));
                        cmd.Parameters.Add(new SQLiteParameter("@cust", ii.cust));
                        cmd.Parameters.Add(new SQLiteParameter("@invdate", ii.invdate));

                        cmd.CommandText = String.Format("UPDATE INV_ITEMS SET qty=(@qty), item=(@item), rate=(@rate), descr=(@descr), type=(@type), grouping=(@grouping),taxable=(@taxable), discountable=(@discountable), printable=(@printable), jobno=(@jobno), line_total=(@line_total), cust=(@cust), invdate=(@invdate) WHERE row_id=(@row_id)");
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
                

            }
        }

        public void InsertNewInvoiceItem(InvoiceItem ii)
        {
            try
            {
                Trace.WriteLine("Inserting New Item: " + ii.item);
                Trace.WriteLine("\tJobno:" + ii.jobno);
                Trace.WriteLine("\tInvno:" + ii.invno);
                Trace.WriteLine("\tType:" + ii.type);
                Trace.WriteLine("\tTaxable:" + ii.taxable.ToString());
                Trace.WriteLine("\tDiscountable:" + ii.discountable.ToString());
                Trace.WriteLine("\tPrintable:" + ii.printable.ToString());
                Trace.WriteLine("\tTax_Total:" + ii.tax_total.ToString());
                Trace.WriteLine("\tLine_Total:" + ii.line_total.ToString());

                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();

                cmd.Parameters.Add(new SQLiteParameter("@qty", ii.qty));
                cmd.Parameters.Add(new SQLiteParameter("@item", ii.item));
                cmd.Parameters.Add(new SQLiteParameter("@rate", ii.rate));
                cmd.Parameters.Add(new SQLiteParameter("@descr", ii.descr));
                cmd.Parameters.Add(new SQLiteParameter("@type", ii.type));
                cmd.Parameters.Add(new SQLiteParameter("@grouping", ii.grouping));
                cmd.Parameters.Add(new SQLiteParameter("@taxable", ii.taxable));
                cmd.Parameters.Add(new SQLiteParameter("@discountable", ii.discountable));
                cmd.Parameters.Add(new SQLiteParameter("@printable", ii.printable));
                cmd.Parameters.Add(new SQLiteParameter("@jobno", ii.jobno));
                cmd.Parameters.Add(new SQLiteParameter("@line_total", ii.line_total));
                cmd.Parameters.Add(new SQLiteParameter("@tax_total", ii.tax_total));
                cmd.Parameters.Add(new SQLiteParameter("@cust", ii.cust));
                cmd.Parameters.Add(new SQLiteParameter("@invno", ii.invno));
                cmd.Parameters.Add(new SQLiteParameter("@invdate", ii.invdate));

                cmd.CommandText = String.Format("INSERT into INV_ITEMS (qty, item, rate, descr, type, grouping, taxable, discountable, printable, jobno, line_total, tax_total, cust, invno, invdate) VALUES (@qty, @item, @rate, @descr, @type, @grouping, @taxable,@discountable,@printable,@jobno,@line_total,@tax_total,@cust,@invno, @invdate)");
                int rowsReturned = cmd.ExecuteNonQuery();
                connection.Close();
                Trace.WriteLine("Number of Rows Inserted: " + rowsReturned.ToString());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                System.Windows.MessageBox.Show(ex.InnerException.Message);
            }
        }

        public void DeleteInvoiceItemRow(int row_id)
        {
            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = String.Format("DELETE FROM INV_ITEMS WHERE row_id={0}", row_id);
                //cmd.CommandText = String.Format("DELETE FROM QTE_ITEMS WHERE jobno=\"{0}\" AND tab_index = {1}", jobno, tab_index, row_index);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public ObservableCollection<Customer> LoadCustomers()
        {
            var customers = new ObservableCollection<Customer>();

            try
            {
                connection = new SQLiteConnection(connection_String);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM CUSTOMER ORDER BY name" +
                    "");
                adapter = new SQLiteDataAdapter(cmd);

                ds = new DataSet();

                adapter.Fill(ds, "CUSTOMERS");


                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int cleaned_cust_id = -1;

                    int int_active = 0;
                    bool parsed_active = false;

                    if (Int32.TryParse(dr[0].ToString(), out cleaned_cust_id)) { }

                    if (Int32.TryParse(dr[9].ToString(), out int_active)) { }
                    if (int_active == 0) { parsed_active = false; } else { parsed_active = true; }

                    customers.Add(new Customer
                    {
                        cust_id = cleaned_cust_id,
                        name = dr[1].ToString(),
                        address1 = dr[2].ToString(),
                        address2 = dr[3].ToString(),
                        address3 = dr[4].ToString(),
                        city = dr[5].ToString(),
                        state = dr[6].ToString(),
                        zip = dr[7].ToString(),
                        terms = dr[8].ToString(),
                        active = parsed_active
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ds = null;
                adapter.Dispose();
                connection.Close();
                connection.Dispose();
            }

            return customers;
        }

        public Customer GetCustomerFromID(int cust_id_in)
        {
            IEnumerable<Customer> customer_list = customers.Where(c=>c.cust_id==cust_id_in);

            /*
            foreach(var temp in customer_list)
            {
                return temp;
            }
            */
            return customer_list.SingleOrDefault<Customer>();
        }

        public string GetNewInvoiceNumber()
        {
            int number = 300;
            string new_invno;

            number = number + 1;

            new_invno = "19-" + number.ToString();

            return new_invno;
        }

        #endregion

        #region Converters
        public class DollarConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                decimal d = decimal.Parse(value.ToString());

                string currency_string = string.Format("{0:C}", d);

                return currency_string;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {

                throw new NotImplementedException();
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handle = PropertyChanged;
            if (handle != null)
            {
                handle(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion






    }
}
