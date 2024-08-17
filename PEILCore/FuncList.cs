using System.Collections.Generic;
using Antlr4.Runtime;

namespace PEILCore
{
    public class FuncList
    {
        public Dictionary<string, ParserRuleContext> FuncTrees = new Dictionary<string, ParserRuleContext>();
        public Dictionary<string, List<string>> FuncParams = new Dictionary<string, List<string>>();
        public FuncList() { }
        public FuncList(Dictionary<string, ParserRuleContext> funcs, Dictionary<string, List<string>> pars)
        {
            FuncTrees = funcs;
            FuncParams = pars;
        }
        public void DefineFunc(string name,List<string> paramlist,ParserRuleContext tree)
        {
            if (FuncTrees.ContainsKey(name))
            {
                FuncTrees[name] = tree;
                FuncParams[name] = paramlist;
            }
            else
            {
                FuncTrees.Add(name, tree);
                FuncParams.Add(name, paramlist);
            }
        }
        public ParserRuleContext GetTree(string name)
        {
            return FuncTrees[name];
        }
        public List<string> GetParams(string name)
        {
            return FuncParams[name];
        }
        public bool Funcexist(string name)
        {
            return FuncTrees.ContainsKey(name);
        }
        public FuncList Clone()
        {
            Dictionary<string, ParserRuleContext> funs = new Dictionary<string, ParserRuleContext>();
            Dictionary<string, List<string>> pars = new Dictionary<string, List<string>>();
            foreach(var item in FuncTrees)
            {
                funs.Add(item.Key, item.Value);
            }
            foreach(var item in FuncParams)
            {
                List<string> par = new List<string>();
                foreach(string s in item.Value)
                {
                    par.Add(s);
                }
                pars.Add(item.Key, par);
            }
            return new FuncList(funs, pars);
        }
    }
}
