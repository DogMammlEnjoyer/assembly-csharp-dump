using System;

namespace Liv.Lck
{
	public class ExamplePlugin : LCKPluginBase
	{
		public override string PluginName
		{
			get
			{
				return "ExamplePlugin";
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
			if (base.HasPlugin<AnotherExamplePlugin>())
			{
				AnotherExamplePlugin plugin = base.GetPlugin<AnotherExamplePlugin>();
				LckLog.Log("Found another plugin: " + plugin.PluginName);
			}
			if (base.HasPlugin("SomeOtherPlugin"))
			{
				ILCKPlugin plugin2 = base.GetPlugin("SomeOtherPlugin");
				LckLog.Log("Found plugin by name: " + plugin2.PluginName);
			}
			base.LckService.OnRecordingStarted += this.OnRecordingStarted;
			base.LckService.OnRecordingStopped += this.OnRecordingStopped;
			LckLog.Log("ExamplePlugin initialized with LCK service");
		}

		protected override void OnShutdown()
		{
			if (base.LckService != null)
			{
				base.LckService.OnRecordingStarted -= this.OnRecordingStarted;
				base.LckService.OnRecordingStopped -= this.OnRecordingStopped;
			}
			LckLog.Log("ExamplePlugin shutdown");
		}

		private void OnRecordingStarted(LckResult result)
		{
			if (result.Success)
			{
				LckLog.Log("ExamplePlugin: Recording started successfully");
				return;
			}
			LckLog.LogError("ExamplePlugin: Recording failed to start: " + result.Message);
		}

		private void OnRecordingStopped(LckResult result)
		{
			if (result.Success)
			{
				LckLog.Log("ExamplePlugin: Recording stopped successfully");
				return;
			}
			LckLog.LogError("ExamplePlugin: Recording failed to stop: " + result.Message);
		}

		public void DoSomething()
		{
			if (!base.IsInitialized)
			{
				LckLog.LogWarning("ExamplePlugin is not initialized");
				return;
			}
			LckLog.Log("ExamplePlugin is doing something!");
		}
	}
}
