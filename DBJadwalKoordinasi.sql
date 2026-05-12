CREATE DATABASE DBJadwalKoor;
GO
DROP DATABASE DBJadwalKoor;

USE DBJadwalKoor;
GO

CREATE TABLE Admin (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Nama VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Mahasiswa (
    MahasiswaID INT IDENTITY(1,1) PRIMARY KEY,
    NIM VARCHAR(20) NOT NULL UNIQUE,
    Nama VARCHAR(100) NOT NULL,
    Email VARCHAR(100)
);

CREATE TABLE Dosen (
    DosenID INT IDENTITY(1,1) PRIMARY KEY,
    NIDN VARCHAR(20) NOT NULL UNIQUE,
    Nama VARCHAR(100) NOT NULL,
    Email VARCHAR(100)
);

CREATE TABLE JadwalDosen (
    JadwalID INT IDENTITY(1,1) PRIMARY KEY,
    DosenID INT NOT NULL,
    Tanggal DATE NOT NULL,
    WaktuMulai TIME NOT NULL,
    WaktuSelesai TIME NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Tersedia'
        CHECK (Status IN ('Available', 'Unavailable', 'Booked')),
    FOREIGN KEY (DosenID) REFERENCES Dosen(DosenID) ON DELETE CASCADE,
    CONSTRAINT CK_Waktu CHECK (WaktuMulai < WaktuSelesai)
);

CREATE TABLE Pertemuan (
    PertemuanID INT IDENTITY(1,1) PRIMARY KEY,
    JadwalID INT NOT NULL,
    MahasiswaID INT NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Pending'
        CHECK (Status IN ('Cancelled', 'Completed')),
    TanggalPermintaan DATETIME NOT NULL DEFAULT GETDATE(),
    CatatanPermintaan VARCHAR(MAX),
    FOREIGN KEY (JadwalID) REFERENCES JadwalDosen(JadwalID),
    FOREIGN KEY (MahasiswaID) REFERENCES Mahasiswa(MahasiswaID)
);

DROP TABLE Pertemuan;

CREATE TABLE RiwayatKonsultasi (
    RiwayatID INT IDENTITY(1,1) PRIMARY KEY,
    PertemuanID INT NOT NULL,
    TanggalKonsultasi DATETIME NOT NULL DEFAULT GETDATE(),
    Catatan VARCHAR(MAX),
    FOREIGN KEY (PertemuanID) REFERENCES Pertemuan(PertemuanID) ON DELETE CASCADE
);

CREATE TABLE LoginSession (
    SessionID INT IDENTITY(1,1) PRIMARY KEY,
    AdminID INT NOT NULL,
    WaktuLogin DATETIME NOT NULL DEFAULT GETDATE(),
    WaktuLogout DATETIME,
    FOREIGN KEY (AdminID) REFERENCES Admin(AdminID) ON DELETE CASCADE
);
GO

CREATE VIEW ReportPertemuan AS
SELECT 
    p.PertemuanID,
    m.NIM AS NIM_Mahasiswa,
    m.Nama AS Nama_Mahasiswa,
    d.NIDN AS NIDN_Dosen,
    d.Nama AS Nama_Dosen,
    jd.Tanggal,
    jd.WaktuMulai,
    jd.WaktuSelesai,
    p.Status AS StatusPertemuan,
    p.TanggalPermintaan,
    rc.Catatan AS CatatanKonsultasi
FROM Pertemuan p
JOIN JadwalDosen jd ON p.JadwalID = jd.JadwalID
JOIN Dosen d ON jd.DosenID = d.DosenID
JOIN Mahasiswa m ON p.MahasiswaID = m.MahasiswaID
LEFT JOIN RiwayatKonsultasi rc ON p.PertemuanID = rc.PertemuanID;
GO

ALTER VIEW ReportPertemuan AS
SELECT 
    p.PertemuanID,
    m.NIM               AS NIM_Mahasiswa,
    m.Nama              AS Nama_Mahasiswa,
    d.NIDN              AS NIDN_Dosen,
    d.Nama              AS Nama_Dosen,
    jd.Tanggal,
    jd.WaktuMulai,
    jd.WaktuSelesai,
    p.Status            AS StatusPertemuan,
    p.TanggalPermintaan,
    p.CatatanPermintaan,                          -- TAMBAHAN
    rc.Catatan          AS CatatanKonsultasi
FROM Pertemuan p
JOIN JadwalDosen jd ON p.JadwalID    = jd.JadwalID
JOIN Dosen       d  ON jd.DosenID    = d.DosenID
JOIN Mahasiswa   m  ON p.MahasiswaID = m.MahasiswaID
LEFT JOIN RiwayatKonsultasi rc ON p.PertemuanID = rc.PertemuanID;


INSERT INTO Admin
(Username, PasswordHash, Nama, Email)
VALUES
('admin', '123', 'Administrator', 'admin@mail.com'),
('supra', '12345', 'Supra Admin', 'supra@mail.com');

SELECT * FROM Admin;

INSERT INTO Mahasiswa
(NIM, Nama, Email)
VALUES
('230001', 'Andi Saputra', 'andi@mail.com'),
('230002', 'Budi Santoso', 'budi@mail.com'),
('230003', 'Citra Dewi', 'citra@mail.com');

-- =========================
-- DUMMY DATA DOSEN
-- =========================
INSERT INTO Dosen
(NIDN, Nama, Email)
VALUES
('D001', 'Dr. Agus Wijaya', 'agus@kampus.ac.id'),
('D002', 'Dr. Budi Hartono', 'budi@kampus.ac.id'),
('D003', 'Dr. Clara Sari', 'clara@kampus.ac.id');

-- =========================
-- DUMMY JADWAL DOSEN
-- =========================
INSERT INTO JadwalDosen
(DosenID, Tanggal, WaktuMulai, WaktuSelesai, Status)
VALUES
(1, '2026-05-10', '08:00', '09:00', 'Available'),
(2, '2026-05-11', '10:00', '11:00', 'Available'),
(3, '2026-05-12', '13:00', '14:00', 'Booked');

-- =========================
-- DUMMY PERTEMUAN
-- =========================
INSERT INTO Pertemuan
(JadwalID, MahasiswaID, Status, CatatanPermintaan)
VALUES
(1, 1, 'Pending', 'Konsultasi proposal'),
(2, 2, 'Completed', 'Diskusi tugas akhir'),
(3, 3, 'Denied', 'Jadwal bentrok');

-- View untuk menampilkan Jadwal Dosen beserta Nama Dosennya
CREATE VIEW View_JadwalDosen AS
SELECT 
    j.JadwalID, 
    j.DosenID, 
    d.NIDN, 
    d.Nama AS NamaDosen, 
    j.Tanggal, 
    j.WaktuMulai, 
    j.WaktuSelesai, 
    j.Status
FROM JadwalDosen j
JOIN Dosen d ON j.DosenID = d.DosenID;
GO

ALTER PROCEDURE sp_InsertJadwalDosen
    @DosenID INT,
    @Tanggal DATE,
    @Mulai TIME,
    @Selesai TIME,
    @Status VARCHAR(20),
	@Lokasi VARCHAR(100)
AS
BEGIN
    -- Logika Tambahan: Cek apakah dosen sudah punya jadwal di jam yang sama
    IF EXISTS (SELECT 1 FROM JadwalDosen 

               WHERE DosenID = @DosenID AND Tanggal = @Tanggal 
               AND ((@Mulai >= WaktuMulai AND @Mulai < WaktuSelesai) 
               OR (@Selesai > WaktuMulai AND @Selesai <= WaktuSelesai)))
    BEGIN
        RAISERROR('Dosen tersebut sudah memiliki jadwal pada jam yang bersinggungan!', 16, 1);
    END
    ELSE
    BEGIN
        INSERT INTO JadwalDosen (DosenID, Tanggal, WaktuMulai, WaktuSelesai, Status, Lokasi)
        VALUES (@DosenID, @Tanggal, @Mulai, @Selesai, @Status, @Lokasi);
    END
END;
GO
CREATE PROCEDURE sp_DeleteJadwalDosen
    @JadwalID INT
AS
BEGIN
    -- Logika Tambahan: Tidak boleh hapus jika status sudah 'Booked'
    IF (SELECT Status FROM JadwalDosen WHERE JadwalID = @JadwalID) = 'Booked'
    BEGIN
        RAISERROR('Jadwal tidak boleh dihapus karena sudah dibooking oleh mahasiswa!', 16, 1);
    END
    ELSE
    BEGIN
        DELETE FROM JadwalDosen WHERE JadwalID = @JadwalID;
    END
END;
GO

CREATE PROCEDURE sp_SearchJadwalDosen
    @Keyword VARCHAR(100)
AS
BEGIN
    SELECT * FROM View_JadwalDosen
    WHERE NamaDosen LIKE '%' + @Keyword + '%' OR NIDN LIKE '%' + @Keyword + '%';
END;
GO

/*
=== VIEW ===
*/

--- 1. View Data Mahasiswa
CREATE VIEW View_DataMahasiswa AS
SELECT 
    MahasiswaID, 
    NIM, 
    Nama, 
    Email 
FROM Mahasiswa;
GO

--- 2. View Data Dosen
CREATE VIEW View_DataDosen AS
SELECT 
    DosenID, 
    NIDN, 
    Nama, 
    Email 
FROM Dosen;
GO

--- 3. View Jadwal Dosen
CREATE VIEW View_JadwalDosenFull AS
SELECT 
    j.JadwalID, 
    j.DosenID, 
    d.Nama AS NamaDosen, 
    d.NIDN,
    j.Tanggal, 
    j.WaktuMulai, 
    j.WaktuSelesai, 
    j.Status
FROM JadwalDosen j
JOIN Dosen d ON j.DosenID = d.DosenID;
GO

--- 4. View Booking Pertemuan
ALTER VIEW View_BookingPertemuan
AS
SELECT 
    p.PertemuanID, 
    m.NIM AS NIM_Mahasiswa, 
    m.Nama AS Nama_Mahasiswa, 
    d.Nama AS Nama_Dosen, 
    j.Tanggal, 
    j.WaktuMulai, 
    j.WaktuSelesai, 

    p.Status AS Status_Booking,

    p.CatatanPermintaan

FROM Pertemuan p

JOIN Mahasiswa m
ON p.MahasiswaID = m.MahasiswaID

JOIN JadwalDosen j
ON p.JadwalID = j.JadwalID

JOIN Dosen d
ON j.DosenID = d.DosenID;
GO

--- 5. View Riwayat Pertemuan
CREATE VIEW View_RiwayatKonsultasiFull AS
SELECT 
    r.RiwayatID,
    p.PertemuanID,
    m.Nama AS Nama_Mahasiswa,
    d.Nama AS Nama_Dosen,
    r.TanggalKonsultasi,
    r.Catatan AS Hasil_Konsultasi
FROM RiwayatKonsultasi r
JOIN Pertemuan p ON r.PertemuanID = p.PertemuanID
JOIN Mahasiswa m ON p.MahasiswaID = m.MahasiswaID
JOIN JadwalDosen j ON p.JadwalID = j.JadwalID
JOIN Dosen d ON j.DosenID = d.DosenID;
GO

/*
=== STORED PROCEDURE - MAHASISWA ===
*/
-- INSERT MAHASISWA (Cek NIM Duplikat)
GO
CREATE PROCEDURE sp_InsertMahasiswa
    @NIM VARCHAR(20), @Nama VARCHAR(100), @Email VARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Mahasiswa WHERE NIM = @NIM)
        RAISERROR('Gagal: NIM tersebut sudah terdaftar!', 16, 1);
    ELSE
        INSERT INTO Mahasiswa (NIM, Nama, Email) VALUES (@NIM, @Nama, @Email);
