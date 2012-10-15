//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="muvee Technologies Pte Ltd">
//   Copyright (c) muvee Technologies Pte Ltd. All rights reserved.
// </copyright>
// <author>Jerry Chong</author>
//-----------------------------------------------------------------------

namespace SysInfoDump
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using SysInfoSharp;

    /// <summary>
    /// Main program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program entry function
        /// </summary>
        /// <param name="args">Command line arguments</param>
        [STAThread]
        public static void Main(string[] args)
        {
            // specify writing destination
            string outputFile = Path.Combine(@"C:\muveeDebug", "sysinfoout.txt");
            string filter = string.Empty;

            // allow specifying an output file
            if (args.Length > 0)
            {
                outputFile = args[0];
            }

            // allow specifying a category to print
            if (args.Length > 1)
            {
                filter = args[1];
            }

            using (var strm = File.Open(outputFile, FileMode.Create))
            {
                // initialize loggers - any output written to the file will also be printed
                Trace.Listeners.Clear();
                Trace.Listeners.Add(new ConsoleTraceListener());
                Trace.Listeners.Add(new TextWriterTraceListener(strm));
                Trace.AutoFlush = true;

                // dumping system info
                var sysInfo = new SysInfoLib();
                sysInfo.Init();
                foreach (var category in sysInfo.GetCategories())
                {
                    if (!string.IsNullOrEmpty(filter) && !string.Equals(filter, category, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    Trace.WriteLine("--------------------------------------------");
                    Trace.WriteLine("Category: " + category);
                    Trace.WriteLine("--------------------------------------------");

                    foreach (var pair in sysInfo[category])
                    {
                        Trace.WriteLine(string.Format("\t{0}={1}", pair.Key, pair.Value));
                    }
                }
            }
        }
    }
}
