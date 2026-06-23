using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDMahasiswaADO
{


    internal class DAL
    {
        static string connectionString = "Data Source=erlinaaa\\ERLINASHAFIRA;Initial Catalog=DBJadwalKoor;Integrated Security=True";

        SqlConnection conn = new SqlConnection(connectionString);

        SqlDataAdapter da;

        public static string GetConnectionString()
        {
            return connectionString;
        }

        // ===========================
        // REKAP DATA PERTEMUAN
        // ===========================
        public DataTable GetDataRekap(int dosenID, DateTime tanggal)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd =
                new SqlCommand("sp_ReportPertemuan", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DosenID", dosenID);

            cmd.Parameters.AddWithValue("@Status", "Completed");

            cmd.Parameters.AddWithValue("@Tanggal", tanggal.Date);

            da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            da.Fill(dt);

            conn.Close();

            return dt;
        }

        // ===========================
        // DASHBOARD
        // ===========================

        public DataTable GetAllDataChart()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd =
            new SqlCommand("sp_Dashboard", conn);

            cmd.CommandType =
            CommandType.StoredProcedure;

            da = new SqlDataAdapter(cmd);

            DataTable dt =
            new DataTable();

            da.Fill(dt);

            conn.Close();

            return dt;
        }


        // ===========================
        // DASHBOARD FILTER TAHUN
        // ===========================

        public DataTable GetDataChartBySemester(string semester)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd =
            new SqlCommand(
            "sp_DashboardBySemester",
            conn
            );

            cmd.CommandType =
            CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue(
            "@Semester",
            semester
            );

            da = new SqlDataAdapter(cmd);

            DataTable dt =
            new DataTable();

            da.Fill(dt);

            conn.Close();

            return dt;
        }

        public void ImportMahasiswa(
string nim,
string nama,
string email)
        {
            if (conn.State ==
            ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd =
            new SqlCommand(
            "sp_ImportMahasiswa",
            conn
            );

            cmd.CommandType =
            CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue(
            "@NIM",
            nim
            );

            cmd.Parameters.AddWithValue(
            "@Nama",
            nama
            );

            cmd.Parameters.AddWithValue(
            "@Email",
            email
            );

            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}
