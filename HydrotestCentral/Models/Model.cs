using System;
using System.Collections.Generic;
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

    // A Source of Quote Header Data from SQLite Database




}
