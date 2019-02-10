using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKTree
{
    class Graph<T>
    {
        public Dictionary<T, HashSet<T>> edges;

        public Graph()
        {
            edges = new Dictionary<T, HashSet<T>>();
        }

        public Graph(T x)
        {
            edges = new Dictionary<T, HashSet<T>>();
            edges[x] = new HashSet<T>();
        }

        public void AddVertex(T x)
        {
            edges[x] = new HashSet<T>();
        }

        public void AddEdge(T x1, T x2)
        {
            edges[x1].Add(x2);
            edges[x2].Add(x1);
        }

        public void Clear()
        {
            edges.Clear();
        }
    }
}
