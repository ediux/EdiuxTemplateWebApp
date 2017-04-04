using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace EdiuxTemplateWebApp.Helpers
{
	public static class WebHelper
	{
		public static T getApplicationGlobalVariable<T>(this object obj, string name)
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

		public static void setApplicationGlobalVariable<T>(this object obj, string name, T value)
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

				newPolicy.RemovedCallback += processCacheEntryRemoved;
				newPolicy.UpdateCallback += processCacheEntryUpdate;

				MemoryCache.Default.Set(name, value, newPolicy);
			}
		}

		internal static void processCacheEntryRemoved(CacheEntryRemovedArguments args)
		{

		}

		internal static void processCacheEntryUpdate(CacheEntryUpdateArguments args)
		{

		}
	}
}