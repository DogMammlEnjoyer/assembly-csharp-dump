using System;

namespace Liv.Lck
{
	public class AnotherExamplePlugin : LCKPluginBase
	{
		public override string PluginName
		{
			get
			{
				return "AnotherExamplePlugin";
			}
		}

		public override string PluginVersion
		{
			get
			{
				return "1.0.0";
			}
		}

		protected override void OnInitialize()
		{
			LckLog.Log("AnotherExamplePlugin initialized");
		}

		protected override void OnShutdown()
		{
			LckLog.Log("AnotherExamplePlugin shutdown");
		}

		public void DoSomethingElse()
		{
			LckLog.Log("AnotherExamplePlugin is doing something else!");
		}
	}
}
