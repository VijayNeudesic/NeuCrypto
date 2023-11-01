using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace NeuCrypto
{
    public class Logger
    {
        public enum LogLevel
        {
            Debug = 0,
            Info,
            Warning,
            Error,
            Fatal
        }

        private bool bInitialized = false;

        public Logger()
        {
        }

        public void InitLogs(string path=@".", ILogger serilogger = null)
        { 
            if(serilogger != null)
            {
                Log.Logger = serilogger;
                bInitialized = true;
                return;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(path + "\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            bInitialized = true;
        }

        public void LogMessage(LogLevel level, string message)
        {
            if (!bInitialized)
            {
                return;
            }

            switch (level)
            {
                case LogLevel.Debug:
                    Log.Debug(message);
                    break;
                case LogLevel.Info:
                    Log.Information(message);
                    break;
                case LogLevel.Warning:
                    Log.Warning(message);
                    break;
                case LogLevel.Error:
                    Log.Error(message);
                    break;
                case LogLevel.Fatal:
                    Log.Fatal(message);
                    break;
            }
        }

    }
}
