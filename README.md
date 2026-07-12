# RDS Appraisal System

Aplikasi web untuk pengelolaan data appraisal (penilaian agunan/jaminan), dibangun dengan **ASP.NET Core Blazor Server (.NET 10)** menggunakan pola **Clean Architecture**.

## Daftar Isi

- [Teknologi](#teknologi)
- [Struktur Project](#struktur-project)
- [Arsitektur](#arsitektur)
- [Cara Menjalankan](#cara-menjalankan)
- [Konfigurasi](#konfigurasi)
- [Data Referensi](#data-referensi)
- [Panduan Development](#panduan-development)
- [Fitur Chat AI](#fitur-chat-ai)
- [Testing](#testing)
- [Git / Push ke GitHub](#git--push-ke-github)

## Teknologi

| Komponen | Teknologi |
|---|---|
| Framework | .NET 10 / ASP.NET Core |
| UI | Blazor Server (Interactive Server Render Mode) |
| ORM | Entity Framework Core |
| Database | SQLite (default), SQL Server, PostgreSQL |
| Autentikasi | Cookie Authentication (user dari konfigurasi) |
| AI/LLM | Anthropic Claude (via `OrchestratorClientService`) |
| Styling | Tailwind CSS (via CDN) + custom CSS variables (`wwwroot/app.css`) |
| Testing | xUnit + bUnit (smoke tests) |

## Struktur Project

```
RDS.Appraisal/
├── AppraisalSystem.slnx                 # Solution file
├── .gitignore
├── README.md
├── .github/                             # CI / workflows (jika ada)
├── src/
│   ├── AppraisalSystem.Domain/          # Lapisan Domain (inti bisnis)
│   │   ├── Entities/                    # Entity domain (Appraisal)
│   │   └── Enums/                       # Enum bisnis (AppraisalStatus, CollateralType, dll.)
│   │
│   ├── AppraisalSystem.Application/     # Lapisan Application (use case)
│   │   ├── Common/                      # AppRoles, PagedResult, EnumDisplayExtensions, dll.
│   │   ├── Dtos/                        # Data Transfer Objects (+ ReferenceItemDto)
│   │   ├── Features/ChatUI/             # Kontrak/DTO terkait Chat AI
│   │   ├── Interfaces/                  # IAppraisalRepository, IAppraisalService, IReferenceDataService
│   │   ├── Services/                    # AppraisalService (business logic)
│   │   └── DependencyInjection.cs
│   │
│   ├── AppraisalSystem.Infrastructure/  # Lapisan Infrastructure (akses data)
│   │   ├── Migrations/                  # EF Core migrations
│   │   ├── Options/                     # DatabaseOptions
│   │   ├── Persistence/                 # DbContext & DbInitializer (seeding)
│   │   ├── Repositories/                # Implementasi repository (EF Core)
│   │   └── DependencyInjection.cs
│   │
│   └── AppraisalSystem.Web/             # Lapisan Presentasi (Blazor Server)
│       ├── Authentication/              # Model user dari konfigurasi
│       ├── Components/
│       │   ├── Features/ChatUI/         # Komponen fitur Chat AI
│       │   ├── Layout/                  # MainLayout, AppSidebar, AppSecondarySidebar, navbar
│       │   ├── Pages/                   # Halaman per modul (lihat tabel di bawah)
│       │   └── Shared/                  # Base, DataDisplay, Feedback
│       ├── Navigation/                  # AppNavigation, AppMenuItem, AppMenuIcon
│       ├── Options/                     # ThemeOptions
│       ├── Services/                    # JsonReferenceDataService, OrchestratorClientService, dll.
│       ├── wwwroot/
│       │   └── data/reference/          # Lookup JSON (gender, province, city, branch, dll.)
│       ├── AppRoutes.cs                 # Definisi terpusat semua route
│       ├── Program.cs
│       └── appsettings.json
│
└── tests/
    └── AppraisalSystem.SmokeTests/      # Smoke test (service, route, reference data, render)
```

### Modul Halaman (`Components/Pages/`)

| Path | Modul | Route utama |
|---|---|---|
| `Dashboard.razor` | Dashboard ringkasan | `/dashboard` |
| `PencarianData/` | List, detail, form, memo, history | `/pencarian-data` |
| `PengkinianData/` | Reopen Approved → Draft untuk diedit ulang | `/pengkinian-data` |
| `Inquiry/` | Inquiry / pencarian baca | `/inquiry` |
| `Report/` | Laporan & export | `/report` |
| `ThemePreview.razor` | Preview tema (Admin) | `/theme-preview` |
| `Login.razor` | Login | `/login` |
| `AccessDenied.razor` / `NotFound.razor` / `Error.razor` | Sistem | `/access-denied`, `/not-found`, `/Error` |

Menu primer didefinisikan di `Navigation/AppNavigation.cs` (bukan hardcode di sidebar). Role menu memakai `AppRoles` di Application.

## Arsitektur

Project mengikuti **Clean Architecture** dengan arah dependensi dari luar ke dalam:

```
Web  ──►  Application  ──►  Domain
 │             ▲
 └──►  Infrastructure ─┘
```

- **Domain** — entity dan enum murni, tanpa dependensi eksternal.
- **Application** — business logic, DTO, interface, dan konstanta role (`AppRoles`).
- **Infrastructure** — EF Core `DbContext`, migrations, repository, seeding database.
- **Web** — UI Blazor, autentikasi, routing, reference JSON, dan komposisi DI.

Aturan penting:

1. **Domain tidak boleh mereferensi project lain.**
2. **Application hanya mereferensi Domain.** Akses data lewat interface (`IAppraisalRepository`).
3. **Web tidak mengakses `DbContext` langsung** — selalu melalui service di Application.
4. **Semua route didefinisikan di `AppRoutes.cs`** — jangan hardcode string route di komponen.
5. **Menu primer** lewat `AppNavigation`; **role name** lewat `AppRoles`.
6. **Lookup/master ringan** (gender, wilayah, cabang, dll.) lewat `IReferenceDataService` + JSON di `wwwroot/data/reference/`.

## Cara Menjalankan

### Prasyarat

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Langkah

```bash
# Clone repository
git clone <url-repository>
cd RDS.Appraisal

# Restore & build
dotnet build

# Jalankan aplikasi
dotnet run --project src/AppraisalSystem.Web
```

Aplikasi berjalan di URL yang tercantum pada `Properties/launchSettings.json`.

Database SQLite lokal (`appraisal-system.db`) **dibuat dan di-seed otomatis** saat startup (`DbInitializer`). File `.db` **tidak di-commit** ke Git — setiap mesin mendapat DB baru dari seed.

> Jika build gagal dengan error `MSB3027` / file locked oleh `AppraisalSystem.Web`, stop proses app yang sedang jalan (Stop Debugging / tutup `dotnet run`) lalu build ulang.

### Akun Default (Development)

| Username | Password | Role |
|---|---|---|
| `admin` | `admin123` | Admin |
| `appraiser` | `appraiser123` | Appraiser |
| `supervisor` | `supervisor123` | Supervisor |
| `checker` | `checker123` | Checker |

> Akun didefinisikan di `appsettings.json` bagian `Authentication:Users`. Ganti sebelum deploy ke production.

## Konfigurasi

Semua konfigurasi ada di `src/AppraisalSystem.Web/appsettings.json`:

### Database

```json
"Database": {
  "Provider": "Sqlite",
  "ConnectionString": "Data Source=appraisal-system.db"
}
```

Provider yang didukung: `Sqlite` | `SqlServer` | `PostgreSQL`.

### Tema / Branding

```json
"Theme": {
  "BrandName": "RDS Appraisal",
  "PrimaryColor": "#113a69",
  "LogoPath": "/images/brand/regalis-logo.jpg"
}
```

Warna divalidasi saat startup (format hex). Gunakan `/theme-preview` untuk melihat hasilnya.

## Data Referensi

Lookup UI (dropdown, filter, cascading wilayah) diambil dari JSON:

`src/AppraisalSystem.Web/wwwroot/data/reference/`

| File | Isi |
|---|---|
| `gender.json` | Jenis kelamin |
| `marital-status.json` | Status perkawinan |
| `legal-entity.json` | Bentuk badan hukum |
| `country.json` | Negara |
| `province.json` / `city.json` / `district.json` / `subdistrict.json` | Wilayah (cascading via `ParentCode`) |
| `branch.json` | Cabang |
| `collateral-subtype.json` | Subtipe jaminan |

Kontrak: `IReferenceDataService` (Application). Implementasi: `JsonReferenceDataService` (Web, cached).

## Panduan Development

### Menambah Halaman Baru

1. Tambahkan konstanta route di `AppRoutes.cs`.
2. Buat file `.razor` di `Components/Pages/<NamaModul>/` dengan `@page` yang mengacu ke konstanta tersebut.
3. Tambahkan item menu di `Navigation/AppNavigation.cs` bila perlu (sertakan role lewat `AppRoles` jika terbatas).
4. Tambahkan smoke test route di `tests/AppraisalSystem.SmokeTests/`.

### Menambah Fitur / Use Case Baru

1. **Domain** — entity / enum.
2. **Application** — DTO, interface, service.
3. **Infrastructure** — repository / `DbContext` / migration bila skema berubah.
4. **Web** — konsumsi service lewat `@inject`.

### Lookup Baru

1. Tambah file JSON di `wwwroot/data/reference/`.
2. Extend `IReferenceDataService` + `JsonReferenceDataService`.
3. Pakai di Form/filter; tambah smoke test reference bila relevan.

### Menggunakan Komponen Shared

- **Form**: `BaseTextField`, `BaseSelectField`, `BaseNumberField`, `BaseCheckboxField`, dll.
- **Tampilan data**: `BaseTable`, `BasePagination`, `BaseBadge`, `ApplicantTypeBadge`.
- **Feedback**: `BaseAlert`, `ConfirmationDialog`, `LoadingSpinner`.

### Konvensi

- Bahasa route/modul mengikuti istilah bisnis (`pencarian-data`, `pengkinian-data`).
- Nullable reference types dan implicit usings aktif.
- Styling: utility Tailwind; warna brand dari CSS variables (`--brand-*`) — jangan hardcode hex di komponen.
- Registrasi service lewat `AddApplication()` dan `AddInfrastructure()`.

## Fitur Chat AI

Chat AI membantu pencarian/analisis properti (OCR sertifikat, crawling, chat streaming).

Komponen kunci:

- `Components/Features/ChatUI/` — UI chat
- `OrchestratorClientService` — komunikasi ke backend AI
- `SavedSessionsService` — sesi tersimpan di `localStorage`

## Testing

```bash
dotnet test
```

| File | Cakupan |
|---|---|
| `AppraisalServiceSmokeTests.cs` | Business logic `AppraisalService` |
| `AppRoutesSmokeTests.cs` | Helper route di `AppRoutes` |
| `AppNavigationSmokeTests.cs` | Manifest menu `AppNavigation` |
| `RouteRegistrationSmokeTests.cs` | Route terdaftar di komponen |
| `ComponentRenderSmokeTests.cs` | Render komponen (bUnit) |
| `JsonReferenceDataServiceSmokeTests.cs` | Load JSON reference |
| `ReferenceDataFoundationSmokeTests.cs` | Fondasi reference data |

## Git / Push ke GitHub

Yang **tidak** ikut di-commit (lihat `.gitignore`):

- Build output (`bin/`, `obj/`)
- IDE (`.vs/`, `.vscode/`, `.idea/`)
- Database lokal SQLite (`*.db`, `*.db-shm`, `*.db-wal`)
- Secret/local override (`appsettings.*.local.json`, dll.)

Setelah clone, jalankan app sekali agar DB lokal + seed terbentuk otomatis.

```bash
# Contoh alur aman
dotnet build
dotnet test
git status          # pastikan tidak ada *.db / bin / obj
git add ...
git commit -m "..."
git push
```

Jika file `.db` pernah ter-track sebelumnya:

```bash
git rm --cached src/AppraisalSystem.Web/appraisal-system.db
git rm --cached src/AppraisalSystem.Web/appraisal-system.db-shm
git rm --cached src/AppraisalSystem.Web/appraisal-system.db-wal
```
