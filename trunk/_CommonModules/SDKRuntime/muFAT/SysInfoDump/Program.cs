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
    using System.Linq;
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
            var outputFile = args.Length > 0 ? args[0] : Path.Combine(@"C:\muveeDebug", "sysinfoout.txt");
            
            // allow specifying a category to print
            var filter = args.Length > 1 ? args[1] : string.Empty;

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
                foreach (var category in sysInfo.GetCategories().Where(
                    category =>
                        string.IsNullOrEmpty(filter) ||
                        string.Equals(filter, category, StringComparison.OrdinalIgnoreCase)))
                {
                    Trace.WriteLine(@"
--------------------------------------------
Category: " + category + @"
--------------------------------------------");

                    foreach (var pair in sysInfo[category])
                    {
                        Trace.WriteLine(string.Format("\t{0}={1}", pair.Key, pair.Value));
                    }
                }
            }
        }
    }
}
