using Contract.Enum.EntityDomain;
using Domain.Common;
using Domain.Definition.EntityDomain;
using Domain.Definition.EntityDomain.Component;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeder
{
    public static class EntitySeeder
    {
        // Shared fallback color for inanimate entity appearances to satisfy the mandatory HSV constructor
        private static readonly HSV NeutralColor = new HSV(0f, 0f, 1f);

        #region 🆔 Core Domain Identifiers
        // 🏹 Archer Identifiers
        public const string ArcherPlayerId = "ent_player_archer";
        public const string ArcherCharacteristicId = "char_archer";
        public const string ArcherInventoryId = "inv_player_archer";

        // 🔨 Joker Identifiers
        public const string JokerPlayerId = "ent_player_joker";
        public const string JokerCharacteristicId = "char_joker";
        public const string JokerInventoryId = "inv_player_joker";

        // ⚔️ Warrior Identifiers
        public const string WarriorPlayerId = "ent_player_warrior";
        public const string WarriorCharacteristicId = "char_warrior";
        public const string WarriorInventoryId = "inv_player_warrior";
        #endregion

        #region 🆔 Projectile, Area & World Object IDs
        // Projectiles
        public const string PROJ_ARROW_WOOD = "proj_arrow_wood";
        public const string PROJ_BOLT_IRON = "proj_bolt_iron";
        public const string PROJ_BOMB_FIRE = "proj_bomb_fire";
        public const string PROJ_BOMB_STUN = "proj_bomb_stun";

        // 🍄 Shroom Elemental Projectiles (New)
        public const string PROJ_SHROOM_FIRE = "proj_shroom_fire";
        public const string PROJ_SHROOM_ICE = "proj_shroom_ice";
        public const string PROJ_SHROOM_EARTH = "proj_shroom_earth";
        public const string PROJ_SHROOM_DARK = "proj_shroom_dark";
        public const string PROJ_SHROOM_LIGHT = "proj_shroom_light";

        // Area Effects
        public const string AREA_SWORD_SLASH = "area_sword_slash";
        public const string AREA_CLUB_SMASH = "area_club_smash";
        public const string AREA_BOMB_FIRE = "area_bomb_fire";
        public const string AREA_BOMB_STUN = "area_bomb_stun";

        // 🍄 Shroom Elemental Area Effects (New)
        public const string AREA_SHROOM_FIRE = "area_shroom_fire";
        public const string AREA_SHROOM_ICE = "area_shroom_ice";
        public const string AREA_SHROOM_EARTH = "area_shroom_earth";
        public const string AREA_SHROOM_DARK = "area_shroom_dark";
        public const string AREA_SHROOM_LIGHT = "area_shroom_light";

        // World Objects
        public const string WORLD_CAMPFIRE = "world_campfire";
        public const string WORLD_CHEST = "world_chest";
        #endregion

        #region 🆔 🍄 Elemental Shroom Domain Identifiers (Extracted Keys)
        // 🔥 Fire Power Shroom
        public const string CreatureFireShroomId = "ent_creature_shroom_fire";
        public const string FireShroomCharacteristicId = "char_shroom_fire";
        public const string FireShroomInventoryId = "inv_creature_shroom_fire";

        // ❄️ Ice Power Shroom
        public const string CreatureIceShroomId = "ent_creature_shroom_ice";
        public const string IceShroomCharacteristicId = "char_shroom_ice";
        public const string IceShroomInventoryId = "inv_creature_shroom_ice";

        // ⛰️ Earth Power Shroom
        public const string CreatureEarthShroomId = "ent_creature_shroom_earth";
        public const string EarthShroomCharacteristicId = "char_shroom_earth";
        public const string EarthShroomInventoryId = "inv_creature_shroom_earth";

        // 🌌 Dark Power Shroom
        public const string CreatureDarkShroomId = "ent_creature_shroom_dark";
        public const string DarkShroomCharacteristicId = "char_shroom_dark";
        public const string DarkShroomInventoryId = "inv_creature_shroom_dark";

        // ☀️ Light Power Shroom
        public const string CreatureLightShroomId = "ent_creature_shroom_light";
        public const string LightShroomCharacteristicId = "char_shroom_light";
        public const string LightShroomInventoryId = "inv_creature_shroom_light";
        #endregion

        #region 🎨 Centralized Frontend Appearance Asset IDs
        public static class Skins
        {
            public const string Base01 = "skin_base_01";
            public const string Shroom = "skin_shroom_base";
        }

        public static class Hair
        {
            public const string Ranger01 = "hair_ranger_01";
            public const string Wild01 = "hair_wild_01";
            public const string KnightClean = "hair_knight_clean";
            public const string BushyBeard = "hair_bushy_beard";
        }

        public static class Eyes
        {
            public const string Focused = "eyes_focused";
            public const string Angry = "eyes_angry";
            public const string Determined = "eyes_determined";
            public const string Jolly = "eyes_jolly";
        }

        public static class Shirts
        {
            public const string TunicGreen = "shirt_tunic_green";
            public const string FursDark = "shirt_furs_dark";
            public const string SteelBreastplate = "shirt_steel_breastplate";
            public const string FlannelRed = "shirt_flannel_red";
        }

        public static class Pants
        {
            public const string LeatherBrown = "pant_leather_brown";
            public const string HideKilts = "pant_hide_kilts";
            public const string IronGreaves = "pant_iron_greaves";
            public const string OverallsBlue = "pant_overalls_blue";
        }
        #endregion

        #region ⚙️ Database Seeding Logic
        public static async Task SeedPlayerDefinitionsAsync(RelationalDB db)
        {
            var playerTemplates = new List<Player>
            {
                CreatePlayerTemplate(
                    id: ArcherPlayerId,
                    localizationKey: "player.archer",
                    characteristicId: ArcherCharacteristicId,
                    inventoryId: ArcherInventoryId,
                    appearance: new Appearance(
                        skinId: Skins.Base01,
                        skinColor: new HSV(0f, 0f, 1f),
                        hairId: Hair.Ranger01,
                        eyesId: Eyes.Focused,
                        shirtId: Shirts.TunicGreen,
                        pantId: Pants.LeatherBrown,
                        hairColor: new HSV(30f, 0.8f, 0.5f),
                        pantColor: new HSV(20f, 0.5f, 0.3f)),
                    collision: new Collision(CollisionShapeType.Box, width: 14f, height: 16f, radius: 0f, isBlocking: true, isTrigger: false)
                ),
                CreatePlayerTemplate(
                    id: JokerPlayerId,
                    localizationKey: "player.joker",
                    characteristicId: JokerCharacteristicId,
                    inventoryId: JokerInventoryId,
                    appearance: new Appearance(
                        skinId: Skins.Base01,
                        skinColor: new HSV(15f, 0.1f, 0.9f),
                        hairId: Hair.Wild01,
                        eyesId: Eyes.Angry,
                        shirtId: Shirts.FursDark,
                        pantId: Pants.HideKilts,
                        hairColor: new HSV(12f, 0.9f, 0.6f),
                        pantColor: new HSV(0f, 0f, 0.2f)),
                    collision: new Collision(CollisionShapeType.Box, width: 18f, height: 18f, radius: 0f, isBlocking: true, isTrigger: false)
                ),
                CreatePlayerTemplate(
                    id: WarriorPlayerId,
                    localizationKey: "player.warrior",
                    characteristicId: WarriorCharacteristicId,
                    inventoryId: WarriorInventoryId,
                    appearance: new Appearance(
                        skinId: Skins.Base01,
                        skinColor: new HSV(0f, 0f, 1f),
                        hairId: Hair.KnightClean,
                        eyesId: Eyes.Determined,
                        shirtId: Shirts.SteelBreastplate,
                        pantId: Pants.IronGreaves,
                        hairColor: new HSV(45f, 0.7f, 0.8f),
                        pantColor: new HSV(220f, 0.4f, 0.5f)),
                    collision: new Collision(CollisionShapeType.Box, width: 16f, height: 16f, radius: 0f, isBlocking: true, isTrigger: false)
                ),
            };

            await db.Set<Player>().AddRangeAsync(playerTemplates);
            await db.SaveChangesAsync();
        }

        public static async Task SeedCreatureDefinitionsAsync(RelationalDB db)
        {
            var creatureTemplates = new List<Creature>
            {
                // 1. 🔥 FirePower Shroom
                CreateCreatureTemplate(
                    id: CreatureFireShroomId,
                    localizationKey: "creature.shroom.fire",
                    characteristicId: FireShroomCharacteristicId,
                    inventoryId: FireShroomInventoryId,
                    appearance: new Appearance(
                        skinId: Skins.Shroom,
                        skinColor: new HSV(12f, 0.9f, 0.95f), // Intense Fire Red/Orange
                        hairId: null, eyesId: null, shirtId: null, pantId: null, hairColor: null, pantColor: null),
                    collision: new Collision(CollisionShapeType.Box, width: 12f, height: 12f, radius: 0f, isBlocking: true, isTrigger: false)
                ),

                // 2. ❄️ IcePower Shroom
                CreateCreatureTemplate(
                    id: CreatureIceShroomId,
                    localizationKey: "creature.shroom.ice",
                    characteristicId: IceShroomCharacteristicId,
                    inventoryId: IceShroomInventoryId,
                    appearance: new Appearance(
                        skinId: Skins.Shroom,
                        skinColor: new HSV(195f, 0.65f, 0.95f), // Frost Cyan/Ice Blue
                        hairId: null, eyesId: null, shirtId: null, pantId: null, hairColor: null, pantColor: null),
                    collision: new Collision(CollisionShapeType.Box, width: 12f, height: 12f, radius: 0f, isBlocking: true, isTrigger: false)
                ),

                // 3. ⛰️ EarthPower Shroom
                CreateCreatureTemplate(
                    id: CreatureEarthShroomId,
                    localizationKey: "creature.shroom.earth",
                    characteristicId: EarthShroomCharacteristicId,
                    inventoryId: EarthShroomInventoryId,
                    appearance: new Appearance(
                        skinId: Skins.Shroom,
                        skinColor: new HSV(32f, 0.7f, 0.5f), // Deep Terracotta / Muddy Earth Brown
                        hairId: null, eyesId: null, shirtId: null, pantId: null, hairColor: null, pantColor: null),
                    collision: new Collision(CollisionShapeType.Box, width: 12f, height: 12f, radius: 0f, isBlocking: true, isTrigger: false)
                ),

                // 4. 🌌 DarkPower Shroom
                CreateCreatureTemplate(
                    id: CreatureDarkShroomId,
                    localizationKey: "creature.shroom.dark",
                    characteristicId: DarkShroomCharacteristicId,
                    inventoryId: DarkShroomInventoryId,
                    appearance: new Appearance(
                        skinId: Skins.Shroom,
                        skinColor: new HSV(275f, 0.85f, 0.35f), // Dark Abyssal Violet
                        hairId: null, eyesId: null, shirtId: null, pantId: null, hairColor: null, pantColor: null),
                    collision: new Collision(CollisionShapeType.Box, width: 12f, height: 12f, radius: 0f, isBlocking: true, isTrigger: false)
                ),

                // 5. ☀️ LightPower Shroom
                CreateCreatureTemplate(
                    id: CreatureLightShroomId,
                    localizationKey: "creature.shroom.light",
                    characteristicId: LightShroomCharacteristicId,
                    inventoryId: LightShroomInventoryId,
                    appearance: new Appearance(
                        skinId: Skins.Shroom,
                        skinColor: new HSV(50f, 0.25f, 1.0f), // Glowing Radiant Off-White/Gold
                        hairId: null, eyesId: null, shirtId: null, pantId: null, hairColor: null, pantColor: null),
                    collision: new Collision(CollisionShapeType.Box, width: 12f, height: 12f, radius: 0f, isBlocking: true, isTrigger: false)
                )
            };

            await db.Set<Creature>().AddRangeAsync(creatureTemplates);
            await db.SaveChangesAsync();
        }

        public static async Task SeedEntityDefinitionsAsync(RelationalDB db)
        {
            await db.Set<Projectile>().ExecuteDeleteAsync();
            await db.Set<AreaEffect>().ExecuteDeleteAsync();
            await db.Set<WorldObject>().ExecuteDeleteAsync();
            await db.Set<EntityRelationship>().ExecuteDeleteAsync();

            var projectiles = new List<Projectile>
            {
                new(PROJ_ARROW_WOOD, EntityType.Projectile, LocalizationFactory.ForEntity("proj.arrow"), new Appearance(PROJ_ARROW_WOOD, NeutralColor), new Collision(CollisionShapeType.Circle, 4f, 4f, 2f, false, true), 300f, 2f),
                new(PROJ_BOLT_IRON, EntityType.Projectile, LocalizationFactory.ForEntity("proj.bolt"), new Appearance(PROJ_BOLT_IRON, NeutralColor), new Collision(CollisionShapeType.Circle, 4f, 4f, 2f, false, true), 450f, 1.5f),
                new(PROJ_BOMB_FIRE, EntityType.Projectile, LocalizationFactory.ForEntity("proj.firebomb"), new Appearance(PROJ_BOMB_FIRE, NeutralColor), new Collision(CollisionShapeType.Circle, 6f, 6f, 3f, false, true), 150f, 3f),
                new(PROJ_BOMB_STUN, EntityType.Projectile, LocalizationFactory.ForEntity("proj.stunbomb"), new Appearance(PROJ_BOMB_STUN, NeutralColor), new Collision(CollisionShapeType.Circle, 6f, 6f, 3f, false, true), 150f, 3f),
                
                // 🍄 Shroom Projectiles
                new(PROJ_SHROOM_FIRE, EntityType.Projectile, LocalizationFactory.ForEntity("proj.shroom.fire"), new Appearance(PROJ_SHROOM_FIRE, NeutralColor), new Collision(CollisionShapeType.Circle, 5f, 5f, 2.5f, false, true), 180f, 2.5f),
                new(PROJ_SHROOM_ICE, EntityType.Projectile, LocalizationFactory.ForEntity("proj.shroom.ice"), new Appearance(PROJ_SHROOM_ICE, NeutralColor), new Collision(CollisionShapeType.Circle, 5f, 5f, 2.5f, false, true), 180f, 2.5f),
                new(PROJ_SHROOM_EARTH, EntityType.Projectile, LocalizationFactory.ForEntity("proj.shroom.earth"), new Appearance(PROJ_SHROOM_EARTH, NeutralColor), new Collision(CollisionShapeType.Circle, 6f, 6f, 3f, false, true), 140f, 3f),
                new(PROJ_SHROOM_DARK, EntityType.Projectile, LocalizationFactory.ForEntity("proj.shroom.dark"), new Appearance(PROJ_SHROOM_DARK, NeutralColor), new Collision(CollisionShapeType.Circle, 5f, 5f, 2.5f, false, true), 160f, 3f),
                new(PROJ_SHROOM_LIGHT, EntityType.Projectile, LocalizationFactory.ForEntity("proj.shroom.light"), new Appearance(PROJ_SHROOM_LIGHT, NeutralColor), new Collision(CollisionShapeType.Circle, 5f, 5f, 2.5f, false, true), 220f, 2f)
            };

            var areaEffects = new List<AreaEffect>
            {
                new(AREA_SWORD_SLASH, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.slash"), new Appearance(AREA_SWORD_SLASH, NeutralColor), new Collision(CollisionShapeType.Box, 20f, 10f, 0f, false, true), 0.5f),
                new(AREA_CLUB_SMASH, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.smash"), new Appearance(AREA_CLUB_SMASH, NeutralColor), new Collision(CollisionShapeType.Circle, 0f, 0f, 15f, false, true), 0.8f),
                new(AREA_BOMB_FIRE, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.fire"), new Appearance(AREA_BOMB_FIRE, NeutralColor), new Collision(CollisionShapeType.Circle, 0f, 0f, 30f, false, true), 5.0f),
                new(AREA_BOMB_STUN, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.stun"), new Appearance(AREA_BOMB_STUN, NeutralColor), new Collision(CollisionShapeType.Circle, 0f, 0f, 20f, false, true), 2.0f),
                
                // 🍄 Shroom Blast Areas
                new(AREA_SHROOM_FIRE, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.shroom.fire"), new Appearance(AREA_SHROOM_FIRE, NeutralColor), new Collision(CollisionShapeType.Circle, 0f, 0f, 24f, false, true), 3.0f),
                new(AREA_SHROOM_ICE, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.shroom.ice"), new Appearance(AREA_SHROOM_ICE, NeutralColor), new Collision(CollisionShapeType.Circle, 0f, 0f, 24f, false, true), 2.5f),
                new(AREA_SHROOM_EARTH, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.shroom.earth"), new Appearance(AREA_SHROOM_EARTH, NeutralColor), new Collision(CollisionShapeType.Circle, 0f, 0f, 28f, false, true), 1.0f),
                new(AREA_SHROOM_DARK, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.shroom.dark"), new Appearance(AREA_SHROOM_DARK, NeutralColor), new Collision(CollisionShapeType.Circle, 0f, 0f, 20f, false, true), 4.0f),
                new(AREA_SHROOM_LIGHT, EntityType.AreaEffect, LocalizationFactory.ForEntity("area.shroom.light"), new Appearance(AREA_SHROOM_LIGHT, NeutralColor), new Collision(CollisionShapeType.Circle, 0f, 0f, 22f, false, true), 0.5f)
            };

            var worldObjects = new List<WorldObject>
            {
                new(WORLD_CAMPFIRE, EntityType.WorldObject, LocalizationFactory.ForEntity("world.campfire"), new Appearance(WORLD_CAMPFIRE, NeutralColor), new Collision(CollisionShapeType.Box, 20f, 20f, 0f, true, false), WorldObjectInteractionType.Decoration, true, false, null),
                new(WORLD_CHEST, EntityType.WorldObject, LocalizationFactory.ForEntity("world.chest"), new Appearance(WORLD_CHEST, NeutralColor), new Collision(CollisionShapeType.Box, 24f, 24f, 0f, true, false), WorldObjectInteractionType.Decoration, true, true, "inv_chest_01")
            };

            // Entity Relationship Mappings
            var relationships = new List<EntityRelationship>
            {
                // Throwable Projectile -> AreaEffect Spawning Relationships
                new(PROJ_BOMB_FIRE, AREA_BOMB_FIRE, EntityRelationshipType.Throwable),
                new(PROJ_BOMB_STUN, AREA_BOMB_STUN, EntityRelationshipType.Throwable),

                // 🍄 Shroom Throwable Handlers
                new(PROJ_SHROOM_FIRE, AREA_SHROOM_FIRE, EntityRelationshipType.Throwable),
                new(PROJ_SHROOM_ICE, AREA_SHROOM_ICE, EntityRelationshipType.Throwable),
                new(PROJ_SHROOM_EARTH, AREA_SHROOM_EARTH, EntityRelationshipType.Throwable),
                new(PROJ_SHROOM_DARK, AREA_SHROOM_DARK, EntityRelationshipType.Throwable),
                new(PROJ_SHROOM_LIGHT, AREA_SHROOM_LIGHT, EntityRelationshipType.Throwable),

                // 🍄 Shroom Combat Activation Techniques (Projectile is activated by Creature)
                new(PROJ_SHROOM_FIRE, CreatureFireShroomId, EntityRelationshipType.ProjectileTriggeredBy),
                new(PROJ_SHROOM_ICE, CreatureIceShroomId, EntityRelationshipType.ProjectileTriggeredBy),
                new(PROJ_SHROOM_EARTH, CreatureEarthShroomId, EntityRelationshipType.ProjectileTriggeredBy),
                new(PROJ_SHROOM_DARK, CreatureDarkShroomId, EntityRelationshipType.ProjectileTriggeredBy),
                new(PROJ_SHROOM_LIGHT, CreatureLightShroomId, EntityRelationshipType.ProjectileTriggeredBy)
            };

            await db.Set<Projectile>().AddRangeAsync(projectiles);
            await db.Set<AreaEffect>().AddRangeAsync(areaEffects);
            await db.Set<WorldObject>().AddRangeAsync(worldObjects);
            await db.Set<EntityRelationship>().AddRangeAsync(relationships);
            await db.SaveChangesAsync();
        }
        #endregion

        #region 🏭 Template Factory Method
        private static Player CreatePlayerTemplate(
            string id,
            string localizationKey,
            string characteristicId,
            string inventoryId,
            Appearance appearance,
            Collision collision)
        {
            return new Player(
                id: id,
                type: EntityType.Player,
                localizedText: LocalizationFactory.ForEntity(localizationKey),
                appearance: appearance,
                collision: collision,
                characteristicId: characteristicId,
                inventoryId: inventoryId,
                level: 1
            );
        }

        /// <summary>
        /// Instantiates an AI Creature layout containing identical shared properties to the baseline Player entity schema.
        /// </summary>
        private static Creature CreateCreatureTemplate(
            string id,
            string localizationKey,
            string characteristicId,
            string inventoryId,
            Appearance appearance,
            Collision collision)
        {
            return new Creature(
                id: id,
                type: EntityType.Creature, // Core AI Creature domain enum assignment
                localizedText: LocalizationFactory.ForEntity(localizationKey),
                appearance: appearance,
                collision: collision,
                characteristicId: characteristicId,
                inventoryId: inventoryId,
                level: 1
            );
        }
        #endregion
    }
}