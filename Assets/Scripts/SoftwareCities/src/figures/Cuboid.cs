using System.Numerics;

namespace SoftwareCities.figures
{
    /// <summary>
    /// Cuboid is a cube with seperate scalings for Length, Width and Height.
    /// </summary>
    public class Cuboid : Figure
    {
        public float length => scale.X;

        public float width => scale.Z;

        public float height => scale.Y;

        /// <summary>Creates a Cuboid with a default material at position 0,0,0.</summary>
        public Cuboid(float length, float width, float height) : this(Position.Zero(), Material.Default, length, width,
            height)
        {
        }

        /// <summary>Creates a Cuboid at a given position with a given material.</summary>
        public Cuboid(Position position, Material material, float length, float width, float height) : this("Cuboid",
            position, material, length, width, height)
        {
        }

        /// <summary>Creates a Cuboid with a custom label.</summary>
        public Cuboid(string label, Position position, Material material, float length, float width, float height) :
            base(label, position, material)
        {
            base.scale = new Vector3(length, height, width);
        }

        /// <summary>Accepts a quader.</summary>
        override
            public void Accept(IFigureVisitor visitor)
        {
            visitor.VisitCuboidEnter(this);
            foreach (var child in GetChildren())
            {
                child.worldpos = child.position.Relative(this.position); // adjust world pos hack - right place?
                child.Accept(visitor);
            }

            visitor.VisitCuboidLeave(this);
        }

        public override string ToString()
        {
            return "Cuboid{" +
                   "Length=" + length +
                   ", Width=" + width +
                   ", Height=" + height + ", " + base.ToString() + " " +
                   '}';
        }
    }
}