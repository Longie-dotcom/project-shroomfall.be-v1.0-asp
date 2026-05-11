using Domain.Definition.LocalizationDomain;
using Domain.Definition.WorldDomain.Enum;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.WorldDomain
{
    public class Tile
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public TileType Type { get; private set; }
        public LocalizedText LocalizedText { get; private set; }
        #endregion

        protected Tile() 
        {
        
        }

        public Tile(
            string id,
            TileType type,
            LocalizedText localizedText)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.Tile_InvalidId);

            if (string.IsNullOrWhiteSpace(localizedText.NameKey))
                throw new BadRequest(ResponseCode.Tile_InvalidName);

            if (string.IsNullOrWhiteSpace(localizedText.DescriptionKey))
                throw new BadRequest(ResponseCode.Tile_InvalidDescription);

            ID = id;
            Type = type;
            LocalizedText = localizedText;
        }

        #region Methods
        #endregion
    }
}