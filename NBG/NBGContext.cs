using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NBG.Models;

#nullable disable

namespace NBG
{
    public partial class NBGContext : DbContext
    {
        private readonly string _connectionString;

        public NBGContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public NBGContext(DbContextOptions<NBGContext> options)
            : base(options)
        {
        }

        public virtual DbSet<NlCeOct19CurrencyExchangeRate437dbf0e84ff417a965dEd2bb9650972> NlCeOct19CurrencyExchangeRate437dbf0e84ff417a965dEd2bb9650972s { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(_connectionString);
                //optionsBuilder.UseSqlServer("Server=WIN-53QAF6QB8FU;Database=BC220-PTC;Trusted_Connection=True;User Id=nbguser;Password=NewPass1;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<NlCeOct19CurrencyExchangeRate437dbf0e84ff417a965dEd2bb9650972>(entity =>
            {
                entity.HasKey(e => new { e.CurrencyCode, e.StartingDate })
                    .HasName("NL-CE Oct 19$Currency Exchange Rate$437dbf0e-84ff-417a-965d-ed2bb9650972$Key1");

                entity.ToTable("NL-CE Oct 19$Currency Exchange Rate$437dbf0e-84ff-417a-965d-ed2bb9650972");

                entity.HasIndex(e => e.SystemId, "NL-CE Oct 19$Currency Exchange Rate$437dbf0e-84ff-417a-965d-ed2bb9650972$$systemId")
                    .IsUnique();

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(10)
                    .HasColumnName("Currency Code");

                entity.Property(e => e.StartingDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Starting Date");

                entity.Property(e => e.AdjustmentExchRateAmount)
                    .HasColumnType("decimal(38, 20)")
                    .HasColumnName("Adjustment Exch_ Rate Amount");

                entity.Property(e => e.ExchangeRateAmount)
                    .HasColumnType("decimal(38, 20)")
                    .HasColumnName("Exchange Rate Amount");

                entity.Property(e => e.FixExchangeRateAmount).HasColumnName("Fix Exchange Rate Amount");

                entity.Property(e => e.RelationalAdjmtExchRateAmt)
                    .HasColumnType("decimal(38, 20)")
                    .HasColumnName("Relational Adjmt Exch Rate Amt");

                entity.Property(e => e.RelationalCurrencyCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("Relational Currency Code")
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.RelationalExchRateAmount)
                    .HasColumnType("decimal(38, 20)")
                    .HasColumnName("Relational Exch_ Rate Amount");

                entity.Property(e => e.SystemCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("$systemCreatedAt")
                    .HasDefaultValueSql("('1753.01.01')");

                entity.Property(e => e.SystemCreatedBy).HasColumnName("$systemCreatedBy");

                entity.Property(e => e.SystemId)
                    .HasColumnName("$systemId")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.SystemModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("$systemModifiedAt")
                    .HasDefaultValueSql("('1753.01.01')");

                entity.Property(e => e.SystemModifiedBy).HasColumnName("$systemModifiedBy");

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("timestamp");
            });

            modelBuilder.Entity<Rate>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.ToTable("rates");

                entity.HasIndex(e => new { e.DateL, e.SourceCurrency, e.TargetCurrency }, "ratdat")
                    .IsUnique();

                entity.HasIndex(e => new { e.SourceCurrency, e.TargetCurrency, e.DateL }, "rates")
                    .IsUnique()
                    .IsClustered();

                entity.HasIndex(e => new { e.TargetCurrency, e.SourceCurrency, e.DateL }, "rattrg")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateL)
                    .HasColumnType("datetime")
                    .HasColumnName("date_l");

                entity.Property(e => e.RateBuy).HasColumnName("rate_buy");

                entity.Property(e => e.RateExchange).HasColumnName("rate_exchange");

                entity.Property(e => e.RateOfficial).HasColumnName("rate_official");

                entity.Property(e => e.RateSell).HasColumnName("rate_sell");

                entity.Property(e => e.SourceCurrency)
                    .HasMaxLength(3)
                    .HasColumnName("source_currency")
                    .IsFixedLength(true);

                entity.Property(e => e.Syscreated)
                    .HasColumnType("datetime")
                    .HasColumnName("syscreated")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Syscreator).HasColumnName("syscreator");

                entity.Property(e => e.Sysguid)
                    .HasColumnName("sysguid")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Sysmodified)
                    .HasColumnType("datetime")
                    .HasColumnName("sysmodified")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Sysmodifier).HasColumnName("sysmodifier");

                entity.Property(e => e.TargetCurrency)
                    .HasMaxLength(3)
                    .HasColumnName("target_currency")
                    .IsFixedLength(true);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("timestamp");
            });

            modelBuilder.HasSequence<int>("SEQ_actlogCode")
                .StartsAt(1000)
                .HasMin(0);

            modelBuilder.HasSequence<int>("SEQ_actrequestCode")
                .StartsAt(1000)
                .HasMin(0);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}