END;
GO

-- UPDATE MAHASISWA (Pastikan NIM tidak bentrok dengan orang lain)
GO
CREATE PROCEDURE sp_UpdateMahasiswa
    @ID INT, @NIM VARCHAR(20), @Nama VARCHAR(100), @Email VARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Mahasiswa WHERE NIM = @NIM AND MahasiswaID <> @ID)
        RAISERROR('NIM sudah digunakan oleh mahasiswa lain!', 16, 1);
    ELSE
        UPDATE Mahasiswa SET NIM = @NIM, Nama = @Nama, Email = @Email WHERE MahasiswaID = @ID;
END;
GO

-- DELETE MAHASISWA (Jangan hapus jika mahasiswa punya history pertemuan)
GO
CREATE PROCEDURE sp_DeleteMahasiswa
    @ID INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Pertemuan WHERE MahasiswaID = @ID)
        RAISERROR('Data tidak bisa dihapus karena mahasiswa memiliki riwayat pertemuan!', 16, 1);
    ELSE
        DELETE FROM Mahasiswa WHERE MahasiswaID = @ID;
END;
GO

-- SEARCH MAHASISWA
GO
CREATE PROCEDURE sp_SearchMahasiswa
    @Keyword VARCHAR(100)
AS
BEGIN
    SELECT * FROM View_DataMahasiswa 
    WHERE Nama LIKE '%' + @Keyword + '%' OR NIM LIKE '%' + @Keyword + '%';
