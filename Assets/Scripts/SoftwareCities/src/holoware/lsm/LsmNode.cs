using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SoftwareCities.holoware.lsm
{
    /// <summary>
    /// BaseClass for LsmPackage / LsmClass.
    /// </summary>
    public abstract class LsmNode
    {
        private readonly string name;
        private readonly string fullName;
        private int level;
        internal readonly List<LsmNode> children;
        internal Dictionary<string, Dependencies> dependencies;
        protected readonly LsmNode parent;

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="name">The short name.</param>
        /// <param name="fullName">The complete dot separated name.</param>
        /// <param name="parent">The parent node or null for the root</param>
        protected LsmNode(string name, string fullName, LsmNode parent)
        {
            children = new List<LsmNode>();
            dependencies = new Dictionary<string, Dependencies>();
            this.parent = parent;
            this.name = name;
            this.fullName = fullName;
            level = 0;
        }

        /// <summary>
        /// Gets the row number for the LSM layout. Level 1 is above Level 2 and so on.
        /// </summary>
        /// <returns>The level.</returns>
        public int GetLevel()
        {
            return level;
        }

        /// <summary>
        /// Sets the level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void SetLevel(int level)
        {
            this.level = level;
        }
        
        /// <summary>
       /// Gets the full qualified name (dotted).
       /// </summary>
       /// <returns>The name.</returns>
        public string GetFullName()
        {
            return fullName;
        }

       /// <summary>
       /// Gets the short name. java.lang.String -> String.
       /// </summary>
       /// <returns>The short name.</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Adds a child dep.
        /// </summary>
        /// <param name="node">The dep.</param>
        public void AddChild(LsmNode node)
        {
            children.Add(node);
        }

        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            var lsmNode = (LsmNode) o;
            return name.Equals(lsmNode.name);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <summary>
        /// LsmNodes support a LsmVisitor.
        /// </summary>
        /// <param name="v">The visitor.</param>
        public abstract void Accept(ILsmVisitor v);

        public override String ToString()
        {
            return "LsmNode{" +
                   "name='" + name + '\'' +
                   ", level=" + level +
                   ", children=" + children +
                   '}';
        }

        /// <summary>
        /// Count graph between this and another dep which could be a class or a package.
        /// </summary>
        /// <param name="nodeB">The other dep.</param>
        /// <param name="graph">The graph.</param>
        /// <returns></returns>
        public int Depends(LsmNode other)
        {
            if (dependencies.ContainsKey(other.GetFullName()))
            {
                return dependencies[other.GetFullName()].GetCount();
            }
            return 0;
        }

        /// <summary>
        /// Checks if the dep is cyclic.
        /// </summary>
        /// <param name="detector">the cycle detector</param>
        /// <returns>if the dep is cyclic</returns>
        public abstract bool IsCyclic(CycleDetector detector);

        /// <summary>
        /// Helper to find a child dep (or itself) by its name.
        /// </summary>
        /// <param name="fullyQualifiedName">The full name.</param>
        /// <returns>A dep or null if not found.</returns>
        public LsmNode GetByName(String fullyQualifiedName)
        {
            if (fullName.Equals(fullyQualifiedName))
            {
                return this;
            }
            return children.Select(next => next.GetByName(fullyQualifiedName)).FirstOrDefault(result => result != null);
        }

        /// <summary>
        /// Gets all dependencies for a given dep.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Dependencies> GetDependencies()
        {
            return dependencies;
        }
        

        /// <summary>
        /// Get the parent node.
        /// </summary>
        /// <returns>The parent node or null for the root node.</returns>
        public LsmNode GetParent()
        {
            return parent;
        }

        /// <summary>
        /// Build dependency information from the Dependency Graph.
        /// </summary>
        /// <param name="graph"></param>
        public abstract void BuildDependencies(DependencyGraph graph);
    }
}