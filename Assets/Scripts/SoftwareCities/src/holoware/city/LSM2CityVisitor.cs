using System.Collections.Generic;
using SoftwareCities.figures;
using SoftwareCities.holoware.lsm;

namespace SoftwareCities.holoware.city
{ 
    /// <summary>
    /// LSMCityVisitor constructs the software city UI from an LsmNode tree.
    /// </summary>
    public class Lsm2CityVisitor : ILsmVisitor
    {
        /**
         * Stores the currentFigure on a stack.
         */
        private readonly Stack<CityElement> currentFigure;

        /**
         * The generated city. It is only set if we applied the visitor the the LsmNode.accept(visitor) method.
         */
        private CityElement city;

        /**
         * The dependency graph to calculate Width (fanIn), Length (fanOut) and Height (Sum(fanIn + fanOut).
         */
        private readonly DependencyGraph graph;

        /// <summary>
        /// The cycle detector to color cyclic components. 
        /// </summary>
        private readonly CycleDetector detector;

        /// <summary>
        /// Construct a visitor
        /// </summary>
        /// <param name="graph">the dependency graph</param>
        public Lsm2CityVisitor(DependencyGraph graph)
        {
            this.graph = graph;
            detector = CycleDetector.ForGraph(graph);
            currentFigure = new Stack<CityElement>();
        }

        public void VisitClass(LsmClass clazz)
        {
            ClassTower tower = new ClassTower(Material.PurpleNeon, clazz, graph);
            // red glass material if class is cyclic
            if (clazz.IsCyclic(detector))
            {
                tower.material = Material.PurpleNeon;
            }

            currentFigure.Peek().AddChild(tower);
        }

        public void VisitPackageEnter(LsmPackage pkg)
        {
            // set material based on package properties 
            Material material;
            if (pkg.IsCyclic(detector) && pkg.IsTopPackage())
            {
                material = Material.NeonFloor;
            }
            else if (!pkg.IsTopPackage())
            {
                material = Material.NeonFloor;
            }
            else
            {
                material = Material.NeonFloor;
            }

            var packageBasement = new PackageBasement(material, pkg);

            if (currentFigure.Count > 0)
            {
                currentFigure.Peek().AddChild(packageBasement);
            }

            currentFigure.Push(packageBasement);
        }

        public void VisitPackageLeave(LsmPackage pkg)
        {
            city = currentFigure.Pop(); // done with this figure
        }

        /// <summary>
        /// Gets the city after the visitor is complete run (with accept).
        /// </summary>
        /// <returns>The complete City.</returns>
        public CityElement GetResult()
        {
            city.Layout();
            return city;
        }
    }
}