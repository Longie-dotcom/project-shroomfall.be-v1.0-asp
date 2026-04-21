using Domain.AttributeDomain.Enum;

namespace Domain.AttributeDomain.Attribute
{
    public class Stats
    {
        #region Attributes
        private readonly Dictionary<Parameter, float> values;
        #endregion

        #region Properties
        public string ID { get; private set; }
        public int Tier { get; private set; }
        #endregion

        public Stats(
            string id,
            int tier,
            Dictionary<Parameter, float> values)
        {
            ID = id;
            Tier = tier;
            this.values = values;
        }

        #region Methods
        public float Get(Parameter parameter)
        {
            return values.TryGetValue(parameter, out var value) ? value : 0f;
        }

        public void Add(Parameter parameter, float value)
        {
            values[parameter] = Get(parameter) + value;
        }

        public void Set(Parameter parameter, float value)
        {
            values[parameter] = value;
        }

        public Dictionary<Parameter, float> GetAll()
        {
            return values;
        }
        #endregion
    }
}
