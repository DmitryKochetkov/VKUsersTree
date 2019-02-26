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
            dat.Add(x, new HashSet<T>());
        }

        public void AddVertex(T x)
        {
            bool f = false;
            foreach (T k in dat.Keys)
                if (x.Equals(k))
                    f = true;
                    if (!f)
                dat.Add(x, new HashSet<T>());
        }

        public void AddEdge(T x1, T x2)
        {
            if (dat.ContainsKey(x1))
                dat[x1].Add(x2);
            if (dat.ContainsKey(x2))
                dat[x2].Add(x1);
        }

        public void Clear()
        {
            dat.Clear();
        }

        public List<T> Vertexes()
        { return dat.Keys.ToList(); }

        public List<T> Edges(T x)
        { return dat[x].ToList(); }
    }
}
