using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Helpers
{
    public class ConfigHelper
    {
        private static Dictionary<string, string> ConfigCache = new Dictionary<string, string>();

        public static string GetConfig(string key)
        {
            string settingValues = ConfigurationManager.AppSettings[key];

            if (!ConfigCache.Keys.Any(a => a == key))
            {
                ConfigCache.Add(key, settingValues);
            }

            return ConfigCache[key];
        }

        public static void SetConfig(string key, string value)
        {
            if (!ConfigCache.Keys.Any(a => a == key))
            {
                if (!ConfigurationManager.AppSettings.AllKeys.Any(a => a == key))
                {
                    ConfigurationManager.AppSettings.Add(key, value);
                    ConfigCache.Add(key, value);
                }
                else
                {
                    ConfigurationManager.AppSettings[key] = value;
                    ConfigCache[key] = value;
                }
            }
            else
            {
                ConfigurationManager.AppSettings[key] = value;
                ConfigCache[key] = value;
            }
        }
    }
}