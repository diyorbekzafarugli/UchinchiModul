namespace Lesson_3_11_;

internal class Program
{
    static void Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");


        Node node = NodeService.CreateNode(6);

    }


    static int GetSummValue(Node node)
    {
        var summ = 0;
        while (true)
        {
            if(node is null)
            {
                break;
            }
            summ += node.Value;
            node = node.Next;
        }
        return summ;
    }


    static Node GetNode(Node node)
    {
        Node maxValueNode = node;

        while (true)
        {
            if (node is null)
            {
                break;
            }
            if (node.Value > maxValueNode.Value) maxValueNode = node;
            
            node = node.Next;
        }
        return maxValueNode;
    }

    static void NodeToDisplay(Node node)
    {

        while (true)
        {
            if (node is null) break;
            
            if (node.Value % 2 == 0)
            {
                Console.WriteLine(node.Value);
            }

            node = node.Next;
        }
    }
}
