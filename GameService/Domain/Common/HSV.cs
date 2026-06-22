namespace Domain.Common
{
    public class HSV
    {
        #region Attributes
        #endregion

        #region Properties
        public float H { get; set; } = 0.0f;
        public float S { get; set; } = 0.0f;
        public float V { get; set; } = 0.0f;
        #endregion

        public HSV() 
        {
            H = 0.0f;
            S = 0.0f; 
            V = 0.0f;
        }

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
        public static HSV Clone(
            HSV hsv)
        {
            return new HSV(hsv.H, hsv.S, hsv.V);
        }
        #endregion
    }
}
