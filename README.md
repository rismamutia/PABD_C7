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
