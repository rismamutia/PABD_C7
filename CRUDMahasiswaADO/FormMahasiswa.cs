using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;



namespace CRUDMahasiswaADO
{

    public partial class FormMahasiswa : Form
    {
        DAL dbLogic = new DAL();
        private int selectedID = 0;
        private BindingSource bindingSource = new BindingSource();
        private DataTable dtMahasiswa = new DataTable();
        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=erlinaaa\\ERLINASHAFIRA;Initial Catalog=DBJadwalKoor;Integrated Security=True";


        public FormMahasiswa()
        {
            InitializeComponent();

            conn = new SqlConnection(connectionString);


            dataGridView1.CellClick +=
            new DataGridViewCellEventHandler(
            dataGridView1_CellContentClick
            );
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
            HitungTotal();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            SqlConnection conn =
                 new SqlConnection(connectionString);

            conn.Open();

            SqlTransaction trans =
                conn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);

                cmd.ExecuteNonQuery();

                SqlCommand cmdLog = new SqlCommand("INSERT INTO LogError (Waktu, Pesan) VALUES (GETDATE(), 'Data mahasiswa berhasil ditambahkan')", conn, trans);
                cmdLog.ExecuteNonQuery();

                trans.Commit();
                MessageBox.Show("Data mahasiswa berhasil ditambahkan!");
                LoadData();
            }
            catch (SqlException ex)
            {
                trans.Rollback();
                SimpanLog("ROLLBACK INSERT : " + ex.Message);
                MessageBox.Show("Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                SimpanLog("GENERAL ERROR : " + ex.Message);
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedID == 0)
            {
                MessageBox.Show("Pilih data di tabel dulu!");
                return;
            }

            if (!Validasi()) return;

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ID", selectedID);
                        cmd.Parameters.AddWithValue("@NIM", txtNIM.Text.Trim());
                        cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data mahasiswa berhasil diperbarui!");

                    ClearForm();
                    LoadData();
                    HitungTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedID == 0)
            {
                MessageBox.Show("Pilih data di tabel dulu!");
                return;
            }

            if (MessageBox.Show("Yakin hapus data ini?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ID", selectedID);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data berhasil dihapus!");

                    ClearForm();
                    LoadData();
                    HitungTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            if (row.Cells["MahasiswaID"].Value != null && row.Cells["MahasiswaID"].Value != DBNull.Value)
            {
                selectedID = Convert.ToInt32(row.Cells["MahasiswaID"].Value);
            }
            else
            {
                selectedID = 0;
            }
            txtNIM.Text = row.Cells["NIM"].Value?.ToString() ?? "";
            txtNama.Text = row.Cells["Nama"].Value?.ToString() ?? "";
            txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
        }
        

        private void ClearForm()
        {
            txtNIM.Clear(); txtNama.Clear(); txtEmail.Clear();
            selectedID = 0;
        }

        private void FormMahasiswaFix_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            bindingNavigator1.BindingSource = bindingSource;

            btnImpDB.Enabled = false;

