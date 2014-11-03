using System;
using System.Collections.Generic;
using System.Text;

namespace SLogoCompiler
{
    public class Compiler
    {
        public string ToCs(string code, out string errorList)
        {
            string retval = Parser.ToCs(code);
            retval += "\n\n/*\n";
            retval += Parser.error_checker.GetReport();
            retval += "\n\n*/\n";
            errorList = Parser.GetErrorList();
            if (String.IsNullOrEmpty(errorList))
                errorList = Parser.error_checker.CheckForErrors();
            return retval;
        }
    }
}
