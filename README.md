# Kenali Benda Sekitar (UTS AR & VR)

Project AR sederhana berbasis **Unity + Vuforia Engine** yang menampilkan model 3D Pikachu dan Jam Casio di atas image target.

![Screenshot](Docs/ss-scene-view.png)

## Fitur

- Marker-based AR menggunakan Vuforia Image Target (2 target).
- Model 3D Pikachu (`Assets/Art/Models/pikachu_new.fbx`) — multi-material, berputar otomatis.
- Model 3D Jam Casio (`Assets/Art/Models/watchr.fbx`) — material silver/gold, berputar otomatis.
- Teks 3D "Pikachu" dan "Casio Watch" di atas model.
- Canvas UI (Screen Space): tombol **Keluar** + **Tampilkan/Sembunyikan Info** + panel informasi 2 entri.
- Konten hanya muncul saat target terdeteksi, sembunyi saat target hilang.
- Mode Play Mode Vuforia menggunakan webcam laptop.
- Render pipeline URP (Universal Render Pipeline).

## Teknologi

- Unity `6000.6.5f1`
- Vuforia Engine `11.4.4`
- Universal Render Pipeline `17.6.0`
- Input System `1.20.0`

## Struktur Project

```text
Assets/
├── Art/
│   ├── Models/             # Model 3D (pikachu_new.fbx, watchr.fbx)
│   └── Textures/           # Tekstur + material (.mat)
├── Editor/                 # Setup scripts (UTSSetup, UTSFinalSetup)
├── Resources/              # VuforiaConfiguration
├── Scenes/                 # MainAR (scene utama AR)
├── Scripts/                # ObjectRotator, ContentVisibility, UIController
├── Settings/               # Konfigurasi URP
└── StreamingAssets/
    └── Vuforia/            # Database image target (PokemonDatabase, AR_DATABASE)
Docs/                       # Laporan + screenshot + image target
```

## Cara Menjalankan

1. Buka project menggunakan **Unity Hub** dengan Unity `6000.6.5f1`.
2. Vuforia Engine `11.4.4` akan di-resolve otomatis dari scoped registry Vuforia.
3. Buka scene `Assets/Scenes/MainAR.unity`.
4. Isi Vuforia license key di `Assets/Resources/VuforiaConfiguration.asset` (sudah terisi).
5. Klik **Play**.
6. Arahkan webcam ke image target:
   - `Docs/image-target-1-pokemon.jpeg` (Pikachu, 15×15)
   - `Docs/image-target-2-jamcasio.png` (Jam Casio, 10×12.5)

## Catatan

- Vuforia Play Mode diatur ke **WEBCAM** pada `Assets/Resources/VuforiaConfiguration.asset`.
- Jika webcam tidak muncul, pastikan webcam tidak sedang digunakan aplikasi lain.
- Image target yang digunakan: `image-pokemon-pikachu` (15×15) dan `image-target-2-jamcasio` (10×12.5).
- `maxSimultaneousImageTargets = 2` — kedua target bisa aktif bersamaan.
- `Application.Quit()` tidak berfungsi di iOS (aturan Apple) — tombol tetap ada sesuai soal; di Android berfungsi normal.
- File build Android: `AdityaBeckhamAR.apk` (root project).

## UTS Requirements (AR & VR — Yayat Sutrayana, S.T., M.Kom.)

| Tugas                                                 | Status |                                                     |
| ----------------------------------------------------- | ------ | --------------------------------------------------- |
| 1. Persiapan Project (Unity 3D + Vuforia + Android)   | ✅     |
| 2. AR Camera + 2 Image Target                         | ✅     |
| 3. Objek 3D sebagai child Image Target + proporsional | ✅     |
| 4. Objek hanya muncul saat target terdeteksi          | ✅     |
| 5. Rotasi otomatis sumbu Y                            | ✅     |
| 6. Teks 3D di atas model                              | ✅     |
| 7. Canvas Screen Space + 2 tombol (Keluar + Info)     | ✅     |
| 8. Build .apk                                         | ✅     | `AdityaBeckhamAR.apk` (min API 29)                  |
| 9. Uji di HP Android                                  | ✅     | Target terdeteksi, objek berputar, tombol berfungsi |

---

**Dosen:** Yayat Sutrayana, S.T., M.Kom.  
**Kelas:** TIF K 24 CID  
**Matakuliah:** Augmented & Virtual Reality / 3 SKS
