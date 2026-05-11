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
        CHECK (Status IN ('Pending', 'Available', 'Denied', 'Completed')),
    TanggalPermintaan DATETIME NOT NULL DEFAULT GETDATE(),
    CatatanPermintaan VARCHAR(MAX),
    FOREIGN KEY (JadwalID) REFERENCES JadwalDosen(JadwalID),
    FOREIGN KEY (MahasiswaID) REFERENCES Mahasiswa(MahasiswaID)
);

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

