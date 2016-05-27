using System;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using log4net;
using log4net.Appender;
using log4net.Config;

using Newtonsoft.Json;
using Common.Logging;

namespace TE.Common.Logging
{
    /// <summary>
    /// Below are guidelines to use to figure out what level messages/exceptions should be logged.
    /// 
    /// FATAL: The app (or at the very least a thread) is about to die horribly. This is where the
    /// info explaining why that's happening goes.
    /// 
    /// ERROR: Something that the app's doing that it shouldn't. This isn't a user error ('invalid search query');
    /// it's an assertion failure, network problem, etc etc., probably one that is going to abort the current operation
    /// 
    /// WARN: Something that's concerning but not causing the operation to abort; # of connections in the DB pool getting low, 
    /// an unusual-but-expected timeout in an operation, etc. I often think of 'WARN' as something that's useful in aggregate; 
    /// e.g. grep, group, and count them to get a picture of what's affecting the system health
    /// 
    /// INFO: Normal logging that's part of the normal operation of the app; diagnostic stuff so you can go back and say 'how 
    /// often did this broad-level operation happen?', or 'how did the user's data get into this state?'
    /// 
    /// DEBUG: Off by default, able to be turned on for debugging specific unexpected problems. This is where you might log 
    /// detailed information about key method parameters or other information that is useful for finding likely problems in
    /// specific 'problematic' areas of the code. 
    /// </summary>
    public sealed class Logger
    {
        /// <summary>
        /// String formatter for function entry
        /// </summary>
        private const string FunctionEntryString = "Entry";

        /// <summary>
        /// String formatter for function exit
        /// </summary>
        private const string FunctionExitString = "Exit";

        private Logger(String type)
        {
            Log4NetLogger = LogManager.GetLogger(type);

            // log4net.ElasticSearch is used indirectly by TE.Common library, but this project does not have any code that explicitly references log4net.ElasticSearch. 
            // Therefore, when another project references this project, this project's assembly but not log4net.ElasticSearch. 
            // So in order to get the required log4net.ElasticSearch.dll copied over, we add some dummy code here (that never
            // gets called) that references log4net.ElasticSearch.dll; this will flag VS/MSBuild to copy the required log4net.ElasticSearch.dll over as well.
            var dummyElasticSearchInstance = typeof(log4net.ElasticSearch.ElasticSearchAppender);
            XmlConfigurator.Configure();
        }
        private static volatile Logger LoggerInstance;

        private static volatile Logger InstanceVerboseInstance;

        private static volatile Logger RequestErrorsInstance;

        private static object syncRoot = new Object();

        /// <summary>
        /// This returns an ID in the format GUID:ThreadId for the current executing thread.
        /// </summary>
        /// <returns></returns>
        public static String GetIdForCurrentThread()
        {
            return Trace.CorrelationManager.ActivityId + ":" + Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// This returns an ID in the format GUID the current executing activity (most of the times this will be an api call)
        /// </summary>
        /// <returns></returns>
        public static Guid GetIdForCurrentActivity()
        {
            return Trace.CorrelationManager.ActivityId;
        }

        private string LogMessageFormat
        {
            get { return GetIdForCurrentThread() + " ClassName: {0} MethodName: {1} Message: {2}"; }
        }

        private string ExceptionLogMessageFormat
        {
            get { return GetIdForCurrentThread() + " ClassName: {0} MethodName: {1} Message: {2} Exception: {3}"; }
        }

        private string ExceptionOnlyMessageFormat
        {
            get { return GetIdForCurrentThread() + " ClassName: {0} MethodName: {1} Exception: {2}"; }
        }

        private ILog Log4NetLogger = null;

        public static Logger Instance
        {
            get
            {
                if (LoggerInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (LoggerInstance == null)
                        {
                            LoggerInstance = new Logger(typeof(Logger).Name);
                        }
                    }
                }
                return LoggerInstance;
            }
        }

