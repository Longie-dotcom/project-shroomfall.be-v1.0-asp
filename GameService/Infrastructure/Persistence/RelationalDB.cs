using Domain.Definition.AttributeDomain;
using Domain.Definition.EntityDomain;
using Domain.Definition.ItemDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.WorldDomain;
using Domain.Other.IdentityDomain;
using Domain.Other.VersionDomain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class RelationalDB : DbContext
    {
        #region Attributes
        #endregion

        #region Properties
        public DbSet<DefinitionVersionLog> DefinitionVersionLogs { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<Characteristic> Characteristics { get; set; }
        public DbSet<Effect> Effects { get; set; }

        public DbSet<Entity> Entities { get; set; }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemEffect> ItemEffects { get; set; }

        public DbSet<Locale> Locales { get; set; }
        public DbSet<LocalizationEntry> LocalizationEntries { get; set; }

        public DbSet<Cell> Cells { get; set; }
        public DbSet<EntitySpawnRule> EntitySpawnRules { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<SpawnArea> SpawnAreas { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        #endregion

        public RelationalDB(
            DbContextOptions<RelationalDB> options) : base(
                options)
        {

        }

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Version Domain
            modelBuilder.Entity<DefinitionVersionLog>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("DefinitionVersions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────

                entity.Property(x => x.Key)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Version)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(500);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────

                entity.HasIndex(x => new { x.Key, x.Version });

                entity.HasIndex(x => x.CreatedAt);

                entity.HasIndex(x => new { x.Key, x.Version })
                    .IsUnique();
            });
            #endregion

            #region Identity Domain
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

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(e => e.Role)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired();

                entity.Property(e => e.PreferredLocale)
                    .IsRequired();

                entity.Property(e => e.Dob);

                entity.Property(e => e.Gender)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255);

                entity.Property(e => e.SteamID);

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(500);

                entity.Property(e => e.RefreshTokenExpiry);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.LastLogin)
                    .IsRequired();

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

            #region Attribute Domain
            modelBuilder.Entity<AttributeValue>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("AttributeValues");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => new { x.CharacteristicID, x.Type, x.Level });

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(x => x.Value)
                    .IsRequired();

                entity.Property(x => x.Min)
                    .IsRequired();

                entity.Property(x => x.Max)
                    .IsRequired();

                entity.HasOne(x => x.Characteristic)
                    .WithMany()
                    .HasForeignKey(x => x.CharacteristicID);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => new { x.CharacteristicID, x.Type, x.Level })
                    .IsUnique();
            });

            modelBuilder.Entity<Characteristic>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Characteristics");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.OwnsOne(x => x.LocalizedText);

                entity.HasMany(x => x.AttributeValues)
                    .WithOne(x => x.Characteristic)
                    .HasForeignKey(x => x.CharacteristicID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Effect>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Effects");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.OwnsOne(x => x.LocalizedText);

                entity.Property(x => x.AttributeType)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(x => x.Value)
                    .IsRequired();

                entity.Property(x => x.Duration);

                entity.Property(x => x.Interval);
            });
            #endregion

            #region Entity Domain
            modelBuilder.Entity<Entity>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Entities");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.OwnsOne(x => x.LocalizedText);

                entity.OwnsOne(x => x.Appearance, a =>
                {
                    a.Property(p => p.SkinID)
                        .IsRequired();

                    a.OwnsOne(p => p.SkinColor);
                });

                entity.OwnsOne(x => x.Collision, c =>
                {
                    c.Property(p => p.ShapeType)
                        .HasConversion<string>()
                        .IsRequired();

                    c.Property(p => p.IsBlocking)
                        .IsRequired();

                    c.Property(p => p.IsTrigger)
                        .IsRequired();
                });

                // ─────────────────────────────
                // Discriminator
                // ─────────────────────────────
                entity.HasDiscriminator<string>("Discriminator")
                    .HasValue<Creature>("Creature")
                    .HasValue<Player>("Player")
                    .HasValue<Projectile>("Projectile")
                    .HasValue<WorldObject>("WorldObject")
                    .HasValue<AreaEffect>("AreaEffect");
            });

            modelBuilder.Entity<Creature>(entity =>
            {
                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.CharacteristicID)
                    .IsRequired();

                entity.Property(x => x.InventoryID)
                    .IsRequired();

                entity.Property(x => x.Level)
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.CharacteristicID);
                entity.HasIndex(x => x.InventoryID);
            });

            modelBuilder.Entity<Projectile>(entity =>
            {

            });

            modelBuilder.Entity<WorldObject>(entity =>
            {
                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.InteractionType)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(x => x.IsInteractable)
                    .IsRequired();

                entity.Property(x => x.IsPickupable)
                    .IsRequired();

                entity.Property(x => x.InventoryID);

                entity.Property(x => x.RoomID);
            });

            modelBuilder.Entity<AreaEffect>(entity =>
            {

            });

            modelBuilder.Entity<Player>(entity =>
            {
                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.OwnsOne(x => x.PlayerAppearance, a =>
                {
                    a.Property(p => p.SkinID)
                        .IsRequired();

                    a.OwnsOne(p => p.SkinColor);

                    a.Property(p => p.HairID)
                        .IsRequired();

                    a.Property(p => p.GlassesID)
                        .IsRequired();

                    a.Property(p => p.ShirtID)
                        .IsRequired();

                    a.Property(p => p.PantID)
                        .IsRequired();

                    a.Property(p => p.ShoeID)
                        .IsRequired();

                    a.Property(p => p.EyesID)
                        .IsRequired();

                    a.OwnsOne(p => p.HairColor);

                    a.OwnsOne(p => p.PantColor);

                    a.OwnsOne(p => p.EyeColor);
                });
            });
            #endregion

            #region Item Domain
            modelBuilder.Entity<Inventory>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Inventories");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.OwnsOne(x => x.LocalizedText);

                entity.Property(x => x.SlotCount)
                    .IsRequired();

                entity.HasMany(x => x.DefaultItems)
                    .WithOne(x => x.Inventory)
                    .HasForeignKey(x => x.InventoryID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InventoryItem>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("InventoryItems");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => new { x.InventoryID, x.ItemID });

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Amount)
                    .IsRequired();

                entity.Property(x => x.Quality)
                    .HasConversion<string>()
                    .IsRequired();

                entity.HasOne(x => x.Inventory)
                    .WithMany(x => x.DefaultItems)
                    .HasForeignKey(x => x.InventoryID);

                entity.HasOne(x => x.Item)
                    .WithMany()
                    .HasForeignKey(x => x.ItemID);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Items");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.OwnsOne(x => x.LocalizedText);

                entity.Property(x => x.Category)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(x => x.Durability);

                entity.Property(x => x.Stackable)
                    .IsRequired();

                entity.Property(x => x.CharacteristicID);

                entity.Property(x => x.ProjectileID);

                entity.Property(x => x.AreaEffectID);

                entity.Property(x => x.WorldObjectID);

                entity.HasMany(x => x.Effects)
                    .WithOne(x => x.Item)
                    .HasForeignKey(x => x.ItemID);

                // ─────────────────────────────
                // Index
                // ─────────────────────────────
                entity.HasIndex(x => x.Type);

                entity.HasIndex(x => x.Category);

                entity.HasIndex(x => x.CharacteristicID);

                entity.HasIndex(x => x.ProjectileID);

                entity.HasIndex(x => x.AreaEffectID);

                entity.HasIndex(x => x.WorldObjectID);
            });

            modelBuilder.Entity<ItemEffect>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("ItemEffects");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => new { x.ItemID, x.EffectID });

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.HasOne(x => x.Item)
                    .WithMany()
                    .HasForeignKey(x => x.ItemID);

                entity.HasOne(x => x.Effect)
                    .WithMany()
                    .HasForeignKey(x => x.EffectID);
            });
            #endregion

            #region Localization Domain
            modelBuilder.Entity<Locale>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Locales");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.Code);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Name)
                    .IsRequired();

                entity.Property(x => x.IsDefault)
                    .IsRequired();

                entity.Property(x => x.IsEnabled)
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.Code);

                entity.HasIndex(x => x.IsEnabled);
            });

            modelBuilder.Entity<LocalizationEntry>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("LocalizationEntries");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Key)
                    .IsRequired();

                entity.Property(x => x.LocaleCode)
                    .IsRequired();

                entity.Property(x => x.Value)
                    .IsRequired();

                entity.Property(x => x.Description);

                entity.Property(x => x.Version)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.UpdatedAt)
                    .IsRequired();

                entity.Property(x => x.IsDeleted)
                    .IsRequired();

                entity.HasOne(x => x.Locale)
                    .WithMany(x => x.LocalizationEntries)
                    .HasForeignKey(x => x.LocaleCode);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────

                entity.HasIndex(x => new { x.Key, x.LocaleCode })
                    .IsUnique();

                entity.HasIndex(x => x.LocaleCode);

                entity.HasIndex(x => x.Key);
            });
            #endregion

            #region World Domain
            modelBuilder.Entity<Cell>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Cells");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => new { x.RoomID, x.X, x.Y, x.Z });

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.TileID)
                    .IsRequired();

                entity.HasOne(x => x.Room)
                    .WithMany(r => r.Cells)
                    .HasForeignKey(x => x.RoomID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Tile)
                    .WithMany()
                    .HasForeignKey(x => x.TileID)
                    .OnDelete(DeleteBehavior.Restrict);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.TileID);

                entity.HasIndex(x => new { x.RoomID, x.X, x.Y, x.Z })
                    .IsUnique();
            });

            modelBuilder.Entity<EntitySpawnRule>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("EntitySpawnRules");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(x => x.RoomID)
                    .IsRequired();

                entity.Property(x => x.EntityID)
                    .IsRequired();

                entity.HasOne(x => x.Room)
                    .WithMany()
                    .HasForeignKey(x => x.RoomID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Entity)
                    .WithMany()
                    .HasForeignKey(x => x.EntityID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.SpawnAreas)
                    .WithOne(x => x.EntitySpawnRule)
                    .HasForeignKey(x => x.EntitySpawnRuleID)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.RoomID);

                entity.HasIndex(x => x.EntityID);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Rooms");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.OwnsOne(x => x.LocalizedText);

                entity.HasMany(x => x.Cells)
                    .WithOne(x => x.Room)
                    .HasForeignKey(x => x.RoomID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.EntitySpawnRules)
                    .WithOne(x => x.Room)
                    .HasForeignKey(x => x.RoomID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SpawnArea>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("SpawnAreas");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.MinX)
                    .IsRequired();

                entity.Property(x => x.MinY)
                    .IsRequired();

                entity.Property(x => x.MaxX)
                    .IsRequired();

                entity.Property(x => x.MaxY)
                    .IsRequired();

                entity.Property(x => x.MinCount)
                    .IsRequired();

                entity.Property(x => x.MaxCount)
                    .IsRequired();

                entity.Property(x => x.Weight)
                    .IsRequired();

                entity.Property(x => x.EntitySpawnRuleID)
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.EntitySpawnRuleID);

                entity.HasIndex(x => new { x.MinX, x.MinY });

                entity.HasIndex(x => new { x.MaxX, x.MaxY });
            });

            modelBuilder.Entity<Tile>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Tiles");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                entity.OwnsOne(x => x.LocalizedText);

                // ─────────────────────────────
                // Index
                // ─────────────────────────────
                entity.HasIndex(x => x.Type);
            });
            #endregion
        }
        #endregion
    }
}