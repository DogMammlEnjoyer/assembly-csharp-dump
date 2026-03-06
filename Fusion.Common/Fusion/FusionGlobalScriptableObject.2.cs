using System;
using UnityEngine;

namespace Fusion
{
	public abstract class FusionGlobalScriptableObject<T> : FusionGlobalScriptableObject where T : FusionGlobalScriptableObject<T>
	{
		public bool IsGlobal { get; private set; }

		protected virtual void OnLoadedAsGlobal()
		{
		}

		protected virtual void OnUnloadedAsGlobal(bool destroyed)
		{
		}

		private static string LogPrefix
		{
			get
			{
				return "[Global " + typeof(T).Name + "]: ";
			}
		}

		private static string AsId(FusionGlobalScriptableObject<T> obj)
		{
			return obj ? string.Format("[IID:{0}]", obj.GetInstanceID()) : "null";
		}

		protected virtual void OnDisable()
		{
			bool flag = !this.IsGlobal;
			if (flag)
			{
				TraceLogStream logTrace = InternalLogStreams.LogTrace;
				if (logTrace != null)
				{
					logTrace.Log(FusionGlobalScriptableObject<T>.LogPrefix + "OnDisable called for " + FusionGlobalScriptableObject<T>.AsId(this) + ", but is not global");
				}
			}
			else
			{
				bool flag2 = FusionGlobalScriptableObject<T>.s_unloadHandler != null;
				if (flag2)
				{
					TraceLogStream logTrace2 = InternalLogStreams.LogTrace;
					if (logTrace2 != null)
					{
						logTrace2.Log(FusionGlobalScriptableObject<T>.LogPrefix + "OnDisable called for " + FusionGlobalScriptableObject<T>.AsId(this) + ", setting global instance to null. The unload handler is still set, not going to be used.");
					}
				}
				else
				{
					TraceLogStream logTrace3 = InternalLogStreams.LogTrace;
					if (logTrace3 != null)
					{
						logTrace3.Log(FusionGlobalScriptableObject<T>.LogPrefix + "OnDisable called for " + FusionGlobalScriptableObject<T>.AsId(this) + ", setting global instance to null.");
					}
				}
				Assert.Check(this == FusionGlobalScriptableObject<T>.s_instance, "Expected this to be the global instance");
				FusionGlobalScriptableObject<T>.s_instance = default(T);
				FusionGlobalScriptableObject<T>.s_unloadHandler = null;
				this.IsGlobal = false;
				this.OnUnloadedAsGlobal(true);
			}
		}

		protected static T GlobalInternal
		{
			get
			{
				T orLoadGlobalInstance = FusionGlobalScriptableObject<T>.GetOrLoadGlobalInstance();
				bool flag = orLoadGlobalInstance == null;
				if (flag)
				{
					throw new InvalidOperationException("Failed to load " + typeof(T).Name + ". If this happens in edit mode, make sure Fusion is properly installed in the Fusion HUB. Otherwise, if the default path does not exist or does not point to a Resource, you need to use FusionGlobalScriptableObjectAttribute attribute to point to a method that will perform the loading.");
				}
				return orLoadGlobalInstance;
			}
			set
			{
				bool flag = value == FusionGlobalScriptableObject<T>.s_instance;
				if (!flag)
				{
					FusionGlobalScriptableObject<T>.SetGlobalInternal(value, null);
				}
			}
		}

		protected static bool IsGlobalLoadedInternal
		{
			get
			{
				return FusionGlobalScriptableObject<T>.s_instance != null;
			}
		}

		protected static bool TryGetGlobalInternal(out T global)
		{
			T orLoadGlobalInstance = FusionGlobalScriptableObject<T>.GetOrLoadGlobalInstance();
			bool flag = orLoadGlobalInstance == null;
			bool result;
			if (flag)
			{
				global = default(T);
				result = false;
			}
			else
			{
				global = orLoadGlobalInstance;
				result = true;
			}
			return result;
		}

		protected static bool UnloadGlobalInternal()
		{
			T t = FusionGlobalScriptableObject<T>.s_instance;
			bool flag = !t;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Assert.Check(t.IsGlobal);
				try
				{
					bool flag2 = FusionGlobalScriptableObject<T>.s_unloadHandler != null;
					if (flag2)
					{
						TraceLogStream logTrace = InternalLogStreams.LogTrace;
						if (logTrace != null)
						{
							logTrace.Log(FusionGlobalScriptableObject<T>.LogPrefix + " Unloading global instance " + FusionGlobalScriptableObject<T>.AsId(t) + " with unloader");
						}
						FusionGlobalScriptableObjectUnloadDelegate fusionGlobalScriptableObjectUnloadDelegate = FusionGlobalScriptableObject<T>.s_unloadHandler;
						FusionGlobalScriptableObject<T>.s_unloadHandler = null;
						fusionGlobalScriptableObjectUnloadDelegate(t);
					}
					else
					{
						TraceLogStream logTrace2 = InternalLogStreams.LogTrace;
						if (logTrace2 != null)
						{
							logTrace2.Log(FusionGlobalScriptableObject<T>.LogPrefix + " Instance " + FusionGlobalScriptableObject<T>.AsId(t) + " has no unloader, simply nulling it out");
						}
					}
				}
				finally
				{
					FusionGlobalScriptableObject<T>.s_instance = default(T);
					bool isGlobal = t.IsGlobal;
					if (isGlobal)
					{
						t.IsGlobal = false;
						t.OnUnloadedAsGlobal(false);
					}
				}
				result = true;
			}
			return result;
		}

