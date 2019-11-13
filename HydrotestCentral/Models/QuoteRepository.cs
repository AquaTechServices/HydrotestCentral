using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.ComponentModel;
using HydrotestCentral.Model;
using System.Windows;

namespace HydrotestCentral.Models
{
    public class QuoteRepository
    {
        public List<Model.QuoteHeader> quoteheaderRepository { get; set; }
        public List<Model.QuoteItem> quoteitemRepository { get; set; }
        static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connection_String"].ConnectionString;

        public static string connString {get; set; }
        SQLiteCommand cmd;
        SQLiteDataAdapter adapter;
        DataSet ds;
        private SQLiteConnection connection;

        public QuoteRepository()
        {
            quoteheaderRepository = GetQuoteHeaderRepo();
            quoteitemRepository = GetQuoteItemRepo();
        }

        public List<QuoteHeader> GetQuoteHeaderRepo()
        {
            List<QuoteHeader> header_list = new List<QuoteHeader>();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in ->Properties-?Settings.settings");
                }

                SQLiteCommand query = new SQLiteCommand("SELECT * FROM QTE_HDR", conn);
                conn.Open();
                SQLiteDataAdapter sqlDataAdapter = new SQLiteDataAdapter(query);
                DataTable dt = new DataTable();
                sqlDataAdapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    QuoteHeader q = new QuoteHeader();
                    q.jobno = row["jobno"].ToString();
                    q.qt_date = row["qt_date"].ToString();
                    q.cust = row["cust"].ToString();
                    q.cust_contact = row["cust_contact"].ToString();
                    q.cust_phone = row["cust_phone"].ToString();
                    q.cust_email = row["cust_email"].ToString();
                    q.loc = row["loc"].ToString();
                    q.salesman = row["salesman"].ToString();
                    q.days_est = row["days_est"] is DBNull ? 0 : Convert.ToInt32(row["days_est"]);
                    q.status = row["status"].ToString();
                    q.pipe_line_size = row["pipe_line_size"].ToString();
                    q.pipe_length = row["pipe_length"].ToString();
                    q.pressure = row["pressure"].ToString();
                    q.endclient = row["endclient"].ToString();
                    q.supervisor = row["supervisor"].ToString();
                    q.est_start_date = row["est_start_date"].ToString();
                    q.est_end_date = row["est_end_date"].ToString();
                    q.value = row["value"] is DBNull ? 0 : Convert.ToDouble(row["value"]);

                    header_list.Add(q);
                }

