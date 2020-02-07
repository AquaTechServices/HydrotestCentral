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
        public ObservableCollection<Customer> customers { get; set; }

        public int selected_tab_index;
        public int selected_row_index;

        private string _jobno;
        private string _invno;

        public string Jobno { get { return _jobno; } set { _jobno = value; OnPropertyChanged("Jobno"); } }
        public string Invno { get { return _invno; } set { _invno = value; OnPropertyChanged("Invno"); } }

        public MainWindowViewModel()
        {
            InitializeComponent();

            quote_headers = new ObservableCollection<QuoteHeader>();
            quote_headers = LoadQuoteHeaderData();
            quote_items = new ObservableCollection<QuoteItem>();
            quote_items = LoadQuoteItemData();
            invoice_headers = new ObservableCollection<InvoiceHeader>();
            invoice_items = new ObservableCollection<InvoiceItem>();

            Jobno = "";
            Invno = "";
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
                    double cleaned_value = 0.00;


                    if (Int32.TryParse(dr[8].ToString(), out cleaned_days)) { }

                    if (Double.TryParse(dr[17].ToString(), out cleaned_value)) { }

                    //if(!string.IsNullOrEmpty(dr[1].ToString())){ cleaned_qt_date= DateTime.Parse(dr[1].ToString());}

                    //if(!string.IsNullOrEmpty(dr[16].ToString())){ cleaned_est_start_date= DateTime.Parse(dr[16].ToString());}
                    //if(!string.IsNullOrEmpty(dr[17].ToString())){ cleaned_est_stop_date= DateTime.Parse(dr[17].ToString());}

                    headers.Add(new QuoteHeader
                    {
                        quoteno = dr[0].ToString(),
                        jobno = dr[0].ToString(),
                        qt_date = dr[1].ToString(),
                        cust = dr[2].ToString(),
                        cust_contact = dr[3].ToString(),
                        cust_phone = dr[4].ToString(),
                        cust_email = dr[5].ToString(),
                        loc = dr[6].ToString(),
                        salesman = dr[7].ToString(),
                        days_est = cleaned_days,
                        status = dr[9].ToString(),
                        jobtype = dr[10].ToString(),
                        pipe_line_size = dr[11].ToString(),
                        pipe_length = dr[12].ToString(),
                        pressure = dr[13].ToString(),
                        endclient = dr[14].ToString(),
                        supervisor = dr[15].ToString(),
                        est_start_date = dr[16].ToString(), 
                        est_stop_date = dr[17].ToString(),
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

        public void UpdateHeaderItem(string jobno)
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

        public void DeleteHeaderItem(String jobno)
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
                    int cleaned_qty = 0;
                    int cleaned_group = 0;

                    double cleaned_rate = 0.00;
                    double cleaned_line_total = 0.00;
                    double cleaned_tax_total = 0.00;

                    DateTime cleaned_invdate = new DateTime();

                    bool parsed_taxable = false;
                    bool parsed_discountable = false;
                    bool parsed_printable = false;

                    if (Int32.TryParse(dr[0].ToString(), out cleaned_qty)) { }
                    if (Int32.TryParse(dr[5].ToString(), out cleaned_group)) { }

                    if (Double.TryParse(dr[2].ToString(), out cleaned_rate)) { }
                    if (Double.TryParse(dr[10].ToString(), out cleaned_line_total)) { }
                    if (Double.TryParse(dr[11].ToString(), out cleaned_tax_total)) { }

                    if (DateTime.TryParse(dr[2].ToString(), out cleaned_invdate)) { }

                    if (Boolean.TryParse(dr[6].ToString(), out parsed_taxable)) { }
                    if (Boolean.TryParse(dr[7].ToString(), out parsed_discountable)) { }
                    if (Boolean.TryParse(dr[8].ToString(), out parsed_printable)) { }

                    items.Add(new InvoiceItem
                    {
                        qty = cleaned_qty,
                        item = dr[1].ToString(),
                        rate = cleaned_rate,
                        descr = dr[3].ToString(),
                        type = dr[4].ToString(),
                        grouping = cleaned_group,
                        taxable = parsed_taxable,
                        discountable = parsed_discountable,
                        printable = parsed_printable,
                        jobno = dr[9].ToString(),
                        line_total = cleaned_line_total,
                        tax_total = cleaned_tax_total,
                        cust = dr[12].ToString(),
                        invno = dr[13].ToString(),
                        invdate = dr[14].ToString()
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
                    double cleaned_taxrate = 0.00;
                    double cleaned_subtotal = 0.00;
                    double cleaned_taxtotal = 0.00;
                    double cleaned_invtotal = 0.00;

                    DateTime cleaned_invdate = new DateTime();
                    DateTime cleaned_duedate = new DateTime();


                    if (Double.TryParse(dr[16].ToString(), out cleaned_taxrate)) { }
                    if (Double.TryParse(dr[18].ToString(), out cleaned_subtotal)) { }
                    if (Double.TryParse(dr[19].ToString(), out cleaned_taxtotal)) { }
                    if (Double.TryParse(dr[20].ToString(), out cleaned_invtotal)) { }

                    if (DateTime.TryParse(dr[2].ToString(), out cleaned_invdate)) { }
                    if (DateTime.TryParse(dr[3].ToString(), out cleaned_duedate)) { }


                    invoice_headers.Add(new InvoiceHeader
                    {
                        jobno = dr[0].ToString(),
                        invno = dr[1].ToString(),
                        invdate = cleaned_invdate.ToString("MM/dd/yy"),
                        duedate = cleaned_duedate.ToString("MM/dd/yy"),
                        terms = dr[4].ToString(),
                        cust = dr[5].ToString(),
                        cust_addr1 = dr[6].ToString(),
                        cust_addr2 = dr[7].ToString(),
                        cust_city = dr[8].ToString(),
                        cust_state = dr[9].ToString(),
                        cust_zip = dr[10].ToString(),
                        loc = dr[11].ToString(),
                        salesman = dr[12].ToString(),
                        jobtype = dr[13].ToString(),
                        supervisor = dr[14].ToString(),
                        po = dr[15].ToString(),
                        tax_rate = cleaned_taxrate,
                        tax_descr = dr[17].ToString(),
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

            return invoice_headers;
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
