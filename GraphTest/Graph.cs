using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKTree
{
    class Graph<T>
    {
        public Dictionary<T, HashSet<T>> dat;

        public Graph()
        {
            dat = new Dictionary<T, HashSet<T>>();
        }

        public Graph(T x)
        {
            dat = new Dictionary<T, HashSet<T>>();
            dat[x] = new HashSet<T>();
        }

        public void AddVertex(T x)
        {
            if (!dat.ContainsKey(x))
                dat[x] = new HashSet<T>();
        }

        public void AddEdge(T x1, T x2)
        {
            dat[x1].Add(x2);
            dat[x2].Add(x1);
        }

        public void Clear()
        {
            dat.Clear();
        }

        public List<T> vertexes()
        { return dat.Keys.ToList(); }
    }
}
