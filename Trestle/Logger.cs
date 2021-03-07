using System;
using System.IO;
using System.Reflection;

namespace Trestle
{
    public class Logger
    {
        private static StreamWriter _writer { get; set; }

        public static void Debug(string message)
            => Log(LogVerbosity.Debug, message);
        
        public static void Info(string message)
            => Log(LogVerbosity.Info, message);
        
        public static void Warn(string message)
            => Log(LogVerbosity.Warn, message);

        public static void Error(string message)
            => Log(LogVerbosity.Error, message);
        
        public static void Special(string message)
            => Log(LogVerbosity.Motd, message);
        
        private enum LogVerbosity
        {
            Debug,
            Info,
            Warn,
            Error,
            Motd
        }
        
        private static void Log(LogVerbosity verbosity, string message)
        {
            if (_writer == null)
                Initialize();

            if (verbosity == LogVerbosity.Debug && !Config.Debug)
                return;
            
            Console.ForegroundColor = verbosity switch
            {
                LogVerbosity.Debug => ConsoleColor.DarkGray,
                LogVerbosity.Warn => ConsoleColor.Yellow,
                LogVerbosity.Error => ConsoleColor.Red,
                LogVerbosity.Motd => ConsoleColor.Cyan,
                _ => Console.ForegroundColor
            };
            
            var data = $"[{DateTime.UtcNow} - {verbosity.ToString().ToUpper()}] {message}";
            lock (_writer)
            {
                _writer.WriteLine(data);
                _writer.Flush();
            }
            
            Console.WriteLine(data);
            Console.ResetColor();
        }

        private static void Initialize()
        {
            var path = Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "logs", "Trestle.log");
            if (File.Exists(path))
                File.Move(path, Path.Join(Path.GetDirectoryName(path), $"Trestle-{((DateTimeOffset)File.GetCreationTime(path)).ToUnixTimeSeconds()}.log"));

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            _writer = new StreamWriter(path);
        }
    }
}