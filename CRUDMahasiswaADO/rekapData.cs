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
    public partial class rekapData : Form
    {
        static string connectionString =
"Data Source=erlinaaa\\ERLINASHAFIRA;Initial Catalog=DBJadwalKoor;Integrated Security=True";

        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtDosen;
        DataTable dtReport;


        public rekapData()
        {
            InitializeComponent();
        }

        private void rekapData_Load(object sender, EventArgs e)
        {
            dtpMasuk.Format = DateTimePickerFormat.Custom;
            dtpMasuk.CustomFormat = "yyyy-MM-dd";
            dtpMasuk.ShowUpDown = false;
            dtpMasuk.MinDate = new DateTime(2000, 1, 1);
            //dtpMasuk.MaxDate = DateTime.Now;

            cmbDosen.DropDownStyle = ComboBoxStyle.DropDownList;

            btnCetak.Enabled = false;

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("SELECT DosenID, Nama FROM Dosen", conn);

                cmd.CommandType = CommandType.Text;

                dtDosen = new DataTable();

                da = new SqlDataAdapter(cmd);

                da.Fill(dtDosen);

                cmbDosen.DataSource = dtDosen;
                cmbDosen.DisplayMember = "Nama";
                cmbDosen.ValueMember = "DosenID";

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                "Gagal load data : " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }


                SqlCommand cmd = new SqlCommand("sp_ReportPertemuan", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DosenID", cmbDosen.SelectedValue);

                cmd.Parameters.AddWithValue("@Status", "Completed");

                cmd.Parameters.AddWithValue("@Tanggal", dtpMasuk.Value.Date);

                da = new SqlDataAdapter(cmd);

                dtReport = new DataTable();

                da.Fill(dtReport);

                dataGridView1.DataSource = dtReport;

                conn.Close();

                if (dtReport.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = false;

                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data : " + ex.Message);
            }
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            HasilReport frm =
        new HasilReport(
            cmbDosen.SelectedValue.ToString(),
            dtpMasuk.Value.Date
        );

            frm.Show();

            this.Hide();
        }
        private void dtpMasuk_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
           Dashboard frm =
            new Dashboard();

            frm.Show();

            this.Hide();
        }
    }
}