END;
GO

/*
=== STORED PROCEDURE - DOSEN ===
*/
-- INSERT DOSEN (Cek NIDN)
GO
CREATE PROCEDURE sp_InsertDosen
    @NIDN VARCHAR(20), @Nama VARCHAR(100), @Email VARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Dosen WHERE NIDN = @NIDN)
        RAISERROR('Gagal: NIDN tersebut sudah terdaftar!', 16, 1);
    ELSE
        INSERT INTO Dosen (NIDN, Nama, Email) VALUES (@NIDN, @Nama, @Email);
END;
GO

-- UPDATE DOSEN (Cek NIDN Duplikat pada ID lain)
GO
CREATE PROCEDURE sp_UpdateDosen
    @ID INT,
    @NIDN VARCHAR(20),
    @Nama VARCHAR(100),
    @Email VARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Dosen WHERE NIDN = @NIDN AND DosenID <> @ID)
        RAISERROR('Gagal: NIDN tersebut sudah digunakan oleh dosen lain!', 16, 1);
    ELSE
        UPDATE Dosen 
        SET NIDN = @NIDN, Nama = @Nama, Email = @Email 
        WHERE DosenID = @ID;
END;
GO

-- DELETE DOSEN
GO
CREATE PROCEDURE sp_DeleteDosen
    @ID INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM JadwalDosen WHERE DosenID = @ID)
        RAISERROR('Gagal: Dosen tidak bisa dihapus karena sudah memiliki jadwal konsultasi!', 16, 1);
    ELSE
        DELETE FROM Dosen WHERE DosenID = @ID;
