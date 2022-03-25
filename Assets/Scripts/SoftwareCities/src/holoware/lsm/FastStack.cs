using System.Collections.Generic;

namespace SoftwareCities.holoware.lsm
{
    /// <summary>
    /// FastStack is a Stack with a fast Contains() method by using an internal HashSet.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FastStack<T>
    {
        private readonly HashSet<T> index = new HashSet<T>();
        private readonly Stack<T> stack = new Stack<T>();

        public void Push(T t)
        {
            stack.Push(t);
            index.Add(t);
        }

        public T Pop()
        {
            T element = stack.Pop();
            index.Remove(element);
            return element;
        }

        public bool Contains(T t)
        {
            return index.Contains(t);
        }

        public int Count()
        {
            return stack.Count;
        }
    }
}