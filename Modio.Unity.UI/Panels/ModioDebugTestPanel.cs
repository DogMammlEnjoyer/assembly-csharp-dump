using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Modio.API;
using Modio.Unity.UI.Input;
using UnityEngine;

namespace Modio.Unity.UI.Panels
{
	public class ModioDebugTestPanel : ModioPanelBase
	{
		protected override void Awake()
		{
			this._modioDebugMenu = base.GetComponent<ModioDebugMenu>();
			this._modioDebugMenu.Awake();
		}

		private void OnEnable()
		{
			ModioEnableDebugMenu modioEnableDebugMenu;
			if (ModioClient.Settings.TryGetPlatformSettings<ModioEnableDebugMenu>(out modioEnableDebugMenu))
			{
				ModioUIInput.AddHandler(ModioUIInput.ModioAction.DeveloperMenu, new Action(base.OpenPanel));
			}
			ModioSettings settings;
			if (!ModioServices.TryResolve<ModioSettings>(out settings))
			{
				ModioUnitySettings modioUnitySettings = Resources.Load<ModioUnitySettings>("mod.io/v3_config_local");
				if (modioUnitySettings == null)
				{
					modioUnitySettings = Resources.Load<ModioUnitySettings>("mod.io/v3_config");
				}
				if (modioUnitySettings == null)
				{
					Debug.LogError("Couldn't find bound Settings or settings file");
					return;
				}
				settings = modioUnitySettings.Settings;
			}
			this._settings = settings.ShallowClone();
		}

		private void OnDisable()
		{
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.DeveloperMenu, new Action(base.OpenPanel));
		}

		public override void OnGainedFocus(ModioPanelBase.GainedFocusCause selectionBehaviour)
		{
			this.FindAllHookups();
			base.OnGainedFocus(selectionBehaviour);
			if (selectionBehaviour == ModioPanelBase.GainedFocusCause.OpeningFromClosed)
			{
				this._modioDebugMenu.SetToDefaults();
			}
		}

		private void FindAllHookups()
		{
			if (this._hasDoneHookup)
			{
				return;
			}
			this._hasDoneHookup = true;
			this._modioDebugMenu.AddAllMethodsOrPropertiesWithAttribute<ModioDebugMenuAttribute>((ModioDebugMenuAttribute attribute) => attribute.ShowInBrowserMenu);
			this._modioDebugMenu.AddLabel("\nNetwork Settings");
			this._modioDebugMenu.AddToggle("Fake Disconnected (global)", () => this.<FindAllHookups>g__Get|7_9<ModioAPITestSettings>().FakeDisconnected, delegate(bool on)
			{
				this.<FindAllHookups>g__Get|7_9<ModioAPITestSettings>().FakeDisconnected = on;
			});
			this._modioDebugMenu.AddTextField("Fake Disconnected (regex)", () => this.<FindAllHookups>g__Get|7_9<ModioAPITestSettings>().FakeDisconnectedOnEndpointRegex, delegate(string regex)
			{
				this.<FindAllHookups>g__Get|7_9<ModioAPITestSettings>().FakeDisconnectedOnEndpointRegex = regex;
			});
			this._modioDebugMenu.AddToggle("Fake Ratelimit (global)", () => this.<FindAllHookups>g__Get|7_9<ModioAPITestSettings>().RateLimitError, delegate(bool on)
			{
				this.<FindAllHookups>g__Get|7_9<ModioAPITestSettings>().RateLimitError = on;
			});
			this._modioDebugMenu.AddTextField("Fake Ratelimit (regex)", () => this.<FindAllHookups>g__Get|7_9<ModioAPITestSettings>().RateLimitOnEndpointRegex, delegate(string regex)
			{
				this.<FindAllHookups>g__Get|7_9<ModioAPITestSettings>().RateLimitOnEndpointRegex = regex;
			});
		}

		[CompilerGenerated]
		private T <FindAllHookups>g__Get|7_9<T>() where T : IModioServiceSettings, new()
		{
			T t = this._settings.GetPlatformSettings<T>();
			if (t == null)
			{
				t = Activator.CreateInstance<T>();
				this._settings.PlatformSettings = this._settings.PlatformSettings.Append(t).ToArray<IModioServiceSettings>();
			}
			return t;
		}

		private bool _hasDoneHookup;

		private ModioDebugMenu _modioDebugMenu;

		private ModioSettings _settings;
	}
}
