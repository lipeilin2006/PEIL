using Antlr4.Runtime;
using PEILCore;

namespace PEIL
{
    class Program
    {
        static void Main(string[] args)
        {
            Golbal golbal = new Golbal();
            if (args.Length == 0)
            {
                VarList varList = new VarList();
                VarList baselist = new VarList();
                FuncList funcList = new FuncList();
                while (true)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("PEIL>>>");
                        Console.ResetColor();
                        string input = Console.ReadLine();
                        Eval(input, varList, baselist, funcList);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
        static void Eval(string input, VarList varList, VarList baselist, FuncList funcList)
        {
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
            PEILCore.PEILCore visitor = new PEILCore.PEILCore(varList, baselist, new VarList(), funcList);
            visitor.Visit(importilb);
            visitor.Visit(defclass);
            visitor.Visit(defFuncs);
            visitor.Visit(tree);
        }
    }
}
