using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class HasilReport : Form
    {
        static string connectionString ="Data Source=erlinaaa\\ERLINASHAFIRA;Initial Catalog=DBJadwalKoor;Integrated Security=True";

        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtReport;

        public HasilReport(string dosenID, DateTime tanggal)
        {
            InitializeComponent();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd =
                    new SqlCommand("sp_ReportPertemuan", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@DosenID",
                    Convert.ToInt32(dosenID)
                );

                cmd.Parameters.AddWithValue(
                    "@Status",
                    "Completed"
                );

                cmd.Parameters.AddWithValue(
                    "@Tanggal",
                    tanggal.Date
                );

                dtReport = new DataTable();

                da = new SqlDataAdapter(cmd);

                da.Fill(dtReport);

                conn.Close();

                HasilCetak report = new HasilCetak();

                report.SetDataSource(dtReport);

                crystalReportViewer1.ReportSource = report;

                crystalReportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Gagal load data : " + ex.Message
                );
            }
        }
        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

    }
    }


