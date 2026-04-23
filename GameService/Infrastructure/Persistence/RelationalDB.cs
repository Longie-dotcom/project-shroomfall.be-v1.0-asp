using Domain.IdentityDomain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class RelationalDB : DbContext
    {
        #region Attributes
        #endregion

        #region Properties
        public DbSet<User> Users { get; set; }
        #endregion

        public RelationalDB(
            DbContextOptions<RelationalDB> options) : base(options) 
        { 
        
        }

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Users");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID)
                    .HasMaxLength(50)
                    .IsRequired();

                // ─────────────────────────────
                // Core fields
                // ─────────────────────────────
                entity.Property(e => e.PlayerID)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                // ─────────────────────────────
                // Profile
                // ─────────────────────────────
                entity.Property(e => e.Dob)
                    .IsRequired(false);

                entity.Property(e => e.Gender)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired(false);

                // ─────────────────────────────
                // Auth
                // ─────────────────────────────
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsRequired(false);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsRequired(false);

                entity.Property(e => e.SteamID)
                    .HasMaxLength(50)
                    .IsRequired(false);

                // ─────────────────────────────
                // Tokens
                // ─────────────────────────────
                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(e => e.RefreshTokenExpiry)
                    .IsRequired(false);

                // ─────────────────────────────
                // Timestamps
                // ─────────────────────────────
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.LastLogin)
                    .IsRequired();

                // ─────────────────────────────
                // Concurrency
                // ─────────────────────────────
                entity.Property(e => e.RowVersion)
                    .IsRowVersion();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(e => e.PlayerID)
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("[Email] IS NOT NULL");

                entity.HasIndex(e => e.SteamID)
                    .IsUnique()
                    .HasFilter("[SteamID] IS NOT NULL");
            });
        }
        #endregion
    }
}