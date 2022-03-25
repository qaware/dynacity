using System.Collections.Generic;

namespace SoftwareCities.holoware.lsm
{
    /// <summary>
    /// Represent Dependencies to a target node which could be a Package or a single Class.
    /// </summary>
    public class Dependencies
    {
        private readonly LsmNode target;
        private int count;
        private Dictionary<string, List<string>> s2t = new Dictionary<string, List<string>>();

        public Dependencies(LsmNode target, string from, string to)
        {
            this.target = target;
            AddDep(from, to);
        }

        public void AddDep(string from, string to)
        {
            if (s2t.ContainsKey(from))
            {
                s2t[from].Add(to);
            }
            else
            {
                List<string> targetList = new List<string> {to};
                s2t.Add(from, targetList);
            }
            count++;
        }

        public LsmNode GetTarget()
        {
            return target;
        }

        public string GetId()
        {
            return target.GetFullName();
        }
        
        public int GetCount()
        {
            return count;
        }

        public override bool Equals(object o)
        {
            return target.Equals(((Dependencies) o)?.target);
        }

        public override int GetHashCode()
        {
            return target.GetHashCode();
        }
    }
}