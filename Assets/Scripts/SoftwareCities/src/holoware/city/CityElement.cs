using System.Collections.Generic;
using System.Numerics;
using SoftwareCities.figures;
using SoftwareCities.holoware.lsm;

namespace SoftwareCities.holoware.city
{
    /// <summary>
    /// CityElement is the base class of all UI elements in the software city. 
    /// </summary>
    public abstract class CityElement : Figure
    {
        protected LsmNode lsmNode; // the corresponding LSM node.

        protected CityElement(Position position, Material material, LsmNode lsmNode) : base(lsmNode.GetFullName(), position,
            material)
        {
            this.lsmNode = lsmNode;
        }

        /// <summary>
        /// Construct the LSM row information from the level field.
        /// </summary>
        /// <returns>A list of rows</returns>
        protected List<List<CityElement>> GetLsmRows()
        {
            List<List<CityElement>> rows = new List<List<CityElement>>();
            foreach (Figure child in base.GetChildren())
            {
                if (!(child is CityElement))
                {
                    continue;
                }

                CityElement element = (CityElement) child;
                int level = element.lsmNode.GetLevel();
                while (rows.Count <= level)
                {
                    rows.Add(new List<CityElement>());
                }

                rows[level].Add(element); // add element to row
            }

            return rows;
        }

        /// <summary>
        /// Find an element or child by its full qualified LSM name.
        /// </summary>
        /// <param name="name">the name of the element</param>
        /// <returns>The found element or null if not found.</returns>
        public CityElement FindElement(string name)
        {
            if (name.Equals(this.lsmNode.GetFullName()))
            {
                return this;
            }

            foreach (Figure figure in this.GetChildren())
            {
                if (figure is CityElement)
                {
                    CityElement element = ((CityElement) figure).FindElement(name);
                    if (element != null)
                    {
                        return element;
                    }
                }
            }

            return null; // not found
        }

        /// <summary>
        /// Layout childrens. This methods should be called only when all childrens are added.
        /// </summary>
        public abstract void Layout();

        /// <summary>
        /// Get the bounds of this City Element.
        /// </summary>
        /// <returns>A vector with the x,y,z dimensions of this element.</returns>
        public abstract Vector3 GetBounds();
    }
}