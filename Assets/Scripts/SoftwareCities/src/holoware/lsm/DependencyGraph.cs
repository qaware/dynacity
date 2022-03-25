using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SoftwareCities.holoware.lsm
{
    /// <summary>
    /// DependencyGraph stores the class dependency information in form a directed graph.
    /// </summary>    
    public sealed class DependencyGraph
    {
        /// <summary>
        /// Adjacence lists containing all links for a given name.
        /// </summary>          
        public Dictionary<string, HashSet<string>> src2dest = new Dictionary<string, HashSet<string>>(10000);

        /// <summary>
        /// Do not use this constructor. Use FromDot() instead.
        /// </summary>
        private DependencyGraph()
        {
        }

        /// <summary>
        /// Read a.dot file and build up the dependency graph.
        /// </summary>
        /// <param name="dotfile">A dot file stream.</param>
        /// <returns>The graph</returns>
        public static DependencyGraph FromDot(Stream dotfile)
        {
            DependencyGraph result = new DependencyGraph();
            StreamReader reader = new StreamReader(dotfile);
            result.ProcessInput(reader);
            return result;
        }

        /// <summary>
        /// Helper to read the content of a reader and build up the internal graph.
        /// </summary>
        /// <param name="reader">The reader</param>
        private void ProcessInput(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Regex regex = new Regex(@"""([\w+\.\$]*\w)""[ ]*-> *""([\w+\.\$]*\w{1})[ ]*.*""",
                    RegexOptions.IgnoreCase);
                Match match = regex.Match(line);
                // whole match, from class, to class 
                if (match.Groups.Count == 3)
                {
                    string source = match.Groups[1].Value;
                    string destination = match.Groups[2].Value;
                    AddLink(source, destination);
                }
                else
                {
                    Console.WriteLine("Unknown Input: " + line);
                }
            }
        }

        /// <summary>
        /// Adds a link S->T to the graph. The link is ignored if the graph contains already the link.
        /// </summary>
        /// <param name="source">Source name.</param>
        /// <param name="target">Target name.</param>
        public void AddLink(string source, string target)
        {
            if (src2dest.ContainsKey(source))
            {
                src2dest.TryGetValue(source, out HashSet<string> dests);
                // multiple references are allowed!
                dests.Add(target);
            }
            else
            {
                HashSet<string> destinations = new HashSet<string>();
                destinations.Add(target);
                src2dest.Add(source, destinations);
            }
        }

        /// <summary>
        /// Get all sources.
        /// </summary>
        /// <returns>The sources.</returns>
        public HashSet<string> GetSources()
        {
            return new HashSet<string>(src2dest.Keys);
        }

        /// <summary>
        /// Gets all targets for a given src name.
        /// </summary>
        /// <param name="src">The src name.</param>
        /// <returns>All targets.</returns>
        public HashSet<string> GetTargets(string src)
        {
            src2dest.TryGetValue(src, out HashSet<string> targets);
            if (targets == null)
            {
                return new HashSet<string>();
            }

            return targets;
        }

        /// <summary>
        /// List of all referencing sources for a given target (fan-in)
        /// </summary>
        /// <param name="target">The target name.</param>
        /// <returns>A set of source names.</returns>
        public HashSet<string> GetSources(string target)
        {
            HashSet<string> result = new HashSet<string>();
            HashSet<string> sources = GetSources();
            foreach (string source in sources)
            {
                HashSet<string> targets = GetTargets(source);
                if (targets.Contains(target))
                {
                    result.Add(source);
                }
            }

            return result;
        }

        /// <summary>
        /// string format.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            SortedSet<string> strings = new SortedSet<string>(src2dest.Keys);
            foreach (string next in strings)
            {
                builder.Append(next);
                builder.Append(": ");
                src2dest.TryGetValue(next, out HashSet<string> dests);
                builder.Append(dests);
                builder.Append("\n");
            }

            return builder.ToString();
        }
    }
}