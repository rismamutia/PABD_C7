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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CRUDMahasiswaADO
{
    public partial class MelihatReport : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-49331NDM\\RIANIINDRI;Initial Catalog=DBJadwalKoor;Integrated Security=True";

        private readonly SqlConnection conn;

        public MelihatReport()
        {
            InitializeComponent();
        }


        private void MelihatReport_Load(object sender, EventArgs e)
        {
            LoadReport();
            SetupGrid();
        }

        private void SetupGrid()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.FixedSingle;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;

            // Header style
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9f, FontStyle.Bold);
            dataGridView1.EnableHeadersVisualStyles = false;

            // Row height
            dataGridView1.RowTemplate.Height = 24;
        }

        private void LoadReport(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            NIM_Mahasiswa     AS [NIM],
                            Nama_Mahasiswa    AS [Nama Mahasiswa],
                            Nama_Dosen        AS [Nama Dosen],
                            Tanggal           AS [Tanggal],
                            WaktuMulai        AS [Jam Mulai],
                            WaktuSelesai      AS [Jam Selesai],
                            StatusPertemuan   AS [Status],
                            CatatanPermintaan AS [Catatan]
                        FROM ReportPertemuan
                        WHERE
                            Nama_Mahasiswa  LIKE @kw OR
                            NIM_Mahasiswa   LIKE @kw OR
                            Nama_Dosen      LIKE @kw OR
                            StatusPertemuan LIKE @kw
                        ORDER BY Tanggal DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;

                    // Format kolom Tanggal agar tampil dd/MM/yyyy
                    if (dataGridView1.Columns["Tanggal"] != null)
                        dataGridView1.Columns["Tanggal"].DefaultCellStyle.Format = "dd/MM/yyyy";

                    // Format kolom jam agar tampil HH:mm
                    foreach (string col in new[] { "Jam Mulai", "Jam Selesai" })
                        if (dataGridView1.Columns[col] != null)
                            dataGridView1.Columns[col].DefaultCellStyle.Format = @"hh\:mm";

                    // Lebar kolom Catatan lebih besar
                    if (dataGridView1.Columns["Catatan"] != null)
                    {
                        dataGridView1.Columns["Catatan"].AutoSizeMode =
                            DataGridViewAutoSizeColumnMode.Fill;
                        dataGridView1.Columns["Catatan"].FillWeight = 200;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load report: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadReport();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MelihatReport_Load_1(object sender, EventArgs e)
        {
            SetupGrid();
            LoadReport();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor =
                System.Drawing.Color.AliceBlue;
        }

        private void btnCari_Click(object sender, EventArgs e)
        {
            LoadReport(txtSearch.Text.Trim());
        }
    }
}
