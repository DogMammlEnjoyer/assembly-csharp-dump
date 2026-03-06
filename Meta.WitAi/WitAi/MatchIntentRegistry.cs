using System;
using System.Reflection;
using System.Threading;
using Meta.Voice.Logging;
using Meta.WitAi.Utilities;

namespace Meta.WitAi
{
	[LogCategory("MatchIntent")]
	internal static class MatchIntentRegistry
	{
		public static IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger("MatchIntent", null);

		public static DictionaryList<string, RegisteredMatchIntent> RegisteredMethods
		{
			get
			{
				if (MatchIntentRegistry.registeredMethods == null)
				{
					MatchIntentRegistry.Initialize();
				}
				return MatchIntentRegistry.registeredMethods;
			}
		}

		internal static void Initialize()
		{
			if (MatchIntentRegistry.registeredMethods != null)
			{
				return;
			}
			MatchIntentRegistry.registeredMethods = new DictionaryList<string, RegisteredMatchIntent>();
			ThreadUtility.Background(MatchIntentRegistry.Logger, new Action(MatchIntentRegistry.RefreshAssemblies)).WrapErrors();
		}

		internal static void RefreshAssemblies()
		{
			if (Thread.CurrentThread.ThreadState == ThreadState.Aborted)
			{
				return;
			}
			DictionaryList<string, RegisteredMatchIntent> dictionaryList = new DictionaryList<string, RegisteredMatchIntent>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					foreach (Type type in assembly.GetTypes())
					{
						try
						{
							foreach (MethodInfo methodInfo in type.GetMethods())
							{
								try
								{
									foreach (Attribute attribute in methodInfo.GetCustomAttributes(typeof(MatchIntent)))
									{
										try
										{
											MatchIntent matchIntent = (MatchIntent)attribute;
											dictionaryList[matchIntent.Intent].Add(new RegisteredMatchIntent
											{
												type = type,
												method = methodInfo,
												matchIntent = matchIntent
											});
										}
										catch (Exception ex)
										{
											MatchIntentRegistry.Logger.Debug(ex.Message, null, null, null, null, "RefreshAssemblies", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\ResponseManager\\MatchIntentRegistry.cs", 84);
										}
									}
								}
								catch (Exception ex2)
								{
									MatchIntentRegistry.Logger.Debug(ex2.Message, null, null, null, null, "RefreshAssemblies", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\ResponseManager\\MatchIntentRegistry.cs", 88);
								}
							}
						}
						catch (Exception ex3)
						{
							MatchIntentRegistry.Logger.Debug(ex3.Message, null, null, null, null, "RefreshAssemblies", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\ResponseManager\\MatchIntentRegistry.cs", 92);
						}
					}
				}
				catch (Exception ex4)
				{
					MatchIntentRegistry.Logger.Debug(ex4.Message, null, null, null, null, "RefreshAssemblies", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\ResponseManager\\MatchIntentRegistry.cs", 96);
				}
			}
			MatchIntentRegistry.registeredMethods = dictionaryList;
		}

		private static DictionaryList<string, RegisteredMatchIntent> registeredMethods;
	}
}
