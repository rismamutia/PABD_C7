using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDMahasiswaADO
{
    class Data
    {
        public string NIM_Mahasiswa { get; set; }

        public string Nama_Mahasiswa { get; set; }

        public string Nama_Dosen { get; set; }

        public DateTime Tanggal { get; set; }

        public TimeSpan WaktuMulai { get; set; }

        public TimeSpan WaktuSelesai { get; set; }

        public string Status_Booking { get; set; }

        public string CatatanPermintaan { get; set; }
    }
}
