using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model;

namespace VKTree
{
    class VKGraph : Graph<User>
    {
        public new void AddVertex(User x)
        {
            bool f = false;
            foreach (User k in dat.Keys)
                if (x.Id == k.Id)
                    f = true;
            if (!f)
                dat.Add(x, new HashSet<User>());
        }

        public new void AddEdge(User x1, User x2)
        {
            //bool f1 = false, f2 = false;
            //foreach (User k in dat.Keys)
            //{
            //    if (k.Id == x1.Id)
            //        f1 = true;
            //    if (k.Id == x2.Id)
            //        f2 = true;
            //}
            //if (f1 && f2)
            //{
            //    dat[x1].Add(x2);
            //    dat[x2].Add(x1);
            //}
            //else throw new ArgumentOutOfRangeException();
            if (dat.ContainsKey(x1) && dat.ContainsKey(x2) && x1.Id != x2.Id)
            {
                AddVertex(x1);
                AddVertex(x2);
                dat[x1].Add(x2);
                dat[x2].Add(x1);
            }
        }
    }
}
