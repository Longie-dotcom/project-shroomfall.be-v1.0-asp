using MongoDB.Driver;

namespace Infrastructure.Persistence
{
    public class GameDBContext
    {
        #region Attributes
        private readonly IMongoDatabase database;
        #endregion

        #region Properties
        public IMongoDatabase Database
        {
            get { return database; }
        }
        #endregion

        public GameDBContext(
            IMongoDatabase database)
        {
            this.database = database;
        }

        #region Methods
        public IMongoCollection<T> GetCollection<T>(
            string name)
        {
            return database.GetCollection<T>(name);
        }
        #endregion
    }
}