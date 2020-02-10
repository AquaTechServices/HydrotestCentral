using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace HydrotestCentral.Models
{
    class PDFCreator
    {
        public Document document { get; set; }
        public TextFrame addressFrame { get; private set; }
        public TextFrame invoiceInfoFrame { get; private set; }
        public TextFrame remitToFrame { get; private set; }
        public Table table { get; private set; }
        public Color TableBorder { get; private set; }
        public Color TableBlue { get; private set; }
        public IEnumerable<object> navigator { get; private set; }
        public Color TableGray { get; private set; }
        public string invoice_notes { get; private set; }

        public Document CreateDocument(InvoiceHeader ih)
        {
            // Create a new MigraDoc document
            this.document = new Document();
            this.document.Info.Title = "Test Invoice";
            this.document.Info.Subject = "Demonstrates how to create an invoice.";
            this.document.Info.Author = "Hydrotest Pros";

            this.invoice_notes = "This is test notes";

            DefineStyles();

            CreatePage();

            if(ih != null)
            {
                FillContent(ih);
            }
            else
            {

            }  

            return this.document;
        }

        void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = this.document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        void CreatePage()
        {
            // Each MigraDoc document needs at least one section.
            Section section = this.document.AddSection();

            // Put a logo in the header
            Image image = section.Headers.Primary.AddImage("../../Assets/hydrotestpros_logo.jpg");
            image.Height = "3cm";
            image.LockAspectRatio = true;
            image.RelativeVertical = RelativeVertical.Line;
            image.RelativeHorizontal = RelativeHorizontal.Margin;
            image.Top = ShapePosition.Top;
            image.Left = ShapePosition.Right;
            image.WrapFormat.Style = WrapStyle.Through;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Hydrotest Pros · 1048 Carlton Rd, Broussard, LA · 70518");
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            
            // Create the text frame for the address
            this.addressFrame = section.AddTextFrame();
            this.addressFrame.Height = "3.0cm";
            this.addressFrame.Width = "7.0cm";
            this.addressFrame.Left = ShapePosition.Left;
            this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            this.addressFrame.Top = "1.0cm";

            // Put sender in address frame
            paragraph = this.addressFrame.AddParagraph("Hydrotest Pros · 1048 Carlton Rd · Broussard, LA · 70518");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 8;
            paragraph.Format.SpaceAfter = 3;

            paragraph = this.addressFrame.AddParagraph("AR@hydrotestpros.com");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 8;
            paragraph.Format.SpaceAfter = 3;

            paragraph = this.addressFrame.AddParagraph("Phone # 337-999-1001 · Fax # 337-999-1002");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 8;
            paragraph.Format.SpaceAfter = 6;

            paragraph.AddLineBreak();

            // Create the text frame for the invoice info
            this.invoiceInfoFrame = section.AddTextFrame();
            this.invoiceInfoFrame.Width = "4.0 cm";
            this.invoiceInfoFrame.RelativeVertical = RelativeVertical.Line;
            this.invoiceInfoFrame.RelativeHorizontal = RelativeHorizontal.Column;
            this.invoiceInfoFrame.Top = ShapePosition.Top;
            this.invoiceInfoFrame.Left = ShapePosition.Right;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "1cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("INVOICE", TextFormat.Bold);
            //paragraph.AddTab();
            //paragraph.AddText("Broussard, ");
            //paragraph.AddDateField("dd/MM/yyyy");

            float sectionWidth = section.PageSetup.PageWidth - section.PageSetup.LeftMargin - section.PageSetup.RightMargin;


            // Create the item table
            this.table = section.AddTable();
            this.table.Style = "Table";
            this.table.Borders.Color = TableBorder;
            this.table.Borders.Width = 0.25;
            this.table.Borders.Left.Width = 0.5;
            this.table.Borders.Right.Width = 0.5;
            this.table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            Column column = this.table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            column = this.table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            column = this.table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            column = this.table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;

            column = this.table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = TableBlue;
            row.Cells[0].AddParagraph("Item");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].MergeDown = 1;
            row.Cells[0].Column.Width = 60;
            row.Cells[1].AddParagraph("Services/Equipment/Header");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].MergeRight = 3;
            row.Cells[5].AddParagraph("Extended Price");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[5].MergeDown = 1;
            row.Cells[5].Column.Width = 100;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = TableBlue;
            row.Cells[1].AddParagraph("Quantity");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].Column.Width = 60;
            row.Cells[2].AddParagraph("Description");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].Column.Width = 120;
            row.Cells[3].AddParagraph("Price Each");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].Column.Width = 60;
            row.Cells[4].AddParagraph("Taxable");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].Column.Width = 60;

            this.table.SetEdge(0, 0, 6, 2, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        void FillContent(InvoiceHeader ih)
        {
            // Fill address in address text frame
            //XPathNavigator item = SelectItem("/invoice/to");
            Paragraph paragraph = this.addressFrame.AddParagraph();
            paragraph.AddText(ih.cust);
            paragraph.AddLineBreak();
            paragraph.AddText(ih.cust_addr1);
            paragraph.AddLineBreak();
            if(ih.cust_addr2!="")
            {
                paragraph.AddText(ih.cust_addr2);
                paragraph.AddLineBreak();
            }
            paragraph.AddText(ih.cust_city + ", " + ih.cust_state + " " + ih.cust_zip);

            Table table = this.invoiceInfoFrame.AddTable();
            table.Format.Alignment = ParagraphAlignment.Right;
            table.Format.Font.Size = 8;
            Column col = table.AddColumn();
            col.Format.Alignment = ParagraphAlignment.Left;
            col = table.AddColumn();
            col.Format.Alignment = ParagraphAlignment.Left;
            Row row1 = table.AddRow();
            row1.Cells[0].AddParagraph("Invoice: ");
            row1.Cells[1].AddParagraph(ih.invno);
            Row row2 = table.AddRow();
            row2.Cells[0].AddParagraph("Invoice Date: ");
            row2.Cells[1].AddParagraph(ih.invdate);
            Row row3 = table.AddRow();
            row3.Cells[0].AddParagraph("Terms: ");
            row3.Cells[1].AddParagraph(ih.terms);
            Row row4 = table.AddRow();
            row4.Cells[0].AddParagraph("Due Date: ");
            row4.Cells[1].AddParagraph(ih.duedate);


            // Iterate the invoice items
            double totalExtendedPrice = 0;
            //XPathNodeIterator iter = this.navigator.Select("/invoice/items/*");
            /*while (iter.MoveNext())
            {
                item = iter.Current;
                double quantity = 0.00;
                double price = 0.00;
                double discount = 0.00;

                // Each item fills two rows
                Row row1 = this.table.AddRow();
                Row row2 = this.table.AddRow();
                row1.TopPadding = 1.5;
                row1.Cells[0].Shading.Color = TableGray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[0].MergeDown = 1;
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[1].MergeRight = 3;
                row1.Cells[5].Shading.Color = TableGray;
                row1.Cells[5].MergeDown = 1;

                row1.Cells[0].AddParagraph("itemNumber");
                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddFormattedText("title"), TextFormat.Bold);
                paragraph.AddFormattedText(" by ", TextFormat.Italic);
                paragraph.AddText("author");
                row2.Cells[1].AddParagraph("quantity"));
                row2.Cells[2].AddParagraph(price.ToString("0.00") + " $");
                row2.Cells[3].AddParagraph(discount.ToString("0.0"));
                row2.Cells[4].AddParagraph();
                row2.Cells[5].AddParagraph(price.ToString("0.00"));
                double extendedPrice = quantity * price;
                extendedPrice = extendedPrice * (100 - discount) / 100;
                row1.Cells[5].AddParagraph(extendedPrice.ToString("0.00") + " $");
                row1.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
                totalExtendedPrice += extendedPrice;

                this.table.SetEdge(0, this.table.Rows.Count - 2, 6, 2, Edge.Box, BorderStyle.Single, 0.75);
            }
            */

            // Add an invisible row as a space line to the table
            Row row = this.table.AddRow();
            row.Borders.Visible = false;

            // Add the subtotal row
            row = this.table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("Subtotal");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;
            row.Cells[5].AddParagraph("$" + ih.sub_total.ToString("0.00"));

            // Add the sales tax row
            row = this.table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("Sales Tax (0.0%)");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;
            row.Cells[5].AddParagraph("$" + ih.tax_total.ToString("0.00"));

            // Add the additional fee row
            ////row = this.table.AddRow();
            //row.Cells[0].Borders.Visible = false;
            //row.Cells[0].AddParagraph("Shipping and Handling");
            //row.Cells[5].AddParagraph("$" + 0.ToString("0.00"));
            //row.Cells[0].Format.Font.Bold = true;
            //row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            //row.Cells[0].MergeRight = 4;

            // Add the total due row
            row = this.table.AddRow();
            row.Cells[0].AddParagraph("Total Due");
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;
            //totalExtendedPrice += 0.19 * totalExtendedPrice;
            row.Cells[5].AddParagraph("$" + ih.inv_total.ToString("0.00"));

            // Set the borders of the specified cell range
            this.table.SetEdge(5, this.table.Rows.Count - 4, 1, 4, Edge.Box, BorderStyle.Single, 0.75);

            // Add the notes paragraph
            if(this.invoice_notes !="")
            {
                paragraph = this.document.LastSection.AddParagraph();
                paragraph.Format.SpaceBefore = "1cm";
                paragraph.Format.Borders.Width = 0.75;
                paragraph.Format.Borders.Distance = 3;
                paragraph.Format.Borders.Color = TableBorder;
                paragraph.Format.Shading.Color = TableGray;
                //item = SelectItem("/invoice");
                paragraph.AddText(invoice_notes);
            }
        }
    }
}