END;
GO

-- SEARCH DOSEN
GO
CREATE PROCEDURE sp_SearchDosen
    @Keyword VARCHAR(100)
AS
BEGIN
    -- Mengambil data dari View_DataDosen
    -- Menambahkan logika pencarian berdasarkan Nama atau NIDN
    SELECT * FROM View_DataDosen
    WHERE Nama LIKE '%' + @Keyword + '%' 
       OR NIDN LIKE '%' + @Keyword + '%';
END;
GO


/*
=== STORED PROCEDURE - JADWAL DOSEN ===
*/
-- INSERT JADWAL DOSEN 
GO
ALTER PROCEDURE sp_InsertJadwalDosen
    @DosenID INT, @Tgl DATE, @Mulai TIME, @Selesai TIME
AS
BEGIN
    -- Isi logikanya tetap sama --
    IF (@Mulai >= @Selesai)
        RAISERROR('Waktu mulai tidak boleh setelah waktu selesai!', 16, 1);
    ELSE IF EXISTS (SELECT 1 FROM JadwalDosen 
                    WHERE DosenID = @DosenID AND Tanggal = @Tgl 
                    AND ((@Mulai >= WaktuMulai AND @Mulai < WaktuSelesai) 
                    OR (@Selesai > WaktuMulai AND @Selesai <= WaktuSelesai)))
        RAISERROR('Gagal: Dosen sudah punya jadwal di jam tersebut!', 16, 1);
    ELSE
        INSERT INTO JadwalDosen (DosenID, Tanggal, WaktuMulai, WaktuSelesai, Status)
        VALUES (@DosenID, @Tgl, @Mulai, @Selesai, 'Available');
