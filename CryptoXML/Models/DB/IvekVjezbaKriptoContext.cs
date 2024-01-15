using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CryptoXML.Models.DB;

public partial class IvekVjezbaKriptoContext : DbContext
{
    public IvekVjezbaKriptoContext()
    {
    }

    public IvekVjezbaKriptoContext(DbContextOptions<IvekVjezbaKriptoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CryptoData> CryptoData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=localhost;initial catalog=IvekVjezbaKripto;persist security info=True;Encrypt=False; user id=Matija;password=FuKru1234!; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CryptoData>(entity =>
        {
            entity.HasKey(e => new { e.Symbol, e.TajmStamp }).HasName("PK__CryptoDa__D9880BB814474AC9");

            entity.Property(e => e.Symbol)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TajmStamp).HasColumnType("datetime");
            entity.Property(e => e.ChangePercent24Hr).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Explorer)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MarketCapUsd).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaxSupply).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PriceUsd).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Supply).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VolumeUsd24Hr).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Vwap24Hr).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