                return header_list;
            }

        }

        public List<QuoteItem> GetQuoteItemRepo()
        {
            List<QuoteItem> item_list = new List<QuoteItem>();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in ->Properties-?Settings.settings");
                }

                SQLiteCommand query = new SQLiteCommand("SELECT * FROM QTE_ITEMS", conn);
                conn.Open();
                SQLiteDataAdapter sqlDataAdapter = new SQLiteDataAdapter(query);
                DataTable dt = new DataTable();
                sqlDataAdapter.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    QuoteItem q = new QuoteItem();
                    q.qty = Convert.ToInt32(row["qty"]);
                    q.item = row["item"].ToString();
                    q.rate = (double)row["rate"];
                    q.descr = row["descr"].ToString();
                    q.group = Convert.ToInt32(row["group"]);
                    q.taxable = Convert.ToBoolean(row["taxable"]);
                    q.discountable = Convert.ToBoolean(row["discountable"]);
                    q.printable = Convert.ToBoolean(row["printable"]);
                    q.jobno = row["jobno"].ToString();
                    q.line_total = (double)row["line_total"];
                    q.tax_total = (double)row["tax_total"];
                    q.tab_index = Convert.ToInt32(row["tab_index"]);
                    q.row_index = Convert.ToInt32(row["row_index"]);

                    item_list.Add(q);
                }

                return item_list;
            }
        }

        public void addNewRecord(QuoteHeader quoteRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in MovieCatalog->Properties-?Settings.settings");
                }
                else if (quoteRecord == null)
                    throw new Exception("The passed argument 'movieRecord' is null");

                SQLiteCommand query = new SQLiteCommand("addRecord", conn);
                conn.Open();
                query.CommandType = CommandType.StoredProcedure;
                SQLiteParameter param1 = new SQLiteParameter("jobno", SqlDbType.VarChar);
                SQLiteParameter param2 = new SQLiteParameter("qt_date", SqlDbType.VarChar);
                SQLiteParameter param3 = new SQLiteParameter("cust", SqlDbType.VarChar);
                SQLiteParameter param4 = new SQLiteParameter("cust_contact", SqlDbType.VarChar);
                SQLiteParameter param5 = new SQLiteParameter("cust_phone", SqlDbType.VarChar);
                SQLiteParameter param6 = new SQLiteParameter("cust_email", SqlDbType.VarChar);
                SQLiteParameter param7 = new SQLiteParameter("loc", SqlDbType.VarChar);
                SQLiteParameter param8 = new SQLiteParameter("salesman", SqlDbType.VarChar);
                SQLiteParameter param9 = new SQLiteParameter("days_est", SqlDbType.Int);
                SQLiteParameter param10 = new SQLiteParameter("status", SqlDbType.VarChar);
                SQLiteParameter param11 = new SQLiteParameter("pipe_line_size", SqlDbType.VarChar);
                SQLiteParameter param12 = new SQLiteParameter("pipe_length", SqlDbType.VarChar);
                SQLiteParameter param13 = new SQLiteParameter("pressure", SqlDbType.VarChar);
                SQLiteParameter param14 = new SQLiteParameter("endclient", SqlDbType.VarChar);
                SQLiteParameter param15 = new SQLiteParameter("supervisor", SqlDbType.VarChar);
                SQLiteParameter param16 = new SQLiteParameter("est_start_date", SqlDbType.VarChar);
                SQLiteParameter param17 = new SQLiteParameter("est_end_date", SqlDbType.VarChar);
                SQLiteParameter param18 = new SQLiteParameter("value", SqlDbType.Real);

                param1.Value = quoteRecord.jobno;
                param2.Value = quoteRecord.qt_date;
                param3.Value = quoteRecord.cust;
                param4.Value = quoteRecord.cust_contact;
                param5.Value = quoteRecord.cust_phone;
                param6.Value = quoteRecord.cust_email;
                param7.Value = quoteRecord.loc;
                param8.Value = quoteRecord.salesman;
                param9.Value = quoteRecord.days_est;
                param10.Value = quoteRecord.status;
                param11.Value = quoteRecord.pipe_line_size;
                param12.Value = quoteRecord.pipe_length;
                param13.Value = quoteRecord.pressure;
                param14.Value = quoteRecord.endclient;
                param15.Value = quoteRecord.supervisor;
                param16.Value = quoteRecord.est_start_date;
                param17.Value = quoteRecord.est_end_date;
                param18.Value = quoteRecord.value;

                query.Parameters.Add(param1);
                query.Parameters.Add(param2);
                query.Parameters.Add(param3);
                query.Parameters.Add(param4);
                query.Parameters.Add(param5);
                query.Parameters.Add(param6);
                query.Parameters.Add(param7);
                query.Parameters.Add(param8);
                query.Parameters.Add(param9);
                query.Parameters.Add(param10);
                query.Parameters.Add(param11);
                query.Parameters.Add(param12);
                query.Parameters.Add(param13);
                query.Parameters.Add(param14);
                query.Parameters.Add(param15);
                query.Parameters.Add(param16);
                query.Parameters.Add(param17);
                query.Parameters.Add(param18);

                query.ExecuteNonQuery();
            }
        }
        public void addNewRecord(QuoteItem quoteRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in MovieCatalog->Properties-?Settings.settings");
                }
                else if (quoteRecord == null)
                    throw new Exception("The passed argument 'movieRecord' is null");

                SQLiteCommand query = new SQLiteCommand("addRecord", conn);
                conn.Open();
                query.CommandType = CommandType.StoredProcedure;
                SQLiteParameter param1 = new SQLiteParameter("qty", SqlDbType.Int);
                SQLiteParameter param2 = new SQLiteParameter("item", SqlDbType.VarChar);
                SQLiteParameter param3 = new SQLiteParameter("rate", SqlDbType.Real);
                SQLiteParameter param4 = new SQLiteParameter("descr", SqlDbType.VarChar);
                SQLiteParameter param5 = new SQLiteParameter("group", SqlDbType.Int);
                SQLiteParameter param6 = new SQLiteParameter("taxable", SqlDbType.Bit);
                SQLiteParameter param7 = new SQLiteParameter("discountable", SqlDbType.Bit);
                SQLiteParameter param8 = new SQLiteParameter("printable", SqlDbType.Bit);
                SQLiteParameter param9 = new SQLiteParameter("jobno", SqlDbType.VarChar);
                SQLiteParameter param10 = new SQLiteParameter("line_total", SqlDbType.Real);
                SQLiteParameter param11 = new SQLiteParameter("tax_total", SqlDbType.Real);
                SQLiteParameter param12 = new SQLiteParameter("tab_index", SqlDbType.Int);
                SQLiteParameter param13 = new SQLiteParameter("row_index", SqlDbType.Int);

                param1.Value = quoteRecord.qty;
                param2.Value = quoteRecord.item;
                param3.Value = quoteRecord.rate;
                param4.Value = quoteRecord.descr;
                param5.Value = quoteRecord.group;
                param6.Value = quoteRecord.taxable;
                param7.Value = quoteRecord.discountable;
                param8.Value = quoteRecord.printable;
                param9.Value = quoteRecord.jobno;
                param10.Value = quoteRecord.line_total;
                param11.Value = quoteRecord.tax_total;
                param12.Value = quoteRecord.tab_index;
                param13.Value = quoteRecord.row_index;

                query.Parameters.Add(param1);
                query.Parameters.Add(param2);
                query.Parameters.Add(param3);
                query.Parameters.Add(param4);
                query.Parameters.Add(param5);
                query.Parameters.Add(param6);
                query.Parameters.Add(param7);
                query.Parameters.Add(param8);
                query.Parameters.Add(param9);
                query.Parameters.Add(param10);
                query.Parameters.Add(param11);
                query.Parameters.Add(param12);
                query.Parameters.Add(param13);

                query.ExecuteNonQuery();
            }
        }

        public void deleteRecord(QuoteHeader quoteRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null.");
                }

                SQLiteCommand query = new SQLiteCommand("deleteRecord", conn);
                conn.Open();
                query.CommandType = CommandType.StoredProcedure;
                SQLiteParameter param1 = new SQLiteParameter("jobno", SqlDbType.VarChar);
                param1.Value = quoteRecord.jobno;
                query.Parameters.Add(param1);

                query.ExecuteNonQuery();
            }
        }

        public void deleteRecord(QuoteItem quoteRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null.");
                }

                SQLiteCommand query = new SQLiteCommand("deleteRecord", conn);
                conn.Open();
                query.CommandType = CommandType.StoredProcedure;
                SQLiteParameter param1 = new SQLiteParameter("jobno", SqlDbType.VarChar);
                param1.Value = quoteRecord.jobno;
                query.Parameters.Add(param1);

                query.ExecuteNonQuery();
            }
        }

        public void updateRecord(QuoteHeader quoteRecord)
        {

        }

        public void updateRecord(QuoteItem quoteRecord)
        {

        }


        public void AddNewHeaderItem(QuoteHeader NewquoteHeaderItem)
        {
            try
            {
                connection = new SQLiteConnection(connectionString);
                connection.Open();
                cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM QTE_HDR");
                adapter = new SQLiteDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "QTE_HDR");
                DataRow HeaderTableRow = ds.Tables["QTE_HDR"].NewRow();
                HeaderTableRow["jobno"] = NewquoteHeaderItem.jobno;
                HeaderTableRow["qt_date"] = NewquoteHeaderItem.qt_date;
                HeaderTableRow["cust"] = NewquoteHeaderItem.cust;
                HeaderTableRow["cust_contact"] = NewquoteHeaderItem.cust_contact;
                HeaderTableRow["cust_phone"] = NewquoteHeaderItem.cust_phone;
                HeaderTableRow["cust_email"] = NewquoteHeaderItem.cust_email;
                HeaderTableRow["loc"] = NewquoteHeaderItem.loc;
                HeaderTableRow["salesman"] = NewquoteHeaderItem.salesman;
                HeaderTableRow["days_est"] = NewquoteHeaderItem.days_est;
                HeaderTableRow["status"] = NewquoteHeaderItem.status;
                HeaderTableRow["jobtype"] = NewquoteHeaderItem.jobtype;
                HeaderTableRow["pipe_line_size"] = NewquoteHeaderItem.pipe_line_size;
                HeaderTableRow["pipe_length"] = NewquoteHeaderItem.pipe_length;
                HeaderTableRow["pressure"] = NewquoteHeaderItem.pressure;
                HeaderTableRow["endclient"] = NewquoteHeaderItem.endclient;
                HeaderTableRow["supervisor"] = NewquoteHeaderItem.supervisor;
                HeaderTableRow["est_start_date"] = NewquoteHeaderItem.est_start_date;
                HeaderTableRow["est_end_date"] = NewquoteHeaderItem.est_end_date;
                HeaderTableRow["value"] = NewquoteHeaderItem.value;
                adapter.InsertCommand = new SQLiteCommandBuilder(adapter).GetInsertCommand();
                ds.Tables["QTE_HDR"].Rows.Add(HeaderTableRow);
                adapter.Update(ds, "QTE_HDR");
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
    }
}
