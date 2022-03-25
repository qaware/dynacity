using System.Collections.Generic;

namespace SoftwareCities.unityadapter
{
    public class RenderQueue
    {
        private Queue<GameObjectProxy> queue = new Queue<GameObjectProxy>();

        public GameObjectProxy Take()
        {
            if (queue.Count > 0)
            {
                return queue.Dequeue();
            }

            return null;
        }

        public void Put(GameObjectProxy go)
        {
            queue.Enqueue(go);
        }
    }

    public class GameObjectProxy
    {
    }
}