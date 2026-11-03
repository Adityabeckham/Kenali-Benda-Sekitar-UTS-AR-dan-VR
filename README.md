# PokemonAR

Project AR sederhana berbasis **Unity + Vuforia Engine** yang menampilkan model 3D Pikachu di atas image target Pokemon.

![Screenshot PokemonAR](Docs/screenshot.png)

## Fitur

- Marker-based AR menggunakan Vuforia Image Target.
- Model 3D Pikachu (`Assets/Art/Models/pikachu.FBX`).
- Material Pikachu otomatis berwarna kuning saat Play Mode.
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
│   └── Models/             # Model 3D Pikachu
├── Editor/                 # Setup/migrasi Vuforia
├── Resources/              # VuforiaConfiguration
├── Scenes/                 # SampleScene (scene utama AR)
├── Scripts/                # PikachuYellow.cs
├── Settings/               # Konfigurasi URP
└── StreamingAssets/
    └── Vuforia/            # Database image target Pokemon
```

## Cara Menjalankan

1. Buka project menggunakan **Unity Hub** dengan Unity `6000.6.5f1`.
2. Vuforia Engine `11.4.4` akan di-resolve otomatis dari scoped registry Vuforia.
3. Buka scene `Assets/Scenes/SampleScene.unity`.
4. Isi Vuforia license key di `Assets/Resources/VuforiaConfiguration.asset`.
5. Klik **Play**.
6. Arahkan webcam ke image target `image-pokemon-pikachu`.

## Catatan

- Vuforia Play Mode diatur ke **WEBCAM** pada `Assets/Resources/VuforiaConfiguration.asset`.
- Jika webcam tidak muncul, pastikan webcam tidak sedang digunakan aplikasi lain dan cek pesan error di Console Unity.
- Image target yang digunakan: `image-pokemon-pikachu` dengan ukuran `15 x 15`.
- Script `PikachuYellow.cs` mengubah material model menjadi kuning saat runtime.
- License key Vuforia **tidak disertakan** di repository ini. Isi license key sendiri secara lokal sebelum menjalankan Play Mode.
