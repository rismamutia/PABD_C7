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
        static string connectionString = "Data Source=erlinaaa\\ERLINASHAFIRA;Initial Catalog=DBJadwalKoor;Integrated Security=True";
        private readonly SqlConnection conn;

        private int selectedMahasiswaID = 0;
        private int selectedPertemuanID = 0;

        private BindingSource bs = new BindingSource();

        private DataTable dtBooking = new DataTable();

        public JadwalPertemuan()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
            dataGridView1.CellClick += dataGridView1_CellContentClick;
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
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetBooking", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dtBooking = new DataTable();

                            da.Fill(dtBooking);

                            bs.DataSource = dtBooking;

                            dataGridView1.DataSource = bs;

                            BindControls();
                        }
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
            txtNIM.DataBindings.Clear();

            txtNamaMahasiswa.DataBindings.Clear();

            txtCatatan.DataBindings.Clear();

            cmbStatus.DataBindings.Clear();

            cmbDosen.DataBindings.Clear();

            txtNIM.DataBindings.Add(
                "Text",
                bs,
                "NIM_Mahasiswa");

            txtNamaMahasiswa.DataBindings.Add(
                "Text",
                bs,
                "Nama_Mahasiswa");

            txtCatatan.DataBindings.Add(
            "Text",
            bs,
            "CatatanPermintaan");

            cmbStatus.DataBindings.Add(
                "Text",
                bs,
                "Status_Booking");

            cmbDosen.DataBindings.Add(
                "Text",
                bs,
                "Nama_Dosen");
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
                SqlCommand cmd = new SqlCommand("sp_InsertBooking", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Jid", jadID);

                cmd.Parameters.AddWithValue("@Mid", selectedMahasiswaID);

                cmd.Parameters.AddWithValue("@Catatan", txtCatatan.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("Booking berhasil!");

                LoadGrid();

                ClearForm();

                LoadJadwalByDosen((int)cmbDosen.SelectedValue);
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (bs.Current != null)
            {
                selectedPertemuanID =
                    Convert.ToInt32(
                        ((DataRowView)bs.Current)["PertemuanID"]);
            }

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
                    SqlCommand cmd = new SqlCommand("sp_UpdateBooking", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Pid", selectedPertemuanID);

                    cmd.Parameters.AddWithValue("@Status", cmbStatus.Text);

                    cmd.Parameters.AddWithValue("@Catatan", txtCatatan.Text.Trim());

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil diperbarui!");
                    LoadGrid();
                }
                catch (SqlException ex)
                {
                    SimpanLog("JadwalPertemuan - Update : " + ex.Message);

                    MessageBox.Show("SQL Error : " + ex.Message);
                }

                catch (Exception ex)
                {
                    SimpanLog("JadwalPertemuan - Update : " + ex.Message);

                    MessageBox.Show("General Error : " + ex.Message);
                }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bs.Current != null)
            {
                selectedPertemuanID =
                    Convert.ToInt32(
                        ((DataRowView)bs.Current)["PertemuanID"]);
            }

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
                    SqlCommand cmd = new SqlCommand("sp_DeleteBooking", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Pid", selectedPertemuanID);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil dihapus!");
                    LoadGrid();
                    ClearForm();
                }
                catch (SqlException ex)
                {
                    SimpanLog("JadwalPertemuan - Delete : " + ex.Message);

                    MessageBox.Show("SQL Error : " + ex.Message);
                }

                catch (Exception ex)
                {
                    SimpanLog("JadwalPertemuan - Delete : " + ex.Message);

                    MessageBox.Show("General Error : " + ex.Message);
                }
            }
            }

        private void dataGridView1_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedPertemuanID =
                    Convert.ToInt32(
                        ((DataRowView)bs.Current)["PertemuanID"]);

                CariMahasiswa();
            }
        }



        private void JadwalPertemuan_Load(object sender, EventArgs e)
        {
            // GRID
            dataGridView1.ReadOnly = true;

            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.MultiSelect = false;

            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // BINDING NAVIGATOR
            bindingNavigator1.BindingSource = bs;

            // STATUS
            cmbStatus.Items.Clear();

            cmbStatus.Items.AddRange(
                new string[]
                {
            "Pending",
            "Available",
            "Denied",
            "Completed"
                });

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
                SqlCommand cmd = new SqlCommand("sp_GetDosen", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbDosen.DisplayMember = "Nama";
                cmbDosen.ValueMember = "DosenID";

                cmbDosen.DataSource = dt;
                cmbDosen.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Error Dosen: " + ex.Message); }
        }

        private void cmbDosen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDosen.SelectedValue == null)
                return;

            if (cmbDosen.SelectedValue is DataRowView)
                return;

            int dosenID = Convert.ToInt32(cmbDosen.SelectedValue);

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

        private void LoadJadwalByDosen(int dosenID)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_GetJadwalByDosen", conn);

                cmd.CommandType = CommandType.StoredProcedure;

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

            // Validasi kosong
            if (string.IsNullOrWhiteSpace(nim))
            {
                txtNamaMahasiswa.Text = "";
                selectedMahasiswaID = 0;
                return;
            }

            try
            {
                // Buka koneksi jika masih tertutup
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                // Panggil Stored Procedure
                SqlCommand cmd = new SqlCommand("sp_GetMahasiswaByNIM", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@NIM", nim);

                // Execute Reader
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Jika mahasiswa ditemukan
                        selectedMahasiswaID =
                            Convert.ToInt32(reader["MahasiswaID"]);

                        txtNamaMahasiswa.Text =
                            reader["Nama"].ToString();
                    }
                    else
                    {
                        // Jika mahasiswa tidak ditemukan
                        txtNamaMahasiswa.Text = "Tidak ditemukan";

                        selectedMahasiswaID = 0;

                        MessageBox.Show(
                            "Mahasiswa dengan NIM " + nim + " tidak ditemukan!",
                            "Peringatan",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error cari mahasiswa: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("sp_CheckBookingValid", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MahasiswaID", mhsID);

                cmd.Parameters.AddWithValue("@JadwalID", jadwalID);

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException ex)
            {
                SimpanLog("JadwalPertemuan - Validasi Booking : " + ex.Message);

                MessageBox.Show("SQL Error : " + ex.Message);

                return false;
            }

            catch (Exception ex)
            {
                SimpanLog("JadwalPertemuan - Validasi Booking : " + ex.Message);

                MessageBox.Show("General Error : " + ex.Message);

                return false;
            }
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

        private void txtNIM_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbJadwal_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SimpanLog(string pesan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO LogError
                        VALUES(GETDATE(), @pesan)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pesan", pesan);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
        }

        private void btnRekapData_Click(object sender, EventArgs e)
        {
            rekapData fm3 = new rekapData();
            fm3.Show();
            this.Hide();


        }
    }
 }
  

