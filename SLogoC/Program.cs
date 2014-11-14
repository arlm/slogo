using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SLogoRuntime;


namespace SLogoC
{
    public class Program
    {
        static Runtime lgoRuntime = null;
        static Bitmap Image = null;
        static Image savedImage = null;
        static Graphics drawingSurface = null;

        static int Main(string[] args)
        {
            DateTime start = DateTime.Now;

            lgoRuntime = new SLogoRuntime.Runtime(args);

            if (lgoRuntime.show_help)
            {
                Console.WriteLine("SLogo Compiler (Console) version 1.00");
                Console.WriteLine("for Windows and Linux (Mono)");
                Console.WriteLine("Copyright © ivica.sindicic@gmail.com 2014-2015. All rights reserved.");
                
                Console.WriteLine("\nUsage:");
                Console.WriteLine("SLogoC.exe filename.lgo [options]");

                Console.WriteLine("\nWhere options can be:");
                Console.WriteLine("--width=x    Where x is a number that define canvas width");
                Console.WriteLine("--height=y   Where y is a number that define canvas height");
                Console.WriteLine("--output=z   Where z is a a name of bitmap filename where result will be saved");
                Console.WriteLine("             or exe file where compiled Logo program will saved");
                Console.WriteLine("--run=c      Where c is a code that will be executed");

                Console.WriteLine("\nFor example:");
                Console.WriteLine("SLogoC.exe test.lgo --output=test.bmp \"--run=cs snowflake 12\"");
                Console.WriteLine("will load and compile test.lgo, and if that pass fine and without any error, ");
                Console.WriteLine("then execute ClearScreen (cs) and call function snowflake with parameter equal to 12:");
                return 0;
            }

            if (!String.IsNullOrEmpty(lgoRuntime.source_code))
            {
                string errors;
                string cs_source_code = lgoRuntime.CompileToCs(lgoRuntime.source_code, out errors);

                if (String.IsNullOrEmpty(errors))
                {
                    if (!String.IsNullOrEmpty(lgoRuntime.cs_source_file))
                        System.IO.File.WriteAllText(lgoRuntime.cs_source_file, cs_source_code);

                    if (lgoRuntime.generate_exe)
                    {
                        errors = lgoRuntime.MakeExe(cs_source_code, lgoRuntime.result_file);

                        if (String.IsNullOrEmpty(errors))
                        {
                            Console.WriteLine("Done in " + elapsedTime(start));
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine(errors);  // Errors 
                            return -1;
                        }
                    }
                    else
                    {
                        InitializeCanvas();
                        drawingSurface.DrawImage(savedImage, 0, 0);

                        errors = lgoRuntime.RunCs(cs_source_code);
                        if (String.IsNullOrEmpty(errors))
                        {
                            savedImage = new Bitmap(Image);
                            lgoRuntime.turtle.Draw();

                            if (!String.IsNullOrEmpty(lgoRuntime.result_file))
                            {
                                string extension = System.IO.Path.GetExtension(lgoRuntime.result_file);
                                System.Drawing.Imaging.ImageFormat imgf = System.Drawing.Imaging.ImageFormat.Bmp;
                                if (!extension.ToLower().Equals(".bmp"))
                                {
                                    if (extension.ToLower().Equals(".png"))
                                        imgf = System.Drawing.Imaging.ImageFormat.Png;
                                    else if (extension.ToLower().Equals(".jpg"))
                                        imgf = System.Drawing.Imaging.ImageFormat.Jpeg;
                                    else if (extension.ToLower().Equals(".jpeg"))
                                        imgf = System.Drawing.Imaging.ImageFormat.Jpeg;
                                    else if (extension.ToLower().Equals(".gif"))
                                        imgf = System.Drawing.Imaging.ImageFormat.Gif;
                                }
                                savedImage.Save(lgoRuntime.result_file, imgf);
                            }
                            Console.WriteLine("Done in " + elapsedTime(start));
                            return 0;
                        }
                        else
                        {
                            Console.WriteLine(errors);  // Errors 
                            return -1;
                        }
                    }
                }
                else
                {
                    Console.WriteLine(errors);  // Errors 
                    return -1;
                }
            }
            else
            {
                Console.WriteLine("Invalid or missing LGO file");  // Errors 
                return -1;
            }

        }

        static string elapsedTime(DateTime start)
        {
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            List<string> lst = new List<string>();
            
            if(span.Duration().Days > 0)
                lst.Add( String.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s"));
            if( span.Duration().Hours > 0)
                lst.Add ( String.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s"));
            if( span.Duration().Minutes > 0 )
                lst.Add( String.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s"));
            if( span.Duration().Seconds > 0 )
                lst.Add(string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s"));
            if (span.Duration().Milliseconds > 0)
                lst.Add(string.Format("{0:0} milisecond{1}", span.Milliseconds, span.Milliseconds == 1 ? String.Empty : "s"));

            string retval = String.Join(", ", lst.ToArray());

            if (string.IsNullOrEmpty(retval))
                retval = "0 seconds";

            return retval;

        }

        private static void InitializeCanvas()
        {
            Image = new Bitmap(lgoRuntime.width, lgoRuntime.height);
            drawingSurface = Graphics.FromImage(Image);
            drawingSurface.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            lgoRuntime.turtle = new TurtleRuntime.Turtle(drawingSurface);
            lgoRuntime.turtle.Reset();

            savedImage = new Bitmap(Image);
            lgoRuntime.turtle.Draw();
        }
    }
}
