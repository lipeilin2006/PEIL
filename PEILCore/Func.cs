using System.Text;
using RestSharp;

namespace PEILCore
{
    public class Func
    {
        public FuncList funcList = new FuncList();
        public VarList varlist = new VarList();
        public VarList baselist = new VarList();
        public Func(FuncList funcs,VarList vars)
        {
            funcList = funcs;
            baselist = vars;
        }
        public Class Run(string funcname,List<Class> tokens)
        {
            if (funcname == "output")
            {
                try
                {
                    Console.WriteLine(tokens[0].GetContent());
                    return null;
                }
                catch (Exception)
                {
                    throw new Exception("Error : Lost params");
                }
            }
            else if (funcname == "input")
            {
                string input = Console.ReadLine();
                return new Class(input, "string", false);
            }
            else if (funcname == "ToInt")
            {
                try
                {
                    return new Class(int.Parse(tokens[0].GetContent()).ToString(), "int", false);
                }
                catch (Exception)
                {
                    throw new Exception("Error : Lost params");
                }
            }
            else if (funcname == "ToFloat")
            {
                try
                {
                    return new Class(float.Parse(tokens[0].GetContent()).ToString(), "float", false);
                }
                catch (Exception)
                {
                    throw new Exception("Error : Lost params");
                }
            }
            else if (funcname == "ToString")
            {
                try
                {
                    return new Class(tokens[0].GetContent(), "string", false);
                }
                catch (Exception)
                {
                    throw new Exception("Error : Lost params");
                }
            }
            else if (funcname == "TypeOf")
            {
                try
                {
                    return new Class(tokens[0].GetT(), "string", false);
                }
                catch (Exception)
                {
                    throw new Exception("Error : Lost params");
                }
            }
            else if (funcname == "not")
            {
                try
                {
                    if (tokens[0].GetT() == "bool")
                    {
                        if (tokens[0].GetContent() == "true")
                        {
                            return new Class("false", "bool", false);
                        }
                        else
                        {
                            return new Class("true", "bool", false);
                        }
                    }
                    else
                    {
                        throw new Exception("Error : Not a bool");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error : Lost params");
                }
            }
            else if (funcname == "exit")
            {
                try
                {
                    if (tokens[0].GetT() == "int")
                    {
                        Environment.Exit(int.Parse(tokens[0].GetContent()));
                        return null;
                    }
                    else
                    {
                        throw new Exception("Error : Must exit with int");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error : Lost params");
                }
            }
            else if (funcname == "GetPassedTime")
            {
                Golbal golbal = new Golbal();
                return new Class(golbal.GetPassedTime(), "int", false);
            }
            else if (funcname == "HttpGet")
            {
                if (tokens.Count > 0)
                {
                    if (tokens[0].GetT() == "string")
                    {
                        string url = tokens[0].GetContent();
                        RestClient client = new RestClient();
                        RestRequest request = new RestRequest(url, Method.Get);
                        var tsk = client.ExecuteAsync(request);
                        while (true)
                        {
                            if (tsk.IsCompleted)
                            {
                                RestResponse res = tsk.Result;
                                if (res.IsSuccessful)
                                {
                                    return new Class(res.Content, "string", false);
                                }
                                else
                                {
                                    throw new Exception("Error with status code : " + res.StatusCode);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Params Error");
                    }
                }
                else
                {
                    throw new Exception("Lost params");
                }
            }
            else if (funcname == "DownloadTo")
            {
                if (tokens.Count >= 2)
                {
                    if (tokens[0].GetT() == "string" && tokens[1].GetT() == "string")
                    {
                        string url = tokens[0].GetContent();
                        string path = tokens[1].GetContent();
                        RestClient client = new RestClient();
                        RestRequest request = new RestRequest(url, Method.Get);
                        var tsk = client.DownloadDataAsync(request);
                        Console.WriteLine("Downloading,please wait.");
                        while (true)
                        {
                            if (tsk.IsCompleted)
                            {
                                Console.WriteLine("Download Succeed.");
                                File.WriteAllBytes(path, tsk.Result);
                                break;
                            }
                        }
                        return null;
                    }
                    else
                    {
                        throw new Exception("Params Error");
                    }
                }
                else
                {
                    throw new Exception("Lost params");
                }
            }
            else if (funcname == "RunInNewThread")
            {
                if (tokens.Count > 0)
                {
                    if (tokens[0].GetT() == "string")
                    {
                        new Thread(() =>
                        {
                            string funcn = tokens[0].GetContent();
                            tokens.RemoveAt(0);
                            try
                            {
                                new Func(funcList, varlist).Run(funcn, tokens);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }).Start();
                        return null;
                    }
                    else
                    {
                        throw new Exception("Params Error");
                    }
                }
                else
                {
                    throw new Exception("Lost Params");
                }
            }
            else if (funcname == "ReadAll")
            {
                if (tokens.Count > 0)
                {
                    if (tokens[0].GetT() == "string")
                    {
                        try
                        {
                            return new Class(File.ReadAllText(tokens[0].GetContent()), "string", false);
                        }catch
                        {
                            throw new Exception("Error read file");
                        }
                    }
                    else
                    {
                        throw new Exception("Params Error");
                    }
                }
                else
                {
                    throw new Exception("Lost Params");
                }
            }
            else if (funcname == "ReadAtLine")
            {
                if (tokens.Count > 1)
                {
                    if (tokens[0].GetT() == "string" && tokens[1].GetT() == "int")
                    {
                        string fn = tokens[0].GetContent();
                        int line = int.Parse(tokens[1].GetContent());
                        try
                        {
                            return new Class(File.ReadAllLines(tokens[0].GetContent())[line], "string", false);
                        }
                        catch
                        {
                            throw new Exception("Error read file");
                        }
                    }
                    else
                    {
                        throw new Exception("Params Error");
                    }
                }
                else
                {
                    throw new Exception("Lost Params");
                }
            }else if (funcname == "WriteAll")
            {
                if (tokens.Count > 1)
                {
                    if (tokens[0].GetT() == "string" && tokens[1].GetT() == "string")
                    {
                        string path = tokens[0].GetContent();
                        string conn = tokens[1].GetContent();
                        try
                        {
                            File.WriteAllText(path, conn);
                            return null;
                        }
                        catch
                        {
                            throw new Exception("Error write file");
                        }
                    }
                    else
                    {
                        throw new Exception("Params Error");
                    }
                }
                else
                {
                    throw new Exception("Lost Params");
                }
            }
            else
            {
                if (funcList.Funcexist(funcname))
                {
                    List<string> funcparams = funcList.GetParams(funcname);
                    for (int i = 0; i < funcparams.Count; i++)
                    {
                        varlist.SetVal(funcparams[i], tokens[i]);
                    }
                    PEILCore parser = new PEILCore(varlist, baselist, new VarList(), funcList);
                    Class val = (Class)parser.Visit(funcList.GetTree(funcname));
                    return val;
                }
                else
                {
                    throw new Exception("Error : Function do not exist");
                }
            }
        }
        private string MakeCSharp(string code)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;");
            sb.Append(Environment.NewLine);
            sb.Append("using System.IO;");
            sb.Append(Environment.NewLine);
            sb.Append("using System.Windows.Forms;");
            sb.Append(Environment.NewLine);
            sb.Append("namespace CSharpEval{");
            sb.Append(Environment.NewLine);
            sb.Append("public class Main{");
            sb.Append(Environment.NewLine);
            sb.Append("public void Eval(){");
            sb.Append(Environment.NewLine);
            sb.Append(code);
            sb.Append(Environment.NewLine);
            sb.Append("}");
            sb.Append(Environment.NewLine);
            sb.Append("}");
            sb.Append(Environment.NewLine);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
