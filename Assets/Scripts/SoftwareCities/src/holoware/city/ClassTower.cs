using System.Collections.Generic;
using System.Numerics;
using SoftwareCities.figures;
using SoftwareCities.holoware.lsm;

namespace SoftwareCities.holoware.city
{
    /// <summary>
    /// The tower represents a class in the software city.
    /// </summary>
    public class ClassTower : CityElement
    {
        private int fanOut = 0;
        private int fanIn = 0;

        private DependencyGraph graph;

        /// <summary>
        /// Construct a class tower.
        /// Visible tower element is constructed lazy after the while package tree is build during the Layout() method.
        /// </summary>
        /// <param name="material">A material.</param>
        /// <param name="lsmClass">The representing class in the LSM structure.</param>
        /// <param name="graph">The dependency graph.</param>
        /// <returns></returns>
        public ClassTower(Material material, LsmClass lsmClass, DependencyGraph graph) : base(Position.Zero(), material,
            lsmClass)
        {
            this.graph = graph;
            CalculateSize();
            // constructed lazy
        }

        public override void Layout()
        {
            int height = fanIn;
            int width = fanOut;
            int length = fanOut;

            AddChild(new Cuboid(Position.Zero(), material, length, width, height));
        }

        private void CalculateSize()
        {
            HashSet<string> fanOut = graph.GetTargets(lsmNode.GetFullName());
            HashSet<string> fanIn = graph.GetSources(lsmNode.GetFullName());
            this.fanIn = fanIn.Count + 1;
            this.fanOut = fanOut.Count + 1;
        }

        public override Vector3 GetBounds()
        {
            return new Vector3(fanOut, fanIn, fanOut);
        }
    }
}