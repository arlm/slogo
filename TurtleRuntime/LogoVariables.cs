using System;
using System.Collections.Generic;
using System.Text;

namespace TurtleRuntime
{
    public class LogoVariables<T>
    {
        private LogoVariables<T> _gVars;
        private Dictionary<string, T> _vars;

        public LogoVariables(LogoVariables<T> parent)
        {
            _gVars = parent;
            _vars = new Dictionary<string, T>();
        }

        public T this[string varname] 
        {
            get {
                if (_vars.ContainsKey(varname))  // Postoji li lokalna ?
                    return _vars[varname];
                else if (_gVars != null && _gVars.ContainVariable(varname))
                    return _gVars[varname];
                else
                    throw new Exception("Undefined variable " + varname);   // This is an error ?!?!
                }  
            set {
                    if (_gVars != null && _gVars.ContainVariable(varname))
                        _gVars[varname] = value;
                    else if (_vars.ContainsKey(varname))
                        _vars[varname] = value;
                    else
                        _vars.Add(varname, value);
                } 
        }

        public bool ContainVariable(string varname)
        {
            return _vars.ContainsKey(varname);
        }

    }
}
