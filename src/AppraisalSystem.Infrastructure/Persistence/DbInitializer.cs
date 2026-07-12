using AppraisalSystem.Domain.Entities;
using AppraisalSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AppraisalSystem.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppraisalDbContext dbContext)
    {
        var logger = dbContext.GetService<ILoggerFactory>().CreateLogger("DbInitializer");

        logger.LogInformation("Starting database initialization for provider {ProviderName}.", dbContext.Database.ProviderName);
        await dbContext.Database.EnsureCreatedAsync();
        await EnsureCompatibilityForLegacySqliteAsync(dbContext, logger);

        if (await dbContext.Appraisals.AnyAsync())
        {
            await BackfillLegacyAppraisalsAsync(dbContext, logger);
            await EnsurePublicIdUniqueIndexAsync(dbContext, logger);
            await EnsureDemoAppraisalsAsync(dbContext, logger);
            return;
        }

        var now = DateTime.UtcNow;
        dbContext.Appraisals.AddRange(CreateDemoAppraisals(now));
        await dbContext.SaveChangesAsync();
        await EnsurePublicIdUniqueIndexAsync(dbContext, logger);
        logger.LogInformation("Seed data inserted successfully ({Count} appraisals).", await dbContext.Appraisals.CountAsync());
    }

    private static async Task EnsureDemoAppraisalsAsync(AppraisalDbContext dbContext, ILogger logger)
    {
        var demo = CreateDemoAppraisals(DateTime.UtcNow);
        var existingNumbers = await dbContext.Appraisals
            .Select(x => x.ApplicationNumber)
            .ToListAsync();

        var missing = demo
            .Where(x => !existingNumbers.Contains(x.ApplicationNumber, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (missing.Count == 0)
        {
            logger.LogInformation("Database already contains appraisal data, demo seed complete.");
            return;
        }

        dbContext.Appraisals.AddRange(missing);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Inserted {Count} additional demo appraisals for grid preview.", missing.Count);
    }

    // Nilai reference (Gender, MaritalStatus, CollateralSubtype, BranchCode, dsb)
    // mengacu ke AppraisalSystem.Web/wwwroot/data/reference/*.json.
    private static IReadOnlyList<Appraisal> CreateDemoAppraisals(DateTime now) =>
    [
        new Appraisal
        {
            ApplicationNumber = "CON.20260706.001",
            Segment = CustomerSegment.Consumer,
            MakerId = "MKT001",
            BranchCode = "001",
            ApplicationDateUtc = now.AddDays(-6),
            ApplicantType = ApplicantType.Business,
            DeedNumber = "AKTA-001",
            BusinessEstablishedDate = new DateTime(2000, 6, 19),
            LegalEntity = "PT",
            AddressLine1 = "Jl. Gatot Subroto No. 88",
            District = "Setiabudi",
            Subdistrict = "Kuningan Timur",
            City = "Jakarta Selatan",
            Province = "DKI Jakarta",
            Country = "Indonesia",
            PhoneNumber = "021-7654321",
            MobileNumber1 = "081234567890",
            Email = "kontak@maju-properti.co.id",
            DebtorName = "PT Maju Properti",
            IdentityNumber = "ID-0001",
            CollateralType = CollateralType.Property,
            CollateralSubtype = "Ruko",
            Location = "Jakarta Selatan",
            Specification = "Ruko 3 lantai, SHM",
            MarketValue = 4_500_000_000,
            LiquidationValue = 3_600_000_000,
            Notes = "Aset dalam kondisi sangat baik.",
            Status = AppraisalStatus.Submitted,
            CreatedBy = "appraiser1",
            CreatedAtUtc = now.AddDays(-5)
        },
        new Appraisal
        {
            ApplicationNumber = "COM.20260706.001",
            Segment = CustomerSegment.Commercial,
            MakerId = "MKT002",
            BranchCode = "005",
            ApplicationDateUtc = now.AddDays(-4),
            ApplicantType = ApplicantType.Business,
            DeedNumber = "AKTA-123",
            BusinessEstablishedDate = new DateTime(2000, 1, 1),
            LegalEntity = "CV",
            AddressLine1 = "Jl. Asia Afrika No. 12",
            District = "Coblong",
            Subdistrict = "Dago",
            City = "Bandung",
            Province = "Jawa Barat",
            Country = "Indonesia",
            PhoneNumber = "022-432100",
            MobileNumber1 = "081355566677",
            Email = "office@citramotor.co.id",
            DebtorName = "CV Citra Motor",
            IdentityNumber = "ID-0002",
            CollateralType = CollateralType.Vehicle,
            CollateralSubtype = "Lain-lain",
            Location = "Bandung",
            Specification = "Truk box 2021",
            MarketValue = 350_000_000,
            LiquidationValue = 275_000_000,
            Notes = "Perlu inspeksi ulang body.",
            Status = AppraisalStatus.Approved,
            SupervisorNote = "Disetujui dengan catatan minor.",
            CreatedBy = "appraiser2",
            CreatedAtUtc = now.AddDays(-3)
        },
        new Appraisal
        {
            ApplicationNumber = "SME.20260706.001",
            Segment = CustomerSegment.SME,
            MakerId = "MKT003",
            BranchCode = "006",
            ApplicationDateUtc = now.AddDays(-2),
            ApplicantType = ApplicantType.Individual,
            DateOfBirth = new DateTime(2000, 3, 11),
            BirthPlace = "Surabaya",
            Gender = "Laki-laki",
            MaritalStatus = "Belum Menikah",
            AddressLine1 = "Jl. Raya Darmo No. 7",
            District = "Wonokromo",
            Subdistrict = "Darmo",
            City = "Surabaya",
            Province = "Jawa Timur",
            Country = "Indonesia",
            MobileNumber1 = "081288899900",
            Email = "udin@example.com",
            DebtorName = "UD Sentosa Niaga",
            IdentityNumber = "ID-0003",
            CollateralType = CollateralType.Inventory,
            CollateralSubtype = "Gudang",
            Location = "Surabaya",
            Specification = "Stok elektronik campuran",
            MarketValue = 900_000_000,
            LiquidationValue = 620_000_000,
            Notes = "Dokumen inventory sudah valid.",
            Status = AppraisalStatus.Draft,
            CreatedBy = "appraiser1",
            CreatedAtUtc = now.AddDays(-1)
        },
        // 5 tambahan untuk preview grid
        new Appraisal
        {
            ApplicationNumber = "CON.20260713.002",
            Segment = CustomerSegment.Consumer,
            MakerId = "MKT001",
            BranchCode = "002",
            ApplicationDateUtc = now.AddDays(-8),
            ApplicantType = ApplicantType.Individual,
            DateOfBirth = new DateTime(1988, 11, 2),
            BirthPlace = "Jakarta",
            Gender = "Perempuan",
            MaritalStatus = "Menikah",
            AddressLine1 = "Jl. Tebet Barat Dalam No. 15",
            District = "Tebet",
            Subdistrict = "Tebet Barat",
            City = "Jakarta Selatan",
            Province = "DKI Jakarta",
            Country = "Indonesia",
            MobileNumber1 = "081211122233",
            Email = "sari.wulandari@example.com",
            DebtorName = "Sari Wulandari",
            IdentityNumber = "3174090211880002",
            CollateralType = CollateralType.Property,
            CollateralSubtype = "Rumah",
            Location = "Jakarta Selatan",
            Specification = "Rumah 2 lantai, LT 120, LB 90",
            MarketValue = 2_750_000_000,
            LiquidationValue = 2_200_000_000,
            Notes = "Lokasi strategis dekat stasiun.",
            Status = AppraisalStatus.Draft,
            CreatedBy = "appraiser",
            CreatedAtUtc = now.AddDays(-8)
        },
        new Appraisal
        {
            ApplicationNumber = "COR.20260713.001",
            Segment = CustomerSegment.Corporate,
            MakerId = "MKT004",
            BranchCode = "001",
            ApplicationDateUtc = now.AddDays(-7),
            ApplicantType = ApplicantType.Business,
            DeedNumber = "AKTA-778",
            BusinessEstablishedDate = new DateTime(1995, 4, 20),
            LegalEntity = "PT",
            AddressLine1 = "Jl. HR Rasuna Said Blok X-5",
            District = "Setiabudi",
            Subdistrict = "Karet",
            City = "Jakarta Selatan",
            Province = "DKI Jakarta",
            Country = "Indonesia",
            PhoneNumber = "021-52990011",
            MobileNumber1 = "081356789012",
            Email = "finance@nusantara-logistik.co.id",
            DebtorName = "PT Nusantara Logistik",
            IdentityNumber = "01.234.567.8-901.000",
            CollateralType = CollateralType.Property,
            CollateralSubtype = "Gudang",
            Location = "Jakarta Selatan",
            Specification = "Gudang industri 2.500 m2",
            MarketValue = 18_000_000_000,
            LiquidationValue = 14_400_000_000,
            Notes = "Akses kontainer 40ft tersedia.",
            Status = AppraisalStatus.Submitted,
            CreatedBy = "appraiser",
            CreatedAtUtc = now.AddDays(-7)
        },
        new Appraisal
        {
            ApplicationNumber = "COM.20260713.002",
            Segment = CustomerSegment.Commercial,
            MakerId = "MKT002",
            BranchCode = "003",
            ApplicationDateUtc = now.AddDays(-5),
            ApplicantType = ApplicantType.Business,
            DeedNumber = "AKTA-445",
            BusinessEstablishedDate = new DateTime(2012, 9, 8),
            LegalEntity = "PT",
            AddressLine1 = "Jl. Thamrin No. 10",
            District = "Menteng",
            Subdistrict = "Gondangdia",
            City = "Jakarta Pusat",
            Province = "DKI Jakarta",
            Country = "Indonesia",
            PhoneNumber = "021-3900112",
            MobileNumber1 = "081298765432",
            Email = "ops@mitra-retail.id",
            DebtorName = "PT Mitra Retailindo",
            IdentityNumber = "02.111.222.3-444.000",
            CollateralType = CollateralType.Property,
            CollateralSubtype = "Perkantoran",
            Location = "Jakarta Pusat",
            Specification = "Kantor strata title lantai 12",
            MarketValue = 7_200_000_000,
            LiquidationValue = 5_760_000_000,
            Notes = "Sertifikat strata sudah dicek.",
            Status = AppraisalStatus.Approved,
            SupervisorNote = "Approved tanpa catatan.",
            CreatedBy = "appraiser",
            CreatedAtUtc = now.AddDays(-5)
        },
        new Appraisal
        {
            ApplicationNumber = "SME.20260713.002",
            Segment = CustomerSegment.SME,
            MakerId = "MKT003",
            BranchCode = "010",
            ApplicationDateUtc = now.AddDays(-3),
            ApplicantType = ApplicantType.Individual,
            DateOfBirth = new DateTime(1979, 7, 21),
            BirthPlace = "Yogyakarta",
            Gender = "Laki-laki",
            MaritalStatus = "Menikah",
            AddressLine1 = "Jl. Affandi No. 45",
            District = "Depok",
            Subdistrict = "Caturtunggal",
            City = "Kabupaten Sleman",
            Province = "Daerah Istimewa Yogyakarta",
            Country = "Indonesia",
            MobileNumber1 = "081322334455",
            Email = "budi.santoso@example.com",
            DebtorName = "Budi Santoso",
            IdentityNumber = "3404012107790001",
            CollateralType = CollateralType.Property,
            CollateralSubtype = "Tanah",
            Location = "Sleman",
            Specification = "Tanah kavling 400 m2",
            MarketValue = 1_100_000_000,
            LiquidationValue = 880_000_000,
            Notes = "Akses jalan lingkungan 6 meter.",
            Status = AppraisalStatus.Rejected,
            SupervisorNote = "Dokumen AJB belum lengkap.",
            CreatedBy = "appraiser",
            CreatedAtUtc = now.AddDays(-3)
        },
        new Appraisal
        {
            ApplicationNumber = "CON.20260713.003",
            Segment = CustomerSegment.Consumer,
            MakerId = "MKT001",
            BranchCode = "004",
            ApplicationDateUtc = now.AddDays(-1),
            ApplicantType = ApplicantType.Individual,
            DateOfBirth = new DateTime(1992, 1, 15),
            BirthPlace = "Tangerang",
            Gender = "Perempuan",
            MaritalStatus = "Belum Menikah",
            AddressLine1 = "Jl. Boulevard BSD No. 8",
            District = "Serpong",
            Subdistrict = "Lengkong Gudang",
            City = "Tangerang Selatan",
            Province = "Banten",
            Country = "Indonesia",
            MobileNumber1 = "081277889900",
            Email = "nina.rahma@example.com",
            DebtorName = "Nina Rahmawati",
            IdentityNumber = "3674011501920003",
            CollateralType = CollateralType.Property,
            CollateralSubtype = "Apartment",
            Location = "Tangerang Selatan",
            Specification = "Apartemen 2BR tower A",
            MarketValue = 980_000_000,
            LiquidationValue = 784_000_000,
            Notes = "Fasilitas full furnished.",
            Status = AppraisalStatus.Submitted,
            CreatedBy = "appraiser",
            CreatedAtUtc = now.AddDays(-1)
        }
    ];

    private static async Task EnsureCompatibilityForLegacySqliteAsync(AppraisalDbContext dbContext, ILogger logger)
    {
        var providerName = dbContext.Database.ProviderName ?? string.Empty;
        if (!providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Skipping SQLite compatibility check for provider {ProviderName}.", providerName);
            return;
        }

        logger.LogInformation("Running SQLite compatibility schema check.");

        var connection = dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using (var pragma = connection.CreateCommand())
        {
            pragma.CommandText = "PRAGMA table_info('Appraisals');";
            await using var reader = await pragma.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                existingColumns.Add(reader.GetString(1));
            }
        }

        if (existingColumns.Count == 0)
        {
            logger.LogWarning("Appraisals table not found during compatibility check.");
            return;
        }

        var requiredColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["PublicId"] = "TEXT NOT NULL DEFAULT ''",
            ["ApplicationNumber"] = "TEXT NOT NULL DEFAULT ''"
            ,["Segment"] = "INTEGER NOT NULL DEFAULT 0"
            ,["MakerId"] = "TEXT NOT NULL DEFAULT ''"
            ,["BranchCode"] = "TEXT NOT NULL DEFAULT ''"
            ,["ApplicationDateUtc"] = "TEXT NULL"
            ,["ApplicantType"] = "INTEGER NOT NULL DEFAULT 0"
            ,["DeedNumber"] = "TEXT NOT NULL DEFAULT ''"
            ,["DateOfBirth"] = "TEXT NULL"
            ,["BusinessEstablishedDate"] = "TEXT NULL"
            ,["LegalEntity"] = "TEXT NOT NULL DEFAULT ''"
            ,["BirthPlace"] = "TEXT NOT NULL DEFAULT ''"
            ,["Gender"] = "TEXT NOT NULL DEFAULT ''"
            ,["MaritalStatus"] = "TEXT NOT NULL DEFAULT ''"
            ,["MotherMaidenName"] = "TEXT NOT NULL DEFAULT ''"
            ,["AddressLine1"] = "TEXT NOT NULL DEFAULT ''"
            ,["AddressLine2"] = "TEXT NOT NULL DEFAULT ''"
            ,["AddressLine3"] = "TEXT NOT NULL DEFAULT ''"
            ,["Rt"] = "TEXT NOT NULL DEFAULT ''"
            ,["Rw"] = "TEXT NOT NULL DEFAULT ''"
            ,["PostalCode"] = "TEXT NOT NULL DEFAULT ''"
            ,["Subdistrict"] = "TEXT NOT NULL DEFAULT ''"
            ,["District"] = "TEXT NOT NULL DEFAULT ''"
            ,["City"] = "TEXT NOT NULL DEFAULT ''"
            ,["Province"] = "TEXT NOT NULL DEFAULT ''"
            ,["Country"] = "TEXT NOT NULL DEFAULT ''"
            ,["PhoneNumber"] = "TEXT NOT NULL DEFAULT ''"
            ,["MobileNumber1"] = "TEXT NOT NULL DEFAULT ''"
            ,["MobileNumber2"] = "TEXT NOT NULL DEFAULT ''"
            ,["Email"] = "TEXT NOT NULL DEFAULT ''"
            ,["CollateralSubtype"] = "TEXT NOT NULL DEFAULT ''"
            ,["InternalMemo"] = "TEXT NOT NULL DEFAULT ''"
            ,["WorkflowHistoryJson"] = "TEXT NOT NULL DEFAULT ''"
        };

        foreach (var column in requiredColumns)
        {
            if (existingColumns.Contains(column.Key))
            {
                continue;
            }

            var sql = $"ALTER TABLE Appraisals ADD COLUMN {column.Key} {column.Value};";
            logger.LogWarning("Applying compatibility schema patch: {Sql}", sql);
            await dbContext.Database.ExecuteSqlRawAsync(sql);
        }

        logger.LogInformation("SQLite compatibility schema check completed.");
    }

    private static async Task EnsurePublicIdUniqueIndexAsync(AppraisalDbContext dbContext, ILogger logger)
    {
        var providerName = dbContext.Database.ProviderName ?? string.Empty;
        if (!providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        await dbContext.Database.ExecuteSqlRawAsync("CREATE UNIQUE INDEX IF NOT EXISTS IX_Appraisals_PublicId ON Appraisals(PublicId);");
        logger.LogInformation("SQLite PublicId unique index ensured.");
    }

    private static async Task BackfillLegacyAppraisalsAsync(AppraisalDbContext dbContext, ILogger logger)
    {
        var appraisals = await dbContext.Appraisals.ToListAsync();
        var updated = 0;

        foreach (var appraisal in appraisals)
        {
            var changed = false;

            if (string.IsNullOrWhiteSpace(appraisal.ApplicationNumber))
            {
                var prefix = appraisal.Segment switch
                {
                    CustomerSegment.Consumer => "CON",
                    CustomerSegment.Commercial => "COM",
                    CustomerSegment.SME => "SME",
                    CustomerSegment.Corporate => "COR",
                    _ => "APP"
                };

                appraisal.ApplicationNumber = $"{prefix}.{appraisal.CreatedAtUtc:yyyyMMdd}.{appraisal.Id:000}";
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(appraisal.PublicId)
                || !Guid.TryParse(appraisal.PublicId, out _))
            {
                appraisal.PublicId = Guid.NewGuid().ToString("D");
                changed = true;
            }

            if (appraisal.Segment == CustomerSegment.Unknown)
            {
                appraisal.Segment = CustomerSegment.Consumer;
                changed = true;
            }

            if (appraisal.ApplicantType == ApplicantType.Unknown)
            {
                appraisal.ApplicantType = string.IsNullOrWhiteSpace(appraisal.DeedNumber)
                    ? ApplicantType.Individual
                    : ApplicantType.Business;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(appraisal.IdentityNumber))
            {
                appraisal.IdentityNumber = $"ID-{appraisal.Id:0000}";
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(appraisal.DebtorName))
            {
                appraisal.DebtorName = $"Debitur {appraisal.Id}";
                changed = true;
            }

            if (!appraisal.ApplicationDateUtc.HasValue)
            {
                appraisal.ApplicationDateUtc = appraisal.CreatedAtUtc;
                changed = true;
            }

            if (changed)
            {
                updated++;
            }
        }

        if (updated > 0)
        {
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Backfilled {Count} legacy appraisal records with intake defaults.", updated);
        }
    }
}