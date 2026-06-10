using Contract.Enum.EntityDomain;
using Contract.Enum.ItemDomain;
using Domain.Definition.ItemDomain;
using Domain.Shared;

namespace Infrastructure.Persistence.Seeder
{
    public static class ItemSeeder
    {
        #region 🆔 Item Registry
        public const string HP_POTION_ID = "item_consumable_hp_01";
        public const string STAMINA_POTION_ID = "item_consumable_stam_01";
        public const string IRON_SWORD_ID = "item_melee_sword_iron";
        public const string WOODEN_CLUB_ID = "item_melee_club_wood";
        public const string WOODEN_BOW_ID = "item_range_bow_wood";
        public const string CROSSBOW_ID = "item_range_crossbow_iron";
        public const string FIRE_BOMB_ID = "item_throw_bomb_fire";
        public const string STUN_GRENADE_ID = "item_throw_stun_grenade";
        public const string CAMPFIRE_ID = "item_place_campfire";
        public const string CHEST_ID = "item_place_chest";
        public const string IRON_ORE_ID = "item_mat_iron_ore";
        public const string WOOD_LOG_ID = "item_mat_wood_log";
        public const string IRON_HELMET_ID = "item_equip_head_iron";
        public const string IRON_CHEST_ID = "item_equip_chest_iron";
        public const string IRON_PANTS_ID = "item_equip_pant_iron";
        public const string IRON_SHOES_ID = "item_equip_shoe_iron";
        #endregion

