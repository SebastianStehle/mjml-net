﻿namespace Mjml.Net.Internal
{
    internal sealed class RenderStack<T>
    {
        private readonly Stack<T> stack = new Stack<T>(10);

        public T? Current;

        public IEnumerable<T> Elements => stack;

        public void Push(T element)
        {
            Current = element;

            stack.Push(element);
        }

        public T? Pop()
        {
            if (stack.Count == 0)
            {
                return default;
            }

            var top = stack.Pop();

            if (stack.Count > 0)
            {
                Current = stack.Peek();
            }
            else
            {
                Current = default;
            }

            return top;
        }

        public void Clear()
        {
            stack.Clear();

            Current = default;
        }
    }
}
