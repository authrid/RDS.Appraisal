# RDS Appraisal System

Aplikasi web untuk pengelolaan data appraisal (penilaian agunan/jaminan), dibangun dengan **ASP.NET Core Blazor Server (.NET 10)** menggunakan pola **Clean Architecture**.

## Daftar Isi

- [Teknologi](#teknologi)
- [Struktur Project](#struktur-project)
- [Arsitektur](#arsitektur)
- [Cara Menjalankan](#cara-menjalankan)
- [Konfigurasi](#konfigurasi)
- [Panduan Development](#panduan-development)
- [Testing](#testing)
- [Dokumentasi Tambahan](#dokumentasi-tambahan)

## Teknologi

| Komponen | Teknologi |
|---|---|
| Framework | .NET 10 / ASP.NET Core |
| UI | Blazor Server (Interactive Server Render Mode) |
| ORM | Entity Framework Core |
| Database | SQLite (default), SQL Server, PostgreSQL |
| Autentikasi | Cookie Authentication (user dari konfigurasi) |
| Styling | Tailwind CSS (via CDN) + custom CSS variables (`wwwroot/app.css`) |
| Testing | xUnit + bUnit (smoke tests) |

## Struktur Project

```
RDS.Core.Apprasial/
├── AppraisalSystem.slnx              # Solution file
├── docs/                             # Dokumentasi workflow & backlog
├── src/
│   ├── AppraisalSystem.Domain/       # Lapisan Domain (inti bisnis)
│   │   ├── Entities/                 # Entity domain (Appraisal)
│   │   └── Enums/                    # Enum bisnis (AppraisalStatus, CollateralType, dll.)
│   │
│   ├── AppraisalSystem.Application/  # Lapisan Application (use case)
│   │   ├── Common/                   # Utilitas umum (PagedResult)
│   │   ├── Dtos/                     # Data Transfer Objects
│   │   ├── Interfaces/               # Kontrak (IAppraisalRepository, IAppraisalService)
│   │   ├── Services/                 # Implementasi business logic (AppraisalService)
│   │   └── DependencyInjection.cs    # Registrasi service lapisan ini
│   │
│   ├── AppraisalSystem.Infrastructure/  # Lapisan Infrastructure (akses data)
│   │   ├── Options/                  # Opsi konfigurasi (DatabaseOptions)
│   │   ├── Persistence/              # DbContext & DbInitializer (seeding)
│   │   ├── Repositories/             # Implementasi repository (EF Core)
│   │   └── DependencyInjection.cs    # Registrasi DbContext & repository
│   │
│   └── AppraisalSystem.Web/          # Lapisan Presentasi (Blazor Server)
│       ├── Authentication/           # Model user dari konfigurasi
│       ├── Components/
│       │   ├── Layout/               # Layout, navbar, sidebar
│       │   ├── Pages/                # Halaman per modul (lihat tabel di bawah)
│       │   └── Shared/               # Komponen reusable
│       │       ├── Base/             # Form input, button, card, dll.
│       │       ├── DataDisplay/      # Table, pagination, badge
│       │       └── Feedback/         # Alert, dialog konfirmasi, spinner
│       ├── Options/                  # ThemeOptions (branding & warna)
│       ├── wwwroot/                  # Asset statis (CSS, JS, gambar)
│       ├── AppRoutes.cs              # Definisi terpusat semua route aplikasi
│       ├── Program.cs                # Entry point & konfigurasi pipeline
│       └── appsettings.json          # Konfigurasi aplikasi
│
└── tests/
    └── AppraisalSystem.SmokeTests/   # Smoke test (service, route, render komponen)
```

### Modul Halaman (`Components/Pages/`)

| Folder | Modul | Route Utama |
|---|---|---|
| `Dashboard.razor` | Dashboard ringkasan | `/dashboard` |
| `PencarianData/` | Pencarian data appraisal (list, detail, form, memo, history) | `/pencarian-data` |
| `PengkinianData/` | Pengkinian data | `/pengkinian-data` |
| `Inquiry/` | Inquiry data | `/inquiry` |
| `Report/` | Laporan & export | `/report` |
| `Login.razor` | Halaman login | `/login` |

## Arsitektur

Project mengikuti **Clean Architecture** dengan arah dependensi dari luar ke dalam:

```
Web  ──►  Application  ──►  Domain
 │             ▲
 └──►  Infrastructure ─┘
```

- **Domain** — entity dan enum murni, tanpa dependensi eksternal.
- **Application** — business logic, DTO, dan interface. Tidak tahu detail database.
- **Infrastructure** — implementasi teknis: EF Core `DbContext`, repository, seeding database.
- **Web** — UI Blazor, autentikasi, routing, dan komposisi dependency injection.

Aturan penting:

1. **Domain tidak boleh mereferensi project lain.**
2. **Application hanya mereferensi Domain.** Akses data dilakukan lewat interface (`IAppraisalRepository`).
3. **Web tidak mengakses `DbContext` langsung** — selalu melalui service di Application.
4. **Semua route didefinisikan di `AppRoutes.cs`** — jangan hardcode string route di komponen.

## Cara Menjalankan

### Prasyarat

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Langkah

```bash
# Clone repository
git clone <url-repository>
cd RDS.Core.Apprasial

# Restore & build
dotnet build

# Jalankan aplikasi
dotnet run --project src/AppraisalSystem.Web
```

Aplikasi berjalan di URL yang tercantum pada `Properties/launchSettings.json`. Database SQLite (`appraisal-system.db`) dibuat dan di-seed otomatis saat pertama kali dijalankan (lihat `DbInitializer`).

### Akun Default (Development)

| Username | Password | Role |
|---|---|---|
| `admin` | `admin123` | Admin |
| `appraiser` | `appraiser123` | Appraiser |
| `supervisor` | `supervisor123` | Supervisor |

> ⚠️ Akun didefinisikan di `appsettings.json` bagian `Authentication:Users`. Ganti sebelum deploy ke production.

## Konfigurasi

Semua konfigurasi ada di `src/AppraisalSystem.Web/appsettings.json`:

### Database

Mendukung tiga provider — atur di section `Database`:

```json
"Database": {
  "Provider": "Sqlite",          // Sqlite | SqlServer | PostgreSQL
  "ConnectionString": "Data Source=appraisal-system.db"
}
```

### Tema / Branding

Warna dan logo aplikasi dapat diubah tanpa menyentuh kode lewat section `Theme`:

```json
"Theme": {
  "BrandName": "RDS Appraisal",
  "PrimaryColor": "#113a69",
  "LogoPath": "/images/brand/regalis-logo.jpg"
}
```

Warna divalidasi saat startup (format hex). Gunakan halaman `/theme-preview` untuk melihat hasilnya.

## Panduan Development

### Menambah Halaman Baru

1. Tambahkan konstanta route di `src/AppraisalSystem.Web/AppRoutes.cs`.
2. Buat file `.razor` di `Components/Pages/<NamaModul>/` dengan direktif `@page` yang mengacu ke konstanta route tersebut.
3. Tambahkan menu navigasi di `Components/Layout/AppSidebar.razor` bila diperlukan.
4. Tambahkan smoke test route di `tests/AppraisalSystem.SmokeTests/RouteRegistrationSmokeTests.cs`.

### Menambah Fitur / Use Case Baru

Ikuti alur lapisan dari dalam ke luar:

1. **Domain** — tambah/ubah entity di `Domain/Entities` atau enum di `Domain/Enums`.
2. **Application** — buat DTO di `Dtos/`, tambahkan method pada interface (`Interfaces/`), lalu implementasikan di `Services/`.
3. **Infrastructure** — implementasikan method repository baru di `Repositories/`, sesuaikan `AppraisalDbContext` bila skema berubah.
4. **Web** — konsumsi service lewat `@inject` di komponen Blazor.

### Menggunakan Komponen Shared

Gunakan komponen di `Components/Shared/` agar UI konsisten:

- **Form**: `BaseTextField`, `BaseSelectField`, `BaseNumberField`, `BaseCheckboxField`, dll.
- **Tampilan data**: `BaseTable`, `BasePagination`, `BaseBadge`.
- **Feedback**: `BaseAlert`, `ConfirmationDialog`, `LoadingSpinner`.

Buat komponen baru di folder `Shared/` bila pola UI dipakai lebih dari satu halaman.

### Konvensi

- Bahasa route/modul mengikuti istilah bisnis (mis. `pencarian-data`, `pengkinian-data`).
- Nullable reference types dan implicit usings aktif di seluruh project.
- Styling menggunakan utility class Tailwind CSS; warna brand diambil dari CSS variables (`--brand-*`) yang di-generate dari `ThemeOptions` — jangan hardcode warna hex di komponen.
- Registrasi service dilakukan lewat extension method `AddApplication()` dan `AddInfrastructure()` — bukan langsung di `Program.cs`.

## Testing

```bash
# Jalankan semua test
dotnet test
```

Smoke test mencakup:

| File | Cakupan |
|---|---|
| `AppraisalServiceSmokeTests.cs` | Business logic `AppraisalService` |
| `AppRoutesSmokeTests.cs` | Helper route di `AppRoutes` |
| `RouteRegistrationSmokeTests.cs` | Semua route terdaftar di komponen |
| `ComponentRenderSmokeTests.cs` | Render komponen (bUnit) |

