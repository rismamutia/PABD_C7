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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class FormDataDosen : Form
    {
        private int selectedID = 0;

        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=erlinaaa\\ERLINASHAFIRA;Initial Catalog=DBJadwalKoor;Integrated Security=True";

        BindingSource bs = new BindingSource();
        private DataTable dtDosen = new DataTable();
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
            LoadData();
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
                cmd.Parameters.AddWithValue("@NIDN", txtNIDN.Text);
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);

                cmd.ExecuteNonQuery();

                SqlCommand cmdLog = new SqlCommand("INSERT INTO LogError (Waktu, Pesan) VALUES (GETDATE(), 'Data dosen berhasil ditambahkan')", conn, trans);
                cmdLog.ExecuteNonQuery();

                trans.Commit();
                MessageBox.Show("Data dosen berhasil ditambahkan!");
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
            selectedID = Convert.ToInt32(
                ((DataRowView)bs.Current)["DosenID"]
            );

            if (!Validasi()) return;

            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("sp_UpdateDosen", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", selectedID);
                cmd.Parameters.AddWithValue("@NIDN", txtNIDN.Text.Trim());
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim().ToLower());

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data berhasil diupdate");
                    ClearForm();
                    LoadData();
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
            selectedID = Convert.ToInt32(
       ((DataRowView)bs.Current)["DosenID"]
   );

            if (selectedID == 0)
            {
                MessageBox.Show("Data tidak ditemukan");
                return;
            }

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
                    SqlCommand cmd = new SqlCommand("sp_DeleteDosen", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID", selectedID);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Data berhasil dihapus");
                        ClearForm();
                        LoadData();
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

                selectedID = Convert.ToInt32(row.Cells["DosenID"].Value);

                txtNIDN.Text = row.Cells["NIDN"].Value.ToString();
                txtNama.Text = row.Cells["Nama"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
            }
        }

        private void ClearForm()
        {
            txtNIDN.Clear();
            txtNama.Clear();
            txtEmail.Clear();
            selectedID = 0;
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

            if (!System.Text.RegularExpressions.Regex.IsMatch(nama, @"^[a-zA-Z\s\.,]+$"))
            {
                MessageBox.Show("Nama hanya boleh berisi huruf, spasi, titik, dan koma!");
                txtNama.Focus();
                return false;
            }

            string email = txtEmail.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email))
            { MessageBox.Show("Email harus diisi!"); txtEmail.Focus(); return false; }

            if (email.Length > 100)
            { MessageBox.Show("Email maksimal 100 karakter!"); txtEmail.Focus(); return false; }

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@mail\.umy\.ac\.id$"))
            {
                MessageBox.Show("Email harus menggunakan domain @mail.umy.ac.id");
                txtEmail.Focus();
                return false;
            }

            if (IsEmailExists(email))
            { MessageBox.Show("Email sudah terdaftar!"); txtEmail.Focus(); return false; }

            return true;
        }

        private bool IsEmailExists(string email)
        {
            using (SqlConnection dbConn = new SqlConnection(connectionString))
            {
                dbConn.Open();

                string query = "SELECT COUNT(*) FROM Dosen " +
                               "WHERE Email = @email AND DosenID <> @id";

                using (SqlCommand cmd = new SqlCommand(query, dbConn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@id", selectedID);

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

            bindingNavigator1.BindingSource = bs;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                string query = "SELECT * FROM View_DataDosen";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                dtDosen = new DataTable();

                da.Fill(dtDosen);

                bs.DataSource = dtDosen;

                dataGridView1.DataSource = bs;

                BindControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BindControls()
        {
            txtNIDN.DataBindings.Clear();
            txtNama.DataBindings.Clear();
            txtEmail.DataBindings.Clear();

            txtNIDN.DataBindings.Add("Text", bs, "NIDN");
            txtNama.DataBindings.Add("Text", bs, "Nama");
            txtEmail.DataBindings.Add("Text", bs, "Email");
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