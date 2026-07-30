using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIDefinitions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeashDistance = table.Column<float>(type: "real", nullable: false),
                    AggroRadius = table.Column<float>(type: "real", nullable: false),
                    ThinkInterval = table.Column<float>(type: "real", nullable: false),
                    IsAIControlled = table.Column<bool>(type: "bit", nullable: false),
                    EquippedItemDefinitionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttackRange = table.Column<float>(type: "real", nullable: false),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AppearanceDefinitions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkinID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SkinColor_H = table.Column<float>(type: "real", nullable: false),
                    SkinColor_S = table.Column<float>(type: "real", nullable: false),
                    SkinColor_V = table.Column<float>(type: "real", nullable: false),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppearanceDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicDefinitions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CollisionDefinitions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollisionRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShapeType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Width = table.Column<float>(type: "real", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Radius = table.Column<float>(type: "real", nullable: false),
                    IsBlocking = table.Column<bool>(type: "bit", nullable: false),
                    Layer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mask = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OffsetX = table.Column<float>(type: "real", nullable: false),
                    OffsetY = table.Column<float>(type: "real", nullable: false),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollisionDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CombatRunDefinitions",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatRunDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DefinitionVersionLogs",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefinitionVersionLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EffectDefinitions",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AttributeType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false),
                    Duration = table.Column<float>(type: "real", nullable: true),
                    Interval = table.Column<float>(type: "real", nullable: true),
                    Presentation_LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_IconID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EntityDefinitions",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_IconID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InventoryDefinitions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlotCount = table.Column<int>(type: "int", nullable: false),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItemDefinitions",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaxStack = table.Column<int>(type: "int", nullable: true),
                    MaxDurability = table.Column<int>(type: "int", nullable: true),
                    TriggeredAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Presentation_LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_IconID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceableConfig_EntityDefinitionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RangedConfig_EntityDefinitionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeleeConfig_EntityDefinitionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostConfig_Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsumableConfig = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquippableConfig = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LifetimeDefinitions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Duration = table.Column<float>(type: "real", nullable: false),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifetimeDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Locales",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locales", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ProjectileDefinitions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Velocity = table.Column<float>(type: "real", nullable: false),
                    OnImpactSpawnEntityDefinitionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectileDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RoomDefinitions",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Presentation_IconID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TriggeredEffectDefinitions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EffectDefinitionIDs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggeredEffectDefinitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SteamID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AttributeValues",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BaseValue = table.Column<float>(type: "real", nullable: false),
                    Min = table.Column<float>(type: "real", nullable: false),
                    Max = table.Column<float>(type: "real", nullable: false),
                    CharacteristicDefinitionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeValues", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AttributeValues_CharacteristicDefinitions_CharacteristicDefinitionID",
                        column: x => x.CharacteristicDefinitionID,
                        principalTable: "CharacteristicDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CombatRunFloors",
                columns: table => new
                {
                    Level = table.Column<int>(type: "int", nullable: false),
                    CombatRunDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatRunFloors", x => new { x.CombatRunDefinitionID, x.Level });
                    table.ForeignKey(
                        name: "FK_CombatRunFloors_CombatRunDefinitions_CombatRunDefinitionID",
                        column: x => x.CombatRunDefinitionID,
                        principalTable: "CombatRunDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryEntries",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InventoryDefinitionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InventoryEntries_InventoryDefinitions_InventoryDefinitionID",
                        column: x => x.InventoryDefinitionID,
                        principalTable: "InventoryDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationEntries",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LocaleCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationEntries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LocalizationEntries_Locales_LocaleCode",
                        column: x => x.LocaleCode,
                        principalTable: "Locales",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    RoomDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    Z = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => new { x.RoomDefinitionID, x.X, x.Y, x.Z });
                    table.ForeignKey(
                        name: "FK_Cells_RoomDefinitions_RoomDefinitionID",
                        column: x => x.RoomDefinitionID,
                        principalTable: "RoomDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntitySpawnRules",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinX = table.Column<int>(type: "int", nullable: false),
                    MinY = table.Column<int>(type: "int", nullable: false),
                    MaxX = table.Column<int>(type: "int", nullable: false),
                    MaxY = table.Column<int>(type: "int", nullable: false),
                    MinCount = table.Column<int>(type: "int", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    RoomDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityDefinitionID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySpawnRules", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EntitySpawnRules_EntityDefinitions_EntityDefinitionID",
                        column: x => x.EntityDefinitionID,
                        principalTable: "EntityDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitySpawnRules_RoomDefinitions_RoomDefinitionID",
                        column: x => x.RoomDefinitionID,
                        principalTable: "RoomDefinitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeGrowthValues",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    GrowthValue = table.Column<float>(type: "real", nullable: false),
                    AttributeValueID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeGrowthValues", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AttributeGrowthValues_AttributeValues_AttributeValueID",
                        column: x => x.AttributeValueID,
                        principalTable: "AttributeValues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIDefinitions_EntityDefinitionID",
                table: "AIDefinitions",
                column: "EntityDefinitionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIDefinitions_IsAIControlled",
                table: "AIDefinitions",
                column: "IsAIControlled");

            migrationBuilder.CreateIndex(
                name: "IX_AppearanceDefinitions_EntityDefinitionID",
                table: "AppearanceDefinitions",
                column: "EntityDefinitionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppearanceDefinitions_SkinID",
                table: "AppearanceDefinitions",
                column: "SkinID");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeGrowthValues_AttributeValueID",
                table: "AttributeGrowthValues",
                column: "AttributeValueID");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeGrowthValues_AttributeValueID_Level",
                table: "AttributeGrowthValues",
                columns: new[] { "AttributeValueID", "Level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValues_CharacteristicDefinitionID",
                table: "AttributeValues",
                column: "CharacteristicDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValues_Type",
                table: "AttributeValues",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Cells_RoomDefinitionID_X_Y_Z",
                table: "Cells",
                columns: new[] { "RoomDefinitionID", "X", "Y", "Z" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicDefinitions_EntityDefinitionID",
                table: "CharacteristicDefinitions",
                column: "EntityDefinitionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollisionDefinitions_EntityDefinitionID",
                table: "CollisionDefinitions",
                column: "EntityDefinitionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollisionDefinitions_IsBlocking",
                table: "CollisionDefinitions",
                column: "IsBlocking");

            migrationBuilder.CreateIndex(
                name: "IX_CollisionDefinitions_ShapeType",
                table: "CollisionDefinitions",
                column: "ShapeType");

            migrationBuilder.CreateIndex(
                name: "IX_CombatRunDefinitions_ID",
                table: "CombatRunDefinitions",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_CombatRunFloors_Level",
                table: "CombatRunFloors",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_CombatRunFloors_RoomDefinitionID",
                table: "CombatRunFloors",
                column: "RoomDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_DefinitionVersionLogs_CreatedAt",
                table: "DefinitionVersionLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DefinitionVersionLogs_Key_Version",
                table: "DefinitionVersionLogs",
                columns: new[] { "Key", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EffectDefinitions_AttributeType",
                table: "EffectDefinitions",
                column: "AttributeType");

            migrationBuilder.CreateIndex(
                name: "IX_EffectDefinitions_Type",
                table: "EffectDefinitions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySpawnRules_EntityDefinitionID",
                table: "EntitySpawnRules",
                column: "EntityDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySpawnRules_RoomDefinitionID",
                table: "EntitySpawnRules",
                column: "RoomDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDefinitions_EntityDefinitionID",
                table: "InventoryDefinitions",
                column: "EntityDefinitionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntries_DefinitionID",
                table: "InventoryEntries",
                column: "DefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntries_InventoryDefinitionID",
                table: "InventoryEntries",
                column: "InventoryDefinitionID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitions_Category",
                table: "ItemDefinitions",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitions_Type",
                table: "ItemDefinitions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_LifetimeDefinitions_Duration",
                table: "LifetimeDefinitions",
                column: "Duration");

            migrationBuilder.CreateIndex(
                name: "IX_LifetimeDefinitions_EntityDefinitionID",
                table: "LifetimeDefinitions",
                column: "EntityDefinitionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locales_Code",
                table: "Locales",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Locales_IsEnabled",
                table: "Locales",
                column: "IsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationEntries_Key",
                table: "LocalizationEntries",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationEntries_Key_LocaleCode",
                table: "LocalizationEntries",
                columns: new[] { "Key", "LocaleCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationEntries_LocaleCode",
                table: "LocalizationEntries",
                column: "LocaleCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectileDefinitions_EntityDefinitionID",
                table: "ProjectileDefinitions",
                column: "EntityDefinitionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectileDefinitions_Velocity",
                table: "ProjectileDefinitions",
                column: "Velocity");

            migrationBuilder.CreateIndex(
                name: "IX_TriggeredEffectDefinitions_EntityDefinitionID",
                table: "TriggeredEffectDefinitions",
                column: "EntityDefinitionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SteamID",
                table: "Users",
                column: "SteamID",
                unique: true,
                filter: "[SteamID] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIDefinitions");

            migrationBuilder.DropTable(
                name: "AppearanceDefinitions");

            migrationBuilder.DropTable(
                name: "AttributeGrowthValues");

            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "CollisionDefinitions");

            migrationBuilder.DropTable(
                name: "CombatRunFloors");

            migrationBuilder.DropTable(
                name: "DefinitionVersionLogs");

            migrationBuilder.DropTable(
                name: "EffectDefinitions");

            migrationBuilder.DropTable(
                name: "EntitySpawnRules");

            migrationBuilder.DropTable(
                name: "InventoryEntries");

            migrationBuilder.DropTable(
                name: "ItemDefinitions");

            migrationBuilder.DropTable(
                name: "LifetimeDefinitions");

            migrationBuilder.DropTable(
                name: "LocalizationEntries");

            migrationBuilder.DropTable(
                name: "ProjectileDefinitions");

            migrationBuilder.DropTable(
                name: "TriggeredEffectDefinitions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AttributeValues");

            migrationBuilder.DropTable(
                name: "CombatRunDefinitions");

            migrationBuilder.DropTable(
                name: "EntityDefinitions");

            migrationBuilder.DropTable(
                name: "RoomDefinitions");

            migrationBuilder.DropTable(
                name: "InventoryDefinitions");

            migrationBuilder.DropTable(
                name: "Locales");

            migrationBuilder.DropTable(
                name: "CharacteristicDefinitions");
        }
    }
}
