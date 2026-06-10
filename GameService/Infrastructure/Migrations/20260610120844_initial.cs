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
                name: "Characteristics",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characteristics", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DefinitionVersions",
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
                    table.PrimaryKey("PK_DefinitionVersions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Effects",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false),
                    Duration = table.Column<float>(type: "real", nullable: true),
                    Interval = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Effects", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Appearance_SkinID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Appearance_SkinColor_H = table.Column<float>(type: "real", nullable: false),
                    Appearance_SkinColor_S = table.Column<float>(type: "real", nullable: false),
                    Appearance_SkinColor_V = table.Column<float>(type: "real", nullable: false),
                    Appearance_HairID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Appearance_EyesID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Appearance_ShirtID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Appearance_PantID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Appearance_HairColor_H = table.Column<float>(type: "real", nullable: true),
                    Appearance_HairColor_S = table.Column<float>(type: "real", nullable: true),
                    Appearance_HairColor_V = table.Column<float>(type: "real", nullable: true),
                    Appearance_PantColor_H = table.Column<float>(type: "real", nullable: true),
                    Appearance_PantColor_S = table.Column<float>(type: "real", nullable: true),
                    Appearance_PantColor_V = table.Column<float>(type: "real", nullable: true),
                    Collision_ShapeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Collision_Width = table.Column<float>(type: "real", nullable: false),
                    Collision_Height = table.Column<float>(type: "real", nullable: false),
                    Collision_Radius = table.Column<float>(type: "real", nullable: false),
                    Collision_IsBlocking = table.Column<bool>(type: "bit", nullable: false),
                    Collision_IsTrigger = table.Column<bool>(type: "bit", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Duration = table.Column<float>(type: "real", nullable: true),
                    CharacteristicID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InventoryID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true),
                    Entrance_ShapeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entrance_Width = table.Column<float>(type: "real", nullable: true),
                    Entrance_Height = table.Column<float>(type: "real", nullable: true),
                    Entrance_Radius = table.Column<float>(type: "real", nullable: true),
                    Entrance_IsBlocking = table.Column<bool>(type: "bit", nullable: true),
                    Entrance_IsTrigger = table.Column<bool>(type: "bit", nullable: true),
                    EntrancePosition_X = table.Column<float>(type: "real", nullable: true),
                    EntrancePosition_Y = table.Column<float>(type: "real", nullable: true),
                    Velocity = table.Column<float>(type: "real", nullable: true),
                    Projectile_Duration = table.Column<float>(type: "real", nullable: true),
                    InteractionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsInteractable = table.Column<bool>(type: "bit", nullable: true),
                    IsPickupable = table.Column<bool>(type: "bit", nullable: true),
                    WorldObject_InventoryID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SlotCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Durability = table.Column<int>(type: "int", nullable: true),
                    Stackable = table.Column<bool>(type: "bit", nullable: false),
                    EntityID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultAction = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ID);
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
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_NameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizedText_DescriptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredLocale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SteamID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CharacteristicID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false),
                    Min = table.Column<float>(type: "real", nullable: false),
                    Max = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeValues", x => new { x.CharacteristicID, x.Type, x.Level });
                    table.ForeignKey(
                        name: "FK_AttributeValues_Characteristics_CharacteristicID",
                        column: x => x.CharacteristicID,
                        principalTable: "Characteristics",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityRelationships",
                columns: table => new
                {
                    SourceEntityID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TargetEntityID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRelationships", x => new { x.SourceEntityID, x.TargetEntityID, x.Type });
                    table.ForeignKey(
                        name: "FK_EntityRelationships_Entities_SourceEntityID",
                        column: x => x.SourceEntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityRelationships_Entities_TargetEntityID",
                        column: x => x.TargetEntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    InventoryID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => new { x.InventoryID, x.ItemID });
                    table.ForeignKey(
                        name: "FK_InventoryItems_Inventories_InventoryID",
                        column: x => x.InventoryID,
                        principalTable: "Inventories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemEffects",
                columns: table => new
                {
                    ItemID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EffectID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemID1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemEffects", x => new { x.ItemID, x.EffectID });
                    table.ForeignKey(
                        name: "FK_ItemEffects_Effects_EffectID",
                        column: x => x.EffectID,
                        principalTable: "Effects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemEffects_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemEffects_Items_ItemID1",
                        column: x => x.ItemID1,
                        principalTable: "Items",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LocalizationEntries",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LocaleCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    Z = table.Column<int>(type: "int", nullable: false),
                    TileID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => new { x.RoomID, x.X, x.Y, x.Z });
                    table.ForeignKey(
                        name: "FK_Cells_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntitySpawnRules",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySpawnRules", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EntitySpawnRules_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitySpawnRules_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomConnections",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceRoomID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceEntityID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DestinationRoomID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DestinationEntityID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomConnections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RoomConnections_Rooms_DestinationRoomID",
                        column: x => x.DestinationRoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomConnections_Rooms_SourceRoomID",
                        column: x => x.SourceRoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpawnAreas",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MinX = table.Column<int>(type: "int", nullable: false),
                    MinY = table.Column<int>(type: "int", nullable: false),
                    MaxX = table.Column<int>(type: "int", nullable: false),
                    MaxY = table.Column<int>(type: "int", nullable: false),
                    MinCount = table.Column<int>(type: "int", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    EntitySpawnRuleID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpawnAreas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpawnAreas_EntitySpawnRules_EntitySpawnRuleID",
                        column: x => x.EntitySpawnRuleID,
                        principalTable: "EntitySpawnRules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValues_CharacteristicID_Type_Level",
                table: "AttributeValues",
                columns: new[] { "CharacteristicID", "Type", "Level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cells_RoomID_X_Y_Z",
                table: "Cells",
                columns: new[] { "RoomID", "X", "Y", "Z" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cells_TileID",
                table: "Cells",
                column: "TileID");

            migrationBuilder.CreateIndex(
                name: "IX_DefinitionVersions_CreatedAt",
                table: "DefinitionVersions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DefinitionVersions_Key_Version",
                table: "DefinitionVersions",
                columns: new[] { "Key", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entities_CharacteristicID",
                table: "Entities",
                column: "CharacteristicID");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_InventoryID",
                table: "Entities",
                column: "InventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRelationships_TargetEntityID",
                table: "EntityRelationships",
                column: "TargetEntityID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySpawnRules_EntityID",
                table: "EntitySpawnRules",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySpawnRules_RoomID",
                table: "EntitySpawnRules",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemID",
                table: "InventoryItems",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemEffects_EffectID",
                table: "ItemEffects",
                column: "EffectID");

            migrationBuilder.CreateIndex(
                name: "IX_ItemEffects_ItemID1",
                table: "ItemEffects",
                column: "ItemID1");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Category",
                table: "Items",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Items_DefaultAction",
                table: "Items",
                column: "DefaultAction");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Type",
                table: "Items",
                column: "Type");

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
                name: "IX_RoomConnections_DestinationRoomID_DestinationEntityID",
                table: "RoomConnections",
                columns: new[] { "DestinationRoomID", "DestinationEntityID" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomConnections_SourceRoomID_SourceEntityID",
                table: "RoomConnections",
                columns: new[] { "SourceRoomID", "SourceEntityID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpawnAreas_EntitySpawnRuleID",
                table: "SpawnAreas",
                column: "EntitySpawnRuleID");

            migrationBuilder.CreateIndex(
                name: "IX_SpawnAreas_MaxX_MaxY",
                table: "SpawnAreas",
                columns: new[] { "MaxX", "MaxY" });

            migrationBuilder.CreateIndex(
                name: "IX_SpawnAreas_MinX_MinY",
                table: "SpawnAreas",
                columns: new[] { "MinX", "MinY" });

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
                name: "AttributeValues");

            migrationBuilder.DropTable(
                name: "Cells");

            migrationBuilder.DropTable(
                name: "DefinitionVersions");

            migrationBuilder.DropTable(
                name: "EntityRelationships");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "ItemEffects");

            migrationBuilder.DropTable(
                name: "LocalizationEntries");

            migrationBuilder.DropTable(
                name: "RoomConnections");

            migrationBuilder.DropTable(
                name: "SpawnAreas");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Characteristics");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Effects");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Locales");

            migrationBuilder.DropTable(
                name: "EntitySpawnRules");

            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
