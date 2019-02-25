using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKTree;

namespace GraphTest
{
    class Program
    {  
        static void Main(string[] args)
        {
            uint com = 0;
            int x, y;
            Graph<int> graph = new Graph<int>();
            
            while (com != 5)
            {
                Console.WriteLine("*** MENU ***\n");
                Console.WriteLine("1. Add a vertex");
                Console.WriteLine("2. Add an edge");
                Console.WriteLine("3. Clear");
                Console.WriteLine("4. Print graph");
                Console.WriteLine("5. Exit");
                Console.WriteLine();
                Console.WriteLine("***  ***");
                com = Convert.ToUInt32(Console.ReadLine());

                switch (com)
                {
                    case 1:
                        Console.WriteLine("Value: ");
                        x = Convert.ToInt32(Console.ReadLine());
                        graph.AddVertex(x);
                        break;
                    case 2:
                        Console.WriteLine("Value 1: ");
                        x = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Value 2: ");
                        y = Convert.ToInt32(Console.ReadLine());
                        graph.AddEdge(x, y);
                        break;
                    case 3:
                        graph.Clear();
                        Console.WriteLine("Done.");
                        break;
                    case 4:
                        Console.WriteLine("Graph:");
                        Console.WriteLine();
                        foreach (var a in graph.vertexes())
                            Console.WriteLine(a);
                        Console.WriteLine();
                        break;
                    case 5:

                        break;
                }
            }

            
        }
    }
}
