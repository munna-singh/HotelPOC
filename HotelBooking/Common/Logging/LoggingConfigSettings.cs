using System;
using Common.Configuration;

namespace Common.Logging
{
    public sealed class LoggingConfigSettings : ConfigSettingsBase
    {
        private static volatile LoggingConfigSettings instance;
        private static object syncRoot = new Object();

        public static LoggingConfigSettings Instance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new LoggingConfigSettings();
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// Base path to use for provider logs.
        /// </summary>
        public string ProviderLogFileLocationBase { get; set; }

        /// <summary>
        /// The list of items to include. If '*' all are to be included, otherwise only specific ones.
        /// </summary>
        public string ProviderPayloadLoggingInclude { get; set; }

        /// <summary>
        /// The list of providers to exclude. If '*' all are to be excluded, unless otherwise explicity set in ProviderPayloadLoggingInclude
        /// </summary>
        public string ProviderPayloadLoggingExclude { get; set; }

        public LoggingConfigSettings()
        {
            this.Init();
        }
    }
}
