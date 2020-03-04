using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using HydrotestCentral.ViewModels;

namespace HydrotestCentral.Controls
{
    /// <summary>
    /// Interaction logic for NewInvoiceWindow.xaml
    /// </summary>
    public partial class NewInvoiceWindow : MahApps.Metro.SimpleChildWindow.ChildWindow
    {
        public static MainWindowViewModel vm;

        public NewInvoiceWindow(ViewModels.MainWindowViewModel main_vm)
        {
            InitializeComponent();

            vm = main_vm;
        }

        private void CustDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.Customer customer = new Models.Customer();

            if(CustDropDown.SelectedItem!=null)
            {
                customer = (Models.Customer)CustDropDown.SelectedItem;

                rTxtBx_CustAddress.Document.Blocks.Clear();
                rTxtBx_CustAddress.AppendText(customer.address1 + "\n");
                if (customer.address2 != null || customer.address2 != "")
                {
                    rTxtBx_CustAddress.AppendText(customer.address2 + "\n");
                }
                if (customer.address3 != null || customer.address3 != "")
                {
                    rTxtBx_CustAddress.AppendText(customer.address3 + "\n");
                }
                rTxtBx_CustAddress.AppendText(customer.city);
                rTxtBx_CustAddress.AppendText(", ");
                rTxtBx_CustAddress.AppendText(customer.state);
                rTxtBx_CustAddress.AppendText(" ");
                rTxtBx_CustAddress.AppendText(customer.zip);

                txtBx_Terms.Text = customer.terms;

                Calculate_Due_Date();
            }
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(vm.CurrentInvoiceHeader!=null)
            {
                // Get Newest Invoice Number
                txtBx_Invno.Text = vm.GetNewInvoiceNumber();

                txtBox_jobno.Text = vm.CurrentInvoiceHeader.jobno;
                txtBx_po.Text = vm.CurrentInvoiceHeader.po;
                txtBx_salesman.Text = vm.CurrentInvoiceHeader.salesman;
                txtBx_loc.Text = vm.CurrentInvoiceHeader.loc;

                if(!vm.CurrentInvoiceHeader.cust_id.Equals(-1))
                {
                    // Get customer with that cust_id
                    Models.Customer customer = new Models.Customer();

                    customer = vm.GetCustomerFromID(vm.CurrentInvoiceHeader.cust_id);

                    if(customer!=null)
                    {
                        //MessageBox.Show(customer.name);
                        CustDropDown.SelectedItem = customer;
                        

                        rTxtBx_CustAddress.Document.Blocks.Clear();
                        rTxtBx_CustAddress.AppendText(customer.address1 + "\n");
                        if (customer.address2 != null || customer.address2 != "")
                        {
                            rTxtBx_CustAddress.AppendText(customer.address2 + "\n");
                        }
                        if (customer.address3 != null || customer.address3 != "")
                        {
                            rTxtBx_CustAddress.AppendText(customer.address3 + "\n");
                        }
                        rTxtBx_CustAddress.AppendText(customer.city);
                        rTxtBx_CustAddress.AppendText(", ");
                        rTxtBx_CustAddress.AppendText(customer.state);
                        rTxtBx_CustAddress.AppendText(" ");
                        rTxtBx_CustAddress.AppendText(customer.zip);

                        txtBx_Terms.Text = customer.terms;

                        Calculate_Due_Date();
                    }
                }
                Calculate_Due_Date();
            }
            else
            {
                // Get Newest Invoice Number
                txtBx_Invno.Text = vm.GetNewInvoiceNumber();

                txtBox_jobno.Text = "";
                txtBx_po.Text = "";
                txtBx_salesman.Text = "";
                txtBx_loc.Text = "";

                Calculate_Due_Date();
            }
        }

        public void Calculate_Due_Date()
        {
            Trace.WriteLine("Calculating Due Date...");
            Trace.WriteLine(txtBx_Terms.Text);
            if(dp_invdate.SelectedDate!=null && txtBx_Terms.Text!="")
            {
                string terms_string = txtBx_Terms.Text;
                Trace.WriteLine("terms_string", terms_string);
                switch (terms_string)
                {
                    case "Due on receipt":
                        dp_duedate.SelectedDate = dp_invdate.SelectedDate;
                        break;
                    case "Net 30":
                        dp_duedate.SelectedDate = dp_invdate.SelectedDate.Value.AddDays(30);
                        break;
                    case "Net 45":
                        dp_duedate.SelectedDate = dp_invdate.SelectedDate.Value.AddDays(45);
                        break;
                    case "Net 60":
                        dp_duedate.SelectedDate = dp_invdate.SelectedDate.Value.AddDays(60);
                        break;
                    default:
                        dp_duedate.SelectedDate = dp_invdate.SelectedDate.Value.AddDays(30);
                        break;
                }
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {

            if (txtBox_jobno.Text.Equals(""))
            {
                MessageBox.Show("Please provide a Jobno");
            }
            if (txtBx_Invno.Text.Equals(""))
            {
                MessageBox.Show("Please provide an Invno");
            }

            try
            {
                if (!txtBx_Invno.Equals("") && !txtBox_jobno.Equals(""))
                {
                    vm.Invno = txtBx_Invno.Text;
                    vm.Jobno = txtBox_jobno.Text;

                    Models.InvoiceHeader inv_hdr = new Models.InvoiceHeader();
                    inv_hdr.invno = txtBx_Invno.Text;
                    inv_hdr.jobno = txtBox_jobno.Text;
                    inv_hdr.invdate = dp_invdate.SelectedDate.Value.Date;
                    inv_hdr.duedate = dp_duedate.SelectedDate.Value.Date;
                    inv_hdr.terms = txtBx_Terms.Text;

                    Models.Customer x = CustDropDown.SelectedItem as Models.Customer;
                    inv_hdr.cust = x.name;
                    inv_hdr.cust_id = x.cust_id;
                    inv_hdr.cust_addr1 = x.address1;
                    inv_hdr.cust_addr2 = x.address2;
                    inv_hdr.cust_city = x.city;
                    inv_hdr.cust_state = x.state;
                    inv_hdr.cust_zip = x.zip;
                    inv_hdr.loc = txtBx_loc.Text;
                    inv_hdr.salesman = txtBx_salesman.Text;
                    inv_hdr.po = txtBx_po.Text;
                    inv_hdr.sub_total = 0.00;
                    inv_hdr.tax_total = 0.00;
                    inv_hdr.inv_total = 0.00;

                    // Add to Invoice Headers
                    vm.invoice_headers.Add(inv_hdr);

                    // Add to database
                    vm.InsertInvoiceHeader(inv_hdr);

                    // Set Current Invoice Header to this one
                    vm.CurrentInvoiceHeader = inv_hdr;
                }
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           


        }
    }
}
