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
| 1. Persiapan Project        | ✅     | Unity 6000.6.5f1 + Vuforia Engine 11.4.4, scene `MainAR`                                                                         |
| 2. AR Camera + Image Target | ✅     | 2 target: `image-pokemon-pikachu` (PokemonDatabase) + `image-target-2-jamcasio` (AR_DATABASE); `maxSimultaneousImageTargets = 2` |
| 3. Objek 3D di atas target  | ✅     | Pikachu (`pikachu_new.fbx`) + Jam Casio (`watchr.fbx`) sebagai child Content masing-masing target                                |
| 4. Animasi + teks 3D        | ✅     | `ObjectRotator` (45°/dtk) di kedua model + TextMesh "Pikachu" & "Casio Watch"                                                    |
| 5. UI (2 tombol)            | ✅     | Canvas: **Tampilkan/Sembunyikan Info** + **Keluar** + panel 2 entri                                                              |
| 6. Build & pengujian        | ✅     | Build berhasil `AdityaBeckhamAR.apk` (min API 29), diuji di HP Android                                                         |

---

## 3. Penjelasan Singkat Fungsi Tiap Script C#

### `Assets/Scripts/ObjectRotator.cs`

```csharp
transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.Self);
```

- **Fungsi:** Memutar objek 3D (Pikachu & Jam Casio) otomatis mengelilingi **sumbu Y** setiap frame.
- **Parameter:** `speed` (default 45°/detik), dapat diubah dari Inspector.
- **Dipasang di:** GameObject `pikachu_pivot` dan `JamCasio_pivot`.

### `Assets/Scripts/ContentVisibility.cs`

- **`Show()`** — Aktifkan GameObject `Content_*` (konten muncul hanya saat target terdeteksi).
- **`Hide()`** — Nonaktifkan GameObject `Content_*` (konten sembunyi saat target hilang).
- **Dipasang di:** masing-masing ImageTarget; di-wire ke event `OnTargetFound` / `OnTargetLost` dari `DefaultObserverEventHandler`.

### `Assets/Scripts/UIController.cs`

- **`ToggleInfo()`** — Menampilkan/menyembunyikan panel informasi (`infoPanel`). Label tombol otomatis berubah antara "Tampilkan Info" ↔ "Sembunyikan Info".
- **`QuitApp()`** — Memanggil `Application.Quit()` untuk menutup aplikasi. Di Editor berhenti Play Mode.
- **`Start()`** — Mendaftarkan listener klik pada kedua tombol.
- **Dipasang di:** GameObject `Canvas`.

### `Assets/Editor/UTSSetup.cs` / `UTSFinalSetup.cs` _(script Editor, tidak ikut build)_

- Menu **UTS → Final Setup**: import `AR_DATABASE.unitypackage`, set `maxSimultaneousImageTargets = 2`, bungkus konten dalam `Content_*` + wire visibility, buat ImageTarget jam Casio + FBX + rotator + label, update panel info.

---

## 4. Konfigurasi AR

| Komponen           | Nilai                                                                              |
| ------------------ | ---------------------------------------------------------------------------------- |
| Engine             | Vuforia Engine 11.4.4                                                              |
| License key        | Terisi di `Assets/Resources/VuforiaConfiguration.asset`                            |
| Play Mode (Editor) | **WEBCAM** (uji di laptop)                                                         |
| Dataset 1          | `Vuforia/PokemonDatabase` — target: `image-pokemon-pikachu` (15 × 15)              |
| Dataset 2          | `Vuforia/AR_DATABASE` — target: `image-target-2-jamcasio` (10 × 12.5)              |
| Max simultaneous   | **2** target aktif bersamaan                                                       |
| ARCamera           | Tag `MainCamera`, komponen `VuforiaBehaviour` + URP Camera                         |
| Visibilitas        | `Content_Pokemon` / `Content_Jam` → `SetActive` via `OnTargetFound`/`OnTargetLost` |
| Model Pikachu      | `pikachu_new.fbx` (multi-material: kuning, coklat, hitam, merah, putih)            |
| Model Jam Casio    | `watchr.fbx` (VRay materials: silver body, digital display, bump detail)           |

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

