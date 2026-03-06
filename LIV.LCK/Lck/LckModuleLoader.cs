using System;
using System.Collections.Generic;
using Liv.Lck.DependencyInjection;

namespace Liv.Lck
{
	public static class LckModuleLoader
	{
		public static void RegisterModule(Action<LckDiContainer> configure, string name)
		{
			LckLog.Log("LCK: Registered module - " + name);
			LckModuleLoader._moduleConfigurators.Add(configure);
		}

		internal static void Configure(LckDiContainer container)
		{
			foreach (Action<LckDiContainer> action in LckModuleLoader._moduleConfigurators)
			{
				action(container);
			}
		}

		private static readonly List<Action<LckDiContainer>> _moduleConfigurators = new List<Action<LckDiContainer>>();
	}
}
