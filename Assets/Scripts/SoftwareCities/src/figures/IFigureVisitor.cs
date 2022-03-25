namespace SoftwareCities.figures
{
    /// <summary>
    /// Visits the figure parent/child tree.
    /// </summary>
    public interface IFigureVisitor
    {
        /// <summary>
        /// Figure
        /// </summary>
        /// <param name="figure"></param>
        void VisitFigureEnter(Figure figure);

        void VisitFigureLeave(Figure figure);

        /// <summary>
        /// Cuboid
        /// </summary>
        /// <param name="cuboid"></param>
        void VisitCuboidEnter(Cuboid cuboid);

        void VisitCuboidLeave(Cuboid cuboid);

        /// <summary>
        /// Cylinder
        /// </summary>
        /// <param name="cylinder"></param>
        void VisitCylinderEnter(Cylinder cylinder);

        void VisitCylinderLeave(Cylinder cylinder);

        /// <summary>
        /// Sphere
        /// </summary>
        /// <param name="sphere"></param>
        void VisitSphereEnter(Sphere sphere);

        void VisitSphereLeave(Sphere sphere);

        /// <summary>
        /// Block
        /// </summary>
        /// <param name="block"></param>
        void VisitBlockEnter(Block block);

        void VisitBlockLeave(Block block);
    }
}