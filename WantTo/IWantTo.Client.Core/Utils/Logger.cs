#define __FILE_LOGGING__

using System;
using System.Threading;

#if __ANDROID__
using Android.Util;
#elif __IOS__
// nothing yet
#else
using log4net;
using log4net.Config;
#endif

#if __FILE_LOGGING__
using System.IO;
#endif

namespace IWantTo.Client.Core.Utils
{
    public class Logger
    {
#if __FILE_LOGGING__
        private const string LOGFILE = "iwantto.log";
        private static readonly object _logLocker = new object();
#endif

#if (!__ANDROID__ && !__IOS__)
        private readonly ILog _log;
#else
        private readonly string _name;
#endif

        static Logger()
        {
#if (!__ANDROID__ && !__IOS__)
            XmlConfigurator.Configure();
#endif
        }

        private Logger(string name)
        {
#if __ANDROID__
            _name = name.Length <= 23 ? name : name.Substring(0, 23); // Android limitation
#elif __IOS__
            _name = name;
#else
            _log = LogManager.GetLogger(name);
#endif
        }

        private Logger(Type type)
        {
#if __ANDROID__
            _name = type.Name.Length <= 23 ? type.Name : type.Name.Substring(0, 23); // Android limitation
#elif __IOS__
            _name = type.Name;
#else
            _log = LogManager.GetLogger(type);
#endif
        }

#if __FILE_LOGGING__
        public static string LogFilePath
        {
            get
            {
#if __ANDROID__
                var libraryPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "JobDispatch", "Logs");
                if (!Directory.Exists(libraryPath))
                {
                    Directory.CreateDirectory(libraryPath);
                }

#elif __IOS__
                // we need to put in /Library/ on iOS 5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
                var libraryPath = Path.Combine (documentsPath, "../Library/"); // Library folder
#else
                var libraryPath = string.Empty;
#endif
                return Path.Combine(libraryPath, LOGFILE);
            }
        }
#endif

        public static bool IsLogFileEnabled
        {
            get
            {
#if __FILE_LOGGING__
                return true;
#else
                return false;
#endif
            }
        }

        public static Logger Create(string name)
        {
            return new Logger(name);
        }

        public static Logger Create(Type type)
        {
            return new Logger(type);
        }

        #region Trace

        public void Trace(string message)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Verbose))
            {
                Log.Verbose(_name, message);
            }
#elif __IOS__
            Write("TRACE", message);
#else
            if (_log.IsDebugEnabled) // log4net does not support verbose or trace messages
            {
                _log.Debug(message);
            }
#endif

#if __FILE_LOGGING__
            LogWrite("TRACE", message);
#endif
        }

        public void Trace(string message, Exception e)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Verbose))
            {
                Log.Verbose(_name, "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
            }
#elif __IOS__
            WriteFormat("TRACE", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#else
            if (_log.IsDebugEnabled) // log4net does not support verbose or trace messages
            {
                _log.Debug(message, e);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("TRACE", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#endif

            if (e.InnerException != null)
            {
                Trace("Inner", e.InnerException);
            }
        }

        public void TraceFormat(string format, params object[] args)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Debug))
            {
                Log.Debug(_name, format, args);
            }
#elif __IOS__
            WriteFormat("TRACE", format, args);
#else
            if (_log.IsDebugEnabled) // log4net does not support verbose or trace messages
            {
                _log.DebugFormat(format, args);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("TRACE", format, args);
#endif
        }

        #endregion Trace

        #region Debug

        public void Debug(string message)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Debug))
            {
                Log.Debug(_name, message);
            }
#elif __IOS__
            Write("DEBUG", message);
#else
            if (_log.IsDebugEnabled)
            {
                _log.Debug(message);
            }
#endif

#if __FILE_LOGGING__
            LogWrite("DEBUG", message);
#endif
        }

        public void Debug(string message, Exception e)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Debug))
            {
                Log.Debug(_name, "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
            }
#elif __IOS__
            WriteFormat("DEBUG", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#else
            if (_log.IsDebugEnabled)
            {
                _log.Debug(message, e);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("DEBUG", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#endif

            if (e.InnerException != null)
            {
                Debug("Inner", e.InnerException);
            }
        }

        public void DebugFormat(string format, params object[] args)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Debug))
            {
                Log.Debug(_name, format, args);
            }
#elif __IOS__
            WriteFormat("DEBUG", format, args);
#else
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat(format, args);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("DEBUG", format, args);
#endif
        }

        #endregion Debug

        #region Info

        public void Info(string message)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Info))
            {
                Log.Info(_name, message);
            }
