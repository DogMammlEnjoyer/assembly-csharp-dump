using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Fusion
{
	[RequireComponent(typeof(FusionBootstrap))]
	[AddComponentMenu("Fusion/Fusion Boostrap Debug GUI")]
	[ScriptHelp(BackColor = ScriptHeaderBackColor.Steel)]
	public class FusionBootstrapDebugGUI : Behaviour
	{
		protected virtual void OnValidate()
		{
			this.ValidateClientCount();
		}

		protected void ValidateClientCount()
		{
			if (this._clientCount == null)
			{
				this._clientCount = "1";
				return;
			}
			this._clientCount = Regex.Replace(this._clientCount, "[^0-9]", "");
		}

		protected int GetClientCount()
		{
			int result;
			try
			{
				result = Convert.ToInt32(this._clientCount);
			}
			catch
			{
				result = 0;
			}
			return result;
		}

		protected virtual void Awake()
		{
			this._nicifiedStageNames = FusionBootstrapDebugGUI.ConvertEnumToNicifiedNameLookup<FusionBootstrap.Stage>("Fusion Status: ", null);
			this._networkDebugStart = this.EnsureNetworkDebugStartExists();
			this._clientCount = this._networkDebugStart.AutoClients.ToString();
			this.ValidateClientCount();
		}

		protected virtual void Start()
		{
			this._isMultiplePeerMode = (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple);
		}

		protected FusionBootstrap EnsureNetworkDebugStartExists()
		{
			if (this._networkDebugStart && this._networkDebugStart.gameObject == base.gameObject)
			{
				return this._networkDebugStart;
			}
			FusionBootstrap fusionBootstrap;
			if (base.TryGetBehaviour<FusionBootstrap>(out fusionBootstrap))
			{
				this._networkDebugStart = fusionBootstrap;
				return fusionBootstrap;
			}
			this._networkDebugStart = base.AddBehaviour<FusionBootstrap>();
			return this._networkDebugStart;
		}

		private void Update()
		{
			FusionBootstrap fusionBootstrap = this.EnsureNetworkDebugStartExists();
			if (!fusionBootstrap.ShouldShowGUI)
			{
				return;
			}
			if (fusionBootstrap.CurrentStage != FusionBootstrap.Stage.Disconnected)
			{
				return;
			}
			if (this.EnableHotkeys)
			{
				if (Input.GetKeyDown(KeyCode.I))
				{
					this._networkDebugStart.StartSinglePlayer();
				}
				if (Input.GetKeyDown(KeyCode.H))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartHostWithClients(this._networkDebugStart);
					}
					else
					{
						this._networkDebugStart.StartHost();
					}
				}
				if (Input.GetKeyDown(KeyCode.S))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartServerWithClients(this._networkDebugStart);
					}
					else
					{
						this._networkDebugStart.StartServer();
					}
				}
				if (Input.GetKeyDown(KeyCode.C))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartMultipleClients(fusionBootstrap);
					}
					else
					{
						fusionBootstrap.StartClient();
					}
				}
				if (Input.GetKeyDown(KeyCode.A))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartMultipleAutoClients(fusionBootstrap);
					}
					else
					{
						fusionBootstrap.StartAutoClient();
					}
				}
				if (Input.GetKeyDown(KeyCode.P))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartMultipleSharedClients(fusionBootstrap);
						return;
					}
					fusionBootstrap.StartSharedClient();
				}
			}
		}

		protected virtual void OnGUI()
		{
			FusionBootstrap fusionBootstrap = this.EnsureNetworkDebugStartExists();
			if (!fusionBootstrap.ShouldShowGUI)
			{
				return;
			}
			FusionBootstrap.Stage currentStage = fusionBootstrap.CurrentStage;
			if (fusionBootstrap.AutoHideGUI && currentStage == FusionBootstrap.Stage.AllConnected)
			{
				return;
			}
			GUISkin skin = GUI.skin;
			float num;
			float num2;
			int num3;
			int num4;
			float x;
			GUI.skin = FusionScalableIMGUI.GetScaledSkin(this.BaseSkin, out num, out num2, out num3, out num4, out x);
			GUILayout.BeginArea(new Rect(x, (float)num4, num2, (float)Screen.height));
			GUILayout.BeginVertical(GUI.skin.window, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(num)
			});
			string text;
			GUILayout.Label(this._nicifiedStageNames.TryGetValue(fusionBootstrap.CurrentStage, out text) ? text : "Unrecognized Stage", new GUIStyle(GUI.skin.label)
			{
				fontSize = (int)((float)GUI.skin.label.fontSize * 0.8f),
				alignment = TextAnchor.UpperLeft
			}, Array.Empty<GUILayoutOption>());
			if (!fusionBootstrap.AutoHideGUI && fusionBootstrap.CurrentStage == FusionBootstrap.Stage.AllConnected && GUILayout.Button("X", new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.Width(num)
			}))
			{
				fusionBootstrap.AutoHideGUI = true;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.BeginVertical(GUI.skin.window, Array.Empty<GUILayoutOption>());
			if (currentStage == FusionBootstrap.Stage.Disconnected)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label("Room:", new GUILayoutOption[]
				{
					GUILayout.Height(num),
					GUILayout.Width(num2 * 0.33f)
				});
				fusionBootstrap.DefaultRoomName = GUILayout.TextField(fusionBootstrap.DefaultRoomName, 25, new GUILayoutOption[]
				{
					GUILayout.Height(num)
				});
				GUILayout.EndHorizontal();
				if (GUILayout.Button(this.EnableHotkeys ? "Start Single Player (I)" : "Start Single Player", new GUILayoutOption[]
				{
					GUILayout.Height(num)
				}))
				{
					fusionBootstrap.StartSinglePlayer();
				}
				if (GUILayout.Button(this.EnableHotkeys ? "Start Shared Client (P)" : "Start Shared Client", new GUILayoutOption[]
				{
					GUILayout.Height(num)
				}))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartMultipleSharedClients(fusionBootstrap);
					}
					else
					{
						fusionBootstrap.StartSharedClient();
					}
				}
				if (GUILayout.Button(this.EnableHotkeys ? "Start Server (S)" : "Start Server", new GUILayoutOption[]
				{
					GUILayout.Height(num)
				}))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartServerWithClients(fusionBootstrap);
					}
					else
					{
						fusionBootstrap.StartServer();
					}
				}
				if (GUILayout.Button(this.EnableHotkeys ? "Start Host (H)" : "Start Host", new GUILayoutOption[]
				{
					GUILayout.Height(num)
				}))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartHostWithClients(fusionBootstrap);
					}
					else
					{
						fusionBootstrap.StartHost();
					}
				}
				if (GUILayout.Button(this.EnableHotkeys ? "Start Client (C)" : "Start Client", new GUILayoutOption[]
				{
					GUILayout.Height(num)
				}))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartMultipleClients(fusionBootstrap);
					}
					else
					{
						fusionBootstrap.StartClient();
					}
				}
				if (GUILayout.Button(this.EnableHotkeys ? "Start Auto Host Or Client (A)" : "Start Auto Host Or Client", new GUILayoutOption[]
				{
					GUILayout.Height(num)
				}))
				{
					if (this._isMultiplePeerMode)
					{
						this.StartMultipleAutoClients(fusionBootstrap);
					}
					else
					{
						fusionBootstrap.StartAutoClient();
					}
				}
				if (this._isMultiplePeerMode)
				{
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Label("Client Count:", new GUILayoutOption[]
					{
						GUILayout.Height(num)
					});
					GUILayout.Label("", new GUILayoutOption[]
					{
						GUILayout.Width(4f)
					});
					string text2 = GUILayout.TextField(this._clientCount, 10, new GUILayoutOption[]
					{
						GUILayout.Width(num2 * 0.25f),
						GUILayout.Height(num)
					});
					if (this._clientCount != text2)
					{
						this._clientCount = text2;
						this.ValidateClientCount();
					}
					GUILayout.EndHorizontal();
				}
			}
			else if (GUILayout.Button("Shutdown", new GUILayoutOption[]
			{
				GUILayout.Height(num)
			}))
			{
				this._networkDebugStart.ShutdownAll();
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
			GUI.skin = skin;
		}

		private void StartHostWithClients(FusionBootstrap nds)
		{
			int clientCount;
			try
			{
				clientCount = Convert.ToInt32(this._clientCount);
			}
			catch
			{
				clientCount = 0;
			}
			nds.StartHostPlusClients(clientCount);
		}

		private void StartServerWithClients(FusionBootstrap nds)
		{
			int clientCount;
			try
			{
				clientCount = Convert.ToInt32(this._clientCount);
			}
			catch
			{
				clientCount = 0;
			}
			nds.StartServerPlusClients(clientCount);
		}

		private void StartMultipleClients(FusionBootstrap nds)
		{
			int clientCount;
			try
			{
				clientCount = Convert.ToInt32(this._clientCount);
			}
			catch
			{
				clientCount = 0;
			}
			nds.StartMultipleClients(clientCount);
		}

		private void StartMultipleAutoClients(FusionBootstrap nds)
		{
			int clientCount;
			int.TryParse(this._clientCount, out clientCount);
			nds.StartMultipleAutoClients(clientCount);
		}

		private void StartMultipleSharedClients(FusionBootstrap nds)
		{
			int clientCount;
			try
			{
				clientCount = Convert.ToInt32(this._clientCount);
			}
			catch
			{
				clientCount = 0;
			}
			nds.StartMultipleSharedClients(clientCount);
		}

		public static Dictionary<T, string> ConvertEnumToNicifiedNameLookup<T>(string prefix = null, Dictionary<T, string> nonalloc = null) where T : Enum
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (nonalloc == null)
			{
				nonalloc = new Dictionary<T, string>();
			}
			else
			{
				nonalloc.Clear();
			}
			string[] names = Enum.GetNames(typeof(T));
			Array values = Enum.GetValues(typeof(T));
			int i = 0;
			int num = names.Length;
			while (i < num)
			{
				stringBuilder.Clear();
				if (prefix != null)
				{
					stringBuilder.Append(prefix);
				}
				string text = names[i];
				for (int j = 0; j < text.Length; j++)
				{
					if (char.IsUpper(text[j]) && j != 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(text[j]);
				}
				nonalloc.Add((T)((object)values.GetValue(i)), stringBuilder.ToString());
				i++;
			}
			return nonalloc;
		}

		[InlineHelp]
		public bool EnableHotkeys;

		[InlineHelp]
		public GUISkin BaseSkin;

		private FusionBootstrap _networkDebugStart;

		private string _clientCount;

		private bool _isMultiplePeerMode;

		private Dictionary<FusionBootstrap.Stage, string> _nicifiedStageNames;
	}
}
