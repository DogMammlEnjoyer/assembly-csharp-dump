using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Utils
{
	internal static class AssemblyParser
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			List<Type> types = AssemblyParser._types;
			if (types != null)
			{
				types.Clear();
			}
			AssemblyParser._assembliesParsed = false;
			AssemblyParser._prebakedRuntimeSettings = null;
		}

		private static event Action<List<Type>> OnAssemblyParsed;

		public static bool Ready
		{
			get
			{
				return AssemblyParser._assembliesParsed;
			}
		}

		private static bool GetImmersiveDebuggerEnabled()
		{
			return RuntimeSettings.Instance.ImmersiveDebuggerEnabled;
		}

		public static bool Enabled
		{
			get
			{
				return AssemblyParser._enabledDelegate();
			}
		}

		private static Assembly[] GetAllAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		[RuntimeInitializeOnLoadMethod]
		private static void OnLoad()
		{
			AssemblyParser.Refresh(false);
		}

		private static void RefreshWhenPlaying()
		{
			AssemblyParser.Refresh(false);
		}

		public static void Refresh(bool ignorePrebakedAsset = false)
		{
			if (AssemblyParser.Enabled)
			{
				AssemblyParser.LoadAssembliesMainThread(ignorePrebakedAsset);
			}
		}

		private static Task LoadAssembliesMainThread(bool ignorePrebakedAsset)
		{
			AssemblyParser.<LoadAssembliesMainThread>d__18 <LoadAssembliesMainThread>d__;
			<LoadAssembliesMainThread>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadAssembliesMainThread>d__.ignorePrebakedAsset = ignorePrebakedAsset;
			<LoadAssembliesMainThread>d__.<>1__state = -1;
			<LoadAssembliesMainThread>d__.<>t__builder.Start<AssemblyParser.<LoadAssembliesMainThread>d__18>(ref <LoadAssembliesMainThread>d__);
			return <LoadAssembliesMainThread>d__.<>t__builder.Task;
		}

		private static Task LoadAssembliesAsync()
		{
			Assembly[] array = AssemblyParser._assembliesDelegate();
			int i = 0;
			while (i < array.Length)
			{
				Assembly assembly = array[i];
				if (!(AssemblyParser._prebakedRuntimeSettings != null))
				{
					goto IL_DC;
				}
				if (AssemblyParser._prebakedRuntimeSettings.debugTypesDict.ContainsKey(assembly.GetName().Name))
				{
					using (List<string>.Enumerator enumerator = AssemblyParser._prebakedRuntimeSettings.debugTypesDict[assembly.GetName().Name].GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string text = enumerator.Current;
							try
							{
								AssemblyParser._types.Add(assembly.GetType(text, true));
							}
							catch (Exception)
							{
								Debug.LogWarning(string.Concat(new string[]
								{
									"Immersive Debugger cannot get ",
									text,
									" type from assembly ",
									assembly.GetName().Name,
									", skipping"
								}));
							}
						}
						goto IL_13B;
					}
					goto IL_DC;
				}
				IL_13B:
				i++;
				continue;
				IL_DC:
				foreach (Type item in from t in assembly.GetTypes()
				where t.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Any((MemberInfo m) => m.GetCustomAttribute<DebugMember>() != null)
				select t)
				{
					AssemblyParser._types.Add(item);
				}
				goto IL_13B;
			}
			return Task.CompletedTask;
		}

		public static void RegisterAssemblyTypes(Action<List<Type>> del)
		{
			if (AssemblyParser.Ready && del != null)
			{
				del(AssemblyParser._types);
			}
			AssemblyParser.OnAssemblyParsed -= del;
			AssemblyParser.OnAssemblyParsed += del;
		}

		public static void Unregister(Action<List<Type>> del)
		{
			AssemblyParser.OnAssemblyParsed -= del;
		}

		private static List<Type> _types = new List<Type>();

		private static bool _assembliesParsed = false;

		private static Func<bool> _enabledDelegate = new Func<bool>(AssemblyParser.GetImmersiveDebuggerEnabled);

		private static Func<Assembly[]> _assembliesDelegate = new Func<Assembly[]>(AssemblyParser.GetAllAssemblies);

		private static RuntimeSettings _prebakedRuntimeSettings;
	}
}
