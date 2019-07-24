using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilsLib
{
    public enum LogType
    {
        Debug,
        Common,
        Warning,
        Error,
        System
    }
    public struct LogMsgType
    {
        public DateTime time { set; get; }
        public LogType type { set; get; }
        public string Msg { set; get; }
    }
    public class Log
    {
        private bool DisplayOnConsole = false;
        private static Log LogInstance = null;
        private string logPath = string.Empty;
        private string fileName = string.Empty;
        private string logDirectory = Environment.CurrentDirectory + "\\Log\\";
        private LogType TraceLevel = LogType.Common;
        private LogMsgType logMsg;
        Dictionary<LogType, string> logPairs;
        public Log(string LogPath)
        {

            if (LogPath.Contains("."))
            {
                fileName = Path.GetFileName(LogPath);
                //D:\\Dir\\Dir\\file.log
                if (LogPath.Contains(":\\"))
                    logPath = LogPath;
                //\\Dir\\file.log
                else if (LogPath.Contains("\\") && LogPath.IndexOf("\\") != LogPath.LastIndexOf("\\"))
                    //This will Transformed as D:\AppDir\LogPath\xxx.log
                    logPath = Environment.CurrentDirectory + "\\" + LogPath;
                else
                    logPath = logDirectory + LogPath;
            }
            else
            {
                //{LogPath}\\{Applications Name}.log
                logPath = Path.Combine(LogPath, Path.GetFileName(Assembly.GetExecutingAssembly().Location), ".log");
            }
            SetTraceLevel(LogType.Common);
            LogTypeFormat();
        }
        public Log()
        {

            SetTraceLevel(LogType.Common);
            LogTypeFormat();
        }
        /// <summary>
        /// Create Log files(include Directory)
        /// </summary>
        /// <param name="filepath">a Full Path</param>
        private void CreateFiles(string filepath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filepath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            }
            if (!File.Exists(filepath))
            {

                File.Create(filepath).Close();
            }
            this.logPath = filepath;
        }
        public static Log Init()
        {
            if (LogInstance == null)
            {
                LogInstance = new Log();
            }
            return LogInstance;
        }
        public static Log Init(string LogPath)
        {
            if (LogInstance == null)
            {
                LogInstance = new Log(LogPath);
            }
            return LogInstance;
        }
        public static Log GetInstance()
        {
            if (LogInstance != null)
            {
                return LogInstance;
            }
            else return null;
        }
        /// <summary>
        /// Change Log Directory to a new one 
        /// </summary>
        /// <param name="path"></param>
        public void SetLogDirectory(string path)
        {
            logDirectory = path;
            logPath = Path.Combine(logDirectory, fileName);
        }
        public void SetLogFileName(string FileName)
        {
            this.fileName = FileName;
           
            this.logPath = logDirectory + fileName;
        }
        public void SetTraceLevel(LogType level)
        {
            this.TraceLevel = level;
        }
        public bool getLastErr(ref string Msg)
        {
            if (LogInstance.logMsg.Msg == null)
            {
                return false;
            }
            Msg = LogInstance.logMsg.Msg;
            return true;
        }
        public virtual bool getLastErr(ref LogMsgType Msg)
        {
            if (logMsg.Msg == null)
                return false;
            Msg = logMsg;
            return true;
        }
        public virtual string BuildLogMsg(string Msg, LogType type)
        {
            logMsg.Msg = Msg;
            logMsg.type = type;
            logMsg.time = DateTime.UtcNow;
            return $"[{logMsg.time}][{logPairs[logMsg.type]}]:{logMsg.Msg}";
        }
        public virtual string BuildLogMsg(string Msg, LogType type, string path)
        {
            logMsg.Msg = Msg;
            logMsg.type = type;
            logMsg.time = DateTime.UtcNow;
            return $"[{logMsg.time}][{Path.GetFileNameWithoutExtension(path)}][{logPairs[logMsg.type]}]:{logMsg.Msg}";

        }
        public virtual void LogTypeFormat(Dictionary<LogType, string> pairs = null)
        {
            logPairs = new Dictionary<LogType, string>();
            logPairs.Add(LogType.Debug, "Debug");
            logPairs.Add(LogType.Common, "Common");
            logPairs.Add(LogType.Warning, "Warning");
            logPairs.Add(LogType.Error, "Error");
            logPairs.Add(LogType.System, "System");
        }
        public virtual bool Write(string Msg, LogType type = LogType.Common)
        {
            lock (this)
            {
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(logPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                    if (!File.Exists(logPath))
                        CreateFiles(logPath);

                    if (type >= TraceLevel)
                        using (FileStream writer = new FileStream(logPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            Byte[] buffer = Encoding.Default.GetBytes(BuildLogMsg(Msg, type) + System.Environment.NewLine);
                            Console.WriteLine(BuildLogMsg(Msg, type));
                            writer.Seek(0, SeekOrigin.End);
                            writer.Write(buffer, 0, buffer.Length);
                            writer.Flush();
                            writer.Close();
                        }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    this.SysErr(e.Message);
                    throw;
                }
            }
        }
        /// <summary>
        /// Only For Core Err Functions logs
        /// </summary>
        /// <param name="Msg">Err Msg</param>
        /// <param name="type">Defalut is LogType.System</param>
        /// <param name="FilePath">a Full Path</param>
        /// <returns>if Success return true</returns>
        public virtual bool Write(string Msg, LogType type, string FilePath)
        {

            lock (this)
            {
                try
                {
                    if (!File.Exists(logPath))
                        CreateFiles(logPath);
                    if (DisplayOnConsole)
                        Console.WriteLine(BuildLogMsg(Msg, type, FilePath));
                    if (type >= TraceLevel)
                        using (FileStream writer = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            Byte[] buffer = Encoding.Default.GetBytes(BuildLogMsg(Msg, type, FilePath) + System.Environment.NewLine);
                            writer.Seek(0, SeekOrigin.End);
                            writer.Write(buffer, 0, buffer.Length);
                            writer.Flush();
                            writer.Close();
                        }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    this.SysErr(e.Message);
                    return false;
                }
            }
        }
        public static int GetLineNum()

        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            return st.GetFrame(0).GetFileLineNumber();
        }
        public static string GetCurSourceFileName()

        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1, true);
            return st.GetFrame(0).GetFileName();
        }
        public virtual void Common(string Msg)
        {
            Write(Msg, LogType.Common);
        }
        public virtual void Common(string Msg, string path)
        {
            Write(Msg, LogType.Common, path);
        }
        public virtual void Debug(string Msg)
        {
            Write(Msg, LogType.Debug);
        }
        public virtual void Debug(string Msg, string path)
        {
            Write(Msg, LogType.Debug, path);
        }
        public virtual void Warning(string Msg)
        {
            Write(Msg, LogType.Warning);
        }
        public virtual void Warning(string Msg, string path)
        {
            Write(Msg, LogType.Warning, path);
        }
        public virtual void Error(string Msg)
        {
            Write(Msg, LogType.Error);
        }
        public virtual void Error(string Msg, string path)
        {
            Write(Msg, LogType.Error, path);
        }
        public virtual void SysErr(string Msg)
        {
            try
            {
                Msg += $" At File{GetCurSourceFileName()} Line:{GetLineNum()}";
                Write(Msg, LogType.System);
            }
            catch (Exception e)
            {
                string TempFilePath = Path.Combine(Path.GetDirectoryName(logPath),
                    "CoreErr.log");
                CreateFiles(
                    TempFilePath
                    );
                Write(Msg, LogType.System, TempFilePath);
                Write(e.Message, LogType.System, TempFilePath);
                throw;
            }

        }
        public virtual void SysErr(string Msg, string path)
        {
            try
            {
                Msg += $" At File{GetCurSourceFileName()} Line:{GetLineNum()}";
                Write(Msg, LogType.System, path);
            }
            catch (Exception e)
            {
                string TempFilePath = Path.Combine(Path.GetDirectoryName(logPath),
                    "CoreErr.log");
                CreateFiles(
                    TempFilePath
                    );
                Write(Msg, LogType.System, TempFilePath);
                Write(e.Message, LogType.System, TempFilePath);
                throw;
            }

        }

        public void SetDisPlay(bool toggle)
        {
            DisplayOnConsole = toggle;
        }
    }
}