        public static Logger InstanceVerbose
        {
            get
            {
                if (InstanceVerboseInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (InstanceVerboseInstance == null)
                        {
                            InstanceVerboseInstance = new Logger("Verbose");
                        }
                    }
                }
                return InstanceVerboseInstance;
            }
        }
        public static Logger InstanceRequestErrors
        {
            get
            {
                if (RequestErrorsInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (RequestErrorsInstance == null)
                        {
                            RequestErrorsInstance = new Logger("RequestErrors");
                        }
                    }
                }
                return RequestErrorsInstance;
            }
        }

        /// <summary>
        /// Use this method to debug messages to and from an external provider like Amadeus
        /// or KT Insurance API. Each request and response should be logged in the message parameter.
        /// Currently, this is wired to the Log4Net logging system. Once the message bus has been figured out, 
        /// the implementation would be switched to 
        /// a message bus.
        /// </summary>
        /// <param name="className">Class form which the logging has been invoked</param>
        /// <param name="methodName">Method form where logging has been enabled</param>
        /// <param name="message">The entire message, in XML or whatever format that has been sent/received from the
        /// provider. Each request should have it's debug call, as should a response.
        /// </param>
        public void DebugAPICall(string className, string methodName, object message)
        {
            //TODO: Refactor in send the messages to a message bus instead of the default Log4Net logger
            Debug(className, methodName, message);
        }

        /// <summary>
        /// Logs a Request to an Uri with an associated Payload. The Payload (request body)
        /// should be JSON convertable.
        /// </summary>
        public void DebugAPIRequest(string className, string methodName, Uri uri, object requestPayload)
        {
            string request = uri.ToString();

            if (requestPayload != null)
                request += " (" + JsonConvert.SerializeObject(requestPayload) + ")";

            DebugAPICall(className, methodName, request);

            //Trace.WriteLine(request); // REMOVED  BY ASHISH
        }

        /// <summary>
        /// Logs a response from an external API
        /// </summary>
        /// <param name="apiResponseId">An ID from the API that represents this response</param>
        public void DebugAPIResponse(string apiName, string methodName, string apiResponseId, HttpResponseMessage response, object responsePayload = null)
        {
            string responseLog = "ApiResponseId:" + apiResponseId;

            if (response != null)
                responseLog += ", " + response.StatusCode + ", " + response.RequestMessage + ", " + response.ReasonPhrase;

            if (responsePayload != null)
                responseLog += " (" + JsonConvert.SerializeObject(responsePayload) + ")";

            //Trace.WriteLine(responseLog); // REMOVED  BY ASHISH
            DebugAPICall("API-" + apiName + "-RES", methodName, responseLog);
        }

        /// Used by the API Logger ONLY (and the api calls made from the ui)
        public void APILogRequest(string apiName, string apiMethodName, string requestFileName = null)
        {
            Warn("API-" + apiName + "-REQ", apiMethodName, requestFileName);
        }

        /// <summary>
        /// Used by the API Logger  ONLY  (and the api calls made from the ui)
        /// </summary>
        public void APILogResponse(string apiName, string apiMethodName, string apiResponseId, string responseFileName = null)
        {
            string responseLog = "";

            if (responseFileName != null)
                responseLog += responseFileName;

            if (apiResponseId != null)
                responseLog = " ApiResponseId:" + apiResponseId;

            Warn("API-" + apiName + "-RES", apiMethodName, responseLog);
        }
        /// <summary>
        /// Used by the API Logger  ONLY  (and the api calls made from the ui)
        /// </summary>
        public void APILogException(string apiName, string apiMethodName, string apiResponseId, string responseFileName = null)
        {
            string responseLog = "";

            if (responseFileName != null)
                responseLog += responseFileName;

            if (apiResponseId != null)
                responseLog = " ApiResponseId:" + apiResponseId;

            Error("API-" + apiName + "-EXP", apiMethodName, responseLog);
        }

        /// <summary>
        /// Logs an exception at the Debug log level.
        /// DEBUG: Off by default, able to be turned on for debugging specific unexpected problems. This is where you might log 
        /// detailed information about key method parameters or other information that is useful for finding likely problems in
        /// specific 'problematic' areas of the code.  
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="exception">The exception to be logged</param>
        /// <param name="message">The message to be logged</param>
        public void Debug(string className, string methodName, Exception exception, string message = "")
        {
            if (message == string.Empty)
            {
                string logMessage = string.Format(ExceptionOnlyMessageFormat, className, methodName, exception);
                Log4NetLogger.Debug(logMessage, exception);
            }
            else
            {
                string logMessage = string.Format(ExceptionLogMessageFormat, className, methodName, message, exception);
                Log4NetLogger.Debug(logMessage, exception);
            }
        }

        /// <summary>
        /// Logs an exception at the Debug log level.
        /// DEBUG: Off by default, able to be turned on for debugging specific unexpected problems. This is where you might log 
        /// detailed information about key method parameters or other information that is useful for finding likely problems in
        /// specific 'problematic' areas of the code. 
        /// Message is assumed to be type of DTO in this case, and it will be converted using JSOn utils.
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="message">The message to be logged</param>
        public void Debug(string className, string methodName, object message)
        {
            var logMessage = LogMessageParser(className, methodName, message);
            Log4NetLogger.Debug(logMessage);
        }

        /// <summary>
        /// just to format the message so that dtos passed in show up properly without duplicate Data elements
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private string LogMessageParser(string className, string methodName, object message)
        {
            if (message == null)
            {
                message = "message passed to the log message was null";
            }

            if (message.GetType().FullName.Contains(message.ToString()))
            {
                message = " Data: " + JsonConvert.SerializeObject(message);
            }

            string logMessage = string.Format(LogMessageFormat, className, methodName,
                message.ToString()); // + " Data: " + JsonConvert.SerializeObject(message));

            return logMessage;
        }

        /// <summary>
        /// Logs an exception at the Error log level.
        /// ERROR: Something that the app's doing that it shouldn't. This isn't a user error ('invalid search query');
        /// it's an assertion failure, network problem, etc etc., probably one that is going to abort the current operation
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="exception">The exception to be logged</param>
        /// <param name="message">The message to be logged</param>
        public void Error(string className, string methodName, Exception exception, string message = "")
        {
            if (message == string.Empty)
            {
                string logMessage = string.Format(ExceptionOnlyMessageFormat, className, methodName, exception);
                Log4NetLogger.Error(logMessage, exception);
            }
            else
            {
                string logMessage = string.Format(ExceptionLogMessageFormat, className, methodName, message, exception);
                Log4NetLogger.Error(logMessage, exception);
            }
        }

        /// <summary>
        /// Logs an exception at the Error log level. 
        /// ERROR: Something that the app's doing that it shouldn't. This isn't a user error ('invalid search query');
        /// it's an assertion failure, network problem, etc etc., probably one that is going to abort the current operation
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="exception">The message to be logged</param>
        public void Error(string className, string methodName, object message)
        {
            var logMessage = LogMessageParser(className, methodName, message);
            Log4NetLogger.Error(logMessage);
        }

        /// <summary>
        /// Logs an exception at the Fatal log level. 
        /// FATAL: The app (or at the very least a thread) is about to die horribly. This is where the
        /// info explaining why that's happening goes.
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="exception">The exception to be logged</param>
        /// <param name="message">The message to be logged</param>
        public void Fatal(string className, string methodName, Exception exception, string message = "")
        {
            if (message == string.Empty)
            {
                string logMessage = string.Format(ExceptionOnlyMessageFormat, className, methodName, exception);
                Log4NetLogger.Fatal(logMessage, exception);
            }
            else
            {
                string logMessage = string.Format(ExceptionLogMessageFormat, className, methodName, message, exception);
                Log4NetLogger.Fatal(logMessage, exception);
            }
        }

        /// <summary>
        /// Logs an exception at the Fatal log level. 
        /// FATAL: The app (or at the very least a thread) is about to die horribly. This is where the
        /// info explaining why that's happening goes.
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="message">The message to be logged</param>
        public void Fatal(string className, string methodName, object message)
        {
            var logMessage = LogMessageParser(className, methodName, message);
            Log4NetLogger.Fatal(logMessage);
        }

        /// <summary>
        /// Logs an exception at the Info log level. 
        /// INFO: Normal logging that's part of the normal operation of the app; diagnostic stuff so you can go back and say 'how 
        /// often did this broad-level operation happen?', or 'how did the user's data get into this state?'
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="exception">The exception to be logged</param>
        /// <param name="message">The message to be logged</param>
        public void Info(string className, string methodName, Exception exception, string message = "")
        {
            if (message == string.Empty)
            {
                string logMessage = string.Format(ExceptionOnlyMessageFormat, className, methodName, exception);
                Log4NetLogger.Info(logMessage, exception);
            }
            else
            {
                string logMessage = string.Format(ExceptionLogMessageFormat, className, methodName, message, exception);
                Log4NetLogger.Info(logMessage, exception);
            }
        }

        /// <summary>
        /// Logs an exception at the Info log level. 
        /// INFO: Normal logging that's part of the normal operation of the app; diagnostic stuff so you can go back and say 'how 
        /// often did this broad-level operation happen?', or 'how did the user's data get into this state?'
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="message">The message to be logged</param>
        public void Info(string className, string methodName, object message)
        {
            var logMessage = LogMessageParser(className, methodName, message);
            Log4NetLogger.Info(logMessage);
        }

        /// <summary>
        /// This will put an entry into the logs with level INFO and then save the payload into
        /// a file. The filename will appear in the main log for reference.
        /// </summary>
        public void InfoPayload(string providerName, string methodName, Func<string> payload)
        {
            using (var acl = new ApiCallLogger(providerName, methodName))
            {
                acl.LogResponse(payload);
            }
        }

        /// <summary>
        /// Function for standizing method entry log messages.  Call this for function entry.
        /// Message are logged to the info log level.
        /// </summary>
        public void LogFunctionEntry(string className, string functionName, params object[] args)
        {
            string argString = "";
            if (args.Any())
                argString = "(" + String.Join(",", args) + ")";

            Debug(className, functionName, FunctionEntryString + argString);
        }

        /// <summary>
        /// Function for standizing function exit log messages.  Call this for function entry.
        /// Message are logged to the info log level.
        /// </summary>
        public void LogFunctionExit(string className, string functionName)
        {
            Debug(className, functionName, FunctionExitString);
        }

        /// <summary>
        /// Logs an exception at the Warn log level. 
        /// WARN: Something that's concerning but not causing the operation to abort; # of connections in the DB pool getting low, 
        /// an unusual-but-expected timeout in an operation, etc. I often think of 'WARN' as something that's useful in aggregate; 
        /// e.g. grep, group, and count them to get a picture of what's affecting the system health
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="exception">The exception to be logged</param>
        /// <param name="message">The message to be logged</param>
        public void Warn(string className, string methodName, Exception exception, string message = "")
        {
            if (message == string.Empty)
            {
                string logMessage = string.Format(ExceptionOnlyMessageFormat, className, methodName, exception);
                Log4NetLogger.Warn(logMessage, exception);
            }
            else
            {
                string logMessage = string.Format(ExceptionLogMessageFormat, className, methodName, message, exception);
                Log4NetLogger.Warn(logMessage, exception);
            }
        }

        /// <summary>
        /// Logs an exception at the Warn log level. 
        /// WARN: Something that's concerning but not causing the operation to abort; # of connections in the DB pool getting low, 
        /// an unusual-but-expected timeout in an operation, etc. I often think of 'WARN' as something that's useful in aggregate; 
        /// e.g. grep, group, and count them to get a picture of what's affecting the system health
        /// </summary>
        /// <param name="className">Name of the class from which the log message is being initiated.</param>
        /// <param name="methodName">The name of the method from which the log message is being initited.</param>
        /// <param name="message">The message to be logged</param>
        public void Warn(string className, string methodName, object message)
        {
            var logMessage = LogMessageParser(className, methodName, message);
            Log4NetLogger.Warn(logMessage);
        }

        /// <summary>
        /// If the debug log level is currently enabled for logging
        /// </summary>
        public bool IsDebugEnabled
        {
            get
            {
                return Log4NetLogger.IsDebugEnabled;
            }
        }

        /// <summary>
        /// If the error log level is currently enabled for logging
        /// </summary>
        public bool IsErrorEnabled
        {
            get
            {
                return Log4NetLogger.IsErrorEnabled;
            }
        }

        /// <summary>
        /// If the fatal log level is currently enabled for logging
        /// </summary>
        public bool IsFatalEnabled
        {
            get
            {
                return Log4NetLogger.IsFatalEnabled;
            }
        }

        /// <summary>
        /// If the into log level is currently enabled for logging
        /// </summary>
        public bool IsInfoEnabled
        {
            get
            {
                return Log4NetLogger.IsInfoEnabled;
            }
        }

        /// <summary>
        /// If the warn log level is currently enabled for logging
        /// </summary>
        public bool IsWarnEnabled
        {
            get
            {
                return Log4NetLogger.IsWarnEnabled;
            }
        }

        /// <summary>
        /// For engine use only! Not thread safe
        /// </summary>
        /// <param name="prefix"></param>
        public void SetLogFilePrefix(string prefix)
        {
            Info(typeof(Logger).Name, "SetLogFilePrefix", "Changing log file -> Adding prefix: '" + prefix + "'");
            try
            {
                log4net.Repository.Hierarchy.Hierarchy h =
                    (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
                foreach (IAppender a in h.Root.Appenders)
                {
                    if (a is FileAppender)
                    {
                        FileAppender fa = (FileAppender)a;

                        FileInfo fileInfo = new FileInfo(fa.File);
                        var logFileName = string.Format("{0}-{1}", prefix, fileInfo.Name);

                        fa.File = Path.Combine(fileInfo.DirectoryName, logFileName);
                        fa.ActivateOptions();
                        break;
                    }
                    else if (a is SmtpAppender)
                    {
                        SmtpAppender sa = (SmtpAppender)a;
                        sa.Subject = sa.Subject + " " + prefix;
                        sa.ActivateOptions();
                    }
                }
            }
            catch (Exception ex)
            {
                Error(typeof(Logger).Name, "SetLogFilePrefix", ex, "Prefix not set - log entries are probably in the base log file");
            }
        }
    }
}
