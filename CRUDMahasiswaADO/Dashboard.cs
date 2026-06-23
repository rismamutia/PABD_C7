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
    public partial class Dashboard : Form
    {
        DAL dbLogic = new DAL();

        bool isInitializing = true;

        DataTable dt;

        public Dashboard()
        {
            InitializeComponent();

            cmbSemester.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbSemester.Items.Add("Genap 2025/2026");

            cmbSemester.Items.Add("Ganjil 2026/2027");

            cmbSemester.SelectedIndex = 0;

            cmbTipe.DropDownStyle = ComboBoxStyle.DropDownList;

            var items = new[]
            {
                new KeyValuePair<string,
                System.Windows.Forms.DataVisualization.Charting.SeriesChartType>
                (
                    "Kolom",
                    System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column
                ),

                new KeyValuePair<string,
                System.Windows.Forms.DataVisualization.Charting.SeriesChartType>
                (
                    "Pie",
                    System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie
                )
            };

            isInitializing = true;

            cmbTipe.DataSource = items;

            cmbTipe.DisplayMember = "Key";

            cmbTipe.ValueMember = "Value";

            cmbTipe.SelectedIndex = 0;

            isInitializing = false;

            loadDataChart();
        }

        private void loadDataChart()
        {
            try
            {
                dt = dbLogic.GetAllDataChart();

                chart1.Series.Clear();

                chart1.Titles.Clear();

                chart1.Titles.Add("Jumlah Pertemuan per Dosen");

                chart1.Series.Add("Jumlah Pertemuan");

                chart1.Series["Jumlah Pertemuan"].XValueMember =
                    "NamaDosen";

                chart1.Series["Jumlah Pertemuan"].YValueMembers =
                    "JumlahPertemuan";

                chart1.DataSource = dt;

                chart1.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Gagal memuat data : " + ex.Message
                );
            }
        }


        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void cmbTipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing)
            {
                return;
            }

            var tipeChart =
            (System.Windows.Forms.DataVisualization.Charting.SeriesChartType)
            cmbTipe.SelectedValue;

            chart1.Series[0].ChartType =
            tipeChart;
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            cmbSemester.SelectedIndex = 0;

            cmbTipe.SelectedIndex = 0;

            loadDataChart();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {

                dt = dbLogic.GetDataChartBySemester(
                    cmbSemester.Text
                );

                chart1.DataSource = dt;

                chart1.DataBind();

                chart1.Series[0].ChartType =
                (System.Windows.Forms.DataVisualization.Charting.SeriesChartType)
                cmbTipe.SelectedValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Gagal memuat data : " + ex.Message
                );
            }
        }
    }
}
