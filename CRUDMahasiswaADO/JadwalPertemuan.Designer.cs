namespace CRUDMahasiswaADO
{
    partial class JadwalPertemuan
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label4 = new System.Windows.Forms.Label();
            this.txtNIM = new System.Windows.Forms.TextBox();
            this.txtCatatan = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnInsert = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtNamaMahasiswa = new System.Windows.Forms.TextBox();
            this.btnCariMahasiswa = new System.Windows.Forms.Button();
            this.cmbDosen = new System.Windows.Forms.ComboBox();
            this.cmbJadwal = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpTanggalPermintaan = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(60, 234);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(191, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Lokasi dan Catatan Tambahan";
            // 
            // txtNIM
            // 
            this.txtNIM.Location = new System.Drawing.Point(72, 31);
            this.txtNIM.Name = "txtNIM";
            this.txtNIM.Size = new System.Drawing.Size(169, 22);
            this.txtNIM.TabIndex = 6;
            // 
            // txtCatatan
            // 
            this.txtCatatan.Location = new System.Drawing.Point(257, 234);
            this.txtCatatan.Name = "txtCatatan";
            this.txtCatatan.Size = new System.Drawing.Size(327, 22);
            this.txtCatatan.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(69, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Status";
            // 
            // cmbStatus
            // 
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Items.AddRange(new object[] {
            "Available",
            "Unavailable",
            "Booked"});
            this.cmbStatus.Location = new System.Drawing.Point(72, 173);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(209, 24);
            this.cmbStatus.TabIndex = 9;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(72, 288);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(648, 150);
            this.dataGridView1.TabIndex = 10;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(648, 35);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(140, 30);
            this.btnConnect.TabIndex = 11;
            this.btnConnect.Text = "Membuat Koneksi";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(648, 71);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(140, 29);
            this.btnLoad.TabIndex = 12;
            this.btnLoad.Text = "Menampilkan Data";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnInsert
            // 
            this.btnInsert.Location = new System.Drawing.Point(648, 106);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(140, 31);
            this.btnInsert.TabIndex = 13;
            this.btnInsert.Text = "Menambahkan Data";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(648, 143);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(140, 28);
            this.btnUpdate.TabIndex = 14;
            this.btnUpdate.Text = "Mengubah Data";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(648, 177);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(140, 28);
            this.btnDelete.TabIndex = 15;
            this.btnDelete.Text = "Menghapus Data";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(69, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 16);
            this.label7.TabIndex = 17;
            this.label7.Text = "NIM Mahasiswa";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(469, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 16);
            this.label3.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(359, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 16);
            this.label8.TabIndex = 19;
            this.label8.Text = "Nama Mahasiswa";
            // 
            // txtNamaMahasiswa
            // 
            this.txtNamaMahasiswa.Location = new System.Drawing.Point(362, 32);
            this.txtNamaMahasiswa.Name = "txtNamaMahasiswa";
            this.txtNamaMahasiswa.Size = new System.Drawing.Size(222, 22);
            this.txtNamaMahasiswa.TabIndex = 20;
            this.txtNamaMahasiswa.TextChanged += new System.EventHandler(this.txtNamaMahasiswa_TextChanged);
            // 
            // btnCariMahasiswa
            // 
            this.btnCariMahasiswa.Location = new System.Drawing.Point(247, 31);
            this.btnCariMahasiswa.Name = "btnCariMahasiswa";
            this.btnCariMahasiswa.Size = new System.Drawing.Size(75, 23);
            this.btnCariMahasiswa.TabIndex = 21;
            this.btnCariMahasiswa.Text = "Cari";
            this.btnCariMahasiswa.UseVisualStyleBackColor = true;
            this.btnCariMahasiswa.Click += new System.EventHandler(this.btnCariMahasiswa_Click);
            // 
            // cmbDosen
            // 
            this.cmbDosen.FormattingEnabled = true;
            this.cmbDosen.Location = new System.Drawing.Point(72, 103);
            this.cmbDosen.Name = "cmbDosen";
            this.cmbDosen.Size = new System.Drawing.Size(209, 24);
            this.cmbDosen.TabIndex = 22;
            this.cmbDosen.SelectedIndexChanged += new System.EventHandler(this.cmbDosen_SelectedIndexChanged);
            // 
            // cmbJadwal
            // 
            this.cmbJadwal.FormattingEnabled = true;
            this.cmbJadwal.Location = new System.Drawing.Point(362, 103);
            this.cmbJadwal.Name = "cmbJadwal";
            this.cmbJadwal.Size = new System.Drawing.Size(222, 24);
            this.cmbJadwal.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 24;
            this.label1.Text = "Pilih Dosen";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(359, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 16);
            this.label2.TabIndex = 25;
            this.label2.Text = "Pilih Jadwal Tersedia";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(359, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(129, 16);
            this.label6.TabIndex = 26;
            this.label6.Text = "Tanggal Permintaan";
            // 
            // dtpTanggalPermintaan
            // 
            this.dtpTanggalPermintaan.Location = new System.Drawing.Point(362, 175);
            this.dtpTanggalPermintaan.Name = "dtpTanggalPermintaan";
            this.dtpTanggalPermintaan.Size = new System.Drawing.Size(222, 22);
            this.dtpTanggalPermintaan.TabIndex = 27;
            // 
            // JadwalPertemuan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dtpTanggalPermintaan);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbJadwal);
            this.Controls.Add(this.cmbDosen);
            this.Controls.Add(this.btnCariMahasiswa);
            this.Controls.Add(this.txtNamaMahasiswa);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnInsert);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCatatan);
            this.Controls.Add(this.txtNIM);
            this.Controls.Add(this.label4);
            this.Name = "JadwalPertemuan";
            this.Text = " k";
            this.Load += new System.EventHandler(this.JadwalPertemuan_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNIM;
        private System.Windows.Forms.TextBox txtCatatan;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtNamaMahasiswa;
        private System.Windows.Forms.Button btnCariMahasiswa;
        private System.Windows.Forms.ComboBox cmbDosen;
        private System.Windows.Forms.ComboBox cmbJadwal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpTanggalPermintaan;
    }
}