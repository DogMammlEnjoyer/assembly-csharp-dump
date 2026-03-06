using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public class InputActionAsset : ScriptableObject, IInputActionCollection2, IInputActionCollection, IEnumerable<InputAction>, IEnumerable
	{
		public bool enabled
		{
			get
			{
				using (ReadOnlyArray<InputActionMap>.Enumerator enumerator = this.actionMaps.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.enabled)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public ReadOnlyArray<InputActionMap> actionMaps
		{
			get
			{
				return new ReadOnlyArray<InputActionMap>(this.m_ActionMaps);
			}
		}

		public ReadOnlyArray<InputControlScheme> controlSchemes
		{
			get
			{
				return new ReadOnlyArray<InputControlScheme>(this.m_ControlSchemes);
			}
		}

		public IEnumerable<InputBinding> bindings
		{
			get
			{
				int numActionMaps = this.m_ActionMaps.LengthSafe<InputActionMap>();
				if (numActionMaps == 0)
				{
					yield break;
				}
				int num;
				for (int i = 0; i < numActionMaps; i = num)
				{
					InputActionMap inputActionMap = this.m_ActionMaps[i];
					InputBinding[] bindings = inputActionMap.m_Bindings;
					int numBindings = bindings.LengthSafe<InputBinding>();
					for (int j = 0; j < numBindings; j = num)
					{
						yield return bindings[j];
						num = j + 1;
					}
					bindings = null;
					num = i + 1;
				}
				yield break;
			}
		}

		public InputBinding? bindingMask
		{
			get
			{
				return this.m_BindingMask;
			}
			set
			{
				if (this.m_BindingMask == value)
				{
					return;
				}
				this.m_BindingMask = value;
				this.ReResolveIfNecessary(true);
			}
		}

		public ReadOnlyArray<InputDevice>? devices
		{
			get
			{
				return this.m_Devices.Get();
			}
			set
			{
				if (this.m_Devices.Set(value))
				{
					this.ReResolveIfNecessary(false);
				}
			}
		}

		public InputAction this[string actionNameOrId]
		{
			get
			{
				InputAction inputAction = this.FindAction(actionNameOrId, false);
				if (inputAction == null)
				{
					throw new KeyNotFoundException(string.Format("Cannot find action '{0}' in '{1}'", actionNameOrId, this));
				}
				return inputAction;
			}
		}

		public string ToJson()
		{
			bool flag = this.m_ActionMaps.LengthSafe<InputActionMap>() > 0 || this.m_ControlSchemes.LengthSafe<InputControlScheme>() > 0;
			return JsonUtility.ToJson(new InputActionAsset.WriteFileJson
			{
				version = (flag ? 1 : 0),
				name = base.name,
				maps = InputActionMap.WriteFileJson.FromMaps(this.m_ActionMaps).maps,
				controlSchemes = InputControlScheme.SchemeJson.ToJson(this.m_ControlSchemes)
			}, true);
		}

		public void LoadFromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				throw new ArgumentNullException("json");
			}
			InputActionAsset.ReadFileJson readFileJson = JsonUtility.FromJson<InputActionAsset.ReadFileJson>(json);
			this.MigrateJson(ref readFileJson);
			readFileJson.ToAsset(this);
		}

		public static InputActionAsset FromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				throw new ArgumentNullException("json");
			}
			InputActionAsset inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
			inputActionAsset.LoadFromJson(json);
			return inputActionAsset;
		}

		public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
		{
			if (actionNameOrId == null)
			{
				throw new ArgumentNullException("actionNameOrId");
			}
			if (this.m_ActionMaps != null)
			{
				int num = actionNameOrId.IndexOf('/');
				if (num >= 0)
				{
					Substring right = new Substring(actionNameOrId, 0, num);
					Substring right2 = new Substring(actionNameOrId, num + 1);
					if (right.isEmpty || right2.isEmpty)
					{
						throw new ArgumentException("Malformed action path: " + actionNameOrId, "actionNameOrId");
					}
					int i = 0;
					while (i < this.m_ActionMaps.Length)
					{
						InputActionMap inputActionMap = this.m_ActionMaps[i];
						if (Substring.Compare(inputActionMap.name, right, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							InputAction[] actions = inputActionMap.m_Actions;
							if (actions != null)
							{
								foreach (InputAction inputAction in actions)
								{
									if (Substring.Compare(inputAction.name, right2, StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										return inputAction;
									}
								}
								break;
							}
							break;
						}
						else
						{
							i++;
						}
					}
				}
				InputAction inputAction2 = null;
				for (int k = 0; k < this.m_ActionMaps.Length; k++)
				{
					InputAction inputAction3 = this.m_ActionMaps[k].FindAction(actionNameOrId, false);
					if (inputAction3 != null)
					{
						if (inputAction3.enabled || inputAction3.m_Id == actionNameOrId)
						{
							return inputAction3;
						}
						if (inputAction2 == null)
						{
							inputAction2 = inputAction3;
						}
					}
				}
				if (inputAction2 != null)
				{
					return inputAction2;
				}
			}
			if (throwIfNotFound)
			{
				throw new ArgumentException(string.Format("No action '{0}' in '{1}'", actionNameOrId, this));
			}
			return null;
		}

		public int FindBinding(InputBinding mask, out InputAction action)
		{
			int num = this.m_ActionMaps.LengthSafe<InputActionMap>();
			for (int i = 0; i < num; i++)
			{
				int num2 = this.m_ActionMaps[i].FindBinding(mask, out action);
				if (num2 >= 0)
				{
					return num2;
				}
			}
			action = null;
			return -1;
		}

		public InputActionMap FindActionMap(string nameOrId, bool throwIfNotFound = false)
		{
			if (nameOrId == null)
			{
				throw new ArgumentNullException("nameOrId");
			}
			if (this.m_ActionMaps == null)
			{
				return null;
			}
			Guid b;
			if (nameOrId.Contains('-') && Guid.TryParse(nameOrId, out b))
			{
				for (int i = 0; i < this.m_ActionMaps.Length; i++)
				{
					InputActionMap inputActionMap = this.m_ActionMaps[i];
					if (inputActionMap.idDontGenerate == b)
					{
						return inputActionMap;
					}
				}
			}
			for (int j = 0; j < this.m_ActionMaps.Length; j++)
			{
				InputActionMap inputActionMap2 = this.m_ActionMaps[j];
				if (string.Compare(nameOrId, inputActionMap2.name, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return inputActionMap2;
				}
			}
			if (throwIfNotFound)
			{
				throw new ArgumentException(string.Format("Cannot find action map '{0}' in '{1}'", nameOrId, this));
			}
			return null;
		}

		public InputActionMap FindActionMap(Guid id)
		{
			if (this.m_ActionMaps == null)
			{
				return null;
			}
			for (int i = 0; i < this.m_ActionMaps.Length; i++)
			{
				InputActionMap inputActionMap = this.m_ActionMaps[i];
				if (inputActionMap.idDontGenerate == id)
				{
					return inputActionMap;
				}
			}
			return null;
		}

		public InputAction FindAction(Guid guid)
		{
			if (this.m_ActionMaps == null)
			{
				return null;
			}
			for (int i = 0; i < this.m_ActionMaps.Length; i++)
			{
				InputAction inputAction = this.m_ActionMaps[i].FindAction(guid);
				if (inputAction != null)
				{
					return inputAction;
				}
			}
			return null;
		}

		public int FindControlSchemeIndex(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (this.m_ControlSchemes == null)
			{
				return -1;
			}
			for (int i = 0; i < this.m_ControlSchemes.Length; i++)
			{
				if (string.Compare(name, this.m_ControlSchemes[i].name, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public InputControlScheme? FindControlScheme(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			int num = this.FindControlSchemeIndex(name);
			if (num == -1)
			{
				return null;
			}
			return new InputControlScheme?(this.m_ControlSchemes[num]);
		}

		public bool IsUsableWithDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			int num = this.m_ControlSchemes.LengthSafe<InputControlScheme>();
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					if (this.m_ControlSchemes[i].SupportsDevice(device))
					{
						return true;
					}
				}
			}
			else
			{
				int num2 = this.m_ActionMaps.LengthSafe<InputActionMap>();
				for (int j = 0; j < num2; j++)
				{
					if (this.m_ActionMaps[j].IsUsableWithDevice(device))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Enable()
		{
			foreach (InputActionMap inputActionMap in this.actionMaps)
			{
				inputActionMap.Enable();
			}
		}

		public void Disable()
		{
			foreach (InputActionMap inputActionMap in this.actionMaps)
			{
				inputActionMap.Disable();
			}
		}

		public bool Contains(InputAction action)
		{
			InputActionMap inputActionMap = (action != null) ? action.actionMap : null;
			return inputActionMap != null && inputActionMap.asset == this;
		}

		public IEnumerator<InputAction> GetEnumerator()
		{
			if (this.m_ActionMaps == null)
			{
				yield break;
			}
			int num;
			for (int i = 0; i < this.m_ActionMaps.Length; i = num)
			{
				ReadOnlyArray<InputAction> actions = this.m_ActionMaps[i].actions;
				int actionCount = actions.Count;
				for (int j = 0; j < actionCount; j = num)
				{
					yield return actions[j];
					num = j + 1;
				}
				actions = default(ReadOnlyArray<InputAction>);
				num = i + 1;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal void MarkAsDirty()
		{
		}

		internal bool IsEmpty()
		{
			return this.actionMaps.Count == 0 && this.controlSchemes.Count == 0;
		}

		internal void OnWantToChangeSetup()
		{
			if (this.m_ActionMaps.LengthSafe<InputActionMap>() > 0)
			{
				this.m_ActionMaps[0].OnWantToChangeSetup();
			}
		}

		internal void OnSetupChanged()
		{
			this.MarkAsDirty();
			if (this.m_ActionMaps.LengthSafe<InputActionMap>() > 0)
			{
				this.m_ActionMaps[0].OnSetupChanged();
				return;
			}
			this.m_SharedStateForAllMaps = null;
		}

		private void ReResolveIfNecessary(bool fullResolve)
		{
			if (this.m_SharedStateForAllMaps == null)
			{
				return;
			}
			this.m_ActionMaps[0].LazyResolveBindings(fullResolve);
		}

		internal void ResolveBindingsIfNecessary()
		{
			if (this.m_ActionMaps.LengthSafe<InputActionMap>() > 0)
			{
				InputActionMap[] actionMaps = this.m_ActionMaps;
				int num = 0;
				while (num < actionMaps.Length && !actionMaps[num].ResolveBindingsIfNecessary())
				{
					num++;
				}
			}
		}

		private void OnDestroy()
		{
			this.Disable();
			if (this.m_SharedStateForAllMaps != null)
			{
				this.m_SharedStateForAllMaps.Dispose();
				this.m_SharedStateForAllMaps = null;
			}
		}

		internal void MigrateJson(ref InputActionAsset.ReadFileJson parsedJson)
		{
			if (parsedJson.version >= 1)
			{
				return;
			}
			InputActionMap.ReadMapJson[] maps = parsedJson.maps;
			if (((maps != null) ? maps.Length : 0) > 0 && parsedJson.version < 1)
			{
				for (int i = 0; i < parsedJson.maps.Length; i++)
				{
					InputActionMap.ReadMapJson readMapJson = parsedJson.maps[i];
					for (int j = 0; j < readMapJson.actions.Length; j++)
					{
						InputActionMap.ReadActionJson readActionJson = readMapJson.actions[j];
						string processors = readActionJson.processors;
						if (!string.IsNullOrEmpty(processors))
						{
							List<NameAndParameters> list = NameAndParameters.ParseMultiple(processors).ToList<NameAndParameters>();
							List<string> list2 = new List<string>(list.Count);
							foreach (NameAndParameters nameAndParameters in list)
							{
								Type type = InputSystem.TryGetProcessor(nameAndParameters.name);
								if (nameAndParameters.parameters.Count == 0 || type == null)
								{
									list2.Add(nameAndParameters.ToString());
								}
								else
								{
									Dictionary<string, string> dictionary = nameAndParameters.parameters.ToDictionary((NamedValue p) => p.name, (NamedValue p) => p.value.ToString());
									bool flag = false;
									foreach (FieldInfo fieldInfo in from f in type.GetFields(BindingFlags.Instance | BindingFlags.Public)
									where f.FieldType.IsEnum
									select f)
									{
										string s;
										int num;
										if (dictionary.TryGetValue(fieldInfo.Name, out s) && int.TryParse(s, out num))
										{
											object[] array = Enum.GetValues(fieldInfo.FieldType).Cast<object>().ToArray<object>();
											if (num >= 0 && num < array.Length)
											{
												dictionary[fieldInfo.Name] = Convert.ToInt32(array[num]).ToString();
												flag = true;
											}
										}
									}
									if (!flag)
									{
										list2.Add(nameAndParameters.ToString());
									}
									else
									{
										string str = string.Join(",", from kv in dictionary
										select kv.Key + "=" + kv.Value);
										list2.Add(nameAndParameters.name + "(" + str + ")");
									}
								}
							}
							readActionJson.processors = string.Join(";", list2);
							readMapJson.actions[j] = readActionJson;
						}
					}
					parsedJson.maps[i] = readMapJson;
				}
			}
			parsedJson.version = 1;
		}

		public const string Extension = "inputactions";

		internal const string kDefaultAssetLayoutJson = "{}";

		[SerializeField]
		internal InputActionMap[] m_ActionMaps;

		[SerializeField]
		internal InputControlScheme[] m_ControlSchemes;

		[SerializeField]
		internal bool m_IsProjectWide;

		[NonSerialized]
		internal InputActionState m_SharedStateForAllMaps;

		[NonSerialized]
		internal InputBinding? m_BindingMask;

		[NonSerialized]
		internal int m_ParameterOverridesCount;

		[NonSerialized]
		internal InputActionRebindingExtensions.ParameterOverride[] m_ParameterOverrides;

		[NonSerialized]
		internal InputActionMap.DeviceArray m_Devices;

		private static class JsonVersion
		{
			public const int Version0 = 0;

			public const int Version1 = 1;

			public const int Current = 1;
		}

		[Serializable]
		internal struct WriteFileJson
		{
			public int version;

			public string name;

			public InputActionMap.WriteMapJson[] maps;

			public InputControlScheme.SchemeJson[] controlSchemes;
		}

		[Serializable]
		internal struct WriteFileJsonNoName
		{
			public InputActionMap.WriteMapJson[] maps;

			public InputControlScheme.SchemeJson[] controlSchemes;
		}

		[Serializable]
		internal struct ReadFileJson
		{
			public void ToAsset(InputActionAsset asset)
			{
				asset.name = this.name;
				InputActionMap.ReadFileJson readFileJson = default(InputActionMap.ReadFileJson);
				readFileJson.maps = this.maps;
				asset.m_ActionMaps = readFileJson.ToMaps();
				asset.m_ControlSchemes = InputControlScheme.SchemeJson.ToSchemes(this.controlSchemes);
				if (asset.m_ActionMaps != null)
				{
					InputActionMap[] actionMaps = asset.m_ActionMaps;
					for (int i = 0; i < actionMaps.Length; i++)
					{
						actionMaps[i].m_Asset = asset;
					}
				}
			}

			public int version;

			public string name;

			public InputActionMap.ReadMapJson[] maps;

			public InputControlScheme.SchemeJson[] controlSchemes;
		}
	}
}