#elif __IOS__
            Write("INFO", message);
#else
            if (_log.IsInfoEnabled)
            {
                _log.Info(message);
            }
#endif

#if __FILE_LOGGING__
            LogWrite("INFO", message);
#endif
        }

        public void Info(string message, Exception e)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Info))
            {
                Log.Info(_name, "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
            }
#elif __IOS__
            WriteFormat("INFO", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#else
            if (_log.IsInfoEnabled)
            {
                _log.Info(message, e);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("INFO", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#endif

            if (e.InnerException != null)
            {
                Info("Inner", e.InnerException);
            }
        }

        public void InfoFormat(string format, params object[] args)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Info))
            {
                Log.Info(_name, format, args);
            }
#elif __IOS__
            WriteFormat("INFO", format, args);
#else
            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat(format, args);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("INFO", format, args);
#endif
        }

        #endregion Info

        #region Warn

        public void Warn(string message)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Warn))
            {
                Log.Warn(_name, message);
            }
#elif __IOS__
            Write("WARN", message);
#else
            if (_log.IsWarnEnabled)
            {
                _log.Warn(message);
            }
#endif

#if __FILE_LOGGING__
            LogWrite("WARN", message);
#endif
        }

        public void Warn(string message, Exception e)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Warn))
            {
                Log.Warn(_name, "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
            }
#elif __IOS__
            WriteFormat("WARN", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#else
            if (_log.IsWarnEnabled)
            {
                _log.Warn(message, e);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("WARN", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#endif

            if (e.InnerException != null)
            {
                Warn("Inner", e.InnerException);
            }
        }

        public void WarnFormat(string format, params object[] args)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Warn))
            {
                Log.Warn(_name, format, args);
            }
#elif __IOS__
            WriteFormat("WARN", format, args);
#else
            if (_log.IsWarnEnabled)
            {
                _log.WarnFormat(format, args);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("WARN", format, args);
#endif
        }

        #endregion Warn

        #region Error

        public void Error(string message)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Error))
            {
                Log.Error(_name, message);
            }
#elif __IOS__
            Write("ERROR", message);
#else
            if (_log.IsErrorEnabled)
            {
                _log.Error(message);
            }
#endif

#if __FILE_LOGGING__
            LogWrite("ERROR", message);
