namespace Domain.Common
{
    public class HSV
    {
        #region Attributes
        #endregion

        #region Properties
        public float H { get; set; }
        public float S { get; set; }
        public float V { get; set; }
        #endregion

        public HSV(
            float h,
            float s,
            float v)
        {
            H = h;
            S = s;
            V = v;
        }

        #region Methods
        #endregion
    }
}