            LoadData();
            HitungTotal();
        }

        private void LoadData(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd;

                    // Kalau search kosong → tampil semua
                    if (string.IsNullOrWhiteSpace(keyword))
                    {
                        cmd = new SqlCommand("sp_GetMahasiswa", conn);
                    }
                    else
                    {
                        cmd = new SqlCommand("sp_SearchMahasiswa", conn);
                        cmd.Parameters.AddWithValue("@Keyword", keyword);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        dtMahasiswa = new DataTable();

                        da.Fill(dtMahasiswa);

                        bindingSource.DataSource = dtMahasiswa;

                        dataGridView1.DataSource = bindingSource;

                        BindControls();

                        if (bindingNavigator1 != null)
                            bindingNavigator1.BindingSource = bindingSource;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BindControls()
        {
            txtNIM.DataBindings.Clear();
            txtNama.DataBindings.Clear();
            txtEmail.DataBindings.Clear();

            txtNIM.DataBindings.Add("Text", bindingSource, "NIM");
            txtNama.DataBindings.Add("Text", bindingSource, "Nama");
            txtEmail.DataBindings.Add("Text", bindingSource, "Email");
        }
        private bool Validasi()
        {
            string nim = txtNIM.Text.Trim();
            if (string.IsNullOrWhiteSpace(nim))
            { MessageBox.Show("NIM harus diisi!"); txtNIM.Focus(); return false; }

            if (!Regex.IsMatch(nim, @"^[0-9]+$"))
            {
                MessageBox.Show("NIM harus berupa angka, tidak boleh mengandung huruf atau simbol!");
                txtNIM.Focus(); return false;
            }

            if (nim.Length != 11)
            {
                MessageBox.Show("NIM harus tepat 11 digit!");
                txtNIM.Focus(); return false;
            }

            string nama = txtNama.Text.Trim();
            if (string.IsNullOrWhiteSpace(nama))
            { MessageBox.Show("Nama harus diisi!"); txtNama.Focus(); return false; }

            if (nama.Length < 3)
            { MessageBox.Show("Nama minimal 3 karakter!"); txtNama.Focus(); return false; }

            if (nama.Length > 100)
            { MessageBox.Show("Nama terlalu panjang (Maksimal 100 karakter)!"); txtNama.Focus(); return false; }

            if (!Regex.IsMatch(nama, @"^[a-zA-Z\s\.,]+$"))
            {
                MessageBox.Show("Nama hanya boleh berisi huruf, spasi, titik, dan koma!");
                txtNama.Focus();
                return false;
            }

            string email = txtEmail.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email))
            { MessageBox.Show("Email harus diisi!"); txtEmail.Focus(); return false; }

            if (email.Length > 100)
            { MessageBox.Show("Email terlalu panjang (Maksimal 100)!"); txtEmail.Focus(); return false; }

            if (!Regex.IsMatch(email, @"^[^@\s]+@mail\.umy\.ac\.id$"))
            {
                MessageBox.Show("Email harus menggunakan domain @mail.umy.ac.id");
                txtEmail.Focus();
                return false;
            }

            if (IsEmailExists(email))
            {
                MessageBox.Show("Email sudah terdaftar!");
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private bool IsEmailExists(string email)
        {
            using (SqlConnection dbConn = new SqlConnection(connectionString))
            {
                dbConn.Open();
                string query = "SELECT COUNT(*) FROM Mahasiswa WHERE Email = @email";
                if (selectedID != 0) query += " AND MahasiswaID <> @id";

                using (SqlCommand cmd = new SqlCommand(query, dbConn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    if (selectedID != 0) cmd.Parameters.AddWithValue("@id", selectedID);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text.Trim());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            txtSearch.Clear();
            LoadData();
        }

        private void HitungTotal()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd =
                    new SqlCommand("sp_CountMahasiswa", conn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    SqlParameter output =
                        new SqlParameter("@Total", SqlDbType.Int);

                    output.Direction =
                        ParameterDirection.Output;

                    cmd.Parameters.Add(output);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    lblTotal.Text =
                        "Total Mahasiswa: " +
                        output.Value.ToString();
                }
            }
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

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query =
                        "UPDATE Mahasiswa SET Nama  = ' " + txtNama.Text + "' WHERE NIM = '" + txtNIM.Text + "'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Update berhasil");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImpEx_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog =
    new OpenFileDialog())
            {
                openFileDialog.Filter =
                "Excel Workbook|*.xls;*.xlsx";

                if (openFileDialog.ShowDialog()
                == DialogResult.OK)
                {
                    string filePath =
                    openFileDialog.FileName;

                    using (var stream =
                    File.Open(filePath,
                    FileMode.Open,
                    FileAccess.Read))
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

                            DataTable dt =
                            result.Tables[0];

                            dataGridView1.DataSource =
                            dt;
                        }
                    }

                    btnImpDB.Enabled = true;
                }
            }
        }

        private void btnImpDB_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt =
                (DataTable)dataGridView1.DataSource;

                if (dt == null ||
                dt.Rows.Count == 0)
                {
                    MessageBox.Show(
                    "Tidak ada data untuk diimport.");

                    return;
                }

                int sukses = 0;

                foreach (DataRow row in dt.Rows)
                {
                    string nim =
                    row["NIM"].ToString().Trim();

                    string nama =
                    row["Nama"].ToString().Trim();

                    string email =
                    row["Email"].ToString().Trim();

                    if (string.IsNullOrEmpty(nim)
                    || string.IsNullOrEmpty(nama))
                    {
                        continue;
                    }

                    dbLogic.ImportMahasiswa(
                    nim,
                    nama,
                    email
                    );

                    sukses++;
                }

                MessageBox.Show(
                sukses +
                " data berhasil diimport.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                ex.Message);
            }


        }
    }
 }

