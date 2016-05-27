using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TE.Common.Logging;

namespace Common.Configuration
{
    public abstract class ConfigSettingsBase
    {
        // Not a property as we don't want this loaded up in the config.
        public bool LoadedWithoutErrors;

        protected ConfigSettingsBase()
        {
            Init();
        }

        /// <summary>
        /// This will use reflection to populate the configuration properties. If it is not found, the
        /// value will be set to NULL and a message logged.
        /// TODO: Unit Tests
        /// </summary>
        public void Init()
        {
            LoadedWithoutErrors = true;

            var ns = GetType().Namespace;

            foreach (var p in GetType().GetProperties())
            {
                if (p.SetMethod == null)
                    continue;

                try
                {
                    var key = ns + "." + p.Name;
                    var val = ConfigurationManager.AppSettings[key];

                    if (val == null)
                    {
                        LoadedWithoutErrors = false;
                        Logger.Instance.Warn(this.GetType().Name, "Init", "Missing config value " + key);
                    }
                    else
                    {
                        if (p.PropertyType == typeof(String))
                            p.SetValue(this, val);
                        else if (p.PropertyType == typeof(String[]))
                        {
                            if (val != null)
                            {
                                if (!string.IsNullOrEmpty(val))
                                {
                                    p.SetValue(this, val.Split(','));
                                }
                                else
                                {
                                    p.SetValue(this, new string[0]);
                                }
                            }
                        }
                        else if (p.PropertyType == typeof(Int32))
                            p.SetValue(this, Int32.Parse(val));
                        else if (p.PropertyType == typeof(decimal))
                            p.SetValue(this, decimal.Parse(val));
                        else if (p.PropertyType == typeof(bool))
                            p.SetValue(this, bool.Parse(val));
                        else if (p.PropertyType == typeof(Uri))
                        {
                            if (!string.IsNullOrEmpty(val))
                                p.SetValue(this, new Uri(val));
                        }
                        else
                            throw new NotSupportedException(p.PropertyType.Name + " not supported - key:" + key);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Warn(this.GetType().Name, "Init", ex);
                    LoadedWithoutErrors = false;
                }
            }

            foreach (var p in this.GetType().GetProperties(BindingFlags.Public).Where(t => t.GetValue(this) == null))
            {
                Logger.Instance.Warn(this.GetType().Name, "Init", "Missing configuration value " + ns + "." + p.Name);
                LoadedWithoutErrors = false;
            }
        }
    }
}
