namespace CRUDMahasiswaADO
{
    partial class FormUtama
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMahasiswa = new System.Windows.Forms.Button();
            this.btnDosen = new System.Windows.Forms.Button();
            this.btnJadwalDosen = new System.Windows.Forms.Button();
            this.btnJadwalPertemuan = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnLaporan = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelMain);
            this.splitContainer1.Size = new System.Drawing.Size(600, 366);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel1.Controls.Add(this.btnLaporan);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnMahasiswa);
            this.panel1.Controls.Add(this.btnDosen);
            this.panel1.Controls.Add(this.btnJadwalDosen);
            this.panel1.Controls.Add(this.btnJadwalPertemuan);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(198, 366);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(14, 326);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "🚪Keluar ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Bell MT", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(33, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "TemuDosen";
            // 
            // btnMahasiswa
            // 
            this.btnMahasiswa.FlatAppearance.BorderSize = 0;
            this.btnMahasiswa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMahasiswa.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMahasiswa.Location = new System.Drawing.Point(0, 110);
            this.btnMahasiswa.Name = "btnMahasiswa";
            this.btnMahasiswa.Size = new System.Drawing.Size(174, 23);
            this.btnMahasiswa.TabIndex = 0;
            this.btnMahasiswa.Text = "👨‍🎓 Data Mahasiswa";
            this.btnMahasiswa.UseVisualStyleBackColor = true;
            this.btnMahasiswa.Click += new System.EventHandler(this.btnMahasiswa_Click);
            // 
            // btnDosen
            // 
            this.btnDosen.FlatAppearance.BorderSize = 0;
            this.btnDosen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDosen.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDosen.Location = new System.Drawing.Point(12, 145);
            this.btnDosen.Name = "btnDosen";
            this.btnDosen.Size = new System.Drawing.Size(125, 23);
            this.btnDosen.TabIndex = 1;
            this.btnDosen.Text = "👨‍🏫 Data Dosen";
            this.btnDosen.UseVisualStyleBackColor = true;
            this.btnDosen.Click += new System.EventHandler(this.btnDosen_Click);
            // 
            // btnJadwalDosen
            // 
            this.btnJadwalDosen.FlatAppearance.BorderSize = 0;
            this.btnJadwalDosen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJadwalDosen.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJadwalDosen.Location = new System.Drawing.Point(12, 180);
            this.btnJadwalDosen.Name = "btnJadwalDosen";
            this.btnJadwalDosen.Size = new System.Drawing.Size(134, 23);
            this.btnJadwalDosen.TabIndex = 2;
            this.btnJadwalDosen.Text = "📅 Jadwal Dosen";
            this.btnJadwalDosen.UseVisualStyleBackColor = true;
            this.btnJadwalDosen.Click += new System.EventHandler(this.btnJadwalDosen_Click);
            // 
            // btnJadwalPertemuan
            // 
            this.btnJadwalPertemuan.FlatAppearance.BorderSize = 0;
            this.btnJadwalPertemuan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJadwalPertemuan.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJadwalPertemuan.Location = new System.Drawing.Point(10, 214);
            this.btnJadwalPertemuan.Name = "btnJadwalPertemuan";
            this.btnJadwalPertemuan.Size = new System.Drawing.Size(163, 23);
            this.btnJadwalPertemuan.TabIndex = 3;
            this.btnJadwalPertemuan.Text = "🤝 Jadwal Pertemuan";
            this.btnJadwalPertemuan.UseVisualStyleBackColor = true;
            this.btnJadwalPertemuan.Click += new System.EventHandler(this.btnJadwalPertemuan_Click);
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelMain.Controls.Add(this.label3);
            this.panelMain.Controls.Add(this.label2);
            this.panelMain.Controls.Add(this.panel2);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(396, 366);
            this.panelMain.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(93, 187);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(206, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pilih menu di samping untuk memulai";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Bell MT", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(58, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(297, 42);
            this.label2.TabIndex = 4;
            this.label2.Text = "Selamat Datang! ";
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(396, 366);
            this.panel2.TabIndex = 6;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // btnLaporan
            // 
            this.btnLaporan.FlatAppearance.BorderSize = 0;
            this.btnLaporan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLaporan.Font = new System.Drawing.Font("Book Antiqua", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLaporan.Location = new System.Drawing.Point(13, 247);
            this.btnLaporan.Name = "btnLaporan";
            this.btnLaporan.Size = new System.Drawing.Size(96, 23);
            this.btnLaporan.TabIndex = 7;
            this.btnLaporan.Text = "📄 Laporan";
            this.btnLaporan.UseVisualStyleBackColor = true;
            this.btnLaporan.Click += new System.EventHandler(this.btnLaporan_Click);
            // 
            // FormUtama
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.splitContainer1);
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormUtama";
            this.Text = "FormUtama";
            this.Load += new System.EventHandler(this.FormUtama_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnMahasiswa;
        private System.Windows.Forms.Button btnJadwalPertemuan;
        private System.Windows.Forms.Button btnJadwalDosen;
        private System.Windows.Forms.Button btnDosen;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnLaporan;
    }
}