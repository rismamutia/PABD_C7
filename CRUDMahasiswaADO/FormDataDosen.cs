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
    public partial class FormDataDosen : Form
    {
        private int selectedID = 0;

        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=LAPTOP-49331NDM\\RIANIINDRI;Initial Catalog=DBJadwalKoor;Integrated Security=True";

        public FormDataDosen()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
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
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("NIDN", "NIDN");
                dataGridView1.Columns.Add("Nama", "Nama");
                dataGridView1.Columns.Add("Email", "Email");

                string query = "SELECT * FROM Dosen";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
                        reader["NIDN"].ToString(),
                        reader["Nama"].ToString(),
                        reader["Email"].ToString()
                    );
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menampilkan data: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Validasi()) return;

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = @"INSERT INTO Dosen (NIDN, Nama, Email) VALUES (@NIDN, @Nama, @Email)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NIDN", txtNIDN.Text.Trim());
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim().ToLower());

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data Dosen berhasil ditambahkan");
                    ClearForm();
                    btnLoad.PerformClick();
                }
                else
                {
                    MessageBox.Show("Data gagal ditambahkan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Validasi()) return;

            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                string query = @"UPDATE Dosen SET Nama = @Nama, Email = @Email WHERE NIDN = @NIDN";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@NIDN", txtNIDN.Text.Trim());
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim().ToLower());

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data berhasil diupdate");
                    ClearForm();
                    btnLoad.PerformClick();
                }
                else
                {
                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                DialogResult resultConfirm = MessageBox.Show(
                    "Yakin ingin menghapus data?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultConfirm == DialogResult.Yes)
                {
                    string query = "DELETE FROM Dosen WHERE NIDN = @NIDN";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NIDN", txtNIDN.Text.Trim());

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Data berhasil dihapus");
                        ClearForm();
                        btnLoad.PerformClick();
                    }
                    else
                    {
                        MessageBox.Show("Data tidak ditemukan");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtNIDN.Text = row.Cells[0].Value?.ToString();
                txtNama.Text = row.Cells[1].Value?.ToString();
                txtEmail.Text = row.Cells[2].Value?.ToString();
            }
        }

        private void ClearForm()
        {
            txtNIDN.Clear();
            txtNama.Clear();
            txtEmail.Clear();
        }

        private bool Validasi()
        {
            string nidn = txtNIDN.Text.Trim();
            if (string.IsNullOrWhiteSpace(nidn))
            { MessageBox.Show("NIDN harus diisi!"); txtNIDN.Focus(); return false; }

            if (!System.Text.RegularExpressions.Regex.IsMatch(nidn, @"^[0-9]+$"))
            { MessageBox.Show("NIDN harus berupa angka!"); txtNIDN.Focus(); return false; }

            if (nidn.Length != 10)
            { MessageBox.Show("NIDN harus tepat 10 digit!"); txtNIDN.Focus(); return false; }

            string nama = txtNama.Text.Trim();
            if (string.IsNullOrWhiteSpace(nama))
            { MessageBox.Show("Nama harus diisi!"); txtNama.Focus(); return false; }

            if (nama.Length < 3 || nama.Length > 100)
            { MessageBox.Show("Nama harus 3-100 karakter!"); txtNama.Focus(); return false; }

            if (!System.Text.RegularExpressions.Regex.IsMatch(nama, @"^[a-zA-Z\s]+$"))
            { MessageBox.Show("Nama hanya boleh berisi huruf!"); txtNama.Focus(); return false; }

            string email = txtEmail.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email))
            { MessageBox.Show("Email harus diisi!"); txtEmail.Focus(); return false; }

            if (email.Length > 100)
            { MessageBox.Show("Email maksimal 100 karakter!"); txtEmail.Focus(); return false; }

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || !email.EndsWith("@gmail.com"))
            { MessageBox.Show("Email harus format @gmail.com yang valid!"); txtEmail.Focus(); return false; }

            if (IsEmailExists(email))
            { MessageBox.Show("Email sudah terdaftar!"); txtEmail.Focus(); return false; }

            return true;
        }

        private bool IsEmailExists(string email)
        {
            using (SqlConnection dbConn = new SqlConnection(connectionString))
            {
                dbConn.Open();
                string query = "SELECT COUNT(*) FROM Dosen WHERE Email = @email AND NIDN <> @nidn";

                using (SqlCommand cmd = new SqlCommand(query, dbConn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@nidn", txtNIDN.Text.Trim());
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

       
        

        private void FormDataDosen_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            btnLoad.PerformClick();
        }
    }
}