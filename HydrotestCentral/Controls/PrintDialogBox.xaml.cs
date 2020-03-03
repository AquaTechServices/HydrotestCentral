using HydrotestCentral.Models;
using HydrotestCentral.ViewModels;
using MahApps.Metro.Controls;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
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

namespace HydrotestCentral.Controls
{
    /// <summary>
    /// Interaction logic for PrintDialogBox.xaml
    /// </summary>
    public partial class PrintDialogBox : MahApps.Metro.SimpleChildWindow.ChildWindow
    {
        public static MainWindowViewModel printdialog_vm;

        public PrintDialogBox(MainWindowViewModel main_vm)
        {
            InitializeComponent();

            printdialog_vm = main_vm;
        }

        private void btn_PrintSummary_Click(object sender, RoutedEventArgs e)
        {
            if (printdialog_vm.invoice_items != null)
            {
                printdialog_vm.grouped_invoice_items = printdialog_vm.GetCollapsedInvoiceItemsData(printdialog_vm.CurrentInvoiceHeader.invno);

                //Get File Name
                string filepath = "C:\\Users\\SFWMD\\Desktop\\";
                string filename = "INV_" + printdialog_vm.CurrentInvoiceHeader.invno + ".pdf";

                if (printdialog_vm.grouped_invoice_items != null)
                {
                    // Configure save file dialog box
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = filepath;
                    dlg.FileName = filename; // Default file name
                    dlg.DefaultExt = ".pdf"; // Default file extension
                    dlg.Filter = "PDF documents (.pdf)|*.pdf"; // Filter files by extension

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();
                    if(result==true)
                    {
                        PDFCreator pd = new PDFCreator();

                        // Create a MigraDoc document
                        Document document = pd.CreateDocument(printdialog_vm.CurrentInvoiceHeader, printdialog_vm.grouped_invoice_items);

                        //string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
                        MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

                        PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
                        renderer.Document = document;

                        renderer.RenderDocument();

                        // Save the document...
                        renderer.PdfDocument.Save(filename);
                        // ...and start a viewer.
                        Process.Start(filename);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a valid invoice!");
                }
            }
        }

        private void btn_PrintDetailed_Click(object sender, RoutedEventArgs e)
        {
            //Get File Name
            string filepath = "C:\\Users\\SFWMD\\Desktop\\";
            string filename = "INV_" + printdialog_vm.CurrentInvoiceHeader.invno + ".pdf";

            if (printdialog_vm.invoice_items != null)
            {
                // Configure save file dialog box
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.InitialDirectory = filepath;
                dlg.FileName = filename; // Default file name
                dlg.DefaultExt = ".pdf"; // Default file extension
                dlg.Filter = "PDF documents (.pdf)|*.pdf"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                if(result==true)
                {
                    PDFCreator pd = new PDFCreator();

                    // Create a MigraDoc document
                    Document document = pd.CreateDocument(printdialog_vm.CurrentInvoiceHeader, printdialog_vm.invoice_items);

                    //string ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
                    MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

                    PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
                    renderer.Document = document;

                    renderer.RenderDocument();

                    // Save the document...
                    renderer.PdfDocument.Save(filename);
                    // ...and start a viewer.
                    Process.Start(filename);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid invoice!");
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