END;
GO 

-- UPDATE JADWAL DOSEN 
ALTER PROCEDURE sp_UpdateJadwalDosen
    @Jid INT,
    @DosenID INT,
    @Tgl DATE,
    @Mulai TIME,
    @Selesai TIME,
    @Status VARCHAR(20),
    @Lokasi VARCHAR(100)
AS
BEGIN

    -- Validasi 1
    IF (SELECT Status
        FROM JadwalDosen
        WHERE JadwalID = @Jid) = 'Booked'
    BEGIN
        RAISERROR(
            'Gagal: Jadwal sudah dibooking!',
            16,
            1
        );
        RETURN;
    END

    -- Validasi 2
    IF (@Mulai >= @Selesai)
    BEGIN
        RAISERROR(
            'Waktu mulai harus lebih awal!',
            16,
            1
        );
        RETURN;
    END

    -- Validasi 3
    IF EXISTS
    (
        SELECT 1
        FROM JadwalDosen
        WHERE
            DosenID = @DosenID
            AND Tanggal = @Tgl
            AND JadwalID <> @Jid
            AND
            (
                (@Mulai >= WaktuMulai
                 AND @Mulai < WaktuSelesai)

                OR

                (@Selesai > WaktuMulai
                 AND @Selesai <= WaktuSelesai)
            )
    )
    BEGIN
        RAISERROR(
            'Bentrok dengan jadwal lain!',
            16,
            1
        );
        RETURN;
    END

    UPDATE JadwalDosen
    SET
        Tanggal = @Tgl,
        WaktuMulai = @Mulai,
        WaktuSelesai = @Selesai,
        Status = @Status,
        Lokasi = @Lokasi
    WHERE JadwalID = @Jid;

END;
GO

-- DELETE JADWAL DOSEN 
GO
ALTER PROCEDURE sp_DeleteJadwalDosen
    @Jid INT
AS
BEGIN
    -- Validasi: Jangan hapus jika sudah ada mahasiswa yang booking
    IF EXISTS (SELECT 1 FROM Pertemuan WHERE JadwalID = @Jid)
        RAISERROR('Gagal: Jadwal tidak bisa dihapus karena memiliki riwayat booking/pertemuan!', 16, 1);
    
    ELSE IF (SELECT Status FROM JadwalDosen WHERE JadwalID = @Jid) = 'Booked'
        RAISERROR('Gagal: Jadwal sedang dalam status Booked!', 16, 1);
    
    ELSE
        DELETE FROM JadwalDosen WHERE JadwalID = @Jid;
END;
GO

-- SEARCH JADWAL DOSEN 
GO
ALTER PROCEDURE sp_SearchJadwalDosen
    @Keyword VARCHAR(100)
AS
BEGIN
    -- Mengambil data dari VIEW (Poin 2 UCP: Query Select diubah menjadi VIEW)
    SELECT * FROM View_JadwalDosenFull
    WHERE NamaDosen LIKE '%' + @Keyword + '%' 
       OR NIDN LIKE '%' + @Keyword + '%'
       OR Status LIKE '%' + @Keyword + '%';
