using System;
using System.Collections.Generic;
using System.Text;

namespace SLogoCompiler
{
    public class ErrorChecker
    {
        Dictionary<string, FuncDef> funcDefs = new Dictionary<string, FuncDef>();
        List<FuncDef> funcCalls = new List<FuncDef>();

        private string currentFunc = null;

        public ErrorChecker()
        {
        }

        public void AddFuncDef(string fname, string prms, long line)
        {
            fname = fname.Trim();
            currentFunc = fname;

            if (funcDefs.ContainsKey(fname))
            {
                funcDefs[fname].doubleDefined = true;
                funcDefs[fname].startingLine = line;
            }
            else
            {
                FuncDef fd = new FuncDef();
                fd.name = fname;
                fd.paramNr = prms.Split(',').Length;
                fd.startingLine = line;
                funcDefs.Add(fname, fd);
            }
        }

        public void CloseFuncDef(string fname, long line)
        {
            fname = fname.Trim();
            funcDefs[fname].endingLine = line;
            currentFunc = null;
        }

        public void AddFuncCall(string fname, string prms, long line)
        {
            FuncDef fd = new FuncDef();
            fd.name = fname.Trim();
            fd.paramNr = prms.Split(',').Length;
            fd.startingLine = line;
            funcCalls.Add(fd);
        }

        public void AddLVarDef(string varname, long line)
        {
            varname = varname.Trim();
            if(!funcDefs[currentFunc].vars.ContainsKey(varname)) 
            {
                VarDef vd = new VarDef ();
                vd.name = varname;
                vd.line = line;
                funcDefs[currentFunc].vars.Add(varname, vd);
            }
        }

        public void AddVarUsage(string varname, long line)
        {
            varname = varname.Trim();
            if (!funcDefs[currentFunc].varsUsage.ContainsKey(varname))
            {
                VarDef vd = new VarDef();
                vd.name = varname;
                vd.line = line;
                funcDefs[currentFunc].varsUsage.Add(varname, vd);
            }
        }

        private class FuncDef
        {
            public bool doubleDefined = false;
            public string name = null;
            public int paramNr = 0;
            public long startingLine = -1;
            public long endingLine = -1;
            public Dictionary<string, VarDef> vars = new Dictionary<string, VarDef>();
            public Dictionary<string, VarDef> varsUsage = new Dictionary<string, VarDef>();
        }

        private class VarDef
        {
            public string name = null;
            public long line = -1;
        }

        public string GetReport()
        {
            string retval = "";

            foreach (string fn in funcDefs.Keys)
            {
                retval += "\nFunction   : " + fn + "\n";
                retval += "Parameters : " + funcDefs[fn].paramNr + "\n";
                retval += "Lines from : " + funcDefs[fn].startingLine + " to " + funcDefs[fn].endingLine + "\n";
                retval += "Local vars : ";
                foreach (string vn in funcDefs[fn].vars.Keys)
                    retval += vn + " ";
                retval += "\n";

            }
            return retval;
        }

        public string CheckForErrors()
        {
            string retval = "";
            foreach (string fname in funcDefs.Keys)
            {
                FuncDef fd = funcDefs[fname];
                if (fd.doubleDefined)
                    retval += "Line " + fd.startingLine + ": Function '" + fname + "' already defined\n";

                foreach (string vn in fd.varsUsage.Keys)
                { 
                    if(!fd.vars.ContainsKey(vn))
                        retval += "Line " + fd.startingLine + ": Using undefined variable '" + vn + "'\n";
                    else if(fd.vars[vn].line > fd.varsUsage[vn].line)
                            retval += "Line " + fd.startingLine + ": Using variable '" + vn + "' before it is actualy defined\n";
                }
            }

            foreach (FuncDef fd in funcCalls)
            { 
                if(!funcDefs.ContainsKey(fd.name))
                    retval += "Line " + fd.startingLine + ": Calling undefined function '" + fd.name + "'\n";
                else if(fd.paramNr != funcDefs[fd.name].paramNr)
                    retval += "Line " + fd.startingLine + ": Calling function '" + fd.name + "' with wrong number of parameters\n";
            }

            return retval;
        }
    }
}