		private static T GetOrLoadGlobalInstance()
		{
			bool flag = FusionGlobalScriptableObject<T>.s_instance;
			T result;
			if (flag)
			{
				result = FusionGlobalScriptableObject<T>.s_instance;
			}
			else
			{
				T t = default(T);
				FusionGlobalScriptableObjectUnloadDelegate unloadHandler = null;
				t = FusionGlobalScriptableObject<T>.LoadPlayerInstance(out unloadHandler);
				bool flag2 = t;
				if (flag2)
				{
					FusionGlobalScriptableObject<T>.SetGlobalInternal(t, unloadHandler);
				}
				result = t;
			}
			return result;
		}

		private static T LoadPlayerInstance(out FusionGlobalScriptableObjectUnloadDelegate unloadHandler)
		{
			FusionGlobalScriptableObjectSourceAttribute[] sourceAttributes = FusionGlobalScriptableObject.SourceAttributes;
			int i = 0;
			while (i < sourceAttributes.Length)
			{
				FusionGlobalScriptableObjectSourceAttribute fusionGlobalScriptableObjectSourceAttribute = sourceAttributes[i];
				bool isEditor = Application.isEditor;
				if (!isEditor)
				{
					goto IL_40;
				}
				bool flag = !Application.isPlaying && !fusionGlobalScriptableObjectSourceAttribute.AllowEditMode;
				if (!flag)
				{
					goto IL_40;
				}
				IL_118:
				i++;
				continue;
				IL_40:
				bool flag2 = fusionGlobalScriptableObjectSourceAttribute.ObjectType != typeof(T) && !typeof(T).IsSubclassOf(fusionGlobalScriptableObjectSourceAttribute.ObjectType);
				if (flag2)
				{
					goto IL_118;
				}
				FusionGlobalScriptableObjectLoadResult fusionGlobalScriptableObjectLoadResult = fusionGlobalScriptableObjectSourceAttribute.Load(typeof(T));
				bool flag3 = fusionGlobalScriptableObjectLoadResult.Object;
				if (flag3)
				{
					T t = (T)((object)fusionGlobalScriptableObjectLoadResult.Object);
					unloadHandler = fusionGlobalScriptableObjectLoadResult.Unloader;
					TraceLogStream logTrace = InternalLogStreams.LogTrace;
					if (logTrace != null)
					{
						logTrace.Log(string.Format("{0} Loader {1} was used to load {2}, has unloader: {3}", new object[]
						{
							FusionGlobalScriptableObject<T>.LogPrefix,
							fusionGlobalScriptableObjectSourceAttribute,
							FusionGlobalScriptableObject<T>.AsId(t),
							unloadHandler != null
						}));
					}
					return t;
				}
				bool flag4 = !fusionGlobalScriptableObjectSourceAttribute.AllowFallback;
				if (flag4)
				{
					break;
				}
				goto IL_118;
			}
			TraceLogStream logTrace2 = InternalLogStreams.LogTrace;
			if (logTrace2 != null)
			{
				logTrace2.Log(FusionGlobalScriptableObject<T>.LogPrefix + " No source attribute was able to load the global instance");
			}
			unloadHandler = null;
			return default(T);
		}

		private static void SetGlobalInternal(T value, FusionGlobalScriptableObjectUnloadDelegate unloadHandler)
		{
			bool flag = FusionGlobalScriptableObject<T>.s_instance;
			if (flag)
			{
				throw new InvalidOperationException("Failed to set " + typeof(T).Name + " as global. A global instance is already loaded - it needs to be unloaded first");
			}
			Assert.Check(value, "Expected value to be non-null");
			bool flag2 = FusionGlobalScriptableObject<T>.s_instance == null;
			if (flag2)
			{
				Assert.Check(FusionGlobalScriptableObject<T>.s_unloadHandler == null, "Expected unload handler to be null");
			}
			bool flag3 = value;
			if (flag3)
			{
				FusionGlobalScriptableObject<T>.s_instance = value;
				FusionGlobalScriptableObject<T>.s_unloadHandler = unloadHandler;
				FusionGlobalScriptableObject<T>.s_instance.IsGlobal = true;
				FusionGlobalScriptableObject<T>.s_instance.OnLoadedAsGlobal();
			}
		}

		private static T s_instance;

		private static FusionGlobalScriptableObjectUnloadDelegate s_unloadHandler;
	}
}
