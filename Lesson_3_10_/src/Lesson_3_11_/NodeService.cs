using System;
using System.Collections.Generic;
using System.Text;

namespace Lesson_3_11_;

public class NodeService
{
    public static Node CreateNode(int length)
    {
        if(length < 0)
        {
            throw new Exception();
        }
        Random random = new();
        Node node = new Node(random.Next(1, 10));
        Node currentNode = node;

        for (int i = 0; i < length - 1; i++)
        {
            Node newNode = new Node(random.Next(1, 10));
            currentNode.Next = newNode;
            currentNode = currentNode.Next;
        }

        return node;
    }
}
