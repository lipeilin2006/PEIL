using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using System.IO;

namespace PEILCore
{
    public class PEILCore : PEILCoreBaseVisitor<object>
    {
        public VarList varList = new VarList();
        public VarList baselist = new VarList();
        public VarList rootlist = new VarList();
        public FuncList funcList = new FuncList();
        public List<string> systemfunc = new List<string> { "output", "input", "not", "exit", "ToInt", "ToFloat", "ToString", "TypeOf", "del", "if", "else", "while", "break", "DownloadTo", "HttpGet", "ReadAll", "ReadAtLine", "WriteAll", "RunInNewThread" };
        public PEILCore(VarList vars,VarList basevars,VarList rootls, FuncList funcs)
        {
            varList = vars;
            baselist = basevars;
            rootlist = rootls;
            funcList = funcs;
        }
        //进入主程序运行状态
        public override object VisitProgram([NotNull] PEILCoreParser.ProgramContext context)
        {
            for(int i = 0; i < context.ChildCount; i++)
            {
                Class ret = (Class)Visit(context.GetChild(i));
                if (ret != null && ret.isReturn())
                {
                    if (ret.isSys())
                    {
                        return new Class(ret.GetContent(), ret.GetT(), false);
                    }
                    else
                    {
                        return new Class(ret.GetContent(), ret.GetT(), false, ret.GetFuncs(), ret.GetVars());
                    }
                }
            }
            return null;
        }
        //赋值语句
        public override object VisitAssign([NotNull] PEILCoreParser.AssignContext context)
        {
            Class val = (Class)Visit(context.expr());
            List<ParserRuleContext> contexts = new List<ParserRuleContext>();
            foreach (ParserRuleContext con in context.inclass())
            {
                contexts.Add(con);
            }
            contexts.Add(context.id());
            Ass(contexts, val.Clone());
            return null;
        }
        //多个表达式嵌套
        public override object VisitMoreExpr([NotNull] PEILCoreParser.MoreExprContext context)
        {
            return Visit(context.expr());
        }
        //加减
        public override object VisitAddSub([NotNull] PEILCoreParser.AddSubContext context)
        {
            string oper = context.oper.Text;
            Class l = (Class)Visit(context.expr(0));
            Class r = (Class)Visit(context.expr(1));
            if (l.isSys() && r.isSys())
            {
                if (oper == "+")
                {
                    if (l.GetT() != "bool" && r.GetT() != "bool")
                    {
                        if (l.GetT() != "string" && r.GetT() != "string")
                        {
                            if (l.GetT() == "int" && r.GetT() == "int")
                            {
                                long num = long.Parse(l.GetContent()) + long.Parse(r.GetContent());
                                return new Class(num.ToString(), "int", false);
                            }
                            else
                            {
                                float num = float.Parse(l.GetContent()) + float.Parse(r.GetContent());
                                return new Class(num.ToString(), "float", false);
                            }
                        }
                        else if (l.GetT() == "string" && r.GetT() == "string")
                        {
                            return new Class(l.GetContent() + r.GetContent(), "string", false);
                        }
                        else
                        {
                            return new Class(l.GetContent() + r.GetContent(), "string", false);
                        }
                    }
                    else
                    {
                        throw new Exception("Error : Cannot do the addition with bool");
                    }
                }
                else
                {
                    if (l.GetT() != "bool" && r.GetT() != "bool")
                    {
                        if (l.GetT() != "string" && r.GetT() != "string")
                        {
                            if (l.GetT() == "int" && r.GetT() == "int")
                            {
                                long num = long.Parse(l.GetContent()) - long.Parse(r.GetContent());
                                return new Class(num.ToString(), "int", false);
                            }
                            else
                            {
                                float num = float.Parse(l.GetContent()) - float.Parse(r.GetContent());
                                return new Class(num.ToString(), "float", false);
                            }
                        }
                        else if (l.GetT() == "string" && r.GetT() == "string")
                        {
                            throw new Exception("Error : Not Support");
                        }
                        else
                        {
                            return new Class(l.GetContent() + r.GetContent(), "string", false);
                        }
                    }
                    else
                    {
                        throw new Exception("Error : Cannot do the subtraction with bool");
                    }
                }
            }
            else
            {
                throw new Exception("Error : Not Support");
            }
        }
        //乘除
        public override object VisitMulDiv([NotNull] PEILCoreParser.MulDivContext context)
        {
            string oper = context.oper.Text;
            Class l = (Class)Visit(context.expr(0));
            Class r = (Class)Visit(context.expr(1));
            if (l.isSys() && r.isSys())
            {
                if (oper == "*")
                {
                    if (l.GetT() == "float" || r.GetT() == "float")
                    {
                        float num = float.Parse(l.GetContent()) * float.Parse(r.GetContent());
                        return new Class(num.ToString(), "float", false);
                    }
                    else if (l.GetT() == "int" && r.GetT() == "int")
                    {
                        long num = long.Parse(l.GetContent()) * long.Parse(r.GetContent());
                        return new Class(num.ToString(), "int", false);
                    }
                    else
                    {
                        throw new Exception("Error : Cannot do multiplication or division with bool or string");
                    }
                }
                else
                {
                    if (l.GetT() == "float" || r.GetT() == "float")
                    {
                        float num = float.Parse(l.GetContent()) / float.Parse(r.GetContent());
                        return new Class(num.ToString(), "float", false);
                    }
                    else if (l.GetT() == "int" && r.GetT() == "int")
                    {
                        long num = long.Parse(l.GetContent()) / long.Parse(r.GetContent());
                        return new Class(num.ToString(), "int", false);
                    }
                    else
                    {
                        throw new Exception("Error : Cannot do multiplication or division with bool or string");
                    }
                }
            }
            else
            {
                throw new Exception("Error : Not Support");
            }
        }
        //数字字面量
        public override object VisitNumber([NotNull] PEILCoreParser.NumberContext context)
        {
            string num = context.NUM().GetText();
            if (num.Contains("."))
            {
                return new Class(float.Parse(num).ToString(), "float", false);
            }
            else
            {
                return new Class(long.Parse(num).ToString(), "int", false);
            }
        }
        //字符串字面量
        public override object VisitStr([NotNull] PEILCoreParser.StrContext context)
        {
            string str = context.STRING().GetText();
            return new Class(str.Substring(1, str.Length - 2).Replace("\\\\","\\").Replace("\\\"", "\"").Replace("\\n",Environment.NewLine), "string", false);
        }
        //标识符
        public override object VisitIdent([NotNull] PEILCoreParser.IdentContext context)
        {
            List<ParserRuleContext> contexts = new List<ParserRuleContext>();
            foreach (ParserRuleContext con in context.inclass())
            {
                contexts.Add(con);
            }
            contexts.Add(context.id());
            return GetFromInside(contexts, varList.Clone());
        }
        //执行函数
        public override object VisitRunFunction([NotNull] PEILCoreParser.RunFunctionContext context)
        {
            if (context.inclass().Length > 0)
            {
                List<ParserRuleContext> contexts = new List<ParserRuleContext>();
                foreach (ParserRuleContext con in context.inclass())
                {
                    contexts.Add(con);
                }
                contexts.Add(context.runFunc());
                return GetFromInside(contexts, varList.Clone());
            }
            else
            {
                return (Class)Visit(context.runFunc());
            }
        }
        public override object VisitRunFunc([NotNull] PEILCoreParser.RunFuncContext context)
        {
            string funcname = context.ID().GetText();
            object vars = Visit(context.@params());
            Func func = new Func(funcList, varList);
            return func.Run(funcname, (List<Class>)vars);
        }
        //获取参数列表
        public override object VisitParams([NotNull] PEILCoreParser.ParamsContext context)
        {
            return Visit(context.vars());
        }
        //获取参数值
        public override object VisitVars([NotNull] PEILCoreParser.VarsContext context)
        {
            List<Class> vars = new List<Class>();
            for(int i = 0; i < context.expr().Length; i++)
            {
                vars.Add((Class)Visit(context.expr(i)));
            }
            return vars;
        }
        //If Else语句
        public override object VisitIfStatement([NotNull] PEILCoreParser.IfStatementContext context)
        {
            Class expr = (Class)Visit(context.expr());
            ParserRuleContext truetree = context.program(0);
            ParserRuleContext falsetree = context.program(1);
            if (expr.GetT() == "bool" && expr.GetContent() == "true")
            {
                Visit(truetree);
                return null;
            }
            else if(expr.GetT() == "bool" && expr.GetContent() == "false")
            {
                if (falsetree != null)
                {
                    Visit(falsetree);
                    return null;
                }
                return null;
            }
            else
            {
                throw new Exception("Error : Expression return as not a bool");
            }
        }
        //比较运算符（返回true或false）
        public override object VisitCompare([NotNull] PEILCoreParser.CompareContext context)
        {
            Class l = (Class)Visit(context.expr(0));
            Class r = (Class)Visit(context.expr(1));
            string oper = context.oper.Text;
            if (l.isSys() && r.isSys())
            {
                if (oper == "==")
                {
                    if (l.GetT() == "string" && r.GetT() == "string")
                    {
                        if (l.GetContent() == r.GetContent())
                        {
                            return new Class("true", "bool", false);
                        }
                        else
                        {
                            return new Class("false", "bool", false);
                        }
                    }
                    else if (l.GetT() != "string" && r.GetT() != "string")
                    {
                        if (l.GetT() != "bool" && r.GetT() != "bool")
                        {
                            if (float.Parse(l.GetContent()) == float.Parse(r.GetContent()))
                            {
                                return new Class("true", "bool", false);
                            }
                            else
                            {
                                return new Class("false", "bool", false);
                            }
                        }
                        else if (l.GetT() == "bool" && r.GetT() == "bool")
                        {
                            if (l.GetContent() == r.GetContent())
                            {
                                return new Class("true", "bool", false);
                            }
                            else
                            {
                                return new Class("false", "bool", false);
                            }
                        }
                        else
                        {
                            throw new Exception("Error : Cannot compare bool value with number value");
                        }
                    }
                    else
                    {
                        throw new Exception("Error : You are comparing a string value with other types");
                    }
                }
                else if (oper == "!=")
                {
                    if (l.GetT() == "string" && r.GetT() == "string")
                    {
                        if (l.GetContent() != r.GetContent())
                        {
                            return new Class("true", "bool", false);
                        }
                        else
                        {
                            return new Class("false", "bool", false);
                        }
                    }
                    else if (l.GetT() != "string" && r.GetT() != "string")
                    {
                        if (l.GetT() != "bool" && r.GetT() != "bool")
                        {
                            if (float.Parse(l.GetContent()) != float.Parse(r.GetContent()))
                            {
                                return new Class("true", "bool", false);
                            }
                            else
                            {
                                return new Class("false", "bool", false);
                            }
                        }
                        else if (l.GetT() == "bool" && r.GetT() == "bool")
                        {
                            if (l.GetContent() != r.GetContent())
                            {
                                return new Class("true", "bool", false);
                            }
                            else
                            {
                                return new Class("false", "bool", false);
                            }
                        }
                        else
                        {
                            throw new Exception("Error");
                        }
                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }
                else if (oper == ">=")
                {
                    if (l.GetT() == "string" && r.GetT() == "string")
                    {
                        throw new Exception("Error");
                    }
                    else if (l.GetT() != "string" && r.GetT() != "string")
                    {
                        if (l.GetT() != "bool" && r.GetT() != "bool")
                        {
                            if (float.Parse(l.GetContent()) >= float.Parse(r.GetContent()))
                            {
                                return new Class("true", "bool", false);
                            }
                            else
                            {
                                return new Class("false", "bool", false);
                            }
                        }
                        else
                        {
                            throw new Exception("Error");
                        }
                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }
                else if (oper == "<=")
                {
                    if (l.GetT() == "string" && r.GetT() == "string")
                    {
                        throw new Exception("Error");
                    }
                    else if (l.GetT() != "string" && r.GetT() != "string")
                    {
                        if (l.GetT() != "bool" && r.GetT() != "bool")
                        {
                            if (float.Parse(l.GetContent()) <= float.Parse(r.GetContent()))
                            {
                                return new Class("true", "bool", false);
                            }
                            else
                            {
                                return new Class("false", "bool", false);
                            }
                        }
                        else
                        {
                            throw new Exception("Error");
                        }
                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }
                else if (oper == "<")
                {
                    if (l.GetT() == "string" && r.GetT() == "string")
                    {
                        throw new Exception("Error");
                    }
                    else if (l.GetT() != "string" && r.GetT() != "string")
                    {
                        if (l.GetT() != "bool" && r.GetT() != "bool")
                        {
                            if (float.Parse(l.GetContent()) < float.Parse(r.GetContent()))
                            {
                                return new Class("true", "bool", false);
                            }
                            else
                            {
                                return new Class("false", "bool", false);
                            }
                        }
                        else
                        {
                            throw new Exception("Error");
                        }
                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }
                else if (oper == ">")
                {
                    if (l.GetT() == "string" && r.GetT() == "string")
                    {
                        throw new Exception("Error");
                    }
                    else if (l.GetT() != "string" && r.GetT() != "string")
                    {
                        if (l.GetT() != "bool" && r.GetT() != "bool")
                        {
                            if (float.Parse(l.GetContent()) > float.Parse(r.GetContent()))
                            {
                                return new Class("true", "bool", false);
                            }
                            else
                            {
                                return new Class("false", "bool", false);
                            }
                        }
                        else
                        {
                            throw new Exception("Error");
                        }
                    }
                    else
                    {
                        throw new Exception("Error");
                    }
                }
                else
                {
                    throw new Exception("Error");
                }
            }
            else
            {
                throw new Exception("Error : Not Support");
            }
        }
        //while语句
        public override object VisitWhileStatement([NotNull] PEILCoreParser.WhileStatementContext context)
        {
            Class expr = (Class)Visit(context.expr());
            ParserRuleContext tree = context.program();
            if (expr.GetT() == "bool")
            {
                while (expr.GetContent() == "true")
                {
                    Class result=(Class)Visit(tree);
                    if (result != null && result.isBreak())
                    {
                        break;
                    }
                    expr = (Class)Visit(context.expr());
                }
                return null;
            }
            else
            {
                throw new Exception("Error : The express type is not a bool");
            }
        }
        //多个函数定义（运行在program之前）
        public override object VisitFunctions([NotNull] PEILCoreParser.FunctionsContext context)
        {
            for(int i = 0; i < context.ChildCount; i++)
            {
                Visit(context.GetChild(i));
            }
            return null;
        }
        //单个函数定义
        public override object VisitDefFunction([NotNull] PEILCoreParser.DefFunctionContext context)
        {
            string name = context.ID().GetText();
            foreach (string n in systemfunc)
            {
                if (name == n)
                {
                    throw new Exception("Error : This function has been exist!");
                }
            }
            List<string> funcparams = new List<string>();
            ParserRuleContext functree = context.program();
            for (int i = 0; i < context.@params().vars().expr().Length; i++)
            {
                funcparams.Add(context.@params().vars().expr(i).GetText());
            }
            funcList.DefineFunc(name, funcparams, functree);
            return null;
        }
        //return语句
        public override object VisitReturn([NotNull] PEILCoreParser.ReturnContext context)
        {
            Class ret = (Class)Visit(context.expr());
            if (ret.isSys())
            {
                return new Class(ret.GetContent(), ret.GetT(), true);
            }
            else
            {
                return new Class(ret.GetContent(), ret.GetT(), true, ret.GetFuncs(), ret.GetVars());
            }
        }
        //class定义
        public override object VisitDefineclass([NotNull] PEILCoreParser.DefineclassContext context)
        {
            FuncList funcls = new FuncList();
            VarList varls = new VarList();
            VarList basels = new VarList();
            PEILCore core = new PEILCore(varls, basels, new VarList(), funcls);
            if (context.ID().Length > 1)
            {
                Class fat = varList.GetVal(context.ID(1).GetText());
                funcls = fat.GetFuncs().Clone();
                varls = fat.GetVars().Clone();
                basels = new VarList();
                core = new PEILCore(varls, basels, new VarList(), funcls);
            }

            string classname = context.ID(0).GetText();
            if (varList.Varexist(classname))
            {
                throw new Exception("Error : Class has been exist");
            }
            for (int i = 0; i < context.defineclass().Length; i++)
            {
                core.Visit(context.defineclass(i));
            }
            for (int i = 0; i < context.assign().Length; i++)
            {
                core.Visit(context.assign(i));
            }
            for (int i = 0; i < context.deffunc().Length; i++)
            {
                core.Visit(context.deffunc(i));
            }
            varList.SetVal(classname, new Class(null, classname, false, funcls, varls));
            return null;
        }
        //多个import语句（最先运行）
        public override object VisitImportlibrary([NotNull] PEILCoreParser.ImportlibraryContext context)
        {
            for (int i = 0; i < context.importlib().Length; i++)
            {
                Visit(context.importlib(i));
            }
            return null;
        }
        //单个imort语句
        public override object VisitImportlib([NotNull] PEILCoreParser.ImportlibContext context)
        {
            string input = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "lib\\" + context.ID().GetText() + ".pei");

            AntlrInputStream inputStream = new AntlrInputStream(input);
            PEILCoreLexer lex = new PEILCoreLexer(inputStream);
            lex.RemoveErrorListeners();
            PEILCoreError error = new PEILCoreError();
            CommonTokenStream tokens = new CommonTokenStream(lex);
            PEILCoreParser parser = new PEILCoreParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(error);
            ParserRuleContext importilb = parser.importlibrary();
            ParserRuleContext defclass = parser.classes();
            ParserRuleContext defFuncs = parser.functions();
            ParserRuleContext tree = parser.program();
            PEILCore visitor = new PEILCore(varList, baselist, new VarList(), funcList);
            visitor.Visit(importilb);
            visitor.Visit(defclass);
            visitor.Visit(defFuncs);
            visitor.Visit(tree);
            return null;
        }
        //执行函数
        public Class RunFunc(List<string> ids, string funcname,List<Class> paramslist)
        {
            if (ids.Count > 0)
            {
                string classname = ids[0];
                ids.RemoveAt(0);
                Class cla;
                if (varList.Varexist(classname))
                {
                    cla = varList.GetVal(classname);
                }
                else
                {
                    cla=baselist.GetVal(classname);
                }
                PEILCore core = new PEILCore(cla.GetVars(), new VarList(), new VarList(), cla.GetFuncs());
                return core.RunFunc(ids, funcname, paramslist);
            }
            else
            {
                Func func = new Func(funcList, varList);
                return func.Run(funcname, paramslist);
            }
        }
        //获取变量值
        public Class GetVar(string varname)
        {
            if (varname == "true")
            {
                return new Class("true", "bool", false);
            }
            else if (varname == "false")
            {
                return new Class("false", "bool", false);
            }
            else
            {
                
                if (varList.Varexist(varname))
                {
                    return varList.GetVal(varname);
                }
                else if (baselist.Varexist(varname))
                {
                    return baselist.GetVal(varname);
                }
                else
                {
                    return rootlist.GetVal(varname);
                }
            }
        }
        //多种类型变量赋值
        public void Ass(List<ParserRuleContext> contexts, Class val)
        {
            if (contexts.Count > 1)
            {
                Class cla = (Class)Visit(contexts[0]);
                PEILCore core = new PEILCore(cla.GetVars(), new VarList(), new VarList(), cla.GetFuncs());
                contexts.RemoveAt(0);
                core.Ass(contexts, val);
            }
            else
            {
                string varname = contexts[0].GetText();
                if (!systemfunc.Contains(varname))
                {
                    if (baselist.Varexist(varname))
                    {
                        baselist.SetVal(varname, val);
                    }
                    else
                    {
                        varList.SetVal(varname, val);
                    }
                }
                else
                {
                    throw new Exception("Error : Cannot assign with this variable");
                }
            }
        }
        //访问类中的方法或成员
        public Class GetFromInside(List<ParserRuleContext> contexts,VarList outerlist)
        {
            if (contexts.Count > 1)
            {
                Class cla = (Class)Visit((PEILCoreParser.InclassContext)contexts[0]);
                PEILCore core = new PEILCore(cla.GetVars(), new VarList(), outerlist, cla.GetFuncs());
                contexts.RemoveAt(0);
                return core.GetFromInside(contexts, outerlist);
            }
            else
            {
                return (Class)Visit(contexts[0]);
            }
        }
        //访问类中的成员
        public override object VisitID_inClass([NotNull] PEILCoreParser.ID_inClassContext context)
        {
            return (Class)Visit(context.id());
        }
        //访问类中的方法
        public override object VisitFunc_inClass([NotNull] PEILCoreParser.Func_inClassContext context)
        {
            return (Class)Visit(context.runFunc());
        }
        //ID常量外嵌套一个id，用于把ID转为ParserRuleTree后的操作
        public override object VisitId([NotNull] PEILCoreParser.IdContext context)
        {
            if (context.ID().GetText() == "break")
            {
                return new BreakClass();
            }
            else
            {
                return GetVar(context.ID().GetText());
            }
        }
        //不赋值，执行函数
        public override object VisitRunsinglefunc([NotNull] PEILCoreParser.RunsinglefuncContext context)
        {
            if (context.inclass().Length > 0)
            {
                List<ParserRuleContext> contexts = new List<ParserRuleContext>();
                foreach (ParserRuleContext con in context.inclass())
                {
                    contexts.Add(con);
                }
                contexts.Add(context.runFunc());
                return GetFromInside(contexts, varList);
            }
            else
            {
                return (Class)Visit(context.runFunc());
            }
        }
        //+=和-=
        public override object VisitAddSubAssign([NotNull] PEILCoreParser.AddSubAssignContext context)
        {
            string oper = context.oper.Text;
            Class val = (Class)Visit(context.expr());
            List<ParserRuleContext> contexts = new List<ParserRuleContext>();
            foreach (ParserRuleContext con in context.inclass())
            {
                contexts.Add(con);
            }
            contexts.Add(context.id());
            if (oper == "+=")
            {
                AddAss(contexts, val);
            }
            else
            {
                SubAss(contexts, val);
            }
            return null;
        }
        //+=语句
        public void AddAss(List<ParserRuleContext> contexts, Class val)
        {
            if (contexts.Count > 1)
            {
                Class cla = (Class)Visit(contexts[0]);
                PEILCore core = new PEILCore(cla.GetVars(), new VarList(), new VarList(), cla.GetFuncs());
                contexts.RemoveAt(0);
                core.AddAss(contexts, val);
            }
            else
            {
                string varname = contexts[0].GetText();
                Class self = GetVar(varname);
                Class final;
                if (self.GetT() != "bool" && val.GetT() != "bool")
                {
                    if (self.GetT() == "int" && val.GetT() == "int")
                    {
                        final = new Class((long.Parse(self.GetContent()) + long.Parse(val.GetContent())).ToString(), "long", false);
                    }
                    else if (self.GetT() == "string" || val.GetT() == "string")
                    {
                        final = new Class(self.GetContent() + val.GetContent(), "string", false);
                    }
                    else
                    {
                        final = new Class((float.Parse(self.GetContent()) + float.Parse(val.GetContent())).ToString(), "float", false);
                    }
                }
                else
                {
                    throw new Exception("Error : Can not do addition with bool");
                }

                if (baselist.Varexist(varname))
                {
                    baselist.SetVal(varname, final);
                }
                else
                {
                    varList.SetVal(varname, final);
                }
            }
        }
        //-=语句
        public void SubAss(List<ParserRuleContext> contexts, Class val)
        {
            if (contexts.Count > 1)
            {
                Class cla = (Class)Visit(contexts[0]);
                PEILCore core = new PEILCore(cla.GetVars(), new VarList(), new VarList(), cla.GetFuncs());
                contexts.RemoveAt(0);
                core.SubAss(contexts, val);
            }
            else
            {
                string varname = contexts[0].GetText();
                Class self = GetVar(varname);
                Class final;
                if (self.GetT() == "int" && val.GetT() == "int")
                {
                    final = new Class((long.Parse(self.GetContent()) - long.Parse(val.GetContent())).ToString(), "float", false);
                }
                else if ((self.GetT() == "float" && val.GetT() == "int") || (self.GetT() == "int" && val.GetT() == "float") || (self.GetT() == "float" && val.GetT() == "float"))
                {
                    final = new Class((float.Parse(self.GetContent()) - float.Parse(val.GetContent())).ToString(), "float", false);
                }
                else
                {
                    throw new Exception("Error : Can not do subtraction with bool or string");
                }

                if (baselist.Varexist(varname))
                {
                    baselist.SetVal(varname, final);
                }
                else
                {
                    varList.SetVal(varname, final);
                }
            }
        }
        //删除变量
        public override object VisitDel([NotNull] PEILCoreParser.DelContext context)
        {
            foreach(var c in context.id())
            {
                string varname = c.ID().GetText();
                if (varList.Varexist(varname))
                {
                    varList.vals.Remove(varname);
                }
                else
                {
                    throw new Exception("Error : Variable do no exist");
                }
            }
            return null;
        }
        public override object VisitSingleid([NotNull] PEILCoreParser.SingleidContext context)
        {
            return (Class)Visit(context.id());
        }
    }
}
