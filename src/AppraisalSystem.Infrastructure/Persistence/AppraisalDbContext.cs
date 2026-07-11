using AppraisalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppraisalSystem.Infrastructure.Persistence;

public sealed class AppraisalDbContext(DbContextOptions<AppraisalDbContext> options) : DbContext(options)
{
    public DbSet<Appraisal> Appraisals => Set<Appraisal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appraisal>(entity =>
        {
            entity.ToTable("Appraisals");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ApplicationNumber).HasMaxLength(40);
            entity.Property(x => x.MakerId).HasMaxLength(50);
            entity.Property(x => x.BranchCode).HasMaxLength(20);
            entity.Property(x => x.DeedNumber).HasMaxLength(60);
            entity.Property(x => x.LegalEntity).HasMaxLength(80);
            entity.Property(x => x.BirthPlace).HasMaxLength(80);
            entity.Property(x => x.Gender).HasMaxLength(30);
            entity.Property(x => x.MaritalStatus).HasMaxLength(30);
            entity.Property(x => x.MotherMaidenName).HasMaxLength(120);
            entity.Property(x => x.AddressLine1).HasMaxLength(250);
            entity.Property(x => x.AddressLine2).HasMaxLength(250);
            entity.Property(x => x.AddressLine3).HasMaxLength(250);
            entity.Property(x => x.Rt).HasMaxLength(10);
            entity.Property(x => x.Rw).HasMaxLength(10);
            entity.Property(x => x.PostalCode).HasMaxLength(10);
            entity.Property(x => x.Subdistrict).HasMaxLength(100);
            entity.Property(x => x.District).HasMaxLength(100);
            entity.Property(x => x.City).HasMaxLength(100);
            entity.Property(x => x.Province).HasMaxLength(100);
            entity.Property(x => x.Country).HasMaxLength(100);
            entity.Property(x => x.PhoneNumber).HasMaxLength(30);
            entity.Property(x => x.MobileNumber1).HasMaxLength(30);
            entity.Property(x => x.MobileNumber2).HasMaxLength(30);
            entity.Property(x => x.Email).HasMaxLength(120);
            entity.Property(x => x.DebtorName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.IdentityNumber).HasMaxLength(60).IsRequired();
            entity.Property(x => x.CollateralSubtype).HasMaxLength(100);
            entity.Property(x => x.Location).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Specification).HasMaxLength(400).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(4000);
            entity.Property(x => x.InternalMemo).HasMaxLength(4000);
            entity.Property(x => x.WorkflowHistoryJson).HasMaxLength(16000);
            entity.Property(x => x.SupervisorNote).HasMaxLength(400);
            entity.Property(x => x.CreatedBy).HasMaxLength(80).IsRequired();

            entity.Property(x => x.MarketValue).HasPrecision(18, 2);
            entity.Property(x => x.LiquidationValue).HasPrecision(18, 2);

            entity.HasIndex(x => x.ApplicationNumber);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.CreatedAtUtc);
        });

        base.OnModelCreating(modelBuilder);
    }
}