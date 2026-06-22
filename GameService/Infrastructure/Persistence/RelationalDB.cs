using Domain.Definition;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Definition.IdentityDomain;
using Domain.Definition.LocalizationDomain;
using Domain.Definition.MetaDomain;
using Domain.Definition.WorldDomain;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System.Text.Json;

namespace Infrastructure.Persistence
{
    public class RelationalDB : DbContext
    {
        #region Attributes
        #endregion

        #region Properties
        public DbSet<AIDefinition> AIDefinitions { get; set; }
        public DbSet<AppearanceDefinition> AppearanceDefinitions { get; set; }
        public DbSet<CollisionDefinition> CollisionDefinitions { get; set; }
        public DbSet<CharacteristicDefinition> CharacteristicDefinitions { get; set; }
        public DbSet<InteractableDefinition> InteractableDefinitions { get; set; }
        public DbSet<InventoryDefinition> InventoryDefinitions { get; set; }
        public DbSet<LifetimeDefinition> LifetimeDefinitions { get; set; }
        public DbSet<PortalDefinition> PortalDefinitions { get; set; }
        public DbSet<ProjectileDefinition> ProjectileDefinitions { get; set; }
        public DbSet<TriggeredEffectDefinition> TriggeredEffectDefinitions { get; set; }
        public DbSet<EntityDefinition> EntityDefinitions { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Locale> Locales { get; set; }
        public DbSet<LocalizationEntry> LocalizationEntries { get; set; }

        public DbSet<EffectDefinition> EffectDefinitions { get; set; }
        public DbSet<ItemDefinition> ItemDefinitions { get; set; }

        public DbSet<RoomConnection> RoomConnections { get; set; }
        public DbSet<RoomDefinition> RoomDefinitions { get; set; }

        public DbSet<DefinitionVersionLog> DefinitionVersionLogs { get; set; }
        #endregion

        public RelationalDB(DbContextOptions<RelationalDB> options) : base(options) { }

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Entity Domain
            modelBuilder.Entity<AIDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("AIDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.LeashDistance)
                    .IsRequired();
                entity.Property(x => x.AggroRadius)
                    .IsRequired();
                entity.Property(x => x.ThinkInterval)
                    .IsRequired();
                entity.Property(x => x.IsAIControlled)
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.EntityDefinitionID)
                    .IsUnique();

                entity.HasIndex(x => x.IsAIControlled);
            });

            modelBuilder.Entity<AppearanceDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("AppearanceDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.SkinID)
                    .IsRequired();
                entity.OwnsOne(x => x.SkinColor, skin =>
                {
                    skin.Property(x => x.H)
                        .IsRequired();
                    skin.Property(x => x.S)
                        .IsRequired();
                    skin.Property(x => x.V)
                        .IsRequired();
                });
                entity.Property(x => x.HairID)
                    .IsRequired(false);
                entity.Property(x => x.EyesID)
                    .IsRequired(false);
                entity.Property(x => x.ShirtID)
                    .IsRequired(false);
                entity.Property(x => x.PantID)
                    .IsRequired(false);
                entity.OwnsOne(x => x.HairColor, hair =>
                {
                    hair.Property(x => x.H)
                        .HasDefaultValue(0.0f);
                    hair.Property(x => x.S)
                        .HasDefaultValue(0.0f);
                    hair.Property(x => x.V)
                        .HasDefaultValue(0.0f);
                });

                entity.OwnsOne(x => x.PantColor, pant =>
                {
                    pant.Property(x => x.H)
                        .HasDefaultValue(0.0f);
                    pant.Property(x => x.S)
                        .HasDefaultValue(0.0f);
                    pant.Property(x => x.V)
                        .HasDefaultValue(0.0f);
                });

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.SkinID);
                entity.HasIndex(x => x.HairID);
                entity.HasIndex(x => x.EyesID);
                entity.HasIndex(x => x.ShirtID);
                entity.HasIndex(x => x.PantID);
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<CollisionDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("CollisionDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.ShapeType)
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(x => x.Width)
                    .IsRequired();
                entity.Property(x => x.Height)
                    .IsRequired();
                entity.Property(x => x.Radius)
                    .IsRequired();
                entity.Property(x => x.IsBlocking)
                    .IsRequired();
                entity.Property(x => x.Layer)
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(x => x.Mask)
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(x => x.OffsetX)
                    .IsRequired();
                entity.Property(x => x.OffsetY)
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.ShapeType);
                entity.HasIndex(x => x.IsBlocking);
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<CharacteristicDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("CharacteristicDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasMany(x => x.AttributeValues)
                    .WithOne(x => x.CharacteristicDefinition)
                    .HasForeignKey(x => x.CharacteristicDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<AttributeValue>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("AttributeValues");

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
                entity.Property(x => x.BaseValue)
                    .IsRequired();
                entity.Property(x => x.Level)
                    .IsRequired();
                entity.Property(x => x.Min)
                    .IsRequired();
                entity.Property(x => x.Max)
                    .IsRequired();
                entity.Property(x => x.CharacteristicDefinitionID)
                    .IsRequired();

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasOne(x => x.CharacteristicDefinition)
                    .WithMany(x => x.AttributeValues)
                    .HasForeignKey(x => x.CharacteristicDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(x => x.AttributeGrowthValues)
                    .WithOne(x => x.AttributeValue)
                    .HasForeignKey(x => x.AttributeValueID)
                    .OnDelete(DeleteBehavior.Cascade);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.Type);
                entity.HasIndex(x => x.CharacteristicDefinitionID);
            });