END;
GO

/*
=== STORED PROCEDURE - BOOKING ===
*/
-- INSERT BOOKING 
GO
CREATE PROCEDURE sp_InsertBooking
    @Jid INT, 
    @Mid INT, 
    @Catatan VARCHAR(MAX)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Logika kamu di sini...
        IF (SELECT Status FROM JadwalDosen WHERE JadwalID = @Jid) <> 'Available'
            THROW 50000, 'Jadwal sudah tidak tersedia!', 1;

        INSERT INTO Pertemuan (JadwalID, MahasiswaID, Status, CatatanPermintaan)
        VALUES (@Jid, @Mid, 'Pending', @Catatan);

        UPDATE JadwalDosen SET Status = 'Booked' WHERE JadwalID = @Jid;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- UPDATE BOOKING 
GO
CREATE PROCEDURE sp_UpdateStatusBooking
    @Pid INT, @Status VARCHAR(20)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Pertemuan WHERE PertemuanID = @Pid)
        RAISERROR('Data tidak ditemukan!', 16, 1);
    ELSE
        UPDATE Pertemuan SET Status = @Status WHERE PertemuanID = @Pid;
END;
GO

-- DELETE BOOKING 
GO
CREATE PROCEDURE sp_DeleteBooking
    @Pid INT -- ID Pertemuan/Booking
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Ambil ID Jadwal terkait sebelum data booking dihapus
        DECLARE @Jid INT;
        SELECT @Jid = JadwalID FROM Pertemuan WHERE PertemuanID = @Pid;

        -- 1. Hapus data dari tabel Pertemuan
        DELETE FROM Pertemuan WHERE PertemuanID = @Pid;

        -- 2. Kembalikan status jadwal menjadi 'Available'
        IF @Jid IS NOT NULL
        BEGIN
            UPDATE JadwalDosen SET Status = 'Available' WHERE JadwalID = @Jid;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- SEARCH BOOKING 
GO
CREATE PROCEDURE sp_SearchBooking
    @Keyword VARCHAR(100)
AS
BEGIN
    -- Poin 2 UCP: Menggunakan VIEW untuk Query Select
    SELECT * FROM View_BookingPertemuan
    WHERE NIM_Mahasiswa LIKE '%' + @Keyword + '%' 
       OR Nama_Mahasiswa LIKE '%' + @Keyword + '%' 
       OR Nama_Dosen LIKE '%' + @Keyword + '%'
       OR Status_Booking LIKE '%' + @Keyword + '%';
END;
GO

GO
/* 1. SP UNTUK MENGHITUNG TOTAL MAHASISWA (OUTPUT PARAMETER) */
-- Berdasarkan Langkah 6 pada modul [cite: 103, 105]
CREATE PROCEDURE sp_CountMahasiswa
    @Total INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @Total = COUNT(*) FROM Mahasiswa
END;
GO

/* 2. SP UNTUK MENGHITUNG JADWAL TERSEDIA (OUTPUT PARAMETER) */
CREATE PROCEDURE sp_CountJadwalTersedia
    @Total INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @Total = COUNT(*) FROM JadwalDosen WHERE Status = 'Available'
END;
GO

--UPDATE
UPDATE Mahasiswa
SET Email = 'andi@mail.umy.ac.id'
WHERE MahasiswaID = 1;

UPDATE Mahasiswa
SET Email = 'budi@mail.umy.ac.id'
WHERE MahasiswaID = 2;

UPDATE Mahasiswa
SET Email = 'citra@mail.umy.ac.id'
WHERE MahasiswaID = 3;

UPDATE Dosen
SET Email = 'agus@mail.umy.ac.id'
WHERE DosenID = 1;

UPDATE Dosen
SET Email = 'budi@mail.umy.ac.id'
WHERE DosenID = 2;

UPDATE Dosen
SET Email = 'clara@mail.umy.ac.id'
WHERE DosenID = 3;