#endif
        }

        public void Error(string message, Exception e)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Error))
            {
                Log.Error(_name, "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
            }
#elif __IOS__
            WriteFormat("ERROR", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#else
            if (_log.IsErrorEnabled)
            {
                _log.Error(message, e);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("ERROR", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#endif

            if (e.InnerException != null)
            {
                Error("Inner", e.InnerException);
            }
        }

        public void ErrorFormat(string format, params object[] args)
        {
#if __ANDROID__
            if (Log.IsLoggable(_name, LogPriority.Error))
            {
                Log.Error(_name, format, args);
            }
#elif __IOS__
            WriteFormat("ERROR", format, args);
#else
            if (_log.IsErrorEnabled)
            {
                _log.ErrorFormat(format, args);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("ERROR", format, args);
#endif
        }

        #endregion Error

        #region Fatal

        public void Fatal(string message)
        {
#if __ANDROID__
            Log.Wtf(_name, message);
#elif __IOS__
            Write("FATAL", message);
#else
            if (_log.IsFatalEnabled)
            {
                _log.Fatal(message);
            }
#endif

#if __FILE_LOGGING__
            LogWrite("FATAL", message);
#endif
        }

        public void Fatal(string message, Exception e)
        {
#if __ANDROID__
            Log.Wtf(_name, "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#elif __IOS__
            WriteFormat("FATAL", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#else
            if (_log.IsFatalEnabled)
            {
                _log.Fatal(message, e);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("FATAL", "{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);
#endif

            if (e.InnerException != null)
            {
                Fatal("Inner", e.InnerException);
            }
        }

        public void FatalFormat(string format, params object[] args)
        {
#if __ANDROID__
            Log.Wtf(_name, format, args);
#elif __IOS__
            WriteFormat("FATAL", format, args);
#else
            if (_log.IsFatalEnabled)
            {
                _log.FatalFormat(format, args);
            }
#endif

#if __FILE_LOGGING__
            LogWriteFormat("FATAL", format, args);
#endif
        }

        #endregion Fatal

        /// <summary>
        /// Console.WriteLine is only emitted in DEBUG mode, so it won't occur
        /// on a real device in the distribution version.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        private void Write(string level, string message)
        {
#if (!__ANDROID__ && !__IOS__)
            Console.WriteLine("{0} {1} - {2} - {3} - {4}", level.PadRight(5), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _log.Logger.Name, message);
#else
            Console.WriteLine("{0} {1} - {2} - {3} - {4}", level.PadRight(5), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _name, message);
#endif
        }

        /// <summary>
        /// Console.WriteLine is only emitted in DEBUG mode, so it won't occur
        /// on a real device in the distribution version.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        private void WriteFormat(string level, string format, params object[] args)
        {
#if (!__ANDROID__ && !__IOS__)
            Console.WriteLine("{0} {1} - {2} - {3} - {4}", level.PadRight(5), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _log.Logger.Name, string.Format(format, args));
#else
            Console.WriteLine("{0} {1} - {2} - {3} - {4}", level.PadRight(5), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _name, string.Format(format, args));
#endif
        }

#if __FILE_LOGGING__
        private void LogWrite(string level, string message)
        {
            lock (_logLocker)
            {
                using (var w = File.AppendText(LogFilePath))
                {
#if (!__ANDROID__ && !__IOS__)
                    w.WriteLine("{0} {1} - {2} - {3} - {4}", level.PadRight(5), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _log.Logger.Name, message);
#else
                    w.WriteLine("{0} {1} - {2} - {3} - {4}", level.PadRight(5), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _name, message);
#endif
                }
            }
        }

        private void LogWriteFormat(string level, string format, params object[] args)
        {
            lock (_logLocker)
            {
                using (var w = File.AppendText(LogFilePath))
                {
#if (!__ANDROID__ && !__IOS__)
                    w.WriteLine("{0} {1} - {2} - {3} - {4}", level.PadRight(5), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _log.Logger.Name, string.Format(format, args));
#else
                    w.WriteLine("{0} {1} - {2} - {3} - {4}", level.PadRight(5), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _name, string.Format(format, args));
#endif
                }
            }
        }

        public void StackTraceDump(string message)
        {
            lock (_logLocker)
            {
                using (var w = File.AppendText(LogFilePath))
                {
                    var logMessage = string.Format("{0}\r\n{1}", message, Environment.StackTrace);

#if (!__ANDROID__ && !__IOS__)
                    w.WriteLine("STACK {0} - {1} - {2} - {3}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _log.Logger.Name, logMessage);
#else
                    w.WriteLine("STACK {0} - {1} - {2} - {3}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _name, logMessage);
#endif
                }
            }
        }

        public void CrashDump(string message, Exception e)
        {
            lock (_logLocker)
            {
                using (var w = File.AppendText(LogFilePath))
                {
                    var logMessage = string.Format("{0} Exception: {1}\r\n{2}", message, e.Message, e.StackTrace);

#if (!__ANDROID__ && !__IOS__)
                    w.WriteLine("CRASH {0} - {1} - {2} - {3}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _log.Logger.Name, logMessage);
#else
                    w.WriteLine("CRASH {0} - {1} - {2} - {3}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, _name, logMessage);
#endif
                }
            }

            if (e.InnerException != null)
            {
                CrashDump("Inner", e.InnerException);
            }
        }

        public static void Read(Stream outputStream)
        {
            Read(outputStream, 0);
        }

        public static void Read(Stream outputStream, long tailBytesCount)
        {
            lock (_logLocker)
            {
                if (!File.Exists(LogFilePath))
                {
                    return;
                }

                using (var logFile = File.OpenRead(LogFilePath))
                {
                    if (tailBytesCount > 0 && logFile.Length > tailBytesCount)
                    {
                        logFile.Seek(-tailBytesCount, SeekOrigin.End);
                    }

                    var buffer = new byte[16384]; // 16 kB
                    int bytesRead;

                    while ((bytesRead = logFile.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        public static void Clear()
        {
            lock (_logLocker)
            {
                if (File.Exists(LogFilePath))
                {
                    File.Delete(LogFilePath);
                }
            }
        }
#endif
    }
}
