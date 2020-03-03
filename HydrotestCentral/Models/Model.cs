using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.ComponentModel;
using HydrotestCentral.ViewModels;
using System.Windows;
using System.Collections.ObjectModel;

using System.Diagnostics;
//using HydrotestCentral.DatasetTableAdapters;

namespace HydrotestCentral.Models
{
    // Data object classes
    public class QuoteHeader
    {
        public string quoteno { get; set; }
        public string jobno { get; set; }
        public string qt_date { get; set; }
        public string cust { get; set; }
        public int cust_id { get; set; }
        public string cust_contact { get; set; }
        public string cust_phone { get; set; }
        public string cust_email { get; set; }
        public string loc { get; set; }
        public string salesman { get; set; }
        public int days_est { get; set; }
        public string status { get; set; }
        public string jobtype { get; set; }
        public string pipe_line_size { get; set; }
        public string pipe_length { get; set; }
        public string pressure { get; set; }
        public string endclient { get; set; }
        public string supervisor { get; set; }
        public string est_start_date { get; set; }
        public string est_stop_date { get; set; }
        public double value { get; set; }
    }

    public class QuoteItem
    {
        public int qty { get; set; }
        public string item { get; set; }
        public double rate { get; set; }
        public string descr { get; set; }
        public int grouping { get; set; }
        public bool taxable { get; set; }
        public bool discountable { get; set; }
        public bool printable { get; set; }
        public string jobno { get; set; }
        public string quoteno { get; set; }
        public double line_total { get; set; }
        public double tax_total { get; set; }
        public int tab_index { get; set; }
        public int row_index { get; set; }

    }

    public class InventoryItem
    {
        public string item { get; set; }
        public string descr { get; set; }
        public double rate { get; set; }
    }

    public class InvoiceHeader
    {
        public string jobno { get; set; }
        public string invno { get; set; }
        public DateTime invdate { get; set; }
        public DateTime duedate { get; set; }
        public string terms { get; set; }
        public string cust { get; set; }
        public int cust_id { get; set; }
        public string cust_addr1 { get; set; }
        public string cust_addr2 { get; set; }
        public string cust_city { get; set; }
        public string cust_state { get; set; }
        public string cust_zip { get; set; }
        public string loc { get; set; }
        public string salesman { get; set; }
        public string jobtype { get; set; }
        public string supervisor { get; set; }
        public string po { get; set; }
        public double tax_rate { get; set; }
        public string tax_descr { get; set; }
        public double sub_total { get; set; }
        public double tax_total { get; set; }
        public double inv_total { get; set; }

    }

    public class InvoiceItem:INotifyPropertyChanged
    {
        private double _line_total;
        private double _tax_total;

        public int row_id { get; set; }
        public int qty { get; set; }
        public string item { get; set; }
        public double rate { get; set; }
        public string descr { get; set; }
        public string type { get; set; }
        public int grouping { get; set; }
        public bool taxable { get; set; }
        public bool discountable { get; set; }
        public bool printable { get; set; }
        public string jobno { get; set; }
        public double line_total
        {
            get { return _line_total;}
            set
            {
                if (value != _line_total)
                {
                    _line_total = value;
                    OnPropertyChanged("line_total");
                }
            }
        }
        public double tax_total
        {
            get { return _tax_total; }
            set
            {
                if (value != _tax_total)
                {
                    _tax_total = value;
                    OnPropertyChanged("tax_total");
                }
            }
        }
        public string cust {get; set;}
        public string invno {get; set;}
        public string invdate {get; set;}

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

    public class Customer
    {
        public int cust_id { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public string qb_id { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string terms { get; set; }
        public bool active { get; set; }
        public string edit_sequence { get; set; }
    }

    public class Job
    {
        public string jobno { get; set; }
        public int cust_id { get; set; }
        public string cust { get; set; }
        public string status { get; set; }
        public string qt_date { get; set; }
        public string salesman { get; set; }
        public string jobtype { get; set; }
        public string qb_id { get; set; }
    }
}
