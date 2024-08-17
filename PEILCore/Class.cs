namespace PEILCore
{
    public class Class
    {
        FuncList funcList = null;
        VarList varList = null;
        string content = null;
        string type = null;
        bool isRet = false;
        bool isSystem = false;
        public Class(string con, string t, bool isReturn, FuncList funcs, VarList vars)
        {
            content = con;
            type = t;
            isRet = isReturn;
            funcList = funcs;
            varList = vars;
            isSystem = false;
        }
        public Class(string con,string t,bool isReturn)
        {
            content = con;
            type = t;
            isRet = isReturn;
            isSystem = true;
        }
        public string GetContent()
        {
            return content;
        }
        public string GetT()
        {
            return type;
        }

        public virtual bool isReturn()
        {
            return isRet;
        }
        public bool isSys()
        {
            return isSystem;
        }
        public FuncList GetFuncs()
        {
            return funcList;
        }
        public VarList GetVars()
        {
            return varList;
        }
        public virtual bool isBreak() => false;
        public Class Clone()
        {
            if (isSys())
            {
                return new Class(content, type, isRet);
            }
            else
            {
                return new Class(content, type, isRet, funcList.Clone(), varList.Clone());
            }
        }
    }

    public class BreakClass : Class
    {
        public BreakClass() : base(null, null, false) { }

        public override bool isBreak() => true;
        public override bool isReturn() => true;
    }
}
