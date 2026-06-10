namespace Domain.Common
{
    public class Vector2
    {
        #region Attributes
        public static Vector2 Zero = new Vector2(0, 0);
        #endregion

        #region Properties
        public float X { get; set; }
        public float Y { get; set; }
        #endregion

        public Vector2(
            float x,
            float y)
        {
            X = x;
            Y = y;
        }

        #region Methods
        // ─────────────────────────────
        // Operators
        // ─────────────────────────────
        public static Vector2 operator +(
            Vector2 a, 
            Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(
            Vector2 a, 
            Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(
            Vector2 v, 
            float scalar)
        {
            return new Vector2(v.X * scalar, v.Y * scalar);
        }

        public bool NearlyEquals(Vector2 other, float epsilon = 0.001f)
        {
            return MathF.Abs(X - other.X) < epsilon &&
                   MathF.Abs(Y - other.Y) < epsilon;
        }

        public static Vector2 operator +(
            float scalar,
            Vector2 v)
        {
            return new Vector2(v.X + scalar, v.Y + scalar);
        }

        public static Vector2 operator +(
            Vector2 v,
            float scalar)
        {
            return new Vector2(v.X + scalar, v.Y + scalar);
        }

        // ─────────────────────────────
        // Length / Magnitude
        // ─────────────────────────────
        public float Length()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y;
        }

        // ─────────────────────────────
        // Normalize (returns new vector)
        // ─────────────────────────────
        public static Vector2 Normalize(
            Vector2 v)
        {
            float length = MathF.Sqrt(v.X * v.X + v.Y * v.Y);

            if (length == 0f)
                return Zero;

            return new Vector2(v.X / length, v.Y / length);
        }

        // ─────────────────────────────
        // Normalize (in-place)
        // ─────────────────────────────
        public void Normalize()
        {
            float length = MathF.Sqrt(X * X + Y * Y);

            if (length == 0f)
                return;

            X /= length;
            Y /= length;
        }
        #endregion
    }
}
