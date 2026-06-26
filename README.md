<h1> Skenario SQL Injection </h2>
Pada form login aplikasi, terdapat kerentanan SQL Injection karena query login dibuat menggunakan penggabungan string langsung dari input pengguna tanpa parameterized query. 
<br>
<br>
<strong> Input yang digunakan </strong> <br>
Username: ' OR 1=1 -- <br>
Password: 123546 (bebas, bukan sandi valid)
<br>
<br>
<strong> Hasil Pengujian </strong> <br>
Sistem berhasil login tanpa memerlukan username dan password tiak valid. <br>
Payload SQL Injection berhasil memanipulasi query login sehingga kondisi autentikasi selalu bernilai TRUE.

<img width="857" height="399" alt="image" src="https://github.com/user-attachments/assets/d63f79dd-f83d-4a41-8d27-052d4c00b2b3" />

<h1> Setup aplikasi </h1>

1. Jalankan file `TemuDosen.exe`.
  <img width="618" height="37" alt="image" src="https://github.com/user-attachments/assets/c5041453-91f0-45e8-83be-9c80cb6c7e6e" />

2. Pilih bahasa yang akan digunakan saat proses instalasi, kemudian klik OK
  <img width="354" height="167" alt="Screenshot 2026-06-25 203121" src="https://github.com/user-attachments/assets/31e27b76-07e6-4f0a-aa84-cc45d1082b38" />

3. Tentukan lokasi instalasi aplikasi, kemudian klik Next
   <img width="589" height="460" alt="image" src="https://github.com/user-attachments/assets/c53fd552-a3ab-41d2-897e-e12c79268602" />

3. Pilih opsi Create a desktop shortcut apabila ingin membuat shortcut di Desktop (opsional), kemudian klik Next
   <img width="595" height="456" alt="Screenshot 2026-06-25 203658" src="https://github.com/user-attachments/assets/f5a349de-5b7b-446f-a67d-e21cfa8a6e79" />
   <img width="593" height="461" alt="Screenshot 2026-06-25 203716" src="https://github.com/user-attachments/assets/6dbbaf90-880e-4084-85d7-1b8ac06e0203" />

4. Klik Install untuk memulai proses instalasi aplikasi.
   <img width="589" height="460" alt="Screenshot 2026-06-25 203731" src="https://github.com/user-attachments/assets/e4987523-f6d1-45b2-8e9e-b658dfb8c2ad" />

5. Setelah proses instalasi selesai, klik Finish untuk menutup installer dan menjalankan aplikasi
   <img width="591" height="460" alt="Screenshot 2026-06-25 203750" src="https://github.com/user-attachments/assets/1176c4fc-b44a-45e3-9dce-5d686d62b7d0" />




   

