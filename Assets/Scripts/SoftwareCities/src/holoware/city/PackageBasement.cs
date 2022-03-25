using System.Collections.Generic;
using System.Numerics;
using SoftwareCities.figures;
using SoftwareCities.holoware.lsm;

namespace SoftwareCities.holoware.city
{
    /// <summary>
    /// The package basement cuboid.
    /// </summary>
    public class PackageBasement : CityElement
    {
        private const int Spacing = 2;

        // cached bounds
        private Vector3 bounds;

        /// <summary>
        /// Constructs a package ui element.
        /// </summary>
        /// <param name="material">the material</param>
        /// <param name="lsmPackage">the lsm package</param>
        public PackageBasement(Material material, LsmPackage lsmPackage) : base(Position.Zero(), material, lsmPackage)
        {
            // construct lazy when all children are set and the bounds are known.
        }

        /// <summary>
        /// Layouts all children and sets the position.
        /// </summary>
        override
            public void Layout()
        {
            // Child positions
            float width = Spacing;
            float y = 1;

            foreach (List<CityElement> row in base.GetLsmRows())
            {
                // Layout in the middle
                float length = (GetBounds().X - GetLengthOfRow(row)) / 2;
                float withOfRow = 0;
                foreach (CityElement elem in row)
                {
                    Vector3 size = elem.GetBounds();
                    if (withOfRow < size.Z)
                    {
                        withOfRow = size.Z;
                    }

                    elem.position = Position.Xyz(length, y, width);
                    elem.Layout();
                    // next pos in row
                    length += size.X + Spacing;
                }

                width += withOfRow + Spacing;
            }

            // create the visible element
            Vector3 sz = this.GetBounds();
            AddChild(new Cuboid(Position.Zero(), material, sz.X, sz.Z, sz.Y));
        }

        /// <summary>
        /// Gets the Length of a LSM row.
        /// </summary>
        /// <param name="row">The list of rows.</param>
        /// <returns>The Length</returns>
        private float GetLengthOfRow(List<CityElement> row)
        {
            float length = 0;
            foreach (var next in row)
            {
                length += next.GetBounds().X;
            }

            if (row.Count > 0)
            {
                length = length + (row.Count - 1) * Spacing;
            }

            return length;
        }

        /// <summary>
        /// Gets the bounds of the Element
        /// </summary>
        /// <returns>the bounds.</returns>
        override
            public Vector3 GetBounds()
        {
            if (this.bounds.Length() != 0f) return bounds;
            
            // LSM Layout
            float width = 0;
            float length = 0;
            foreach (List<CityElement> row in GetLsmRows())
            {
                float maxLengthOfRow = 0;
                float maxWidthOfRow = 0;
                foreach (var elem in row)
                {
                    var b = elem.GetBounds();
                    if (b.Z > maxWidthOfRow)
                    {
                        maxWidthOfRow = b.Z;
                    }

                    maxLengthOfRow += b.X + Spacing;
                }

                width += maxWidthOfRow + Spacing;
                if (maxLengthOfRow > length)
                {
                    length = maxLengthOfRow;
                }
            }

            // inclusive spacing/insets
            bounds = new Vector3(length + Spacing, 1, width + Spacing); // Xx1xZ

            return bounds;
        }
    }
}