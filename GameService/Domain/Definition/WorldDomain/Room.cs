using Contract.Enum.WorldDomain;
using Domain.Definition.LocalizationDomain;
using Domain.DomainException;
using Domain.Shared;

namespace Domain.Definition.WorldDomain
{
    public class Room
    {
        #region Attributes
        #endregion

        #region Properties
        public string ID { get; private set; }
        public RoomType Type { get; private set; }
        public LocalizedText LocalizedText { get; private set; }

        public ICollection<Cell> Cells {  get; private set; } = new List<Cell>();
        public ICollection<EntitySpawnRule> EntitySpawnRules { get; private set; } = new List<EntitySpawnRule>();
        #endregion

        protected Room() 
        { 
        
        }

        public Room(
            string id,
            RoomType type,
            LocalizedText localizedText)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequest(ResponseCode.Room_InvalidId);

            if (string.IsNullOrWhiteSpace(localizedText.NameKey))
                throw new BadRequest(ResponseCode.Room_InvalidName);

            if (string.IsNullOrWhiteSpace(localizedText.DescriptionKey))
                throw new BadRequest(ResponseCode.Room_InvalidDescription);

            ID = id;
            Type = type;
            LocalizedText = localizedText;
        }

        #region Methods
        #endregion
    }
}
