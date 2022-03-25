using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoftwareCities.figures
{
    /// <summary>
    /// Base class for all figures (Cuboid, Cylinder, Block ...).
    /// </summary>
    public class Figure
    {
        /// <summary>Each figure has a label. Default is the class name.</summary>
        public string label { get; set; }

        /// <summary>The (local) position.</summary>
        public Position position { get; set; } = Position.Zero();

        /// <summary>The world position.</summary>
        public Position worldpos { get; set; } = Position.Zero();

        /// <summary>The scale</summary>
        public Vector3 scale { get; set; } = new Vector3(1, 1, 1);

        /// <summary>Optional rotation.</summary>
        public Rotation rotation { get; set; }

        /// <summary>Material</summary>
        public Material material { get; set; }

        /// <summary>The list of children.</summary>
        private readonly List<Figure> children = new List<Figure>();

        /// <summary>Parent</summary>
        public Figure parent { get; set; }

        /// <summary>A empty figure</summary>
        public Figure() : this(Position.Zero(), Material.Default)
        {
        }

        /// <summary>A empty figure</summary>
        public Figure(Position position) : this()
        {
        }

        /// <summary>Figure at a position an a material</summary>
        public Figure(Position position, Material material)
        {
            this.label = GetType().Name;
            this.position = position;
            this.material = material;
        }

        /// <summary>Figure with a label.</summary>
        public Figure(string label, Position position, Material material) : this(position, material)
        {
            this.label = label;
        }

        /// <summary>Figure with a label.</summary>
        public Matrix4x4 GetTransformation()
        {
            Matrix4x4 dreh = rotation.GetTransform();
            Matrix4x4 trans =
                Matrix4x4.CreateTranslation(new Vector3(position.GetX(), position.GetY(), position.GetZ()));
            Matrix4x4 ziel = Matrix4x4.Multiply(trans, dreh);
            return ziel;
        }

        /// <summary>returns the list childrens.</summary>
        public List<Figure> GetChildren()
        {
            return children;
        }

        /// <summary>Add a child.</summary>
        public void AddChild(Figure kind)
        {
            children.Add(kind);
            kind.parent = this;
        }

        /// <summary>
        /// Updates the origin and updates the world positions. Should be called before the visitor runs.
        /// </summary>
        /// <param name="origin"></param>
        public void UpdateOrigin(Position origin)
        {
            worldpos = origin.Relative(position);
            foreach (Figure child in children)
            {
                child.UpdateOrigin(worldpos);
            }
        }

        /// <summary>Traverses the child tree with an visitor.</summary>
        virtual
            public void Accept(IFigureVisitor visitor)
        {
            visitor.VisitFigureEnter(this);
            foreach (Figure child in children)
            {
                child.Accept(visitor);
            }

            visitor.VisitFigureLeave(this);
        }

        /// <summary>Equality based on position.</summary>
        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            Figure figur = (Figure) o;
            return Equals(position, figur.position);
        }

        /// <summary>HashCode based on position.</summary>
        public override int GetHashCode()
        {
            return position.GetHashCode();
        }

        public override string ToString()
        {
            return "Figure {" +
                   "label=" + label +
                   "position=" + position +
                   "scale=" + scale +
                   ", material=" + material +
                   ", parent=" + parent +
                   "}";
        }
    }
}