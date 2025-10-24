using System.Collections.Generic;

namespace Spawning.Pooling
{
    public class MultiPool<ID, T>
    {
        protected Dictionary<ID, Stack<T>> multiPool = new();
        public T Get(ID id)
        {
            if (multiPool.TryGetValue(id, out var stack))
            {
                if (stack.Count > 0)
                {
                    return stack.Pop();
                }
            }
            return default;
        }
        public void Push(ID id, T item)
        {
            Stack<T> stack;
            if (!multiPool.TryGetValue(id, out stack))
            {
                stack = new();
                multiPool.Add(id, stack);
            }
            stack.Push(item);
        }
    }
}