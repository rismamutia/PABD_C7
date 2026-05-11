using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq.Expressions;

namespace CRUDMahasiswaADO
{
    public partial class FormDosen : Form
    {
        int SelectedID = 0;
        int SelectedDosenID = 0;

        public int userID;
        public string nama;
        public string email;
        public string role;

        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=LAPTOP-49331NDM\\RIANIINDRI;Initial Catalog=DBJadwalKoor;Integrated Security=True";
        public FormDosen()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
            dataGridView1.CellClick += dataGridView1_CellClick;

        }

        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                MessageBox.Show("Koneksi berhasil");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectDatabase();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                // QUERY YANG DIUBAH:
                string query = @"
            SELECT j.JadwalID, j.DosenID, d.NIDN, d.Nama, 
                   j.Tanggal, j.WaktuMulai, j.WaktuSelesai, j.Status
            FROM JadwalDosen j
            JOIN Dosen d ON j.DosenID = d.DosenID
            WHERE j.Tanggal >= CAST(GETDATE() AS DATE) 
              AND j.Tanggal <= DATEADD(month, 6, GETDATE())
            ORDER BY j.Tanggal ASC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menampilkan data: " + ex.Message);
            }
        }

        private bool Validasi()
        {
            if (string.IsNullOrWhiteSpace(cmbStatus.Text))
            { MessageBox.Show("Status harus dipilih!"); cmbStatus.Focus(); return false; }

            if (dtpWaktuMulai.Value.TimeOfDay >= dtpWaktuSelesai.Value.TimeOfDay)
            {
                MessageBox.Show("Logika waktu salah! Waktu selesai harus lebih besar dari waktu mulai.");
                return false;
            }

            return true;
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (SelectedDosenID == 0) { MessageBox.Show("Pilih dosen!"); return; }
            if (!ValidasiLogikaInput()) return;

            // Cek Bentrok
            if (IsJadwalBentrok(SelectedDosenID, dtpTanggalKetersediaan.Value, dtpWaktuMulai.Value.TimeOfDay, dtpWaktuSelesai.Value.TimeOfDay))
            {
                MessageBox.Show("Dosen sudah memiliki jadwal lain di jam yang bersinggungan!");
                return;
            }

            if (SelectedDosenID == 0)
            {
                MessageBox.Show("Pilih dosen terlebih dahulu!");
                return;
            }

            if (dtpWaktuMulai.Value.TimeOfDay >= dtpWaktuSelesai.Value.TimeOfDay)
            {
                MessageBox.Show("Waktu selesai harus lebih besar dari waktu mulai!");
                return;
            }

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                string query = @"INSERT INTO JadwalDosen (DosenID, Tanggal, WaktuMulai, WaktuSelesai, Status) 
                         VALUES (@DosenID, @Tanggal, @Mulai, @Selesai, @Status)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DosenID", SelectedDosenID);
                cmd.Parameters.AddWithValue("@Tanggal", dtpTanggalKetersediaan.Value.Date);
                cmd.Parameters.AddWithValue("@Mulai", dtpWaktuMulai.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@Selesai", dtpWaktuSelesai.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@Status", cmbStatus.Text);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Jadwal berhasil ditambahkan!");
                    ClearForm();
                    btnLoad.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (SelectedDosenID == 0) { MessageBox.Show("Pilih dosen!"); return; }
            if (!ValidasiLogikaInput()) return;

            // Cek Bentrok
            if (IsJadwalBentrok(SelectedDosenID, dtpTanggalKetersediaan.Value, dtpWaktuMulai.Value.TimeOfDay, dtpWaktuSelesai.Value.TimeOfDay))
            {
                MessageBox.Show("Dosen sudah memiliki jadwal lain di jam yang bersinggungan!");
                return;
            }

            if (SelectedID == 0)
            {
                MessageBox.Show("Pilih data di tabel terlebih dahulu!");
                return;
            }

            if (!Validasi()) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                string queryDosen = @"UPDATE Dosen 
                            SET NIDN = @NIDN, 
                                Nama = @Nama 
                            WHERE DosenID = @DosenID";

                SqlCommand cmdDosen = new SqlCommand(queryDosen, conn);
                cmdDosen.Parameters.AddWithValue("@DosenID", SelectedDosenID);
                cmdDosen.ExecuteNonQuery();

                string queryJadwal = @"UPDATE JadwalDosen 
                             SET Tanggal = @Tanggal, 
                                 WaktuMulai = @Mulai, 
                                 WaktuSelesai = @Selesai, 
                                 Status = @Status 
                             WHERE JadwalID = @JadwalID";

                SqlCommand cmdJadwal = new SqlCommand(queryJadwal, conn);
                cmdJadwal.Parameters.AddWithValue("@Tanggal", dtpTanggalKetersediaan.Value.Date);
                cmdJadwal.Parameters.AddWithValue("@Mulai", dtpWaktuMulai.Value.TimeOfDay);
                cmdJadwal.Parameters.AddWithValue("@Selesai", dtpWaktuSelesai.Value.TimeOfDay);
                cmdJadwal.Parameters.AddWithValue("@Status", cmbStatus.Text);
                cmdJadwal.Parameters.AddWithValue("@JadwalID", SelectedID);

                int result = cmdJadwal.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Jadwal Dosen berhasil diperbarui!");
                    ClearForm();
                    SelectedID = 0;
                    SelectedDosenID = 0;
                    btnLoad.PerformClick();
                }
                else
                {
                    MessageBox.Show("Gagal memperbarui data.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();

                    if (SelectedID == 0)
                    {
                        MessageBox.Show("Pilih data dulu di tabel!");
                        return;
                    }
                }

                DialogResult resultConfirm = MessageBox.Show(
                    "Apakah Anda yakin ingin menghapus data ini?",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultConfirm == DialogResult.Yes)
                {
                    string query = "DELETE FROM JadwalDosen WHERE JadwalID = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", SelectedID);
                    cmd.Parameters.AddWithValue("@tgl", dtpTanggalKetersediaan.Value.Date);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Data dosen berhasil dihapus");
                        ClearForm();
                        btnLoad.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("Gagal menghapus data");
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                SelectedID = Convert.ToInt32(row.Cells["JadwalID"].Value);
                SelectedDosenID = Convert.ToInt32(row.Cells["DosenID"].Value);

                cmbStatus.Text = row.Cells["Status"].Value?.ToString();

                if (row.Cells["Tanggal"].Value != DBNull.Value)
                    dtpTanggalKetersediaan.Value = Convert.ToDateTime(row.Cells["Tanggal"].Value);

                if (row.Cells["WaktuMulai"].Value != DBNull.Value)
                    dtpWaktuMulai.Value = DateTime.Today.Add((TimeSpan)row.Cells["WaktuMulai"].Value);

                if (row.Cells["WaktuSelesai"].Value != DBNull.Value)
                    dtpWaktuSelesai.Value = DateTime.Today.Add((TimeSpan)row.Cells["WaktuSelesai"].Value);


            }
        }

        private void ClearForm()
        {
            cmbDosen.SelectedIndex = 0;   // reset ke dosen pertama
            txtNIDN.Clear();
            SelectedDosenID = 0;
            SelectedID = 0;
            dtpTanggalKetersediaan.Value = DateTime.Now;
            dtpWaktuMulai.Value = DateTime.Now;
            dtpWaktuSelesai.Value = DateTime.Now;
            cmbStatus.SelectedIndex = 0;
        }

        private void FormDosen_Load(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Available");
            cmbStatus.Items.Add("Unavailable");
            cmbStatus.Items.Add("Booked");
            cmbStatus.SelectedIndex = 0;

            LoadDosen(); // ← tambahkan ini

            dtpTanggalKetersediaan.Value = DateTime.Now;
            dtpWaktuMulai.Value = DateTime.Now;
            dtpWaktuSelesai.Value = DateTime.Now;

            btnLoad.PerformClick();
        }

        private void cmbJK_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void FormDosen_Load_1(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                string query = "SELECT DosenID, NIDN, Nama FROM Dosen ORDER BY Nama";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dtpTanggalKetersediaan.MinDate = DateTime.Today;
                dtpTanggalKetersediaan.MaxDate = DateTime.Today.AddMonths(6);

                cmbDosen.DataSource = dt;
                cmbDosen.DisplayMember = "Nama";   // yang ditampilkan
                cmbDosen.ValueMember = "DosenID";  // yang disimpan sebagai value
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load dosen: " + ex.Message);
            }
        }

        private void LoadDosen()
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                string query = "SELECT DosenID, NIDN, Nama FROM Dosen ORDER BY Nama";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbDosen.DataSource = dt;
                cmbDosen.DisplayMember = "Nama";   // yang ditampilkan
                cmbDosen.ValueMember = "DosenID";  // yang disimpan sebagai value
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load dosen: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbDosen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDosen.SelectedItem is DataRowView row)
            {
                txtNIDN.Text = row["NIDN"].ToString();
                SelectedDosenID = Convert.ToInt32(row["DosenID"]);
            }
        }



        private bool ValidasiLogikaInput()
        {
            TimeSpan mulai = dtpWaktuMulai.Value.TimeOfDay;
            TimeSpan selesai = dtpWaktuSelesai.Value.TimeOfDay;
            TimeSpan jamMasuk = new TimeSpan(7, 0, 0);  // 07:00
            TimeSpan jamPulang = new TimeSpan(17, 0, 0); // 17:00

            // 1. Cek Jam Kerja
            if (mulai < jamMasuk || selesai > jamPulang)
            {
                MessageBox.Show("Jadwal harus di dalam jam kerja (07:00 - 17:00)!");
                return false;
            }

            // 2. Cek Urutan Waktu
            if (mulai >= selesai)
            {
                MessageBox.Show("Waktu mulai harus lebih awal dari waktu selesai!");
                return false;
            }

            // 3. Cek Durasi Minimal (15 Menit)
            if ((selesai - mulai).TotalMinutes < 15)
            {
                MessageBox.Show("Durasi jadwal minimal adalah 15 menit!");
                return false;
            }

            return true;
        }

        private bool IsJadwalBentrok(int dosenID, DateTime tanggal, TimeSpan mulai, TimeSpan selesai, int currentJadwalID = 0)
        {
            bool bentrok = false;
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Query untuk mengecek irisan waktu
                string query = @"SELECT COUNT(*) FROM JadwalDosen 
                         WHERE DosenID = @DosenID 
                         AND Tanggal = @Tanggal 
                         AND JadwalID <> @CurrentID
                         AND ((@Mulai >= WaktuMulai AND @Mulai < WaktuSelesai) 
                              OR (@Selesai > WaktuMulai AND @Selesai <= WaktuSelesai)
                              OR (WaktuMulai >= @Mulai AND WaktuMulai < @Selesai))";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DosenID", dosenID);
                cmd.Parameters.AddWithValue("@Tanggal", tanggal.Date);
                cmd.Parameters.AddWithValue("@Mulai", mulai);
                cmd.Parameters.AddWithValue("@Selesai", selesai);
                cmd.Parameters.AddWithValue("@CurrentID", currentJadwalID);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0) bentrok = true;
            }
            catch (Exception ex) { MessageBox.Show("Error Cek Bentrok: " + ex.Message); }
            return bentrok;
        }


    }
}