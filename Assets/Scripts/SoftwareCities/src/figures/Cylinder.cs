using System.Numerics;

namespace SoftwareCities.figures
{
    /// <summary>
    /// A cylinder. 
    /// </summary>
    public class Cylinder : Figure
    {
        public float Length
        {
            get { return base.scale.X; }
        }

        public float Width
        {
            get { return base.scale.Z; }
        }

        public float Heigth
        {
            get { return base.scale.Y; }
        }

        /// <summary>Creates a cylinder.</summary>
        public Cylinder(string label, Position position, Material material, float length, float width, float height) :
            base(label, position, material)
        {
            base.scale = new Vector3(length, height, width);
        }

        /// <summary>Accepts a visitor.</summary>
        override
            public void Accept(IFigureVisitor visitor)
        {
            visitor.VisitCylinderEnter(this);
            foreach (Figure child in GetChildren())
            {
                child.worldpos = child.position.Relative(this.position); // adjust world pos hack - right place?
                child.Accept(visitor);
            }

            visitor.VisitCylinderLeave(this);
        }

        public override string ToString()
        {
            return "Cylinder{" +
                   "Length=" + Length +
                   ", Width=" + Width +
                   ", Height=" + Heigth + ", " + base.ToString() + " " +
                   '}';
        }
    }
}