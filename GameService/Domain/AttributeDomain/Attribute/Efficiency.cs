using Domain.AttributeDomain.Enum;

namespace Domain.AttributeDomain.Attribute
{
    public class Efficiency
    {
        #region Attributes
        private readonly Dictionary<Parameter, float> values;
        #endregion

        #region Properties
        public string ID { get; private set; }
        #endregion

        public Efficiency(
            string id,
            Dictionary<Parameter, float> values)
        {
            ID = id;
            this.values = values;
        }

        #region Methods
        public float Get(Parameter parameter)
        {
            return values.TryGetValue(parameter, out var value) ? value : 0f;
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