## 6. Screenshot

### 1. Scene view MainAR (2 ImageTarget + Content + Canvas)
![Scene view MainAR](ss-scene-view.png)

### 2. Play Mode: target Pikachu terdeteksi, model tampil & berputar
![Play Mode Pikachu](ss-gameview-pikachu-target.png)

### 3. Play Mode: target Jam Casio terdeteksi, model tampil & berputar
![Play Mode Jam Casio](ss-gameview-jamcasio.png)

### 4. Teks 3D "Pikachu" / "Casio Watch" di atas model
![Scene view MainAR](ss-scene-view.png)

### 5. Panel Tampilkan Info/Sembunyikan Info 
![Panelinfo](ss-gameview-tampilkan-info.png)
![Panel info](ss-gameview-sembunyikaninfo.png)

### 6. Tampilan di layar HP (setelah build)
Build berhasil: `AdityaBeckhamAR.apk` (min API 29), diuji di HP Android.

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
- `cameraUsageDescription` iOS **sudah diisi** di Player Settings (syarat wajib agar kamera tidak blank).
- `Application.Quit()` **tidak berfungsi di iOS** (aturan Apple) — tombol tetap ada sesuai soal; di Android berfungsi normal.

---

## 8. Image Target (2 Target) — Selesai ✅

1. Foto target 1: `Docs/image-target-1-pokemon.jpeg` (Pikachu, 15 × 15).
2. Foto target 2: `Docs/image-target-2-jamcasio.png` (Jam Casio, 10 × 12.5).
3. Dataset hasil download dari Vuforia Target Manager: `AR_DATABASE.unitypackage`.
4. File sudah di-import ke `Assets/StreamingAssets/Vuforia/AR_DATABASE.*` dan `PokemonDatabase.*`.
5. Scene `MainAR` punya 2 ImageTarget + Content + FBX model + rotator + teks 3D + preview quad (Scene view).

---

## 9. Struktur Project

```text
Assets/
├── Art/
│   ├── Models/
│   │   ├── pikachu_new.fbx       # Model 3D Pikachu (multi-material)
│   │   └── watchr.fbx            # Model 3D Jam Casio (VRay materials)
│   └── Textures/
│       ├── casio silver watch_Base_color_4.png
│       ├── casio watch_Base_color_1.png
│       ├── casio watch_Base_metalness*.png
│       ├── casio watch_Normal_OpenGL_0.png
│       ├── casio watch_Normal_OpenGL_3.png
│       ├── internal_ground_ao_texture.jpeg
│       └── VRay*.mat             # 6 material jam Casio
├── Editor/
│   ├── UTSFinalSetup.cs          # Setup scene UTS (menu UTS)
│   └── UTSSetup.cs               # Setup awal
├── Resources/
│   └── VuforiaConfiguration.asset
├── Scenes/
│   └── MainAR.unity              # Scene utama AR
├── Scripts/
│   ├── ObjectRotator.cs          # Rotasi sumbu Y
│   ├── ContentVisibility.cs      # Show/Hide konten saat target ada/hilang
│   ├── UIController.cs           # Tombol Keluar + Info
├── Settings/                     # URP
└── StreamingAssets/Vuforia/      # Database image target
    ├── PokemonDatabase.*         # Target 1: Pikachu
    └── AR_DATABASE.*            # Target 2: Jam Casio (+ Pikachu)
Docs/
├── image-target-1-pokemon.jpeg
├── image-target-2-jamcasio.png
├── ss-scene-view.png
├── ss-gameview-pikachu-target.png
├── ss-gameview-jamcasio.png
├── ss-gameview-tampilkan-info.png
└── ss-gameview-sembunyikaninfo.png
```
