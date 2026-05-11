using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class JadwalPertemuan : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-49331NDM\\RIANIINDRI;Initial Catalog=DBJadwalKoor;Integrated Security=True";
        private readonly SqlConnection conn;

        private int selectedMahasiswaID = 0;
        private int selectedPertemuanID = 0;

        public JadwalPertemuan()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
            dataGridView1.CellClick += dataGridView1_CellContentClick;
        }

        private void LoadData()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "SELECT * FROM Pertemuan";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load gagal: " + ex.Message);
            }
        }
        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                MessageBox.Show("Koneksi berhasil");
            }
            catch (Exception ex) { MessageBox.Show("Koneksi gagal: " + ex.Message); }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectDatabase();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void LoadGrid()
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                string query = @"
            SELECT p.PertemuanID, m.NIM, m.Nama AS Mahasiswa, d.Nama AS Dosen,
                   jd.Tanggal, LEFT(CONVERT(VARCHAR, jd.WaktuMulai, 108), 5) + '-' + 
                   LEFT(CONVERT(VARCHAR, jd.WaktuSelesai, 108), 5) AS Jam,
                   p.Status, p.TanggalPermintaan, p.CatatanPermintaan
            FROM Pertemuan p
            JOIN Mahasiswa m ON p.MahasiswaID = m.MahasiswaID
            JOIN JadwalDosen jd ON p.JadwalID = jd.JadwalID
            JOIN Dosen d ON jd.DosenID = d.DosenID
            ORDER BY p.PertemuanID DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }


        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (!CekValidasi()) return;

            int jadID = Convert.ToInt32(cmbJadwal.SelectedValue);
            if (!IsBookingValid(selectedMahasiswaID, jadID)) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Gunakan Transaction agar jika salah satu gagal, semua dibatalkan
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 1. Simpan data pertemuan
                    string queryInsert = @"INSERT INTO Pertemuan (JadwalID, MahasiswaID, Status, TanggalPermintaan, CatatanPermintaan)
                                   VALUES (@Jid, @Mid, 'Pending', GETDATE(), @Catatan)";
                    SqlCommand cmdInsert = new SqlCommand(queryInsert, conn, trans);
                    cmdInsert.Parameters.AddWithValue("@Jid", jadID);
                    cmdInsert.Parameters.AddWithValue("@Mid", selectedMahasiswaID);
                    cmdInsert.Parameters.AddWithValue("@Catatan", txtCatatan.Text.Trim());
                    cmdInsert.ExecuteNonQuery();

                    // 2. UPDATE Status Jadwal menjadi 'Booked' agar tidak bisa dipilih orang lain
                    string queryUpdate = "UPDATE JadwalDosen SET Status = 'Booked' WHERE JadwalID = @Jid";
                    SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conn, trans);
                    cmdUpdate.Parameters.AddWithValue("@Jid", jadID);
                    cmdUpdate.ExecuteNonQuery();

                    trans.Commit(); // Simpan permanen ke DB

                    MessageBox.Show("Booking berhasil! Jadwal telah dipesan.");

                    LoadGrid();
                    ClearForm();

                    // Refresh ComboBox agar jadwal yang baru saja dibooking menghilang
                    LoadJadwalByDosen((int)cmbDosen.SelectedValue);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex) { MessageBox.Show("Gagal: " + ex.Message); }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedPertemuanID == 0)
            {
                MessageBox.Show("Silakan pilih data yang ingin diubah dari tabel terlebih dahulu!", "Peringatan");
                return;
            }

            DialogResult dialog = MessageBox.Show("Apakah Anda yakin ingin memperbarui data booking ini?", "Konfirmasi Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string query = "UPDATE Pertemuan SET Status = @Status, CatatanPermintaan = @Catatan WHERE PertemuanID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Status", cmbStatus.Text);
                    cmd.Parameters.AddWithValue("@Catatan", txtCatatan.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID", selectedPertemuanID);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil diperbarui!");
                    LoadGrid();
                }
                catch (Exception ex) { MessageBox.Show("Gagal Update: " + ex.Message); }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedPertemuanID == 0)
            {
                MessageBox.Show("Pilih data yang ingin dihapus terlebih dahulu!", "Peringatan");
                return;
            }

            // Validasi: Tidak bisa menghapus booking yang sudah disetujui (Available/Completed)
            if (cmbStatus.Text == "Available" || cmbStatus.Text == "Completed")
            {
                MessageBox.Show("Booking yang sudah disetujui atau selesai tidak dapat dihapus!", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult res = MessageBox.Show("Apakah Anda yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string query = "DELETE FROM Pertemuan WHERE PertemuanID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", selectedPertemuanID);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil dihapus!");
                    LoadGrid();
                    ClearForm();
                }
                catch (Exception ex) { MessageBox.Show("Hapus Gagal: " + ex.Message); }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)

        {

            if (e.RowIndex >= 0)
            {
                // Mengambil baris yang diklik
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // 1. Simpan ID Utama untuk keperluan Update/Delete
                selectedPertemuanID = Convert.ToInt32(row.Cells["PertemuanID"].Value);

                // 2. Isi data ke TextBox dan ComboBox
                txtNIM.Text = row.Cells["NIM"].Value?.ToString();
                txtNamaMahasiswa.Text = row.Cells["Mahasiswa"].Value?.ToString();
                cmbStatus.Text = row.Cells["Status"].Value?.ToString();
                txtCatatan.Text = row.Cells["CatatanPermintaan"].Value?.ToString();

                // 3. Set ComboBox Dosen berdasarkan nama di Grid
                // Ini akan memicu event SelectedIndexChanged untuk mengisi cmbJadwal
                cmbDosen.Text = row.Cells["Dosen"].Value?.ToString();

                // 4. Panggil fungsi CariMahasiswa untuk mendapatkan selectedMahasiswaID yang asli
                CariMahasiswa();
            }
        }

        

        private void JadwalPertemuan_Load(object sender, EventArgs e)
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Pending", "Available", "Denied", "Completed" });
            cmbStatus.SelectedIndex = 0;
            LoadDosen();
            LoadGrid();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtNamaMahasiswa_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadDosen()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT DosenID, Nama FROM Dosen", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // ATUR INI DULU
                cmbDosen.DisplayMember = "Nama";
                cmbDosen.ValueMember = "DosenID";

                // BARU ISI DATANYA
                cmbDosen.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show("Error Dosen: " + ex.Message); }
        }

        private void cmbDosen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDosen.SelectedValue is int dosenID)
            {
                if (dosenID == 0)
                {
                    cmbJadwal.DataSource = null;
                    cmbJadwal.Items.Clear();
                    cmbJadwal.Items.Add("-- Pilih dosen dulu --");
                    cmbJadwal.SelectedIndex = 0;
                }
                else
                {
                    LoadJadwalByDosen(dosenID);
                }
            }
        }

        private void LoadJadwalByDosen(int dosenID)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Ditambahkan filter: Tanggal >= hari ini agar jadwal lama tidak muncul
                string query = @"SELECT JadwalID, 
                                CONVERT(VARCHAR, Tanggal, 105) + ' ' + 
                                LEFT(CONVERT(VARCHAR, WaktuMulai, 108), 5) + '-' + 
                                LEFT(CONVERT(VARCHAR, WaktuSelesai, 108), 5) AS Info
                         FROM JadwalDosen 
                         WHERE DosenID = @DosenID 
                         AND Status = 'Available'
                         AND Tanggal >= CAST(GETDATE() AS DATE)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DosenID", dosenID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    cmbJadwal.DataSource = null;
                    cmbJadwal.Items.Clear();
                    cmbJadwal.Items.Add("-- Tidak ada jadwal tersedia --");
                    cmbJadwal.SelectedIndex = 0;
                }
                else
                {
                    cmbJadwal.DataSource = dt;
                    cmbJadwal.DisplayMember = "Info";
                    cmbJadwal.ValueMember = "JadwalID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load jadwal: " + ex.Message);
            }
        }

        private void btnCariMahasiswa_Click(object sender, EventArgs e)
        {
            CariMahasiswa();
        }

        private void CariMahasiswa()
        {
            string nim = txtNIM.Text.Trim();
            if (string.IsNullOrWhiteSpace(nim))
            {
                txtNamaMahasiswa.Text = "";
                selectedMahasiswaID = 0;
                return;
            }

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                string query = "SELECT MahasiswaID, Nama FROM Mahasiswa WHERE NIM = @NIM";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIM", nim);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    selectedMahasiswaID = Convert.ToInt32(reader["MahasiswaID"]);
                    txtNamaMahasiswa.Text = reader["Nama"].ToString();
                }
                else
                {
                    txtNamaMahasiswa.Text = "Tidak ditemukan";
                    selectedMahasiswaID = 0;
                    MessageBox.Show("Mahasiswa dengan NIM " + nim + " tidak ditemukan!");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cari mahasiswa: " + ex.Message);
            }
        }

        private bool CekValidasi()
        {
            // 1. Validasi ComboBox Dosen & Jadwal
            if (cmbDosen.SelectedValue == null || cmbDosen.SelectedIndex == -1)
            {
                MessageBox.Show("Silakan pilih Dosen terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbJadwal.SelectedValue == null || cmbJadwal.Text.Contains("Tidak ada") || cmbJadwal.Text.Contains("Pilih dosen"))
            {
                MessageBox.Show("Silakan pilih Jadwal yang tersedia!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 2. Validasi NIM & Nama (Existing)
            if (txtNIM.Text.Trim().Length != 11 || !txtNIM.Text.All(char.IsDigit))
            {
                MessageBox.Show("NIM harus 11 digit angka!", "Validasi Gagal");
                return false;
            }
            if (selectedMahasiswaID == 0)
            {
                MessageBox.Show("Mahasiswa belum ditemukan. Silakan klik 'Cari' atau masukkan NIM yang benar.", "Peringatan");
                return false;
            }

            // 3. Validasi Lokasi (Min 5, Max 255)
            string lokasi = txtCatatan.Text.Trim(); // Mengasumsikan txtCatatan digunakan untuk Lokasi & Catatan
            if (lokasi.Length < 5 || lokasi.Length > 255)
            {
                MessageBox.Show("Lokasi dan Catatan minimal 5 karakter dan maksimal 255 karakter!", "Validasi Gagal");
                txtCatatan.Focus();
                return false;
            }

            return true;
        }

        private bool IsBookingValid(int mhsID, int jadwalID)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // 1. Cek apakah Mahasiswa sudah booking jadwal yang sama
                string sqlCekMhs = "SELECT COUNT(*) FROM Pertemuan WHERE MahasiswaID = @mID AND JadwalID = @jID";
                SqlCommand cmdMhs = new SqlCommand(sqlCekMhs, conn);
                cmdMhs.Parameters.AddWithValue("@mID", mhsID);
                cmdMhs.Parameters.AddWithValue("@jID", jadwalID);

                if ((int)cmdMhs.ExecuteScalar() > 0)
                {
                    MessageBox.Show("Anda sudah mengajukan booking untuk jadwal ini!");
                    return false;
                }

                // 2. Cek apakah jadwal sudah di-booking (Status di JadwalDosen harus Available)
                string sqlCekStatus = "SELECT Status FROM JadwalDosen WHERE JadwalID = @jID";
                SqlCommand cmdStatus = new SqlCommand(sqlCekStatus, conn);
                cmdStatus.Parameters.AddWithValue("@jID", jadwalID);
                string statusJadwal = cmdStatus.ExecuteScalar()?.ToString();

                if (statusJadwal != "Available")
                {
                    MessageBox.Show("Maaf, jadwal ini baru saja penuh atau tidak tersedia lagi.");
                    return false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Validasi Booking: " + ex.Message); return false; }
            return true;
        }
        private void txtNIM_Leave(object sender, EventArgs e)
        {
            CariMahasiswa();
        }

            private void ClearForm()
        {
            txtNIM.Clear();
            txtNamaMahasiswa.Clear();
            txtCatatan.Clear();
            selectedMahasiswaID = 0;
            selectedPertemuanID = 0;
        }
    }
    }

