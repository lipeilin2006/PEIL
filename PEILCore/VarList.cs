using System;
using System.Collections.Generic;

namespace PEILCore
{
    public class VarList
    {
        public Dictionary<string, Class> vals = new Dictionary<string, Class>();
        public VarList() { }
        public VarList(Dictionary<string, Class> varls)
        {
            vals = varls;
        }
        public void SetVal(string name,Class t)
        {
            if (vals.ContainsKey(name))
            {
                vals[name] = t;
            }
            else
            {
                vals.Add(name, t);
            }
        }
        public Class GetVal(string name)
        {
            if (vals.ContainsKey(name))
            {
                return vals[name];
            }
            else
            {
                throw new Exception("Error : Variable do not exist");
            }
        }
        public bool Varexist(string name)
        {
            return vals.ContainsKey(name);
        }
        public VarList Clone()
        {
            Dictionary<string, Class> vars = new Dictionary<string, Class>();
            foreach (var item in vals)
            {
                vars.Add(item.Key, item.Value);
            }
            return new VarList(vars);
        }
    }
}
