using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TE.Common.Logging;

namespace Common.Logging
{
    public class ApiCallLogger : IDisposable
    {

        private static readonly Object lockObj = new object();
        private static int safeInstanceCount = 0;

        private const string REQUEST_SUFFIX = "RQ";
        private const string RESPONSE_SUFFIX = "RES";
        private const string TRANSFORMED_SUFFIX = "TRA";
        private const string EXCEPTION_SUFFIX = "EXP";

        private string ApiWorkerName;
        private string ApiMethodName;

        private readonly bool logProviderNameDirectory;

        private readonly bool LogPayload;

        public string FileNameRoot { get; set; }

        protected string LogFileExtension;

        /// <summary>
        /// if false, we do not log any payloads
        /// </summary>
        private readonly static bool payloadLoggingDisabled;

        static ApiCallLogger()
        {
            try
            {
                // Ensure we have a directory to place logs in
                if (LoggingConfigSettings.Instance().ProviderLogFileLocationBase == null)
                {
                    payloadLoggingDisabled = true;
                }
                else if (!Directory.Exists(LoggingConfigSettings.Instance().ProviderLogFileLocationBase))
                {
                    Directory.CreateDirectory(LoggingConfigSettings.Instance().ProviderLogFileLocationBase);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(typeof(ApiCallLogger).Name, "ApiCallLogger", ex, "Could not create directory " + LoggingConfigSettings.Instance().ProviderLogFileLocationBase);
            }
        }

        // use to group logging statements when not in a using statement
        public ApiCallLogger(string provider, string methodName, string fileNameRoot, string logFileExtension = "txt")
        {
            ApiWorkerName = provider;
            ApiMethodName = methodName;
            FileNameRoot = fileNameRoot;
            LogPayload = fileNameRoot != null;
            LogFileExtension = logFileExtension;
        }

        public ApiCallLogger(string provider, string methodName, bool logProviderNameDirectory = true, string logFileExtension = "txt")
        {
            if (payloadLoggingDisabled)
                return;

            ApiWorkerName = provider;
            ApiMethodName = methodName;
            LogFileExtension = logFileExtension;

            if (payloadLoggingDisabled)
                LogPayload = false;
            else
                LogPayload = ShouldProviderPayloadBeLogged(provider);

            if (LogPayload)
            {
                this.logProviderNameDirectory = logProviderNameDirectory;
                FileNameRoot = GetLogRoot(provider);
            }
        }

        private bool ShouldProviderPayloadBeLogged(string provider)
        {
            var include = (LoggingConfigSettings.Instance().ProviderPayloadLoggingInclude ?? "").ToUpper();
            var exclude = (LoggingConfigSettings.Instance().ProviderPayloadLoggingExclude ?? "*").ToUpper();

            // Explicit exclude overrides all
            if (exclude.Contains(provider.ToUpper()))
            {
                return false;
            }

            // Explicit include overrides excludes
            if (include.Contains(provider.ToUpper()))
            {
                return true;
            }

            // Default - only if include is *
            return include == "*";
        }

        private string GetLogRoot(string provider)
        {
            string fileNameRoot;

            string userId = "NA";
            try
            {
                if (System.Threading.Thread.CurrentPrincipal != null)
                    userId = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("ApiCallLogger", "GetLogRoot", ex, "Could not locate the user identity");
            }

            lock (lockObj)
            {
                if (++safeInstanceCount >= 100)
                {
                    safeInstanceCount = 1;
                }

                var logPath = LoggingConfigSettings.Instance().ProviderLogFileLocationBase;
                if (this.logProviderNameDirectory)
                {
                    logPath = Path.Combine(logPath, provider);
                }
                if (!Directory.Exists(logPath))
                {
                    try
                    {
                        Directory.CreateDirectory(logPath);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error(typeof(ApiCallLogger).Name, "GetLogRoot", ex, "Could not create directory " + logPath);
                    }
                }

                fileNameRoot = Path.Combine(
                    logPath,
                    string.Format(
                        "{9}-LOG-{8}-{0}{1}{2}_{3}.{4}.{5}.{6}_{7}",
                        DateTime.Now.Year,
                        DateTime.Now.Month.ToString("00"),
                        DateTime.Now.Day.ToString("00"),
                        DateTime.Now.Hour.ToString("00"),
                        DateTime.Now.Minute.ToString("00"),
                        DateTime.Now.Second.ToString("00"),
                        DateTime.Now.Millisecond.ToString("000"),
                        safeInstanceCount,
                        userId.Replace(Path.DirectorySeparatorChar.ToString(), string.Empty)
                            .Replace(Path.AltDirectorySeparatorChar.ToString(), string.Empty),
                        provider));
            }

            return fileNameRoot;
        }

        public void LogRequestJson(Func<object> obj)
        {
            LogRequest(() => JsonConvert.SerializeObject(obj()));
        }

        public void LogRequest(Func<string> request = null)
        {
            if (payloadLoggingDisabled)
                return;
            Logger.Instance.LogFunctionEntry("ApiCallLogger", "LogRequest");
            try
            {
                if (LogPayload && request != null)
                {
                    string htmlDecodedRQ = request();

                    string fileName = String.Format("_{0}.{1}", REQUEST_SUFFIX, LogFileExtension);

                    File.AppendAllText(FileNameRoot + fileName, htmlDecodedRQ);

                    Logger.Instance.APILogRequest(ApiWorkerName, ApiMethodName, Path.GetFileName(FileNameRoot + fileName));
                }
                else
                {
                    Logger.Instance.APILogRequest(ApiWorkerName, ApiMethodName);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("ApiCallLogger", "LogRequest", ex, "ERROR WRITING RESPONSE TO LOGS");
            }

            Logger.Instance.LogFunctionExit("ApiCallLogger", "LogRequest");
        }

        public void LogTransformedResponse(Func<string> response = null, string apiResponseId = null)
        {
            if (payloadLoggingDisabled)
                return;
            Logger.Instance.LogFunctionEntry("ApiCallLogger", "LogTransformedResponse");

            try
            {
                if (LogPayload && response != null)
                {
                    string htmlDecodedRES = response();

                    string fileName = String.Format("_{0}.{1}", TRANSFORMED_SUFFIX, LogFileExtension);
                    File.AppendAllText(FileNameRoot + fileName, htmlDecodedRES);

                    Logger.Instance.APILogResponse(ApiWorkerName, ApiMethodName, apiResponseId, Path.GetFileName(FileNameRoot + fileName));
                }
                else
                {
                    Logger.Instance.APILogResponse(ApiWorkerName, ApiMethodName, apiResponseId);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("ApiCallLogger", "LogTransformedResponse", ex, "ERROR WRITING RESPONSE TO LOGS");
            }

            Logger.Instance.LogFunctionEntry("ApiCallLogger", "LogTransformedResponse");
        }

        public void LogResponse(Func<string> response = null, string apiResponseId = null)
        {
            if (payloadLoggingDisabled)
                return;
            Logger.Instance.LogFunctionEntry("ApiCallLogger", "LogResponse");

            try
            {
                if (LogPayload && response != null)
                {
                    string htmlDecodedRES = response();

                    string fileName = String.Format("_{0}.{1}", RESPONSE_SUFFIX, LogFileExtension);
                    File.AppendAllText(FileNameRoot + fileName, htmlDecodedRES);

                    Logger.Instance.APILogResponse(ApiWorkerName, ApiMethodName, apiResponseId, Path.GetFileName(FileNameRoot + fileName));
                }
                else
                {
                    Logger.Instance.APILogResponse(ApiWorkerName, ApiMethodName, apiResponseId);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("ApiCallLogger", "LogResponse", ex, "ERROR WRITING RESPONSE TO LOGS");
            }

            Logger.Instance.LogFunctionEntry("ApiCallLogger", "LogResponse");
        }

        public void LogException(Func<Exception> exception, String apiResponseId = null)
        {
            if (payloadLoggingDisabled)
                return;
            Logger.Instance.LogFunctionEntry("ApiCallLogger", "LogException");
            try
            {
                String exceptionStr = exception().Message;

                if (LogPayload)
                {
                    var file = FileNameRoot + String.Format("_{0}.{1}", EXCEPTION_SUFFIX, LogFileExtension);

                    File.AppendAllText(file, exceptionStr);

                    Logger.Instance.APILogException(ApiWorkerName, ApiMethodName, apiResponseId, file);
                }
                else
                {
                    Logger.Instance.APILogException(ApiWorkerName, ApiMethodName, apiResponseId);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("ApiCallLogger", "LogException", ex, "ERROR WRITING RESPONSE TO LOGS");
            }

            Logger.Instance.LogFunctionEntry("ApiCallLogger", "LogException");
        }

        public void Dispose()
        {
        }
    }
}