        public static async Task SeedAsync(RelationalDB db)
        {
            var items = new List<Item>
            {
                CreateItem(HP_POTION_ID, ItemType.Consumable, ItemCategory.Consumable, "hp_potion", null, true),
                CreateItem(STAMINA_POTION_ID, ItemType.Consumable, ItemCategory.Consumable, "stam_potion", null, true),
                CreateItem(IRON_SWORD_ID, ItemType.MeleeWeapon, ItemCategory.MeleeWeapon, "iron_sword", 100, false, EntitySeeder.AREA_SWORD_SLASH),
                CreateItem(WOODEN_CLUB_ID, ItemType.MeleeWeapon, ItemCategory.MeleeWeapon, "wood_club", 50, false, EntitySeeder.AREA_CLUB_SMASH),
                CreateItem(WOODEN_BOW_ID, ItemType.RangedWeapon, ItemCategory.RangedWeapon, "wood_bow", 150, false, EntitySeeder.PROJ_ARROW_WOOD),
                CreateItem(CROSSBOW_ID, ItemType.RangedWeapon, ItemCategory.RangedWeapon, "iron_crossbow", 200, false, EntitySeeder.PROJ_BOLT_IRON),
                CreateItem(FIRE_BOMB_ID, ItemType.ThrowableWeapon, ItemCategory.ThrowableWeapon, "fire_bomb", null, true, EntitySeeder.PROJ_BOMB_FIRE),
                CreateItem(STUN_GRENADE_ID, ItemType.ThrowableWeapon, ItemCategory.ThrowableWeapon, "stun_grenade", null, true, EntitySeeder.PROJ_BOMB_STUN),
                CreateItem(CAMPFIRE_ID, ItemType.Placeable, ItemCategory.Placeable, "campfire", null, false, EntitySeeder.WORLD_CAMPFIRE),
                CreateItem(CHEST_ID, ItemType.Placeable, ItemCategory.Placeable, "storage_chest", null, false, EntitySeeder.WORLD_CHEST),
                CreateItem(IRON_ORE_ID, ItemType.Material, ItemCategory.Material, "iron_ore", null, true),
                CreateItem(WOOD_LOG_ID, ItemType.Material, ItemCategory.Material, "wood_log", null, true),
                CreateItem(IRON_HELMET_ID, ItemType.Equippable, ItemCategory.Head, "iron_helmet", 200, false),
                CreateItem(IRON_CHEST_ID, ItemType.Equippable, ItemCategory.Chest, "iron_chest", 200, false),
                CreateItem(IRON_PANTS_ID, ItemType.Equippable, ItemCategory.Pant, "iron_pants", 200, false),
                CreateItem(IRON_SHOES_ID, ItemType.Equippable, ItemCategory.Shoe, "iron_shoes", 200, false)
            };

            await db.Set<Item>().AddRangeAsync(items);
            await db.SaveChangesAsync();

            // ═══════════════════════════════════════════════════════════════════
            // 🔗 SEED ITEM EFFECTS (Bridging cleanly to active Effect definitions)
            // ═══════════════════════════════════════════════════════════════════
            var itemEffects = new List<ItemEffect>();

            // 🧪 Consumables
            itemEffects.Add(new ItemEffect(HP_POTION_ID, EffectSeeder.HpId(HP_POTION_ID)));
            itemEffects.Add(new ItemEffect(STAMINA_POTION_ID, EffectSeeder.StaminaId(STAMINA_POTION_ID)));

            // ⚔️ Melee Weapons
            itemEffects.AddRange(LinkWeaponVitals(IRON_SWORD_ID, hp: true, stamina: true));
            itemEffects.AddRange(LinkWeaponVitals(WOODEN_CLUB_ID, hp: true, stamina: true));

            // 🏹 Ranged Weapons
            itemEffects.AddRange(LinkWeaponVitals(WOODEN_BOW_ID, hp: true, stamina: true));
            itemEffects.AddRange(LinkWeaponVitals(CROSSBOW_ID, hp: true, stamina: true));

            // 💣 Throwables (Linking clean basic vitals + global meaningful special constants)
            itemEffects.AddRange(LinkWeaponVitals(FIRE_BOMB_ID, hp: true));
            itemEffects.Add(new ItemEffect(FIRE_BOMB_ID, EffectSeeder.EFFECT_BURN_ID)); // Matches "status_effect_burn"

            itemEffects.AddRange(LinkWeaponVitals(STUN_GRENADE_ID, stamina: true));
            itemEffects.Add(new ItemEffect(STUN_GRENADE_ID, EffectSeeder.EFFECT_SLOW_ID)); // Matches "status_effect_slow"

            // 🛡️ Armor Layer Protective Permanent Effects
            itemEffects.Add(new ItemEffect(IRON_HELMET_ID, EffectSeeder.FireResPermId(IRON_HELMET_ID)));
            itemEffects.Add(new ItemEffect(IRON_HELMET_ID, EffectSeeder.IceResPermId(IRON_HELMET_ID)));
            itemEffects.Add(new ItemEffect(IRON_HELMET_ID, EffectSeeder.EarthResPermId(IRON_HELMET_ID)));

            // Iron Chestpiece (Armor Resists + Global Special Weight slow constraint)
            itemEffects.Add(new ItemEffect(IRON_CHEST_ID, EffectSeeder.FireResPermId(IRON_CHEST_ID)));
            itemEffects.Add(new ItemEffect(IRON_CHEST_ID, EffectSeeder.IceResPermId(IRON_CHEST_ID)));
            itemEffects.Add(new ItemEffect(IRON_CHEST_ID, EffectSeeder.EarthResPermId(IRON_CHEST_ID)));
            itemEffects.Add(new ItemEffect(IRON_CHEST_ID, EffectSeeder.EFFECT_WEIGHT_SLOW_ID)); // Matches "status_effect_weight_slow"

            itemEffects.Add(new ItemEffect(IRON_PANTS_ID, EffectSeeder.FireResPermId(IRON_PANTS_ID)));
            itemEffects.Add(new ItemEffect(IRON_PANTS_ID, EffectSeeder.IceResPermId(IRON_PANTS_ID)));
            itemEffects.Add(new ItemEffect(IRON_PANTS_ID, EffectSeeder.EarthResPermId(IRON_PANTS_ID)));

            itemEffects.Add(new ItemEffect(IRON_SHOES_ID, EffectSeeder.FireResPermId(IRON_SHOES_ID)));
            itemEffects.Add(new ItemEffect(IRON_SHOES_ID, EffectSeeder.IceResPermId(IRON_SHOES_ID)));
            itemEffects.Add(new ItemEffect(IRON_SHOES_ID, EffectSeeder.EarthResPermId(IRON_SHOES_ID)));

            await db.Set<ItemEffect>().AddRangeAsync(itemEffects);
            await db.SaveChangesAsync();
        }

        #region 🛠️ Private Mapping Helpers
        private static Item CreateItem(string id, ItemType type, ItemCategory cat, string locKey, int? dur, bool stack, string? entityId = null)
        {
            EntityAction action = type switch
            {
                ItemType.RangedWeapon => EntityAction.SHOOT,
                ItemType.MeleeWeapon => EntityAction.SWING,
                _ => EntityAction.NONE
            };

            return new Item(id, type, LocalizationFactory.ForItem(locKey), cat, dur, stack, entityId ?? string.Empty, action);
        }

        private static List<ItemEffect> LinkWeaponVitals(string baseItemId, bool hp = false, bool stamina = false, bool energy = false)
        {
            var relations = new List<ItemEffect>();
            if (hp) relations.Add(new ItemEffect(baseItemId, EffectSeeder.HpId(baseItemId)));
            if (stamina) relations.Add(new ItemEffect(baseItemId, EffectSeeder.StaminaId(baseItemId)));
            if (energy) relations.Add(new ItemEffect(baseItemId, EffectSeeder.EnergyId(baseItemId)));
            return relations;
        }
        #endregion
    }
}