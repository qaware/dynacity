namespace SoftwareCities.figures
{
    /// <summary>
    /// A Sphere object.
    /// </summary>
    public class Sphere : Figure
    {
        /// <summary>Accepts a quader.</summary>
        override
            public void Accept(IFigureVisitor visitor)
        {
            visitor.VisitSphereEnter(this);
            foreach (Figure child in GetChildren())
            {
                child.worldpos = child.position.Relative(this.position); // adjust world pos hack - right place?
                child.Accept(visitor);
            }

            visitor.VisitSphereLeave(this);
        }
    }
}