            modelBuilder.Entity<AttributeGrowthValue>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("AttributeGrowthValues");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.Level)
                    .IsRequired();
                entity.Property(x => x.GrowthValue)
                    .IsRequired();
                entity.Property(x => x.AttributeValueID)
                    .IsRequired();

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasOne(x => x.AttributeValue)
                    .WithMany(x => x.AttributeGrowthValues)
                    .HasForeignKey(x => x.AttributeValueID)
                    .OnDelete(DeleteBehavior.Cascade);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.AttributeValueID);
                entity.HasIndex(x => new { x.AttributeValueID, x.Level })
                    .IsUnique();
            });

            modelBuilder.Entity<InteractableDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("InteractableDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.Type);
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<InventoryDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("InventoryDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.SlotCount)
                    .IsRequired();

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasMany(x => x.DefaultItems)
                    .WithOne(x => x.InventoryDefinition)
                    .HasForeignKey(x => x.InventoryDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<InventoryEntry>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("InventoryEntries");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.DefinitionID)
                    .IsRequired();
                entity.Property(x => x.Amount)
                    .IsRequired();
                entity.Property(x => x.Quality)
                    .HasConversion<string>()
                    .IsRequired();
                entity.Ignore(x => x.Durability);
                entity.Property(x => x.InventoryDefinitionID)
                    .IsRequired();

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasOne(x => x.InventoryDefinition)
                    .WithMany(x => x.DefaultItems)
                    .HasForeignKey(x => x.InventoryDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.InventoryDefinitionID);
                entity.HasIndex(x => x.DefinitionID);
            });

            modelBuilder.Entity<LifetimeDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("LifetimeDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.Lifetime)
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.Lifetime);
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<PortalDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("PortalDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.LocalTriggerOffsetX)
                    .IsRequired();
                entity.Property(x => x.LocalTriggerOffsetY)
                    .IsRequired();
                entity.Property(x => x.TriggerWidth)
                    .IsRequired();
                entity.Property(x => x.TriggerHeight)
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<ProjectileDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("ProjectileDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.Velocity)
                    .IsRequired();
                entity.Property(x => x.OnImpactSpawnEntityDefinitionID)
                    .IsRequired(false);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.Velocity);
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<TriggeredEffectDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("TriggeredEffectDefinitions");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired();
                entity.Property(x => x.EffectDefinitionIDs)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                    )
                    .IsRequired();

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.EntityDefinitionID).IsUnique();
            });

            modelBuilder.Entity<EntityDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("EntityDefinitions");

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
                entity.OwnsOne(x => x.Presentation, presentation =>
                {
                    presentation.OwnsOne(x => x.LocalizedText, localization =>
                    {
                        localization.Property(x => x.NameKey)
                            .IsRequired();
                        localization.Property(x => x.DescriptionKey)
                            .IsRequired();
                    });
                    presentation.Property(x => x.IconID)
                        .IsRequired(false);
                });
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
                entity.Property(e => e.Dob)
                    .IsRequired(false);
                entity.Property(e => e.Gender)
                    .HasConversion<string>()
                    .IsRequired(false);
                entity.Property(e => e.Email)
                    .IsRequired(false);
                entity.Property(e => e.PasswordHash)
                    .IsRequired(false);
                entity.Property(e => e.SteamID)
                    .IsRequired(false);
                entity.Property(e => e.RefreshToken)
                    .IsRequired(false);
                entity.Property(e => e.RefreshTokenExpiry)
                    .IsRequired(false);
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
                entity.Property(x => x.Value)
                    .IsRequired();
                entity.Property(x => x.Description)
                    .IsRequired(false);
                entity.Property(x => x.Version)
                    .IsRequired();
                entity.Property(x => x.CreatedAt)
                    .IsRequired();
                entity.Property(x => x.UpdatedAt)
                    .IsRequired();
                entity.Property(x => x.IsDeleted)
                    .IsRequired();
                entity.Property(x => x.LocaleCode)
                    .IsRequired();

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
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

            #region Meta Domain
            modelBuilder.Entity<EffectDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("EffectDefinitions");

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
                entity.Property(x => x.AttributeType)
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(x => x.SourceType)
                    .HasConversion<string>()
                    .IsRequired(false);
                entity.Property(x => x.Value)
                    .IsRequired();
                entity.Property(x => x.Duration)
                    .IsRequired(false);
                entity.Property(x => x.Interval)
                    .IsRequired(false);
                entity.OwnsOne(x => x.Presentation, presentation =>
                {
                    presentation.OwnsOne(x => x.LocalizedText, localization =>
                    {
                        localization.Property(x => x.NameKey)
                            .IsRequired();
                        localization.Property(x => x.DescriptionKey)
                            .IsRequired();
                    });
                    presentation.Property(x => x.IconID)
                        .IsRequired(false);
                });

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.AttributeType);
            });

            modelBuilder.Entity<ItemDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("ItemDefinitions");

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
                entity.Property(x => x.Category)
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(x => x.MaxStack)
                    .IsRequired(false);
                entity.Property(x => x.MaxDurability)
                    .IsRequired(false);
                entity.Property(x => x.TriggeredAction)
                    .HasConversion<string>()
                    .IsRequired(false);
                entity.OwnsOne(x => x.Presentation, presentation =>
                {
                    presentation.OwnsOne(x => x.LocalizedText, localization =>
                    {
                        localization.Property(x => x.NameKey)
                            .IsRequired();
                        localization.Property(x => x.DescriptionKey)
                            .IsRequired();
                    });
                    presentation.Property(x => x.IconID)
                        .IsRequired(false);
                });

                // ─────────────────────────────
                // Owned Types (Configurations)
                // ─────────────────────────────
                entity.OwnsOne(x => x.SpawnEntityConfig, spawn =>
                {
                    spawn.Property(x => x.EntityDefinitionID)
                        .IsRequired();
                    spawn.Property(x => x.TargetType)
                        .HasConversion<string>()
                        .IsRequired();
                    spawn.Property(x => x.MaxRange)
                        .IsRequired();
                });

                entity.Navigation(x => x.SpawnEntityConfig)
                    .IsRequired(false);


                entity.OwnsOne(x => x.ApplyEffectConfig, effect =>
                {
                    effect.ToJson();

                    effect.Property(x => x.EffectDefinitionIDs)
                        .IsRequired();
                });

                entity.Navigation(x => x.ApplyEffectConfig)
                    .IsRequired(false);


                entity.OwnsOne(x => x.EquipConfig, equip =>
                {
                    equip.Property(x => x.Slot)
                        .HasConversion<string>()
                        .IsRequired();
                });

                entity.Navigation(x => x.EquipConfig)
                    .IsRequired(false);


                entity.OwnsOne(x => x.CostConfig, cost =>
                {
                    cost.Property(x => x.Method)
                        .HasConversion<string>()
                        .IsRequired();

                    cost.Property(x => x.Value)
                        .IsRequired();
                });

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.Type);
                entity.HasIndex(x => x.Category);
            });
            #endregion

            #region World Domain
            modelBuilder.Entity<RoomDefinition>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("RoomDefinitions");

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
                entity.OwnsOne(x => x.Presentation, presentation =>
                {
                    presentation.OwnsOne(x => x.LocalizedText, localization =>
                    {
                        localization.Property(x => x.NameKey)
                            .IsRequired();
                        localization.Property(x => x.DescriptionKey)
                            .IsRequired();
                    });
                    presentation.Property(x => x.IconID)
                        .IsRequired(false);
                });

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasMany(x => x.Cells)
                    .WithOne(x => x.RoomDefinition)
                    .HasForeignKey(x => x.RoomDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(x => x.EntitySpawnRules)
                    .WithOne(x => x.RoomDefinition)
                    .HasForeignKey(x => x.RoomDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cell>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("Cells");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => new { x.RoomDefinitionID, x.X, x.Y, x.Z });

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.TileID)
                    .IsRequired();
                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .IsRequired();

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasOne(x => x.RoomDefinition)
                    .WithMany(r => r.Cells)
                    .HasForeignKey(x => x.RoomDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.TileID);
                entity.HasIndex(x => new { x.RoomDefinitionID, x.X, x.Y, x.Z })
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
                entity.Property(x => x.RoomDefinitionID)
                    .IsRequired();
                entity.Property(x => x.EntityDefinitionID)
                    .IsRequired(false);

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasOne(x => x.RoomDefinition)
                    .WithMany()
                    .HasForeignKey(x => x.RoomDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.EntityDefinition)
                    .WithMany()
                    .HasForeignKey(x => x.EntityDefinitionID)
                    .OnDelete(DeleteBehavior.Cascade);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => x.RoomDefinitionID);
                entity.HasIndex(x => x.EntityDefinitionID);
            });

            modelBuilder.Entity<RoomConnection>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("RoomConnections");

                // ─────────────────────────────
                // Primary Key
                // ─────────────────────────────
                entity.HasKey(x => x.ID);

                // ─────────────────────────────
                // Properties
                // ─────────────────────────────
                entity.Property(x => x.SourceRoomID)
                    .IsRequired();
                entity.Property(x => x.SourceEntityID)
                    .IsRequired();
                entity.Property(x => x.DestinationRoomID)
                    .IsRequired();
                entity.Property(x => x.DestinationEntityID)
                    .IsRequired();

                // ─────────────────────────────
                // Relationships
                // ─────────────────────────────
                entity.HasOne(x => x.SourceRoom)
                    .WithMany()
                    .HasForeignKey(x => x.SourceRoomID)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.DestinationRoom)
                    .WithMany()
                    .HasForeignKey(x => x.DestinationRoomID)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.SourceEntity)
                    .WithMany()
                    .HasForeignKey(x => x.SourceEntityID)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.DestinationEntity)
                    .WithMany()
                    .HasForeignKey(x => x.DestinationEntityID)
                    .OnDelete(DeleteBehavior.Restrict);

                // ─────────────────────────────
                // Indexes
                // ─────────────────────────────
                entity.HasIndex(x => new { x.SourceRoomID, x.SourceEntityID })
                    .IsUnique();
                entity.HasIndex(x => new { x.DestinationRoomID, x.DestinationEntityID });
            });
            #endregion

            #region Global
            modelBuilder.Entity<DefinitionVersionLog>(entity =>
            {
                // ─────────────────────────────
                // Table
                // ─────────────────────────────
                entity.ToTable("DefinitionVersionLogs");

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
        }
        #endregion
    }
}