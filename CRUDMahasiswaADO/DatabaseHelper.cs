using System.Data.SqlClient;

namespace CRUDMahasiswaADO
{
    public static class DatabaseHelper
    {
        public static readonly string ConnectionString =
            "Data Source=LAPTOP-49331NDM\\RIANIINDRI;Initial Catalog=DBJadwalKoor;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}