using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace escript
{
    public class EVariable
    {
        public string Name = "null";
        public string Value = "null";
        public DateTime Created;
        public DateTime Edited;
        public bool CanBeRemoved = true;

        public EVariable(string n, string v, bool r = true)
        {
            Name = n;
            CanBeRemoved = r;
            Created = DateTime.Now;
            Edit(v);
        }

        public void Edit(string v)
        {
            Value = v;
            Edited = DateTime.Now;
        }
    }
}
