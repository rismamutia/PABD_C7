using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace CRUDMahasiswaADO
{
    public partial class FormDosen : Form
    {
        DAL dbLogic = new DAL();
        int SelectedID = 0;
        int SelectedDosenID = 0;

        public int userID;
        public string nama;
        public string email;
        public string role;

        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=erlinaaa\\ERLINASHAFIRA;Initial Catalog=DBJadwalKoor;Integrated Security=True";

        private BindingSource bs = new BindingSource();
        private DataTable dtJadwal = new DataTable();
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
            LoadData();
        }

        private bool Validasi()
        {
            if (dtpWaktuMulai.Value.TimeOfDay >= dtpWaktuSelesai.Value.TimeOfDay)
            {
                MessageBox.Show("Logika waktu salah! Waktu selesai harus lebih besar dari waktu mulai.");
                return false;
            }

            return true;
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Validasi()) return;

            if (SelectedDosenID == 0) { MessageBox.Show("Pilih dosen!"); return; }
            if (!ValidasiLogikaInput()) return;

            if (IsJadwalBentrok(SelectedDosenID, dtpTanggalKetersediaan.Value, dtpWaktuMulai.Value.TimeOfDay, dtpWaktuSelesai.Value.TimeOfDay))
            {
                SimpanLog("FormJadwalDosen - Jadwal bentrok");

                MessageBox.Show(
                "Dosen sudah memiliki jadwal lain di jam yang bersinggungan!");

                return;
            }

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                SqlCommand cmd = new SqlCommand("sp_InsertJadwalDosen", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DosenID", SelectedDosenID);
                cmd.Parameters.AddWithValue("@Tanggal", dtpTanggalKetersediaan.Value.Date);
                cmd.Parameters.AddWithValue("@Mulai", dtpWaktuMulai.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@Selesai", dtpWaktuSelesai.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@Lokasi", cmbLokasi.Text);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Jadwal berhasil ditambahkan!");
                    ClearForm();
                    LoadData(); ;
                }
            }
            catch (SqlException ex)
            {
                SimpanLog("FormJadwalDosen - Insert : " + ex.Message);

                MessageBox.Show("SQL Error : " + ex.Message);
            }

            catch (Exception ex)
            {
                SimpanLog("FormJadwalDosen - Insert : " + ex.Message);

                MessageBox.Show("General Error : " + ex.Message);
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (SelectedDosenID == 0)
            {
                MessageBox.Show("Pilih dosen!");
                return;
            }

            if (SelectedID == 0)
            {
                MessageBox.Show("Pilih data di tabel terlebih dahulu!");
                return;
            }

            if (!Validasi()) return;

            if (!ValidasiLogikaInput()) return;

            // Cek bentrok jadwal
            if (IsJadwalBentrok(
                SelectedDosenID,
                dtpTanggalKetersediaan.Value,
                dtpWaktuMulai.Value.TimeOfDay,
                dtpWaktuSelesai.Value.TimeOfDay,
                SelectedID))
            {
                MessageBox.Show("Dosen sudah memiliki jadwal lain di jam yang bersinggungan!");
                return;
            }

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_UpdateJadwalDosen", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Jid", SelectedID);
                cmd.Parameters.AddWithValue("@DosenID", SelectedDosenID);
                cmd.Parameters.AddWithValue("@Tgl", dtpTanggalKetersediaan.Value.Date);
                cmd.Parameters.AddWithValue("@Mulai", dtpWaktuMulai.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@Selesai", dtpWaktuSelesai.Value.TimeOfDay);
                cmd.Parameters.AddWithValue("@Lokasi", cmbLokasi.Text);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Jadwal Dosen berhasil diperbarui!");

                    ClearForm();

                    SelectedID = 0;
                    SelectedDosenID = 0;

                    LoadData();
                }
                else
                {
                    MessageBox.Show("Gagal memperbarui data.");
                }
            }
            catch (SqlException ex)
            {
                SimpanLog("FormJadwalDosen - Update : " + ex.Message);

                MessageBox.Show("SQL Error : " + ex.Message);
            }

            catch (Exception ex)
            {
                SimpanLog("FormJadwalDosen - Update : " + ex.Message);

                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (SelectedID == 0)
            {
                MessageBox.Show("Pilih data dulu di tabel!");
                return;
            }

            DialogResult resultConfirm = MessageBox.Show(
                "Apakah Anda yakin ingin menghapus data ini?",
                "Konfirmasi Hapus",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultConfirm != DialogResult.Yes)
                return;

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("sp_DeleteJadwalDosen", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Jid", SelectedID);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data jadwal berhasil dihapus!");

                    ClearForm();

                    SelectedID = 0;
                    SelectedDosenID = 0;

                    LoadData();
                }
                else
                {
                    MessageBox.Show("Gagal menghapus data.");
                }
            }
            catch (SqlException ex)
            {
                SimpanLog("FormJadwalDosen - Delete : " + ex.Message);

                MessageBox.Show("SQL Error : " + ex.Message);
            }

            catch (Exception ex)
            {
                SimpanLog("FormJadwalDosen - Delete : " + ex.Message);

                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                SelectedID = Convert.ToInt32(row.Cells["JadwalID"].Value);
                SelectedDosenID = Convert.ToInt32(row.Cells["DosenID"].Value);

                if (row.Cells["Tanggal"].Value != DBNull.Value)
                    dtpTanggalKetersediaan.Value = Convert.ToDateTime(row.Cells["Tanggal"].Value);

                if (row.Cells["WaktuMulai"].Value != DBNull.Value)
                    dtpWaktuMulai.Value = DateTime.Today.Add((TimeSpan)row.Cells["WaktuMulai"].Value);

                if (row.Cells["WaktuSelesai"].Value != DBNull.Value)
                    dtpWaktuSelesai.Value = DateTime.Today.Add((TimeSpan)row.Cells["WaktuSelesai"].Value);

                cmbLokasi.Text =
                    row.Cells["Lokasi"].Value?.ToString();

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
        }

        private void FormDosen_Load(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            bindingNavigator1.BindingSource = bs;

            // COMBOBOX LOKASI
            cmbLokasi.Items.Clear();

            cmbLokasi.Items.Add("Ruang Dosen");

            cmbLokasi.Items.Add("Lab UI/UX");

            cmbLokasi.Items.Add("Lab Networking");

            cmbLokasi.Items.Add("Lab Data");

            cmbLokasi.Items.Add("Lab Pemrograman");

            cmbLokasi.SelectedIndex = 0;

            LoadDosen();

            dtpTanggalKetersediaan.Value = DateTime.Now;
            // RANGE TANGGAL AMAN UNTUK BINDING 😭🔥
            dtpTanggalKetersediaan.MinDate =
                new DateTime(2020, 1, 1);

            dtpTanggalKetersediaan.MaxDate =
                DateTime.Today.AddYears(5);

            dtpWaktuMulai.Value = DateTime.Now;
            dtpWaktuSelesai.Value = DateTime.Now;

            dtpWaktuMulai.Format = DateTimePickerFormat.Custom;
            dtpWaktuMulai.CustomFormat = "HH:mm";
            dtpWaktuMulai.ShowUpDown = true;

            dtpWaktuSelesai.Format = DateTimePickerFormat.Custom;
            dtpWaktuSelesai.CustomFormat = "HH:mm";
            dtpWaktuSelesai.ShowUpDown = true;

            LoadData();
        }

        private void cmbJK_SelectedIndexChanged(object sender, EventArgs e)
        {

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
            if (dtpTanggalKetersediaan.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Tanggal tidak boleh kurang dari hari ini!");
                return false;
            }

            // Maksimal 6 bulan ke depan
            if (dtpTanggalKetersediaan.Value.Date >
                DateTime.Today.AddMonths(6))
            {
                MessageBox.Show(
                    "Jadwal maksimal hanya boleh 6 bulan dari hari ini!");

                return false;
            }

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

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM View_JadwalDosenFull";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        dtJadwal = new DataTable();

                        da.Fill(dtJadwal);

                        bs.DataSource = dtJadwal;

                        dataGridView1.DataSource = bs;

                        BindControls();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void BindControls()
        {
            txtNIDN.DataBindings.Clear();
            cmbDosen.DataBindings.Clear();

            dtpTanggalKetersediaan.DataBindings.Clear();
            dtpWaktuMulai.DataBindings.Clear();
            dtpWaktuSelesai.DataBindings.Clear();

            txtNIDN.DataBindings.Add("Text", bs, "NIDN");

            cmbDosen.DataBindings.Add("Text", bs, "NamaDosen");

            dtpTanggalKetersediaan.DataBindings.Add("Value", bs, "Tanggal");

            cmbLokasi.DataBindings.Clear();
        }

        private void bindingNavigator2_RefreshItems(object sender, EventArgs e)
        {

        }

        private void cmbLokasi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SimpanLog(string pesan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query =
                @"INSERT INTO LogError
        (waktu, pesan_error)
        VALUES(GETDATE(), @pesan)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pesan", pesan);

                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter =
                "Excel Files|*.xls;*.xlsx";

                if (ofd.ShowDialog() ==
                DialogResult.OK)
                {
                    using (var stream =
                    new FileStream(
                        ofd.FileName,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite))
                    {
                        using (var reader =
                        ExcelReaderFactory.CreateReader(stream))
                        {
                            var result =
                            reader.AsDataSet(
                            new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable =
                                (_) =>
                                new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });

                            dataGridView1.DataSource =
                            result.Tables[0];

                            btnImpDB.Enabled = true;
                        }
                    }
                }
            }
        }


        private void btnImpDB_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt =
                (DataTable)
                dataGridView1.DataSource;

                if (dt == null)
                {
                    MessageBox.Show(
                    "Tidak ada data."
                    );

                    return;
                }

                int sukses = 0;

                foreach (DataRow row
                in dt.Rows)
                {
                    int dosenID = Convert.ToInt32(row["DosenID"]);

                    DateTime tanggal =Convert.ToDateTime(row["Tanggal"]);

                    TimeSpan mulai = Convert.ToDateTime(row["WaktuMulai"]).TimeOfDay;

                    TimeSpan selesai =Convert.ToDateTime(row["WaktuSelesai"]).TimeOfDay;

                    string lokasi =row["Lokasi"].ToString();

                    dbLogic.ImportJadwalDosen(

                    dosenID,

                    tanggal,

                    mulai,

                    selesai,

                    lokasi

                    );

                    sukses++;
                }

                MessageBox.Show(
                sukses +
                " data berhasil diimport."
                );

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                ex.Message
                );
            }
        }
    }
}