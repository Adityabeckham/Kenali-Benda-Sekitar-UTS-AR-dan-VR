# Laporan Singkat UTS — Augmented & Virtual Reality

**Nama:** Aditya Beckham  
**NIM:** 24552011066  
**Kelas:** TIF RP 24B CID  
**Matakuliah:** Augmented & Virtual Reality / 3 SKS  
**Dosen:** Yayat Sutrayana, S.T., M.Kom.  
**Aplikasi:** "Kenali Benda Sekitar" — AR edukasi berbasis Vuforia Image Target

---

## 1. Skenario

Ketika kamera diarahkan ke gambar target (Image Target), aplikasi menampilkan objek 3D (Pikachu / Jam Casio) beserta informasi pendukung, lengkap dengan animasi rotasi dan tombol interaksi di layar. Konten hanya muncul saat target terdeteksi dan sembunyi saat target hilang.

---

## 2. Tugas yang Dikerjakan

| Tugas | Status | Keterangan |
|---|---|---|
| 1. Persiapan Project | ✅ | Unity 6000.6.5f1 + Vuforia Engine 11.4.4, scene `MainAR` |
| 2. AR Camera + Image Target | ✅ | 2 target: `image-pokemon-pikachu` (PokemonDatabase) + `image-target-2-jamcasio` (AR_DATABASE); `maxSimultaneousImageTargets = 2` |
| 3. Objek 3D di atas target | ✅ | Pikachu (`pikachu_new.fbx`) + Jam Casio (`watchr.fbx`) sebagai child Content masing-masing target |
| 4. Animasi + teks 3D | ✅ | `ObjectRotator` (45°/dtk) di kedua model + TextMesh "Pikachu" & "Casio Watch" |
| 5. UI (2 tombol) | ✅ | Canvas: Tampilkan/Sembunyikan Info + Keluar + panel 2 entri |
| 6. Build & pengujian | ✅ | Build berhasil `AdityaBeckhamAR.apk` (min API 29), diuji di HP Android |

---

## 3. Penjelasan Singkat Fungsi Tiap Script C#

### `Assets/Scripts/ObjectRotator.cs`

```csharp
transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
