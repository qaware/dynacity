using System;

namespace SoftwareCities.figures
{
    /// <summary>
    /// The position.
    /// </summary>
    public sealed class Position
    {
        /// <summary>
        /// The coordinates.
        /// </summary>
        private readonly float x, y, z;

        /// <summary>
        /// Constant to represent the 0,0,0 origin.
        /// </summary>
        private static readonly Position zero = new Position(0, 0, 0);

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="z">z</param>
        private Position(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Construct a new position.
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <returns></returns>
        public static Position Xyz(float x, float y, float z)
        {
            return new Position(x, y, z);
        }

        /// <summary>
        /// Zero constant.
        /// </summary>
        public static Position Zero()
        {
            return zero;
        }

        /// <summary>
        /// Construct a new position.
        /// </summary>
        /// <param name="x">x</param>
        /// <returns>The new position</returns>
        public Position AddX(float x)
        {
            return Xyz(this.x + x, y, z);
        }

        public Position AddY(int y)
        {
            return Xyz(x, this.y + y, z);
        }

        public Position AddZ(int z)
        {
            return Xyz(x, y, this.z + z);
        }

        public float GetX()
        {
            return x;
        }

        public float GetY()
        {
            return y;
        }

        public float GetZ()
        {
            return z;
        }

        public Position Relative(Position delta)
        {
            return new Position(
                x + delta.x,
                y + delta.y,
                z + delta.z);
        }

        public Position Relative(int x, int y, int z)
        {
            return Relative(Xyz(x, y, z));
        }

        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            Position position = (Position) o;
            return x == position.x &&
                   y == position.y &&
                   z == position.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() + y.GetHashCode() + z.GetHashCode();
        }

        public override string ToString()
        {
            return "Position{" +
                   "x=" + x +
                   ", y=" + y +
                   ", z=" + z +
                   '}';
        }
    }
}