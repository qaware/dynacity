namespace SoftwareCities.holoware.lsm
{
    /// <summary>
    /// The visitor enabled decoupled traversal of the LsmNode tree structure.
    /// It is used for the later generation of the software city. It must be implemented and can be applied to the
    /// LsmNode.accept(visitor) method. This will traverse the whole tree structure and call for each node the appropriate
    /// callbacks (see below).
    /// </summary>
    public interface ILsmVisitor
    {
        /// <summary>
        /// This method will be called for every LsmClass object in the LsmNode tree.
        /// </summary>
        /// <param name="clazz">The clazz.</param>
        void VisitClass(LsmClass clazz);

       /// <summary>
       /// VisitPackageEnter is called before the recursive descent of a package. If your VisitClass method required
       /// knowledge about the current path in the tree, it is common to use a stack on which packages are
       /// pushed during VisitPackageEnter() and popped during VisitPackageLeave(). -> Known as hierarchical visitor pattern.
       /// </summary>
       /// <param name="pkg">The package.</param>
        void VisitPackageEnter(LsmPackage pkg);

        /// <summary>
        /// VisitPackageLeave is called after the recursive descent into the content of the package.
        /// </summary>
        /// <param name="pkg">The package.</param>
       void VisitPackageLeave(LsmPackage pkg);
    }
}