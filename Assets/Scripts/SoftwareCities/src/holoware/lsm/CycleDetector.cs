using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareCities.holoware.lsm
{
    /// <summary>
    /// Detects cycles in a graph using the Tarjan algorithm. 
    /// <see href="https://en.wikipedia.org/wiki/Tarjan%27s_strongly_connected_components_algorithm>"/>
    /// <see href="https://rosettacode.org/wiki/Tarjan"/> 
    /// </summary>
    public class CycleDetector
    {
        /// <summary>
        /// the tarjan graph nodes for the given graph
        /// </summary>
        private readonly HashSet<GraphNode> graphNodes = new HashSet<GraphNode>();

        /// <summary>
        /// the dependencies 
        /// </summary>
        private readonly Dictionary<GraphNode, HashSet<GraphNode>>
            dependencies = new Dictionary<GraphNode, HashSet<GraphNode>>();

        /// <summary>
        /// The detected cycles for a given dependency graph 
        /// </summary>
        public readonly HashSet<List<string>> cycles = new HashSet<List<string>>();


        /// <summary>
        /// Generates a cycle detector for the given dependency graph. 
        /// </summary>
        /// <param name="dependencyGraph">the dependency graph to be analyzed</param>
        /// <returns>the generated cycle detector</returns>
        public static CycleDetector ForGraph(DependencyGraph dependencyGraph)
        {
            CycleDetector cycleDetector = new CycleDetector();
            GenerateTarjanGraph(dependencyGraph, cycleDetector);
            cycleDetector.FindScCs();
            return cycleDetector;
        }

        /// <summary>
        /// Detect cycles within the given nodes. Needed to remove cyclic dependencies before building the LSM. 
        /// </summary>
        /// <param name="nodes">to detect cycles in</param>
        /// <returns>the cycle detector holding the cycles</returns>
        public static List<KeyValuePair<string, string>> RemoveCyclicDependencies(List<LsmNode> nodes)
        {
            CycleDetector cycleDetector = new CycleDetector();
            List<KeyValuePair<string, string>> removedDeps = new List<KeyValuePair<string, string>>();
            GenerateTarjanGraph(nodes, cycleDetector);
            cycleDetector.FindScCs();
            if (cycleDetector.cycles.Count <= 0) return null;
            foreach (List<KeyValuePair<LsmNode, LsmNode>> removalCandidates in cycleDetector.cycles.Select(cycle =>
                CycleRemovalFirstHeuristics(nodes, cycle)))
            {
                if (removalCandidates.Count == 1)
                {
                    RemoveDependency(removalCandidates.First().Key, removalCandidates.First().Value);
                    removedDeps.Add(new KeyValuePair<string, string>(removalCandidates.First().Key.GetFullName(),
                        removalCandidates.First().Value.GetFullName()));
                }
                else
                {
                    // Second heuristics: Remove the dependency of the node with the least outgoing edges 
                    (LsmNode key, LsmNode value) = removalCandidates.Aggregate((l, r) =>
                        l.Key.dependencies.Count < r.Key.dependencies.Count ? l : r);
                    RemoveDependency(key, value);
                    removedDeps.Add(new KeyValuePair<string, string>(key.GetFullName(), value.GetFullName()));
                }
            }

            List<KeyValuePair<string, string>> removeCyclicDependencies = RemoveCyclicDependencies(nodes);
            if (removeCyclicDependencies != null)
            {
                removedDeps.AddRange(removeCyclicDependencies);
            }

            return removedDeps;
        }

        /// <summary>
        /// Remove the dependency between the two nodes. 
        /// </summary>
        /// <param name="from">from node</param>
        /// <param name="to">to node</param>
        private static void RemoveDependency(LsmNode from, LsmNode to)
        {
            Dictionary<string, Dependencies> fromDependencies = from.dependencies;
            from.dependencies = (from kv in fromDependencies
                where !kv.Key.Contains(to.GetFullName())
                select kv).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        /// <summary>
        /// First heuristics: Remove edge with the least weight. 
        /// </summary>
        /// <param name="parent">holding the child nodes containing a cycle</param>
        /// <param name="cycle">the actual cycle</param>
        /// <returns>the removal candidates</returns>
        private static List<KeyValuePair<LsmNode, LsmNode>> CycleRemovalFirstHeuristics(List<LsmNode> nodes,
            IReadOnlyList<string> cycle)
        {
            List<KeyValuePair<LsmNode, LsmNode>> removalCandidates = new List<KeyValuePair<LsmNode, LsmNode>>();

            int minDepCount = int.MaxValue;
            for (int j = 0; j < cycle.Count; j++)
            {
                LsmNode from = nodes.Find(node => node.GetFullName().Equals(cycle[j]));
                LsmNode to = nodes.Find(node =>
                    node.GetFullName().Equals(cycle[j + 1 < cycle.Count ? j + 1 : 0]));
                if (from == null || to == null || from.Equals(to)) continue;
                int depCount = from.Depends(to);
                if (depCount < minDepCount && depCount > 0)
                {
                    removalCandidates = new List<KeyValuePair<LsmNode, LsmNode>>
                    {
                        new KeyValuePair<LsmNode, LsmNode>(from, to)
                    };
                    minDepCount = depCount;
                }
                else if (depCount == minDepCount)
                {
                    removalCandidates.Add(new KeyValuePair<LsmNode, LsmNode>(from, to));
                }
            }

            return removalCandidates;
        }

        /// <summary>
        /// Generates a Tarjan graph with the internal GraphNode class.
        /// We only need this for cycle detection so we don't put the index stuff in the original dependency graph. 
        /// </summary>
        /// <param name="nodes">the nodes to analyze for cycles</param>
        /// <param name="detector">the cycle detector to be generated for this graph</param>
        private static void GenerateTarjanGraph(List<LsmNode> nodes, CycleDetector detector)
        {
            foreach (LsmNode node in nodes)
            {
                HashSet<GraphNode> dests = new HashSet<GraphNode>(node.dependencies.Keys
                    .Where(dep => nodes.Exists(n => n.GetFullName().Equals(dep))).Select(s => new GraphNode(s)));
                GraphNode src = new GraphNode(node.GetFullName());
                detector.dependencies.Add(src, dests);
                detector.graphNodes.Add(src);
            }
        }

        /// <summary>
        /// Generates a Tarjan graph with the internal GraphNode class.
        /// We only need this for cycle detection so we don't put the index stuff in the original dependency graph. 
        /// </summary>
        /// <param name="graph">the dependency graph</param>
        /// <param name="detector">the cycle detector to be generated for this graph</param>
        private static void GenerateTarjanGraph(DependencyGraph graph, CycleDetector detector)
        {
            foreach ((string key, HashSet<string> value) in graph.src2dest)
            {
                HashSet<GraphNode> dests = new HashSet<GraphNode>();
                foreach (GraphNode dest in value.Select(s => new GraphNode(s)))
                {
                    dests.Add(dest);
                    detector.graphNodes.Add(dest);
                }

                GraphNode src = new GraphNode(key);
                detector.dependencies.Add(src, dests);
                detector.graphNodes.Add(src);
            }
        }

        /// <summary>
        /// Find cycles via Tarjan's strongly connected components algorithm.
        /// SCCs with more than one node are cycles. 
        /// </summary>
        private void FindScCs()
        {
            int index = 0; // number of nodes
            FastStack<GraphNode> s = new FastStack<GraphNode>();
            //Stack<GraphNode> s = new Stack<GraphNode>(); // slow !

            void StrongConnect(GraphNode v)
            {
                // Set the depth index for v to the smallest unused index
                v.Index = index;
                v.LowLink = index;

                index++;
                s.Push(v);
                // Consider successors of v
                if (dependencies.TryGetValue(v, out HashSet<GraphNode> adjSet))
                {
                    foreach (GraphNode w in adjSet)
                    {
                        if (w.Index < 0)
                        {
                            // Successor w has not yet been visited; recurse on it
                            StrongConnect(w);
                            v.LowLink = Math.Min(v.LowLink, w.LowLink);
                        }
                        else if (s.Contains(w))
                            // Successor w is in stack S and hence in the current SCC
                            v.LowLink = Math.Min(v.LowLink, w.Index);
                    }
                }

                // If v is a root node, pop the stack and generate an SCC
                if (v.LowLink == v.Index)
                {
                    List<string> cycle = new List<string>();
                    GraphNode w;
                    do
                    {
                        w = s.Pop();
                        // add to the beginning to keep the correct order
                        cycle.Insert(0, w.Name);
                    } while (!Equals(w, v));

                    // SCC with more than one component is a cycle 
                    if (cycle.Count > 1)
                    {
                        cycles.Add(cycle);
                    }
                }
            }

            foreach (GraphNode v in graphNodes.Where(v => v.Index < 0))
                StrongConnect(v);
        }

        /// <summary>
        /// Indicates whether the given class is cyclic 
        /// </summary>
        /// <param name="name">the name of the class</param>
        /// <returns>If the given class is in a cycle or not</returns>
        public bool IsCyclic(string name)
        {
            return cycles.Any(cycle => cycle.Contains(name));
        }

        /// <summary>
        /// Internal graph node class for Tarjan algorithm. 
        /// </summary>
        private class GraphNode : IComparable<GraphNode>
        {
            public int LowLink { get; set; }
            public int Index { get; set; }
            public string Name { get; }

            public GraphNode(string name)
            {
                Name = name;
                Index = -1;
                LowLink = 0;
            }

            public override bool Equals(object obj)
            {
                if (this == obj) return true;
                if (obj == null || GetType() != obj.GetType()) return false;
                GraphNode graphNode = (GraphNode) obj;
                return Name.Equals(graphNode.Name);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            public int CompareTo(GraphNode other)
            {
                if (other == null)
                {
                    return 1;
                }

                return Name.CompareTo(other.Name);
            }
        }
    }

    /// <summary>
    /// This is necessary for older .NET versions
    /// </summary>
    static class KvpExtensions
    {
        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> kvp,
            out TKey key,
            out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}