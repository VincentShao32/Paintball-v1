using Unity.Collections;
using System;

namespace PaintWars.FPS.DataStructures
{
    public class CustomQueue<T>
    {
        private Node<T> head;
        private Node<T> tail;
        public void Enqueue(T element)
        {
            var temp = new Node<T>(element);

            if (head == null)
            {
                head = tail = temp;
            }
            else
            {
                tail.next = temp;
                tail = temp;
            }
        }
        public T Dequeue()
        {
            if (head == null)
            {
                throw new Exception("Queue Empty");
            }
            var temp = head.val;
            head = (Node<T>)head.next;
            return temp;
        }
        public bool isEmpty()
        {
            return head == null;
        }
        public int size()
        {
            return getLength(head, 0);
        }
        int getLength(Node<T> node, int count)
        {
            if (node == null)
            {
                return count;
            }
            return getLength(node.next, count + 1);
        }


    }

    class Node<T>
    {
        public T val { get; set; }
        public Node<T> next { get; set; }
        public Node(T val)
        {
            this.val = val;
        }
    }
}