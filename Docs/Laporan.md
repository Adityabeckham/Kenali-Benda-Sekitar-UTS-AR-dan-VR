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

| Tugas                       | Status | Keterangan                                                                                                                       |
| --------------------------- | ------ | -------------------------------------------------------------------------------------------------------------------------------- |
| 1. Persiapan Project        | ✅     | Unity 3D + Vuforia Engine 11.4.4, scene `MainAR`                                                                                 |
| 2. AR Camera + Image Target | ✅     | 2 target: `image-pokemon-pikachu` (PokemonDatabase) + `image-target-2-jamcasio` (AR_DATABASE); `maxSimultaneousImageTargets = 2` |
| 3. Objek 3D di atas target  | ✅     | Pikachu + FBX Jam Casio sebagai child Content masing-masing target                                                               |
| 4. Animasi + teks 3D        | ✅     | `ObjectRotator` (45°/dtk) di kedua model + TextMesh "Pikachu" & "Casio Watch"                                                    |
| 5. UI (2 tombol)            | ✅     | Canvas: **Tampilkan/Sembunyikan Info** + **Keluar** + panel 2 entri                                                              |
| 6. Build & pengujian        | ⚠️     | Menunggu instalasi module Android Build Support + test di HP                                                                     |

---

## 3. Penjelasan Singkat Fungsi Tiap Script C#

### `Assets/Scripts/ObjectRotator.cs`

```csharp
transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
```

- **Fungsi:** Memutar objek 3D (Pikachu & Jam Casio) otomatis mengelilingi **sumbu Y** setiap frame.
- **Parameter:** `speed` (default 45°/detik), dapat diubah dari Inspector.
- **Dipasang di:** GameObject `pikachu` dan `JamCasio`.

### `Assets/Scripts/ContentVisibility.cs`

- **`Show()`** — Aktifkan GameObject `Content_*` + nyalakan kembali semua Renderer (konten muncul hanya saat target terdeteksi).
- **`Hide()`** — Nonaktifkan GameObject `Content_*` (konten sembunyi saat target hilang).
- **Dipasang di:** masing-masing ImageTarget; di-wire ke event `OnTargetFound` / `OnTargetLost` dari `DefaultObserverEventHandler`.

### `Assets/Scripts/UIController.cs`

- **`ToggleInfo()`** — Menampilkan/menyembunyikan panel informasi (`infoPanel`). Label tombol otomatis berubah antara "Tampilkan Info" ↔ "Sembunyikan Info".
- **`QuitApp()`** — Memanggil `Application.Quit()` untuk menutup aplikasi. Di Editor berhenti Play Mode.
- **`Start()`** — Mendaftarkan listener klik pada kedua tombol.
- **Dipasang di:** GameObject `Canvas`.

### `Assets/Scripts/PikachuYellow.cs` _(bonus, script lama)_

- **Fungsi:** Saat scene di-Play, mengubah seluruh material model Pikachu menjadi warna kuning (termasuk emission halus) agar tampil tanpa texture bawaan FBX.

### `Assets/Editor/UTSSetup.cs` / `UTSFinalSetup.cs` _(script Editor, tidak ikut build)_

- Menu **UTS → Final Setup**: import `AR_DATABASE.unitypackage`, set `maxSimultaneousImageTargets = 2`, bungkus konten dalam `Content_*` + wire visibility, buat ImageTarget jam Casio + FBX + rotator + label, update panel info.

---

## 4. Konfigurasi AR

| Komponen           | Nilai                                                                              |
| ------------------ | ---------------------------------------------------------------------------------- |
| Engine             | Vuforia Engine 11.4.4                                                              |
| License key        | Terisi di `Assets/Resources/VuforiaConfiguration.asset`                            |
| Play Mode (Editor) | **WEBCAM** (uji di laptop)                                                         |
| Dataset 1          | `Vuforia/PokemonDatabase` — target: `image-pokemon-pikachu`                        |
| Dataset 2          | `Vuforia/AR_DATABASE` — target: `image-target-2-jamcasio` (10 × 12.5)              |
| Max simultaneous   | **2** target aktif bersamaan                                                       |
| ARCamera           | Tag `MainCamera`, komponen `VuforiaBehaviour` + URP Camera                         |
| Visibilitas        | `Content_Pokemon` / `Content_Jam` → `SetActive` via `OnTargetFound`/`OnTargetLost` |

---

## 5. Cara Test di Unity (sebelum build HP)

