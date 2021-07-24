namespace European_Roulette_Main_Version
{
    public class CircularLinkedList<T>
    {
        public Node<T> head = null;
        public Node<T> tail = null;
        int count = 0;
        public void AddLast(T item)
        {
            if (head == null)
                this.AddFirstItem(item);
            else
            {
                Node<T> newNode = new Node<T>(item);
                tail.Next = newNode;
                newNode.Next = head;
                newNode.Previous = tail;
                tail = newNode;
                head.Previous = tail;
            }
            ++count;
        }

        void AddFirstItem(T item)
        {
            head = new Node<T>(item);
            tail = head;
            head.Next = tail;
            head.Previous = tail;
        }
        public sealed class Node<T>
        {
            public T Value { get; private set; }

            public Node<T> Next { get; internal set; }

            public Node<T> Previous { get; internal set; }

            public Node(T item)
            {
                this.Value = item;
            }
        }
    }
}
