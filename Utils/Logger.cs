using System;
using System.IO;
using System.Text;

namespace Utils.Logging
{
    public sealed class Logger
    {
        public enum Severity
        {
            Debug,
            Warning,
            Error
        }

        private static volatile Logger _instance;
        private static object _syncRoot = new object();


        private StreamWriter _logStream;
        private volatile bool _isOpen;

        private Logger (string path)
        {
            _logStream = new StreamWriter(path, true);
            _logStream.AutoFlush = true;
            _isOpen = true;
        }

        public void Write (Severity sev, string msg)
        {
            if (_isOpen == false)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            
            sb.Append("[");
            if (sev == Severity.Debug)
            {
                sb.Append("D");
            }
            else if (sev == Severity.Warning)
            {
                sb.Append("W");
            }
            else if (sev == Severity.Error)
            {
                sb.Append("E");
            }
            sb.Append("]");

            sb.Append("[");
            sb.Append(DateTime.Now.ToString("dd/MM/yy HH:mm:ss"));
            sb.Append("] ");
            sb.Append(msg);

			string line = sb.ToString ();
			_logStream.WriteLine (line);
#if DEBUG
			Console.WriteLine (line);
#endif
        }

        public void Close ()
        {
            if (_isOpen == true)
            {
                _isOpen = false;
                _logStream.Close ();
            }
        }

        public static Logger Setup (string path) 
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger(path);
                    }
                }
            }

            return _instance;
        }

        public static Logger Instance 
        {
            get
            {
                lock (_syncRoot)
                {
                    return _instance;
                }
            }
        }
    }
}

