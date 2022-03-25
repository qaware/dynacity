using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareCities.holoware.lsm
{
    /// <summary>
    /// Leaf in the LSM tree.
    /// </summary>
    public class LsmClass : LsmNode
    {
        public LsmClass(String name, String fullName, LsmNode parent) : base (name, fullName, parent)
        {
        }

        public override void Accept(ILsmVisitor v)
        {
            v.VisitClass(this);
        }

        public override bool IsCyclic(CycleDetector detector)
        {
            return detector.IsCyclic(GetFullName());
        }

        public override void BuildDependencies(DependencyGraph graph)
        {
            HashSet<string> myDeps = graph.GetTargets(this.GetFullName());
            LsmNode parentNode = this.GetParent(); // always works for classes -> a class should be in any package or root
            
            // Build dictionary for faster access to children
            Dictionary<string, LsmNode> ctxChildren = new Dictionary<string, LsmNode>();
            foreach (LsmNode child in parentNode.children)
            {
                ctxChildren.Add(child.GetFullName(), child);
            }
            
            Dictionary<string, Dependencies> result = new Dictionary<string, Dependencies>();
            
            IEnumerable<string> depsInContext = myDeps.Where(d => d.StartsWith(parentNode.GetFullName() + "."));
            foreach (string dep in depsInContext)
            {
                Dependencies targetDep;
                if (ctxChildren.ContainsKey(dep))
                {
                    targetDep = new Dependencies(ctxChildren[dep], GetFullName(), dep);
                }
                else
                { 
                    // subpackage dep -> add dep to package, not to class !!!
                    String subPath = dep.Substring(parentNode.GetFullName().Length + 1); 
                    String pckgName = subPath.Substring(0, subPath.IndexOf(".", StringComparison.Ordinal));
                    LsmNode package = ctxChildren[parentNode.GetFullName() + "." + pckgName];
                   
                    targetDep = new Dependencies(package, GetFullName(), dep);
                }  
                
                if (result.ContainsKey(targetDep.GetId()))
                {
                    Dependencies t = result[targetDep.GetId()];
                    t.AddDep(GetFullName(), dep);
                }
                else
                {
                    result.Add(targetDep.GetId(), targetDep);
                }
            }
            dependencies = result;
        }
    }
}