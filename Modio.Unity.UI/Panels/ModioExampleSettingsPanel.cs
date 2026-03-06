using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Modio.API;
using Modio.Authentication;
using Modio.Extensions;
using Modio.FileIO;
using Modio.Unity.Settings;
using UnityEngine;

namespace Modio.Unity.UI.Panels
{
	public class ModioExampleSettingsPanel : ModioPanelBase
	{
		private void OnEnable()
		{
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

		public override void OnGainedFocus(ModioPanelBase.GainedFocusCause selectionBehaviour)
		{
			base.OnGainedFocus(selectionBehaviour);
			if (selectionBehaviour == ModioPanelBase.GainedFocusCause.OpeningFromClosed)
			{
				this.SetupButtons();
			}
		}

		private void SetupButtons()
		{
			if (this._hasDoneSetup)
			{
				return;
			}
			this._hasDoneSetup = true;
			this._debugMenu = base.GetComponent<ModioDebugMenu>();
			this._debugMenu.AddLabel("Enter the Game Id and Key for the game you'd like to browse mods for.\nThen, scroll down and hit 'Apply changed settings'");
			this._debugMenu.AddTextField("Game Id:", () => this._settings.GameId, delegate(long id)
			{
				this._settings.GameId = id;
				if (this._settings.ServerURL.Contains("api-staging"))
				{
					this._settings.ServerURL = this.StagingUrl();
					return;
				}
				if (this._settings.ServerURL.Contains("test"))
				{
					this._settings.ServerURL = this.TestUrl(this._settings.GameId);
					return;
				}
				this._settings.ServerURL = this.ProductionUrl(this._settings.GameId);
			});
			this._debugMenu.AddTextField("Game Key:", () => this._settings.APIKey, delegate(string key)
			{
				this._settings.APIKey = key;
			});
			this._debugMenu.AddToggle("Production Environment", () => !this._settings.ServerURL.Contains("api-staging") && !this._settings.ServerURL.Contains("test"), delegate(bool production)
			{
				if (production)
				{
					this._settings.ServerURL = this.ProductionUrl(this._settings.GameId);
				}
				this._debugMenu.SetToDefaults();
			});
			this._debugMenu.AddToggle("Staging Environment", () => this._settings.ServerURL.Contains("api-staging"), delegate(bool staging)
			{
				if (staging)
				{
					this._settings.ServerURL = this.StagingUrl();
				}
				this._debugMenu.SetToDefaults();
			});
			this._debugMenu.AddToggle("Test Environment", () => this._settings.ServerURL.Contains("test"), delegate(bool test)
			{
				if (test)
				{
					this._settings.ServerURL = this.TestUrl(this._settings.GameId);
				}
				this._debugMenu.SetToDefaults();
			});
			this._debugMenu.AddTextField("Default Language:", () => this._settings.DefaultLanguage, delegate(string isoCode)
			{
				this._settings.DefaultLanguage = isoCode;
			});
			this._debugMenu.AddLabel("\nTUI Settings");
			this._debugMenu.AddToggle("Show monetization", () => this.<SetupButtons>g__Get|5_35<ModioComponentUISettings>().ShowMonetizationUI, delegate(bool on)
			{
				this.<SetupButtons>g__Get|5_35<ModioComponentUISettings>().ShowMonetizationUI = on;
			});
			this._debugMenu.AddToggle("Show enabled", () => this.<SetupButtons>g__Get|5_35<ModioComponentUISettings>().ShowEnableModToggle, delegate(bool on)
			{
				this.<SetupButtons>g__Get|5_35<ModioComponentUISettings>().ShowEnableModToggle = on;
			});
			this._debugMenu.AddToggle("Fallback to email authentication", () => this.<SetupButtons>g__Get|5_35<ModioComponentUISettings>().FallbackToEmailAuthentication, delegate(bool on)
			{
				this.<SetupButtons>g__Get|5_35<ModioComponentUISettings>().FallbackToEmailAuthentication = on;
			});
			this._debugMenu.AddLabel("\nDisk Settings");
			this._debugMenu.AddToggle("Override Disk Space Remaining", () => this.<SetupButtons>g__Get|5_35<ModioDiskTestSettings>().OverrideDiskSpaceRemaining, delegate(bool on)
			{
				this.<SetupButtons>g__Get|5_35<ModioDiskTestSettings>().OverrideDiskSpaceRemaining = on;
			});
			this._debugMenu.AddTextField("Fake Bytes Remaining", () => this.<SetupButtons>g__Get|5_35<ModioDiskTestSettings>().BytesRemaining, delegate(int on)
			{
				this.<SetupButtons>g__Get|5_35<ModioDiskTestSettings>().BytesRemaining = on;
			});
			this._debugMenu.AddLabel("\nNetwork Settings");
			this._debugMenu.AddToggle("Fake Disconnected (global)", () => this.<SetupButtons>g__Get|5_35<ModioAPITestSettings>().FakeDisconnected, delegate(bool on)
			{
				this.<SetupButtons>g__Get|5_35<ModioAPITestSettings>().FakeDisconnected = on;
			});
			this._debugMenu.AddTextField("Fake Disconnected (regex)", () => this.<SetupButtons>g__Get|5_35<ModioAPITestSettings>().FakeDisconnectedOnEndpointRegex, delegate(string regex)
			{
				this.<SetupButtons>g__Get|5_35<ModioAPITestSettings>().FakeDisconnectedOnEndpointRegex = regex;
			});
			this._debugMenu.AddToggle("Fake Ratelimit (global)", () => this.<SetupButtons>g__Get|5_35<ModioAPITestSettings>().RateLimitError, delegate(bool on)
			{
				this.<SetupButtons>g__Get|5_35<ModioAPITestSettings>().RateLimitError = on;
			});
			this._debugMenu.AddTextField("Fake Ratelimit (regex)", () => this.<SetupButtons>g__Get|5_35<ModioAPITestSettings>().RateLimitOnEndpointRegex, delegate(string regex)
			{
				this.<SetupButtons>g__Get|5_35<ModioAPITestSettings>().RateLimitOnEndpointRegex = regex;
			});
			this._debugMenu.AddLabel("\nIn browser debug menu");
			this._debugMenu.AddToggle("Enable", delegate
			{
				ModioEnableDebugMenu modioEnableDebugMenu;
				return this._settings.TryGetPlatformSettings<ModioEnableDebugMenu>(out modioEnableDebugMenu);
			}, delegate(bool on)
			{
				if (on)
				{
					this.<SetupButtons>g__Get|5_35<ModioEnableDebugMenu>();
					return;
				}
				this._settings.PlatformSettings = (from s in this._settings.PlatformSettings
				where !(s is ModioEnableDebugMenu)
				select s).ToArray<IModioServiceSettings>();
			});
			this._debugMenu.AddLabel("");
			this._debugMenu.AddButton("Apply Changed Settings", delegate
			{
				ModioServices.BindInstance<ModioSettings>(this._settings, ModioServicePriority.DeveloperOverride);
				ModioClient.Shutdown().ForgetTaskSafely();
				base.ClosePanel();
			});
			this._debugMenu.AddButton("Cancel Changed Settings", delegate
			{
				ModioSettings modioSettings;
				if (ModioServices.TryResolve<ModioSettings>(out modioSettings))
				{
					this._settings = modioSettings.ShallowClone();
				}
				this._debugMenu.SetToDefaults();
			});
			this._debugMenu.AddLabel("\nAuth Platform");
			ModioMultiplatformAuthResolver.Initialize();
			using (IEnumerator<IModioAuthService> enumerator = ModioMultiplatformAuthResolver.AuthBindings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IModioAuthService modioAuthPlatform = enumerator.Current;
					this._debugMenu.AddToggle(ModioDebugMenu.Nicify(modioAuthPlatform.GetType().Name), () => ModioMultiplatformAuthResolver.ServiceOverride == modioAuthPlatform, delegate(bool on)
					{
						if (on)
						{
							ModioMultiplatformAuthResolver.ServiceOverride = modioAuthPlatform;
						}
						this._debugMenu.SetToDefaults();
						if (ModioClient.IsInitialized)
						{
							ModioClient.Shutdown().ForgetTaskSafely();
						}
					});
				}
			}
			this._debugMenu.AddLabel("\nMisc Discovered Settings");
			this._debugMenu.AddAllMethodsOrPropertiesWithAttribute<ModioDebugMenuAttribute>((ModioDebugMenuAttribute attribute) => attribute.ShowInSettingsMenu);
			this._debugMenu.SetToDefaults();
		}

		private string StagingUrl()
		{
			return "https://api-staging.moddemo.io/v1";
		}

		private string ProductionUrl(long gameId)
		{
			return string.Format("https://g-{0}.modapi.io/v1", gameId);
		}

		private string TestUrl(long gameId)
		{
			return string.Format("https://g-{0}.test.mod.io/v1", gameId);
		}

		[CompilerGenerated]
		private T <SetupButtons>g__Get|5_35<T>() where T : IModioServiceSettings, new()
		{
			T t = this._settings.GetPlatformSettings<T>();
			if (t == null)
			{
				t = Activator.CreateInstance<T>();
				this._settings.PlatformSettings = this._settings.PlatformSettings.Append(t).ToArray<IModioServiceSettings>();
			}
			return t;
		}

		private bool _hasDoneSetup;

		private ModioSettings _settings = new ModioSettings();

		private ModioDebugMenu _debugMenu;
	}
}
