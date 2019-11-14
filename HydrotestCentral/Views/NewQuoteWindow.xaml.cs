using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SQLite;
using HydrotestCentral.Model;
using HydrotestCentral.Models;
using System.Collections.ObjectModel;
using HydrotestCentral.ViewModels;

namespace HydrotestCentral
{
    /// <summary>
    /// Interaction logic for NewQuoteWindow.xaml
    /// </summary>
    public partial class NewQuoteWindow : Window
    {
        public SQLiteConnection connection;
        public SQLiteDataAdapter dataAdapter;
        string connection_String = System.Configuration.ConfigurationManager.ConnectionStrings["connection_String"].ConnectionString;

        public static QuoteRepository main_QuoteRepository;
        public static MainWindowViewModel main_vm;

        public NewQuoteWindow(MainWindowViewModel incoming_vm)
        {
            InitializeComponent();

            //set the QuoteHeaderDataProvider
            //main_Quoteheader = new QuoteHeaderDataProvider();
            main_QuoteRepository = new QuoteRepository();
            main_vm = new MainWindowViewModel();
            main_vm = incoming_vm;
            DataContext = main_vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txt_jobno.Text = getNextJobNumber(getLastJobNumber());
            txt_qtdate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txt_status.Text = "";
        }

        public string getLastJobNumber()
        {
            connection = new SQLiteConnection(connection_String);
            connection.Open();
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("SELECT jobno FROM QTE_HDR ORDER BY jobno DESC LIMIT 1");
            string returnString = cmd.ExecuteScalar().ToString();
            connection.Close();

            return returnString;
        }

        public string getNextJobNumber(string lastJobNo)
        {
            char[] remChars = { 'A', 'T', 'H', 'S', '-' };

            string returnString = lastJobNo.TrimStart(remChars);
            if(returnString.StartsWith("C2019-"))
            {
                returnString = returnString.Remove(0, 6);
            }
            Trace.WriteLine("removed chars from lastJobNo: " + lastJobNo);
            int num = 0;
            if(Int32.TryParse(returnString, out num))
            {
                num += 1;
                returnString = "C2019-" + num.ToString();
            }
            else
            {
                MessageBox.Show("Error Getting Job Number!");
            }
            Trace.WriteLine("new job no created: " + returnString);

            return returnString;
        }

        private void Btn_AddQuote_Click(object sender, RoutedEventArgs e)
        {
            QuoteHeader headeritem = new QuoteHeader();
            headeritem.jobno = txt_jobno.Text;
            headeritem.qt_date = txt_qtdate.Text;
            headeritem.cust = txt_cust.Text;
            //headeritem.cust_contact = "NULL";
            //headeritem.cust_phone = "NULL";
            //headeritem.cust_email = "NULL";
            headeritem.loc = txt_loc.Text;
            headeritem.salesman = txt_salesman.Text;
            headeritem.days_est = Int32.Parse(txt_daysest.Text);
            headeritem.status = txt_status.Text;
            headeritem.jobtype = txt_jobtype.Text;
            headeritem.pipe_line_size = txt_pipelinesize.Text;
            headeritem.pipe_length = txt_pipelength.Text;
            headeritem.pressure = txt_pressure.Text;
            headeritem.endclient = txt_endclient.Text;
            headeritem.supervisor = txt_supervisor.Text;
            //headeritem.est_start_date = "NULL";
            //headeritem.est_stop_date = "NULL";
            headeritem.value = 0;
            main_QuoteRepository.AddNewHeaderItem(headeritem);
            main_vm.quote_headers = main_vm.LoadQuoteHeaderData();
            MessageBox.Show("Quote Added");
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
