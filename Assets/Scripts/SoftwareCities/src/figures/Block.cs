using System;
using System.Numerics;

namespace SoftwareCities.figures
{
    /// <summary>
    /// A block is a cube where all dimensions are equal. Default scale is 1,1,1.
    /// </summary>
    public sealed class Block : Figure
    {
        public float Length => scale.X;

        public float Width => scale.Y;

        public float Height => scale.Z;

        /// <summary>
        /// Block constructs a block at position 0,0,0 with default material.
        /// </summary>
        /// <returns>A block</returns>
        public Block() : this(Position.Xyz(0, 0, 0), Material.Default)
        {
        }

        /// <summary>
        ///  Block constructs a block at position 0,0,0 with a given material.
        /// </summary>
        /// <param name="material">The material</param>
        /// <returns>A block</returns>
        public Block(Material material) : this(Position.Xyz(0, 0, 0), material)
        {
        }

        /// <summary>
        /// Block constructs a block at a position with a given material.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="material">The material</param>
        /// <returns>A block</returns>
        public Block(Position position, Material material) : base(position, material)
        {
            base.scale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Accept a visitor. The Accept method traverses the tree.
        /// </summary>
        /// <param name="visitor">A visitor.</param>
        override
            public void Accept(IFigureVisitor visitor)
        {
            visitor.VisitBlockEnter(this);
            foreach (Figure child in GetChildren())
            {
                child.worldpos = child.position.Relative(this.position); // adjust world pos hack - right place?
                child.Accept(visitor);
            }

            visitor.VisitBlockLeave(this);
        }

        /// <summary>Blocks are equal when they are on the same position.</summary>
        override
            public bool Equals(Object o)
        {
            return base.position.Equals(((Block) o).position);
        }

        /// <summary>Blocks are equal when they are on the same position.</summary>
        public override int GetHashCode()
        {
            return position.GetHashCode();
        }

        /// <summary>Return a string representation</summary>
        public override string ToString()
        {
            return "Block{" + base.ToString() + "}";
        }
    }
}