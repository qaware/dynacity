using System;
using System.Collections.Generic;

namespace SoftwareCities.holoware.lsm
{
    public class LsmBuilder
    {
        private DependencyGraph graph;
        public LsmNode RootNode;

        public readonly List<KeyValuePair<string, string>> cycles = new List<KeyValuePair<string, string>>();

        private LsmBuilder(DependencyGraph dgraph)
        {
            graph = dgraph;
            RootNode = new LsmPackage("root", "root", null);
        }

        public static LsmBuilder LsmFromGraph(DependencyGraph graph)
        {
            LsmBuilder result = new LsmBuilder(graph);
            result.RootNode = result.Construct();
            return result;
        }

        /// <summary>
        /// The LsmFromGraph method constructs the LSM out of the dependency graph.
        /// </summary>
        /// <param fullQualifiedName="dependencies">The graph.</param>
        /// <returns>The root node.</returns>
        private LsmNode Construct()
        {
            // build the structure (packages contains packages or classes) of the LSM
            HashSet<string> sources = graph.GetSources();
            foreach (string source in sources)
            {
                AddElement(RootNode, source);
                HashSet<string> targets = graph.GetTargets(source);
                foreach (string target in targets)
                {
                    AddElement(RootNode, target);
                }
            }

            // Construct Dependencies
            ConstructDeps(RootNode);

            // setup the levels for later Layout
            Levelize(RootNode);
            return RootNode;
        }

        /// <summary>
        /// Build the relevant dependencies for the later LSM levelizing. 
        /// </summary>
        /// <param name="root">The root node.</param>
        private void ConstructDeps(LsmNode root)
        {
            root.BuildDependencies(graph);
        }

        /// <summary>
        /// Adds an element (class or package) to the LSM graph.
        /// </summary>
        /// <param fullQualifiedName="parent">The parent node.</param>
        /// <param fullQualifiedName="fullQualifiedName">The full qualified fullQualifiedName.</param>
        private void AddElement(LsmNode parent, String fullQualifiedName)
        {
            string[] names = fullQualifiedName.Split('.');
            string name = ""; // todo: calculate from parent (Add parent to LsmNode)
            for (int i = 0; i < names.Length; i++)
            {
                string s = names[i];

                LsmNode element;
                // assumes the last split in a dotted String is a class !
                if (i == names.Length - 1)
                {
                    name += "." + s; // assuming a class is in a package
                    element = new LsmClass(s, name, parent);
                }
                else
                {
                    if (name.Length == 0)
                    {
                        name = s;
                    }
                    else
                    {
                        name = name + "." + s;
                    }

                    element = new LsmPackage(s, name, parent);
                }

                if (!parent.children.Contains(element))
                {
                    parent.AddChild(element);
                    parent = element;
                }
                else
                {
                    parent = parent.children[parent.children.IndexOf(element)];
                }
            }
        }

        /// <summary>
        /// The LSM levelize algorithm traverses each node and sets the level appropriate to the dependencies.
        /// </summary>
        /// <param name="root">The root node</param>
        private void Levelize(LsmNode root)
        {
            // if the node has only one child, we're done
            if (root.children.Count == 1)
            {
                Levelize(root.children[0]);
                return;
            }

            if (root.children.Count <= 1) return;
            List<KeyValuePair<string, string>> removedDeps = CycleDetector.RemoveCyclicDependencies(root.children);
            if (removedDeps != null)
            {
                cycles.AddRange(removedDeps);
            }

            // calculate and set level for all children
            foreach (LsmNode child in root.children)
            {
                child.SetLevel(GetPathDepth(new FastStack<LsmNode>(), child));
            }

            // recursive descent for all children
            foreach (LsmNode child in root.children)
            {
                Levelize(child);
            }
        }


        /// <summary>
        /// Traverses node structure on their dependencies and returns the length of the maximum path until
        /// a cycle is detected or no outgoing dependencies are found.
        /// </summary>
        /// <param name="stack">The stack to store the current node path</param>
        /// <param name="node">The current node (not on the stack)</param>
        /// <returns></returns>
        private static int GetPathDepth(FastStack<LsmNode> stack, LsmNode node)
        {
            if (stack.Contains(node))
            {
                return -1; // cycle
            }

            // The stack contains the current path. Count is the current path length.
            int result = stack.Count();
            stack.Push(node);

            // Copy dependencies for later removal
            Dictionary<string, Dependencies> dependencies = new Dictionary<string, Dependencies>(node.dependencies);
            foreach (Dependencies dependency in dependencies.Values)
            {
                // optimize already visited nodes (level is already set)
                if (dependency.GetTarget().GetLevel() > 0)
                {
                    result = dependency.GetTarget().GetLevel() + 1;
                    continue;
                }

                if (node.Depends(dependency.GetTarget()) <= 0) continue;
                // The main concept: Get the max path from a given node into all other nodes in this LSM container
                int pathDepth = GetPathDepth(stack, dependency.GetTarget());
                if (pathDepth == -1)
                {
                    node.dependencies.Remove(dependency.GetId()); // remove cyclic dep: TODO Remove up dependencies
                }

                result = Math.Max(pathDepth, result);
            }

            stack.Pop();
            return result;
        }
    }
}