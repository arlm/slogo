using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using TurtleRuntime;

namespace SLogoRuntime
{
    /// <summary>
    /// A class that provide some things that are common 
    /// to both IDE and/or Console version like command 
    /// line switches processing, compilation and execution
    /// </summary>
    public class Runtime
    {
        private List<string> largv = null;
        CSharpCodeProvider provider;
        ICodeCompiler compiler;

        // The drawing surface is the canvas on which the
        // turtle draws.
        public Turtle turtle = null;

        public bool show_help = false;
        public string source_file = null;
        public string source_code = null;
        public string cs_source_file = null;
        public string result_file = null;
        public string code_2_run = null;
        public int width = 1000;
        public int height = 1000;

        public bool generate_exe = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="argv">Command line options</param>
        public Runtime(string[] argv)
        {
            provider = new CSharpCodeProvider();
            compiler = provider.CreateCompiler();
            largv = new List<string>(argv);

            foreach (string cp in largv)
            {
                List<string> arr = new List<string>(cp.Split('='));
                if(arr.Count == 1)
                    arr.Add("");

                if (arr[0].Equals("--output"))
                {
                    result_file = arr[1];
                    generate_exe = Path.GetExtension(this.result_file)
                                       .ToLower()
                                       .Equals(".exe");
                }
                else if (arr[0].Equals("--run"))
                {
                    code_2_run = arr[1];
                }
                else if (arr[0].Equals("--width"))
                {
                    Int32.TryParse(arr[1], out width);
                }
                else if (arr[0].Equals("--height"))
                {
                    Int32.TryParse(arr[1], out height);
                }
                else if (arr[0].Equals("--cs"))
                {
                    cs_source_file = arr[1];
                }
                else if (arr[0].Equals("--help"))
                {
                    show_help = true;
                }
                else
                {
                    if (File.Exists(cp))
                    {
                        source_file = cp;
                        source_code = File.ReadAllText(cp);
                    }
                }
            }
        }

        public string CompileToCs(string srcCode, out string errorList)
        {
            SLogoCompiler.Compiler c = new SLogoCompiler.Compiler();
            string lgo_code = srcCode;
            if (!String.IsNullOrEmpty(code_2_run))
            {
                lgo_code += "\n\nto __code_2_run\n" + code_2_run + "\nend\n";
            }
            return c.ToCs(lgo_code, out errorList);
        }

        public string RunCs(string code)
        {
            string csCode = code;
            csCode = @"public class LogoProgram : TurtleRuntime.CompiledLogoProgram
                       {
                        " +
                        csCode +
                        @"}";

            CompilerParameters compilerparams = new CompilerParameters();
            compilerparams.GenerateExecutable = false;
            compilerparams.GenerateInMemory = true;
            compilerparams.IncludeDebugInformation = true;
            //compilerparams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerparams.ReferencedAssemblies.Add("TurtleRuntime.dll");

            CompilerResults results = compiler.CompileAssemblyFromSource(compilerparams, csCode);
            Assembly _compiledAssembly = !results.Errors.HasErrors ? results.CompiledAssembly : null;
            if (_compiledAssembly != null)
            {
                object _instance = _compiledAssembly.CreateInstance("LogoProgram");
                Type _type = _instance.GetType();

                //
                // Call _initialize() and pass turtle object as parameter
                //
                _type.GetMethod("_initialize").Invoke(_instance, new object[] { turtle });

                MethodInfo _method = _type.GetMethod("__code_2_run");
                if (_method == null)
                    _method = _type.GetMethod("main");

                try
                {
                    _method.Invoke(_instance, new object[0] { });
                }
                catch (TargetInvocationException se)
                {
                    // Stop command issued.. It is a normal stop !!!
                    if (se.InnerException is TurtleStopException)
                    {
                        // Do nothing !!! This is intentional exception
                    }
                    else
                    {
                        return se.InnerException.Message;   // Runtime error
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;   // Runtime error
                }

                /*
                savedImage = new Bitmap(this.pictureBox.Image);
                turtle.Draw();
                this.pictureBox.Refresh();

                if (!String.IsNullOrEmpty(cmdline.result_file))
                    savedImage.Save(cmdline.result_file, System.Drawing.Imaging.ImageFormat.Bmp);
                */
                return null;
            }
            else
            {
                string _expressionError = "";
                foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                    _expressionError += error.ErrorText + "\n";

                return _expressionError;
            }
        }

        public string MakeExe(string CScode, string exePath)
        {
            try
            {
                string template;

                //string[] gg = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                using (Stream stream = Assembly.GetExecutingAssembly()
                                               .GetManifestResourceStream("SLogoRuntime.ExeSkeleton.cs"))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        template = reader.ReadToEnd();
                    }
                }

                template = template.Replace("/*Width*/",  width.ToString());
                template = template.Replace("/*Height*/", height.ToString());
                template = template.Replace("/*CompiledCode*/", CScode);

                CompilerParameters compilerparams = new CompilerParameters();
                compilerparams.GenerateExecutable = true;
                compilerparams.GenerateInMemory = false;
                compilerparams.OutputAssembly = exePath;
                compilerparams.IncludeDebugInformation = false;
                compilerparams.ReferencedAssemblies.Add("System.dll");
                compilerparams.ReferencedAssemblies.Add("System.Drawing.dll");
                compilerparams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                compilerparams.ReferencedAssemblies.Add("TurtleRuntime.dll");

                CompilerResults results = compiler.CompileAssemblyFromSource(compilerparams, template);

                if (results.Errors.HasErrors)
                {
                    string _expressionError = "";
                    foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                        _expressionError += error.ErrorText + "\n";

                    return _expressionError;
                }

                return null;
            }
            catch  (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
