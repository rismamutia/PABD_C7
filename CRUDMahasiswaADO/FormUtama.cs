using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class FormUtama : Form
    {
        public FormUtama()
        {
            InitializeComponent();
            this.Resize += FormUtama_Resize;
        }

        private void OpenForm(Form child)
        {
            panelMain.Controls.Clear();

            child.TopLevel = false;

            child.FormBorderStyle = FormBorderStyle.None;

            child.Dock = DockStyle.Fill;

            panelMain.Controls.Add(child);

            child.Show();
        }

        private void FormUtama_Load(object sender, EventArgs e)
        {
            CenterWelcome();
        }
        private void CenterWelcome()
        {
            label2.Left =
            (panelMain.Width -
            label2.Width) / 2;

            label3.Left =
            (panelMain.Width -
            label3.Width) / 2;

            label2.Top =
            (panelMain.Height / 2) - 40;

            label3.Top =
            label2.Bottom + 10;
        }

        private void FormUtama_Resize(object sender,EventArgs e)
        {
            CenterWelcome();
        }

        private void btnMahasiswa_Click(object sender, EventArgs e)
        {
            OpenForm(new FormMahasiswa());
        }

        private void btnDosen_Click(object sender, EventArgs e)
        {
            OpenForm(new FormDataDosen());
        }

        private void btnJadwalDosen_Click(object sender, EventArgs e)
        {
            OpenForm(new FormDosen());
        }

        private void btnJadwalPertemuan_Click(object sender, EventArgs e)
        {
            OpenForm(new JadwalPertemuan());
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yakin ingin keluar?","Konfirmasi", MessageBoxButtons.YesNo)== DialogResult.Yes)

                Application.Exit();
        }



        private void btnLaporan_Click(object sender, EventArgs e)
        {
            OpenForm(new MelihatReport());
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
    }

