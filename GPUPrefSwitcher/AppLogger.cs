using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPUPrefSwitcher
{
    /// <summary>
    /// Instantiates a logger that logs to two distinct files: a standard log, and an error log.
    /// The Standard Log is buffered (not written immediately) by default. It is only dumped to the file upon an unhandled exception, or if live logging is set to true.
    /// Logs that have their file size exceed 1MB are moved into an archive folder.
    /// 
    /// If the app throws an unhandled exception before it successfully initializes, the Event Viewer can still help to find errors.
    /// </summary>
    public class AppLogger
    {
        private string LogFolderPath_Error { get; init; }
        private string LogFileName_Error { get; init; }
        public string LogFilePath_Error { get => Path.Combine(LogFolderPath_Error, LogFileName_Error); }

        private SemaphoreSlim semaphoreSlim_Error = new SemaphoreSlim(1);
        private StreamWriter outputFile_Error;

        private string LogFolderPath_Standard { get; init; }
        private string LogFileName_Standard { get; init; }
        public string LogFilePath_Standard { get => Path.Combine(LogFolderPath_Standard, LogFileName_Standard); }

        private SemaphoreSlim semaphoreSlim_Standard = new SemaphoreSlim(1);
        private StreamWriter outputFile_Standard;

        public string ArchiveFolderPath { get; init; }

        /// <summary>
        /// Controls whether <see cref="Log"/> will also log the same message using <see cref="System.Diagnostics.Debug.WriteLine"/>
        /// </summary>
        public bool AlsoWriteToSystemDiagnosticsDebug = true;

        internal static int LogFileSizeLimitKB = 1024 * 1024; //1MB

        private readonly int WriteBlockWarningLengthMillis = 1000 * 5; //TODO what if the log file is also locked?

        private AppLogger()
        {
            
        }

        /// <summary>
        /// Creates a new Logger that subscribes to <see cref="AppDomain.CurrentDomain.UnhandledException"/> and works with the provided paths 
        /// for logging (which will also be created if they don't exist). 
        /// If there are already files in the location, if they are over <see cref="LogFileSizeLimitKB"/>, it will move them to the archive path
        /// and create a new file in place.
        /// It will also append some messages marking the beginning of logging (the same time as this was called).
        /// </summary>
        /// <param name="logFolderPath_Error"></param>
        /// <param name="logFileName_Error"></param>
        /// <param name="logFolderPath_Standard"></param>
        /// <param name="logFileName_Standard"></param>
        /// <param name="archiveFolderPath"></param>
        /// <returns></returns>
        public static AppLogger Initialize (string logFolderPath_Error, string logFileName_Error, string logFolderPath_Standard, string logFileName_Standard, string archiveFolderPath)
        {

            AppLogger logger = new AppLogger
            {
                LogFolderPath_Error = logFolderPath_Error,
                LogFileName_Error = logFileName_Error,
                LogFolderPath_Standard = logFolderPath_Standard,
                LogFileName_Standard = logFileName_Standard,
                ArchiveFolderPath = archiveFolderPath
            };

            //create the oldDirectory directory if it does not exist
            if (!Directory.Exists(logger.ArchiveFolderPath)) Directory.CreateDirectory(logger.ArchiveFolderPath);

            string currentStandardLogFilePath = logger.LogFilePath_Standard;
            //move the log file to archive if it's full
            if (File.Exists(currentStandardLogFilePath) && new FileInfo(currentStandardLogFilePath).Length > LogFileSizeLimitKB)
            {
                logger.ArchiveLogFile(currentStandardLogFilePath);
            }

            string currentErrorLogFilePath = logger.LogFileName_Error;
            //move the log file to the archive if it's full
            if (File.Exists(currentErrorLogFilePath) && new FileInfo(currentErrorLogFilePath).Length > LogFileSizeLimitKB)
            {
                logger.ArchiveLogFile(currentErrorLogFilePath);
            }

            //these will create a new file if they do not exist
            logger.outputFile_Error = new StreamWriter(logger.LogFilePath_Error, true);
            logger.outputFile_Standard = new StreamWriter(logger.LogFilePath_Standard, true);

            logger.outputFile_Error.WriteLine(); //prepend newlines, looks better
            logger.outputFile_Standard.WriteLine();

            logger.ErrorLog("---<<<<< BEGIN ERROR LOG SESSION >>>>>--- \n(Note that newer logs go at the end of the file, you can press CTRL+END to skip there in most editors)\n").Wait();
            logger.ForceStandardLog("---<<<<< BEGIN STANDARD LOG SESSION >>>>>--- \n(Note that newer logs go at the end of the file, you can press CTRL+END to skip there in most editors)\n");

            //AppDomain.CurrentDomain.FirstChanceException += (sender, e) => //FirstChanceException triggers upon ALL exceptions, even those caught; this is excessive, and they can still be observed from Event Viewer

            //Might trigger StackOverflow if the functions inside this errors (log error -> error -> log error -> error ...)
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => //Only handles uncaught ones (that terminate the app), which is what we want
            {
                logger.DumpStandardLogBufferToStandardLog().Wait();
                logger.DumpStandardLogBufferToErrorLog().Wait(); //do this before the app terminates
                logger.ErrorLog("<<<AN UNHANDLED ERROR HAS OCCURRED>>>").Wait();
                logger.ErrorLog(e.ExceptionObject.ToString()).Wait();
            };

            //useful for capturing fire-and-forget tasks that error out
            ///TODO this doesn't work?
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) => 
            {
                logger.DumpStandardLogBufferToStandardLog().Wait();
                logger.DumpStandardLogBufferToErrorLog().Wait(); //do this before the app terminates
                logger.ErrorLog("<<<AN UNOBSERVED TASK EXCEPTION HAS OCCURRED>>>").Wait();
                logger.ErrorLog(e.ToString()).Wait();
                
            };

            return logger;
        }

        private void ArchiveLogFile(string logFilePath)
        {
            string fileName = Path.GetFileName(logFilePath);

            string archiveFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{fileName}";

            File.Move(logFilePath, Path.Combine(ArchiveFolderPath, archiveFileName));
        }


        private long errorLogCount = 0;
        private long standardLogCount = 0;
        private long TotalLogCount
        {
            get => errorLogCount + standardLogCount;
        }

        public bool EnableRealtimeStandardLogWrites { get; set; } = false; //constant I/O operations like these will take power
        private List<string> StandardLogBuffer = new();
        internal Task DumpStandardLogBufferToStandardLog()
        {

            Log("DumpStandardLogBufferToStandardLog() triggered");//needs to come before otherwise it doesn't get added

            StringBuilder sb = new StringBuilder();
            foreach(string message in StandardLogBuffer)
            {
                sb.AppendLine(message);
            }
            return WriteAsyncInternal_Standard(sb.ToString());
        }
        internal Task DumpStandardLogBufferToErrorLog()
        {
            Log("DumpStandardLogBufferToErrorLog() triggered");//needs to come before otherwise it doesn't get added

            StringBuilder sb = new StringBuilder();
            foreach (string message in StandardLogBuffer)
            {
                sb.AppendLine(message);
            }
            
            return WriteAsyncInternal_Error(sb.ToString());
        }

        /// <summary>
        /// Unconditionally writes a string to the error file (with info like log count and time appended). 
        /// Awaiting this is not necessary because it will block if the writer is busy.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Task ErrorLog(string str)
        {

            errorLogCount += 1;

            string toPrint = $"[(T:{TotalLogCount}, {errorLogCount}) {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}]: {str}";

            semaphoreSlim_Error.AvailableWaitHandle.WaitOne();//block if unavailable

            return WriteAsyncInternal_Error(toPrint);
        }

        public int GlobalLogLevel
        {
            get
            {
                return globalLogLevel;
            }
            set
            {
                if (value < 0) throw new ArgumentException("Can't set LogLevel to negative number");
                ForceStandardLog($"LogLevel set to {value}");
                globalLogLevel = value;
            }
        }
        private int globalLogLevel = 1000;
        //In general, higher LogLevels than this can be used for messages that are logged frequently.
        public static int skippedLogs = 0;

        /// <summary>
        /// Queues a string to the "Standard" log file if and only if the LogLevel parameter (default=1000) is equal to or lower than <see cref="GlobalLogLevel"/> (default=1000).
        /// The string will be prepended with extra info such as Log count, time, and LogLevel. 
        /// Unless <see cref="EnableRealtimeStandardLogWrites"/> is true, the string will also be buffered and written to the file only when the app encounters an
        /// an unhandled exception or gracefully terminates.
        /// If realtime writes are enabled, this will block if an Async write is already in progress.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public void Log(string str, int logLevel = 1000)
        {

            if(logLevel < 0) { throw new ArgumentOutOfRangeException("logLevel must be positive"); }
            if(logLevel > GlobalLogLevel)
            {
                skippedLogs++;
                return;
            }

            standardLogCount += 1;

            string toWrite = FormatMessageForStandardLog(str, logLevel);

            if (EnableRealtimeStandardLogWrites)
            {
                semaphoreSlim_Standard.AvailableWaitHandle.WaitOne();//block if there's still a write in progress
                _ = WriteAsyncInternal_Standard(toWrite);
            }
            
            StandardLogBuffer.Add(toWrite);
            return;
             
        }

        /// <summary>
        /// Writes a (formatted for log) string to the standard log file regardless of LogLevel, skips buffering, and blocks
        /// </summary>
        /// <param name="str"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        private void ForceStandardLog(string str)
        {
            WriteAsyncInternal_Standard(FormatMessageForStandardLog(str, -1)).Wait();
        }

        public string FormatMessageForStandardLog(string str, int logLevel)
        {
            return $"[(T:{TotalLogCount}, {standardLogCount}) {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} (LL={logLevel})]: {str}";
        }

        /// <summary>
        /// Unconditionally writes the string to the standard log file. Try not to call this; other standard log functions just defer to this to reduce code duplication
        /// </summary>
        /// <param name="str"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        private async Task WriteAsyncInternal_Standard(string str)
        {
            await semaphoreSlim_Standard.WaitAsync();

            try
            {
                await outputFile_Standard.WriteLineAsync(str);
                await outputFile_Standard.FlushAsync();
                System.Diagnostics.Debug.WriteLine(str);
            }
            finally
            {
                semaphoreSlim_Standard.Release();
            }

        }

        /// <summary>
        /// Unconditionally writes to the error log, does not modify the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private async Task WriteAsyncInternal_Error(string str)
        {

            await semaphoreSlim_Error.WaitAsync();

            try
            {
                await outputFile_Error.WriteLineAsync(str);
                await outputFile_Error.FlushAsync();
                System.Diagnostics.Debug.WriteLine(str);
            }
            finally
            {
                semaphoreSlim_Error.Release();
            }
        }

        public void WaitForFinishAndRelease()
        {
            semaphoreSlim_Error.Wait();
            semaphoreSlim_Standard.Wait();
            semaphoreSlim_Error.Release();
            semaphoreSlim_Standard.Release();
        }

        public void Dispose()
        {
            outputFile_Error.Close();
            outputFile_Standard.Close();
            semaphoreSlim_Error.Dispose();
            semaphoreSlim_Standard.Dispose();
        }
    }
}
