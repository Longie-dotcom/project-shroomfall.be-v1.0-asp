using Domain.Definition.LocalizationDomain;

namespace Domain.Shared
{
    public static class LocalizationFactory
    {
        /// <summary>
        /// Generates localized keys for system Attributes (e.g., parameter.attack_damage.name)
        /// </summary>
        public static LocalizedText ForAttribute(
            string attributeKeyName)
        {
            return new LocalizedText
            {
                NameKey = $"parameter.{attributeKeyName.ToLowerShared()}.name",
                DescriptionKey = $"parameter.{attributeKeyName.ToLowerShared()}.description"
            };
        }

        /// <summary>
        /// Generates localized keys for Items (e.g., item.iron_sword.name)
        /// </summary>
        public static LocalizedText ForItem(
            string itemId)
        {
            return new LocalizedText
            {
                NameKey = $"item.{itemId.ToLowerShared()}.name",
                DescriptionKey = $"item.{itemId.ToLowerShared()}.description"
            };
        }

        /// <summary>
        /// Generates localized keys for system Effect (e.g., parameter.burn.name)
        /// </summary>
        public static LocalizedText ForEffect(
            string effectId)
        {
            return new LocalizedText
            {
                NameKey = $"effect.{effectId.ToLowerShared()}.name",
                DescriptionKey = $"effect.{effectId.ToLowerShared()}.description"
            };
        }

        /// <summary>
        /// Generates localized keys for Game Entities (e.g., entity.slime_king.name)
        /// </summary>
        public static LocalizedText ForEntity(
            string entityId)
        {
            return new LocalizedText
            {
                NameKey = $"entity.{entityId.ToLowerShared()}.name",
                DescriptionKey = $"entity.{entityId.ToLowerShared()}.description"
            };
        }

        #region Helper Extension
        private static string ToLowerShared(
            this string input)
        {
            return string.IsNullOrWhiteSpace(input) ? "unknown" : input.Trim().ToLowerInvariant();
        }
        #endregion
    }
}