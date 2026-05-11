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
            if (selectedMahasiswaID == 0) { MessageBox.Show("Cari NIM dulu!"); return; }
            if (cmbJadwal.SelectedValue == null) { MessageBox.Show("Pilih Jadwal!"); return; }

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                string query = @"INSERT INTO Pertemuan (JadwalID, MahasiswaID, Status, TanggalPermintaan, CatatanPermintaan)
                                 VALUES (@Jid, @Mid, @Status, @Tgl, @Catatan)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Jid", cmbJadwal.SelectedValue);
                cmd.Parameters.AddWithValue("@Mid", selectedMahasiswaID);
                cmd.Parameters.AddWithValue("@Status", cmbStatus.Text);
                cmd.Parameters.AddWithValue("@Tgl", dtpTanggalPermintaan.Value.Date);
                cmd.Parameters.AddWithValue("@Catatan", txtCatatan.Text.Trim());
                cmd.ExecuteNonQuery();

                MessageBox.Show("Data berhasil disimpan!");
                ClearForm();
                LoadGrid();
            }
            catch (Exception ex) { MessageBox.Show("Simpan gagal: " + ex.Message); }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedPertemuanID == 0) return;
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                string query = "UPDATE Pertemuan SET Status = @Status WHERE PertemuanID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", cmbStatus.Text);
                cmd.Parameters.AddWithValue("@ID", selectedPertemuanID);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Update Berhasil!");
                LoadGrid();
            }
            catch (Exception ex) { MessageBox.Show("Update Gagal: " + ex.Message); }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedPertemuanID == 0) return;
            if (MessageBox.Show("Hapus data?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string query = "DELETE FROM Pertemuan WHERE PertemuanID = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", selectedPertemuanID);
                    cmd.ExecuteNonQuery();
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
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedPertemuanID = Convert.ToInt32(row.Cells["PertemuanID"].Value);
                cmbStatus.Text = row.Cells["Status"].Value?.ToString();
                txtCatatan.Text = row.Cells["CatatanPermintaan"].Value?.ToString();
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

                string query = @"SELECT JadwalID, 
                                        CONVERT(VARCHAR, Tanggal, 105) + ' ' + 
                                        LEFT(CONVERT(VARCHAR, WaktuMulai, 108), 5) + '-' + 
                                        LEFT(CONVERT(VARCHAR, WaktuSelesai, 108), 5) AS Info
                                 FROM JadwalDosen 
                                 WHERE DosenID = @DosenID AND Status = 'Available'";

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
                    return;
                }

                cmbJadwal.DataSource = dt;
                cmbJadwal.DisplayMember = "Info";
                cmbJadwal.ValueMember = "JadwalID";
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
            // 1. Validasi NIM (Harus 11 digit)
            if (txtNIM.Text.Trim().Length != 11)
            {
                MessageBox.Show("NIM harus tepat 11 digit!", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIM.Focus();
                return false;
            }

            // 2. Validasi NIM (Hanya boleh angka)
            if (!txtNIM.Text.All(char.IsDigit))
            {
                MessageBox.Show("NIM hanya boleh berisi angka!", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIM.Focus();
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
    }
    }

