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

            #region Identity
            modelBuilder.Entity<User>(entity =>
            {
                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(e => e.ID);

                // ─────────────────────────────
                // Required fields
                // ─────────────────────────────
                entity.Property(e => e.ID)
                    .IsRequired();

                entity.Property(e => e.PlayerID)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                // ─────────────────────────────
                // Optional profile fields
                // ─────────────────────────────
                entity.Property(e => e.Dob)
                    .IsRequired(false);

                entity.Property(e => e.Gender)
                    .IsRequired(false);

                // ─────────────────────────────
                // Auth fields
                // ─────────────────────────────
                entity.Property(e => e.Email)
                    .IsRequired(false);

                entity.Property(e => e.PasswordHash)
                    .IsRequired(false);

                entity.Property(e => e.SteamID)
                    .IsRequired(false);

                // ─────────────────────────────
                // Tokens
                // ─────────────────────────────
                entity.Property(e => e.RefreshToken)
                    .IsRequired(false);

                entity.Property(e => e.RefreshTokenExpiry)
                    .IsRequired(false);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasFilter("[Email] IS NOT NULL");

                entity.HasIndex(e => e.SteamID)
                    .IsUnique()
                    .HasFilter("[SteamID] IS NOT NULL");
            });
            #endregion
        }
        #endregion
    }
}