UPDATE Mahasiswa
SET NIM = '23000123456'
WHERE MahasiswaID = 1;

UPDATE Mahasiswa
SET NIM = '23000123457'
WHERE MahasiswaID = 2;

UPDATE Mahasiswa
SET NIM = '23000123458'
WHERE MahasiswaID = 3;

--SP
CREATE PROCEDURE sp_GetMahasiswa
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MahasiswaID,
        NIM,
        Nama,
        Email
    FROM View_DataMahasiswa;
END;
GO

CREATE PROCEDURE sp_GetMahasiswaByNIM
    @NIM VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MahasiswaID,
        NIM,
        Nama,
        Email
    FROM View_DataMahasiswa
    WHERE NIM = @NIM;
END;

EXEC sp_GetMahasiswa

EXEC sp_SearchMahasiswa
    @Keyword = 'Andi'

	EXEC sp_InsertMahasiswa
    @NIM = '23000199999',
    @Nama = 'Budi',
    @Email = 'budi@mail.umy.ac.id'

EXEC sp_UpdateMahasiswa
    @ID = 1,
    @NIM = '23000111111',
    @Nama = 'Andi Baru',
    @Email = 'andi@mail.umy.ac.id'

EXEC sp_DeleteMahasiswa
    @ID = 1


ALTER PROCEDURE sp_GetBooking
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PertemuanID,
        NIM_Mahasiswa,
        Nama_Mahasiswa,
        Nama_Dosen,
        Tanggal,
        WaktuMulai,
        WaktuSelesai,
        Status_Booking,
        CatatanPermintaan
    FROM View_BookingPertemuan;
END;
GO

CREATE VIEW View_JadwalDosenLokasi
AS
SELECT
    j.JadwalID,
    d.DosenID,
    d.NIDN,
    d.Nama AS NamaDosen,
    j.Tanggal,
    j.WaktuMulai,
    j.WaktuSelesai,
    j.Status,
    j.Lokasi
FROM JadwalDosen j
JOIN Dosen d
ON j.DosenID = d.DosenID;
GO

CREATE PROCEDURE sp_GetJadwalDosenLokasi
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        JadwalID,
        DosenID,
        NIDN,
        NamaDosen,
        Tanggal,
        WaktuMulai,
        WaktuSelesai,
        Status,
        Lokasi
    FROM View_JadwalDosenLokasi
    ORDER BY Tanggal ASC;
END;
GO

--SELECT BY NIM

CREATE PROCEDURE sp_GetBookingByNIM
    @NIM VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PertemuanID,
        NIM_Mahasiswa,
        Nama_Mahasiswa,
        Nama_Dosen,
        Tanggal,
        WaktuMulai,
        WaktuSelesai,
        Status_Booking,
        Lokasi_Catatan
    FROM View_BookingPertemuan
    WHERE NIM_Mahasiswa = @NIM;
END;
GO

CREATE PROCEDURE sp_InsertBooking
    @Jid INT,
    @Mid INT,
    @Catatan VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY

        -- Cek jadwal masih tersedia
        IF (SELECT Status 
            FROM JadwalDosen 
            WHERE JadwalID = @Jid) <> 'Available'
        BEGIN
            RAISERROR('Jadwal sudah tidak tersedia!', 16, 1);
            RETURN;
        END

        -- Insert booking
        INSERT INTO Pertemuan
        (
            JadwalID,
            MahasiswaID,
            Status,
            TanggalPermintaan,
            CatatanPermintaan
        )
        VALUES
        (
            @Jid,
            @Mid,
            'Pending',
            GETDATE(),
            @Catatan
        );

        -- Update status jadwal
        UPDATE JadwalDosen
        SET Status = 'Booked'
        WHERE JadwalID = @Jid;

        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO

