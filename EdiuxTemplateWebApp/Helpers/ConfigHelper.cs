using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp
{
    public class ConfigHelper
    {
        private static Dictionary<string, string> ConfigCache =
             new Dictionary<string, string>();

        /// <summary>
        /// 取得設定檔中的應用程式設定值。
        /// </summary>
        /// <param name="key">設定值的鍵值。</param>
        /// <returns></returns>
        public static string GetConfig(string key)
        {
            if (!ConfigCache.Keys.Contains(key))
            {
                ConfigCache.Add(key, ConfigurationManager.AppSettings[key]);
            }
            return ConfigCache[key];
        }

        /// <summary>
        /// 設定設定檔中的應用程式設定值對內容。
        /// </summary>
        /// <param name="key">設定值的鍵值。</param>
        /// <param name="value">新的設定內容</param>
        public static void SetConfig(string key, string value)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key) == false)
            {
                ConfigurationManager.AppSettings.Add(key, value);
                if (!ConfigCache.Keys.Contains(key))
                {
                    ConfigCache.Add(key, ConfigurationManager.AppSettings[key]);
                }
                else
                {
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