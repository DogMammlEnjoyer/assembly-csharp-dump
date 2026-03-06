using System;
using System.Reflection;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal static class ClassLibraryInitializer
	{
		[RequiredByNativeCode]
		private static void Init()
		{
			UnityLogWriter.Init();
		}

		[RequiredByNativeCode(Optional = true)]
		private static void InitAssemblyRedirections()
		{
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object _, ResolveEventArgs args)
			{
				AssemblyName assemblyName = new AssemblyName(args.Name);
				Assembly result;
				try
				{
					Assembly assembly = AppDomain.CurrentDomain.Load(assemblyName.Name);
					result = assembly;
				}
				catch
				{
					result = null;
				}
				return result;
			};
		}
	}
}