CREATE PROCEDURE sp_UpdateBooking
    @Pid INT,
    @Status VARCHAR(20),
    @Catatan VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM Pertemuan
        WHERE PertemuanID = @Pid
    )
    BEGIN
        RAISERROR('Data booking tidak ditemukan!', 16, 1);
        RETURN;
    END

    UPDATE Pertemuan
    SET
        Status = @Status,
        CatatanPermintaan = @Catatan
    WHERE PertemuanID = @Pid;
END;
GO

CREATE PROCEDURE sp_DeleteBooking
    @Pid INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY

        DECLARE @Jid INT;

        -- Ambil JadwalID
        SELECT @Jid = JadwalID
        FROM Pertemuan
        WHERE PertemuanID = @Pid;

        -- Hapus booking
        DELETE FROM Pertemuan
        WHERE PertemuanID = @Pid;

        -- Kembalikan status jadwal
        UPDATE JadwalDosen
        SET Status = 'Available'
        WHERE JadwalID = @Jid;

        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO

CREATE PROCEDURE sp_SearchBooking
    @Keyword VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM View_BookingPertemuan
    WHERE
        NIM_Mahasiswa LIKE '%' + @Keyword + '%'
        OR Nama_Mahasiswa LIKE '%' + @Keyword + '%'
        OR Nama_Dosen LIKE '%' + @Keyword + '%'
        OR Status_Booking LIKE '%' + @Keyword + '%';
END;
GO

CREATE PROCEDURE sp_CountBooking
    @Total INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @Total = COUNT(*)
    FROM Pertemuan;
END;
GO

CREATE PROCEDURE sp_GetJadwalByDosen
    @DosenID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        JadwalID,
        CONVERT(VARCHAR, Tanggal, 105) + ' ' +
        LEFT(CONVERT(VARCHAR, WaktuMulai, 108), 5)
        + '-' +
        LEFT(CONVERT(VARCHAR, WaktuSelesai, 108), 5)
        AS Info
    FROM JadwalDosen
    WHERE
        DosenID = @DosenID
        AND Status = 'Available'
        AND Tanggal >= CAST(GETDATE() AS DATE);
END;
GO

CREATE PROCEDURE sp_CheckBookingValid
    @MahasiswaID INT,
    @JadwalID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Sudah booking?
    IF EXISTS
    (
        SELECT 1
        FROM Pertemuan
        WHERE
            MahasiswaID = @MahasiswaID
            AND JadwalID = @JadwalID
    )
    BEGIN
        RAISERROR('Anda sudah booking jadwal ini!', 16, 1);
        RETURN;
    END

    -- Jadwal tersedia?
    IF NOT EXISTS
    (
        SELECT 1
        FROM JadwalDosen
        WHERE
            JadwalID = @JadwalID
            AND Status = 'Available'
    )
    BEGIN
        RAISERROR('Jadwal tidak tersedia!', 16, 1);
        RETURN;
    END
END;
GO

-- tes sp jadwal booking
EXEC sp_GetBooking

EXEC sp_InsertBooking
    @Jid = 1,
    @Mid = 1,
    @Catatan = 'Konsultasi DPA'

EXEC sp_InsertBooking
    @Jid = 1,
    @Mid = 1,
    @Catatan = 'Tes'

EXEC sp_UpdateBooking
    @Pid = 1,
    @Status = 'Completed',
    @Catatan = 'Sudah selesai'

EXEC sp_DeleteBooking
    @Pid = 1

ALTER PROCEDURE sp_GetBooking
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PertemuanID,
        NIM_Mahasiswa,
        Nama_Mahasiswa,
        Nama_Dosen,
        Tanggal,
        WaktuMulai,
        WaktuSelesai,
        Status_Booking,
        CatatanPermintaan
    FROM View_BookingPertemuan;
END;
GO


ALTER TABLE JadwalDosen
ADD Lokasi VARCHAR(100);

EXEC sp_GetJadwalDosenLokasi

UPDATE JadwalDosen
SET Lokasi = 'Ruang Dosen'
WHERE Lokasi IS NULL;
