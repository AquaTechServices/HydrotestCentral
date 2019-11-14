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
using HydrotestCentral.Model;
using System.Data;
using System.Windows;

namespace HydrotestCentral.ViewModels
{
    public partial class MainWindowViewModel: INotifyPropertyChanged
    {
        SQLiteConnection connection;
        SQLiteCommand cmd;
        SQLiteDataAdapter adapter;
        DataSet ds;
        string connection_String = System.Configuration.ConfigurationManager.ConnectionStrings["connection_String"].ConnectionString;

        private ObservableCollection<QuoteHeader> quote_header_data = null;

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

        public MainWindowViewModel()
        {
            InitializeComponent();

            quote_headers = new ObservableCollection<QuoteHeader>();
            quote_headers = LoadQuoteHeaderData();
            quote_items = new ObservableCollection<QuoteItem>();
            quote_items = LoadQuoteItemData();
        }

        public ObservableCollection<QuoteHeader> LoadQuoteHeaderData()
        {
            var headers = new ObservableCollection<QuoteHeader>();

            try
            {
                connection = new SQLiteConnection(connection_String);
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
                        int cleaned_group = 1;
                        bool cleaned_taxable = false;
                        bool cleaned_discountable = false;
                        bool cleaned_printable = false;
                        double cleaned_line_total = 0.00;
                        double cleaned_tax_total = 0.00;
                        int cleaned_tab_index = 0;
                        int cleaned_row_index = 0;

                        if (Int32.TryParse(dr[0].ToString(), out cleaned_qty)) { }
                        if (Double.TryParse(dr[2].ToString(), out cleaned_rate)) { }
                        if (Int32.TryParse(dr[4].ToString(), out cleaned_group)) { }
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
                            group = cleaned_group,
                            taxable = cleaned_taxable,
                            discountable = cleaned_discountable,
                            printable = cleaned_printable,
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

            foreach(QuoteHeader header in quote_headers)
            {
                if(header.jobno == jobno)
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

            quote_items = new_collection;
        }

        public void DeleteQuoteItem(String jobno, int tab_index)
        {
            try
            {
                var start_collection = new ObservableCollection<QuoteItem>();
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

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handle = PropertyChanged;
            if(handle != null)
            {
                handle(this, new PropertyChangedEventArgs(propertyName));
            }            
        }
        #endregion
    }
}