1. Buka project di Unity Hub (Unity `6000.6.5f1`).
2. Buka scene **`Assets/Scenes/MainAR.unity`**.
3. Klik **Play** (▶) — kamera webcam menyala.
4. Arahkan webcam ke **cetak gambar target** (kartu Pokémon / foto jam) yang ditampilkan di layar kedua/HP.
5. **Yang harus terlihat:**
   - Saat kamera menyala → **belum ada** model/teks 3D (konten tersembunyi) ✅
   - Arahkan ke kartu **Pikachu** → model muncul + **berputar** + teks "Pikachu" ✅
   - Arahkan ke foto **Jam Casio** → model muncul + **berputar** + teks "Casio Watch" ✅
   - Jauhkan target → konten **sembunyi** otomatis ✅
   - Tombol **Tampilkan/Sembunyikan Info** membuka/tutup panel ✅
   - Tombol **Keluar** menghentikan Play Mode ✅

---

## 6. Screenshot (isi sendiri)

| No  | Bukti                                                           | Path screenshot |
| --- | --------------------------------------------------------------- | --------------- |
| 1   | Scene hierarchy MainAR (2 ImageTarget + Content + Canvas)       | _(isi)_         |
| 2   | Play Mode: target Pikachu terdeteksi, model tampil & berputar   | _(isi)_         |
| 3   | Play Mode: target Jam Casio terdeteksi, model tampil & berputar | _(isi)_         |
| 4   | Teks 3D "Pikachu" / "Casio Watch" di atas model                 | _(isi)_         |
| 5   | Panel info terbuka setelah klik tombol                          | _(isi)_         |
| 6   | Tampilan di layar HP (setelah build)                            | _(isi)_         |

> Taruh screenshot di folder `Docs/` dan cantumkan path-nya di tabel.

---

## 7. Build & Pengujian HP

### Android (sesuai soal)

1. Unity Hub → Instal **Android Build Support** (+ SDK/NDK/OpenJDK) untuk Editor 6000.6.5f1.
2. Unity → `File → Build Settings` → platform **Android** → **Switch Platform**.
3. Player Settings: ganti `productName` & package ID (`AR_NamaMahasiswa_NIM` / `com.<nim>.aruts`).
4. **Build** → file `.apk`.
5. Instal di HP Android → uji target terdeteksi, objek berputar, tombol berfungsi.

### iPhone (catatan khusus)

- Build iOS **wajib macOS + Xcode** — tidak bisa dari Windows.
- Opsi: pinjam HP Android untuk demo sesuai soal, atau pinjam Mac untuk build `.ipa`.
- `cameraUsageDescription` iOS **sudah diisi** di Player Settings (syarat wajib agar karma tidak blank).
- `Application.Quit()` **tidak berfungsi di iOS** (aturan Apple) — tombol tetap ada sesuai soal; di Android berfungsi normal.

---

## 8. Image Target ke-2 (Jam Casio) — Selesai ✅

1. Foto target: `Docs/image-target-2-jamcasio.png`.
2. Dataset hasil download dari Vuforia Target Manager: `AR_DATABASE.unitypackage` (berisi `image-pokemon-pikachu` + `image-target-2-jamcasio`).
3. File sudah di-import ke `Assets/StreamingAssets/Vuforia/AR_DATABASE.*`.
4. Scene `MainAR` sudah punya `ImageTarget_JamCasio` (dataset `Vuforia/AR_DATABASE.xml`, ukuran 10 × 12.5) + FBX jam + `ObjectRotator` + teks 3D "Casio Watch".
5. Jika ganti foto/ganti nama target di Vuforia: download ulang dataset, timpa file `.dat`/`.xml`, lalu jalankan menu **UTS → Final Setup** di Unity.

---

## 9. Struktur Project

```text
Assets/
├── Art/Models/
│   ├── pikachu.FBX              # Model 3D Pikachu
│   └── casio watch improved.fbx # Model 3D Jam Casio
├── Editor/
│   ├── UTSSetup.cs              # Setup scene UTS (menu UTS)
│   ├── UTSFinalSetup.cs         # Final: 2 target + visibility
│   └── Vuforia/                 # Cache texture target
├── Resources/VuforiaConfiguration.asset
├── Scenes/MainAR.unity          # Scene utama AR
├── Scripts/
│   ├── ObjectRotator.cs         # Rotasi sumbu Y
│   ├── ContentVisibility.cs     # Show/Hide konten saat target ada/hilang
│   ├── UIController.cs          # Tombol Keluar + Info
│   └── PikachuYellow.cs         # Warna material (bonus)
├── Settings/                    # URP
└── StreamingAssets/Vuforia/     # Database image target
    ├── PokemonDatabase.*        # Target 1: Pikachu
    └── AR_DATABASE.*            # Target 2: Jam Casio (+ Pikachu)
```
