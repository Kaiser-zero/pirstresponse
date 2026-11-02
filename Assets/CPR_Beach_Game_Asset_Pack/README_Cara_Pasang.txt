PAKET ASET GAME CPR PANTAI – UNITY 6

ISI PAKET
01_Character_Atlas
- Character_UI_Props_Atlas.png: lembar referensi karakter Lifeguard, NPC, pose rescue/CPR, dan beberapa elemen UI.

02_Background
- Background_Pantai_Panorama.png: potongan panorama pantai dari atlas.
- Background_Laut_Langit.png: potongan laut dan langit.

03_UI_Props_Atlas
- UI_Props_Reference_Atlas.png: referensi panel tutorial, panel misi, tombol, ikon, AED, kotak P3K, menara penjaga, pohon, papan, dan dekorasi.

04_Reference
- Lifeguard_CPR_Pose_Reference.png: referensi pose karakter dan NPC.

PENTING – STATUS ASET
- File atlas adalah lembar referensi gabungan, bukan sprite individual yang sudah dipotong.
- Pola kotak-kotak pada atlas karakter/UI merupakan bagian dari gambar (bukan alpha transparan). Jangan langsung memakai seluruh atlas sebagai sprite karakter.
- Untuk karakter, gunakan Sprite Editor > Sprite Mode: Multiple > Slice (Grid/Automatic), lalu rapikan Rect tiap sprite secara manual. Karena ukuran pose berbeda-beda, hasil Slice otomatis mungkin perlu disesuaikan.
- Background panorama hasil crop bisa langsung dipakai sebagai gambar latar, tetapi mungkin perlu penyesuaian skala/aspect ratio.
- Beberapa tulisan/ikon pada atlas hanya referensi visual; lebih baik buat teks UI interaktif menggunakan TextMeshPro di Unity agar mudah diedit.
- Aset ini merupakan konsep visual untuk prototipe game. Untuk materi CPR, gunakan panduan pertolongan pertama resmi dan arahan layanan darurat; game bukan pengganti pelatihan CPR.

CARA MASUKKAN KE UNITY
1. Ekstrak ZIP.
2. Salin folder aset yang diperlukan ke Assets/CPRAssets di project Unity.
3. Klik PNG di Project window.
4. Untuk atlas karakter: Texture Type = Sprite (2D and UI), Sprite Mode = Multiple.
5. Klik Apply, buka Sprite Editor, lalu Slice dan Apply.
6. Untuk background: Texture Type = Sprite (2D and UI), Sprite Mode = Single, lalu Apply.

Saran: mulai dari Background_Pantai_Panorama.png, satu sprite Lifeguard, satu sprite NPC terbaring, lalu UI dibuat dengan Canvas/TextMeshPro.
