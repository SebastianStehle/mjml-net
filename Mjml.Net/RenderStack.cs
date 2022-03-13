namespace Mjml.Net
{
    internal sealed class RenderStack<T>
    {
        private readonly Stack<T> stack = new Stack<T>(10);
        private T? current;

        public T? Current => current;

        public void Push(T element)
        {
            current = element;

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
                current = stack.Peek();
            }
            else
            {
                current = default;
            }

            return top;
        }

        public void Clear()
        {
            stack.Clear();

            current = default;
        }
    }
}
