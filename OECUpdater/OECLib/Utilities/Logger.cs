#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECLib.Utilities
{
    public class Logger
    {
		public static void Initialize() {
			Trace.Listeners.Add (new TextWriterTraceListener ("test.log"));
			Trace.AutoFlush = true;
		}

        public static void WriteLine(string format, params object[] args) 
        {
            Trace.WriteLine(String.Format("["+DateTime.Now.ToString("yy-MM-dd hh:mm:ss")+"] "+format, args));
        }

        public static void WriteLine(string text)
        {
            Trace.WriteLine("[" + DateTime.Now.ToString("yy-MM-dd hh:mm:ss") + "] " + text);
        }

        public static void WriteError(string text)
        {
            Trace.TraceError("[" + DateTime.Now.ToString("yy-MM-dd hh:mm:ss") + "] " + text);
        }

        public static void WriteError(string format, params object[] args)
        {
            Trace.TraceError("[" + DateTime.Now.ToString("yy-MM-dd hh:mm:ss") + "] " + format, args);
            
        }

        public static void WriteWarning(string text)
        {
            Trace.TraceWarning("[" + DateTime.Now.ToString("yy-MM-dd hh:mm:ss") + "] " + text);
        }

        public static void WriteWarning(string format, params object[] args)
        {
            Trace.TraceWarning("[" + DateTime.Now.ToString("yy-MM-dd hh:mm:ss") + "] " + format, args);

        }
    }
}
