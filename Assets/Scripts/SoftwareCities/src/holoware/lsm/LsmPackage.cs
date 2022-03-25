using System.Collections.Generic;
using System.Linq;

namespace SoftwareCities.holoware.lsm
{
    /// <summary>
    /// LSMPackage represents a package element in the LsmNode tree structure.
    /// </summary>
    public class LsmPackage : LsmNode
    {
        public LsmPackage(string name, string fullName, LsmNode parent) : base(name, fullName, parent)
        {
        }

        /// <summary>
        /// Accepts a visitor and calls v.VisitPackageEnter/v.VisitPackageLeave before and after child processing.
        /// </summary>
        /// <param name="v">The visitor</param>
        public override void Accept(ILsmVisitor v)
        {
            v.VisitPackageEnter(this);
            foreach (var c in children)
            {
                c.Accept(v);
            }

            v.VisitPackageLeave(this);
        }

        /// <summary>
        /// Test if this package is on top (the last package containing only classes. 
        /// </summary>
        /// <returns></returns>
        public bool IsTopPackage()
        {
            return !children.OfType<LsmPackage>().Any();
        }

        public override bool IsCyclic(CycleDetector detector)
        {
            return children.Any(next => next.IsCyclic(detector));
        }

        public override void BuildDependencies(DependencyGraph graph)
        {
            dependencies = ConstructPackageDependencies(graph);
            foreach (LsmNode child in children)
            {
                child.BuildDependencies(graph);
            }
        }
        
        /// <summary>
        /// Construct package dependencies.
        /// </summary>
        /// <param name="graph">The basic</param>
        /// <returns></returns>
        private Dictionary<string, Dependencies> ConstructPackageDependencies(DependencyGraph graph)
        {
            if (parent == null)
            {
                return null; // ignore for "root"
            }
            
            Dictionary<string, Dependencies> result = new Dictionary<string, Dependencies>();
            
            string myPackageName = this.GetFullName();
            List<string> sourcesInsidePackage = graph.GetSources().Where(s => s.StartsWith(myPackageName)).ToList();
            foreach (string source in sourcesInsidePackage)
            {
                HashSet<string> targets = graph.GetTargets(source);
                foreach (string target in targets)
                {
                    if (target.StartsWith(myPackageName))
                    {
                        continue; // ignore dependencies inside my package
                    }

                    foreach (LsmNode child in parent.children)
                    {
                        if (target.StartsWith(child.GetFullName()))
                        {
                            Dependencies d = new Dependencies(child, source, target);
                            if (result.ContainsKey(d.GetId()))
                            {
                                result[d.GetId()].AddDep(source, target);
                            }
                            else
                            {
                                result.Add(d.GetId(), d);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}