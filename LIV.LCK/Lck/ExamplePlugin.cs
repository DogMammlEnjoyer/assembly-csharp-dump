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
				LckLog.Log("Found another plugin: " + plugin.PluginName, "OnInitialize", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 19);
			}
			if (base.HasPlugin("SomeOtherPlugin"))
			{
				ILCKPlugin plugin2 = base.GetPlugin("SomeOtherPlugin");
				LckLog.Log("Found plugin by name: " + plugin2.PluginName, "OnInitialize", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 26);
			}
			base.LckService.OnRecordingStarted += this.OnRecordingStarted;
			base.LckService.OnRecordingStopped += this.OnRecordingStopped;
			LckLog.Log("ExamplePlugin initialized with LCK service", "OnInitialize", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 33);
		}

		protected override void OnShutdown()
		{
			if (base.LckService != null)
			{
				base.LckService.OnRecordingStarted -= this.OnRecordingStarted;
				base.LckService.OnRecordingStopped -= this.OnRecordingStopped;
			}
			LckLog.Log("ExamplePlugin shutdown", "OnShutdown", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 45);
		}

		private void OnRecordingStarted(LckResult result)
		{
			if (result.Success)
			{
				LckLog.Log("ExamplePlugin: Recording started successfully", "OnRecordingStarted", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 52);
				return;
			}
			LckLog.LogError("ExamplePlugin: Recording failed to start: " + result.Message, "OnRecordingStarted", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 56);
		}

		private void OnRecordingStopped(LckResult result)
		{
			if (result.Success)
			{
				LckLog.Log("ExamplePlugin: Recording stopped successfully", "OnRecordingStopped", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 64);
				return;
			}
			LckLog.LogError("ExamplePlugin: Recording failed to stop: " + result.Message, "OnRecordingStopped", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 68);
		}

		public void DoSomething()
		{
			if (!base.IsInitialized)
			{
				LckLog.LogWarning("ExamplePlugin is not initialized", "DoSomething", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 79);
				return;
			}
			LckLog.Log("ExamplePlugin is doing something!", "DoSomething", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\ExamplePlugin.cs", 83);
		}
	}
}
