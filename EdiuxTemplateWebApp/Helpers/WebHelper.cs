using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace EdiuxTemplateWebApp.Helpers
{
    public static class WebHelper
    {
        /// <summary>
        /// 讀取二進制的檔案
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pathOrUrl"></param>
        /// <returns></returns>
        public static byte[] ReadFile(this object obj, string pathOrUrl)
        {
            string internalFilePath = "";

            if (System.IO.File.Exists(pathOrUrl))
            {
                internalFilePath = pathOrUrl;
            }
            else
            {
                if (pathOrUrl.Contains("\\"))
                {
                    pathOrUrl = pathOrUrl.Replace("\\", "/");
                    internalFilePath = HttpContext.Current.Server.MapPath(pathOrUrl);
                }
                else
                {
                    throw new System.IO.FileNotFoundException("File '{0}' not found.", System.IO.Path.GetFileName(pathOrUrl));
                }
            }

            System.IO.FileStream fs = System.IO.File.OpenRead(internalFilePath);
            System.IO.BinaryReader fsr = new System.IO.BinaryReader(fs);

            byte[] outputMemory = Array.CreateInstance(typeof(byte), fsr.BaseStream.Length) as byte[];

            int lastblock = (int)(fsr.BaseStream.Length % 4096L);
            int readpostion = 0;
            fsr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

            do
            {
                if (fsr.BaseStream.Length > 4096)
                {
                    byte[] buffermemory = fsr.ReadBytes(4096);
                    buffermemory.CopyTo(outputMemory, readpostion);
                    readpostion += 4096;
                }


            } while (fsr.BaseStream.Position <= (fsr.BaseStream.Length - lastblock));

            if (lastblock > 0)
            {
                byte[] buffermemory = fsr.ReadBytes(lastblock);
                buffermemory.CopyTo(outputMemory, lastblock);
                readpostion += lastblock;
            }

            fsr.Close();

            return outputMemory;
        }

        public static string ReadTextFile(this object obj, string pathOrUrl)
        {
            string internalFilePath = "";

            if (System.IO.File.Exists(pathOrUrl))
            {
                internalFilePath = pathOrUrl;
            }
            else
            {
                if (pathOrUrl.Contains("\\"))
                {
                    pathOrUrl = pathOrUrl.Replace("\\", "/");
                    internalFilePath = HttpContext.Current.Server.MapPath(pathOrUrl);
                }
                else
                {
                    throw new System.IO.FileNotFoundException("File '{0}' not found.", System.IO.Path.GetFileName(pathOrUrl));
                }
            }

            return System.IO.File.ReadAllText(internalFilePath);
        }

        public static T GetApplicationGlobalVariable<T>(this object obj, string name)
        {
            if (HttpContext.Current != null)
            {
                return (T)HttpContext.Current.Application[name];
            }
            else
            {
                CacheItemPolicy newPolicy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddSeconds(38400)),
                    Priority = CacheItemPriority.Default,
                    SlidingExpiration = new TimeSpan(0, 30, 0)
                };

                return (T)MemoryCache.Default.AddOrGetExisting(name, default(T), newPolicy);
            }
        }

        public static void SetApplicationGlobalVariable<T>(this object obj, string name, T value)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Application.AllKeys.Any(a => a == name))
                {
                    HttpContext.Current.Application[name] = value;
                }
                else
                {
                    HttpContext.Current.Application.Add(name, value);
                }
            }
            else
            {

                CacheItemPolicy newPolicy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddSeconds(38400)),
                    Priority = CacheItemPriority.Default,
                    SlidingExpiration = new TimeSpan(0, 30, 0)
                };

                newPolicy.RemovedCallback += new CacheEntryRemovedCallback(ProcessCacheEntryRemoved);
                newPolicy.UpdateCallback += new CacheEntryUpdateCallback(ProcessCacheEntryUpdate);

                MemoryCache.Default.Set(name, value, newPolicy);
            }
        }

        internal static void ProcessCacheEntryRemoved(CacheEntryRemovedArguments args)
        {

        }

        internal static void ProcessCacheEntryUpdate(CacheEntryUpdateArguments args)
        {

        }
    }
}