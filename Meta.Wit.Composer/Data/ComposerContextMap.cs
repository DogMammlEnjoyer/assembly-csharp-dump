using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meta.Voice.Logging;
using Meta.WitAi.Json;
using UnityEngine.Events;

namespace Meta.WitAi.Composer.Data
{
	public class ComposerContextMap : PluggableBase<IContextMapReservedPathExtension>
	{
		public WitResponseClass Data { get; private set; }

		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Composer, null);

		public UnityEvent OnContextMapChanged { get; } = new UnityEvent();

		public UnityEvent<string, string, string> OnContextMapValueChanged { get; } = new UnityEvent<string, string, string>();

		public UnityEvent<string> OnContextMapValueRemoved { get; } = new UnityEvent<string>();

		public ComposerContextMap()
		{
			PluggableBase<IContextMapReservedPathExtension>.CheckForPlugins();
			this.Data = new WitResponseClass();
		}

		public bool HasData(string key)
		{
			return this.Data != null && this.Data.HasChild(key);
		}

		private WitResponseClass GetParentAndNodeName(string key, out string childNodeName)
		{
			string[] array = key.Split(".", StringSplitOptions.None);
			WitResponseClass witResponseClass = this.Data;
			int i = 0;
			while (i < array.Length - 1)
			{
				string text = array[i];
				if (witResponseClass.HasChild(text))
				{
					goto IL_90;
				}
				string text2;
				int num;
				this.GetArrayNameAndIndex(text, out text2, out num);
				if (string.IsNullOrEmpty(text2) || !witResponseClass.HasChild(text2))
				{
					goto IL_90;
				}
				WitResponseArray asArray = witResponseClass[text2].AsArray;
				if (num >= 0 && num < asArray.Count)
				{
					witResponseClass = asArray[num].AsObject;
				}
				else
				{
					if (asArray.Count <= 0)
					{
						goto IL_90;
					}
					witResponseClass = asArray[0].AsObject;
				}
				IL_9D:
				i++;
				continue;
				IL_90:
				witResponseClass = witResponseClass[text].AsObject;
				goto IL_9D;
			}
			childNodeName = array.Last<string>();
			return witResponseClass;
		}

		private void GetArrayNameAndIndex(string childName, out string arrayName, out int arrayIndex)
		{
			int num = childName.IndexOf('[');
			if (num == -1)
			{
				arrayName = string.Empty;
				arrayIndex = -1;
				return;
			}
			arrayName = childName.Substring(0, num);
			string text = childName.Substring(num + 1);
			int num2 = text.IndexOf(']');
			int num3;
			if (num2 != -1 && int.TryParse(text.Substring(0, num2), out num3))
			{
				arrayIndex = num3;
				return;
			}
			VLog.W(base.GetType().Name, "Could not determine array index for child: " + childName, null);
			arrayIndex = -1;
		}

		public T GetData<T>(string key, T defaultValue = default(T))
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Invalid key");
			}
			string aKey;
			WitResponseClass parentAndNodeName = this.GetParentAndNodeName(key, out aKey);
			if (parentAndNodeName == null)
			{
				return defaultValue;
			}
			return parentAndNodeName.GetChild<T>(aKey, defaultValue);
		}

		public void SetData<T>(string key, T newValue)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Invalid key");
			}
			string aKey;
			WitResponseClass parentAndNodeName = this.GetParentAndNodeName(key, out aKey);
			WitResponseNode witResponseNode = newValue as WitResponseNode;
			if (witResponseNode != null)
			{
				parentAndNodeName[aKey] = witResponseNode;
			}
			else
			{
				parentAndNodeName[aKey] = JsonConvert.SerializeToken<T>(newValue, null, false);
			}
			UnityEvent<string, string, string> onContextMapValueChanged = this.OnContextMapValueChanged;
			if (onContextMapValueChanged != null)
			{
				onContextMapValueChanged.Invoke(key, "", this.Data[key]);
			}
			UnityEvent onContextMapChanged = this.OnContextMapChanged;
			if (onContextMapChanged == null)
			{
				return;
			}
			onContextMapChanged.Invoke();
		}

		public void ClearData(string key, bool notifyContextMapChanged = true)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}
			WitResponseClass data = this.Data;
			if (data != null)
			{
				data.Remove(key);
			}
			UnityEvent<string> onContextMapValueRemoved = this.OnContextMapValueRemoved;
			if (onContextMapValueRemoved != null)
			{
				onContextMapValueRemoved.Invoke(key);
			}
			if (notifyContextMapChanged)
			{
				UnityEvent onContextMapChanged = this.OnContextMapChanged;
				if (onContextMapChanged == null)
				{
					return;
				}
				onContextMapChanged.Invoke();
			}
		}

		public void ClearAllNonReservedData()
		{
			foreach (string text in this.Data.ChildNodeNames)
			{
				if (!ComposerContextMap.ReservedPaths.Contains(text))
				{
					this.ClearData(text, false);
				}
			}
			UnityEvent onContextMapChanged = this.OnContextMapChanged;
			if (onContextMapChanged == null)
			{
				return;
			}
			onContextMapChanged.Invoke();
		}

		public List<string> GetReservedPaths()
		{
			return ComposerContextMap.ReservedPaths.ToList<string>();
		}

		public string GetJson(bool ignoreEmptyFields = false)
		{
			if (this.Data == null)
			{
				return "{}";
			}
			try
			{
				return this.Data.ToString(ignoreEmptyFields);
			}
			catch (Exception arg)
			{
				VLog.E(string.Format("Composer Context Map - Decode Failed\n{0}", arg), null);
			}
			return "{}";
		}

		public override string ToString()
		{
			if (this.Data == null || this.Data.ChildNodeNames.Length == 0)
			{
				return "No Data";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in this.Data.ChildNodeNames)
			{
				stringBuilder.AppendLine("\t" + text + ": " + this.GetData<string>(text, "-"));
			}
			return stringBuilder.ToString();
		}

		public bool UpdateData(WitResponseNode responseNode)
		{
			WitResponseClass witResponseClass;
			if (responseNode == null)
			{
				witResponseClass = null;
			}
			else
			{
				WitResponseNode witResponseNode = responseNode["context_map"];
				witResponseClass = ((witResponseNode != null) ? witResponseNode.AsObject : null);
			}
			WitResponseClass witResponseClass2 = witResponseClass;
			if (witResponseClass2 == null || witResponseClass2.Count == 0)
			{
				return false;
			}
			bool flag = this.UpdateDataObject(this.Data, witResponseClass2);
			if (flag)
			{
				UnityEvent onContextMapChanged = this.OnContextMapChanged;
				if (onContextMapChanged == null)
				{
					return flag;
				}
				onContextMapChanged.Invoke();
			}
			return flag;
		}

		private bool UpdateDataObject(WitResponseClass oldClass, WitResponseClass newClass)
		{
			bool result = false;
			foreach (string text in newClass.ChildNodeNames)
			{
				WitResponseNode witResponseNode = oldClass[text];
				WitResponseNode witResponseNode2 = newClass[text];
				if (!ComposerContextMap.ReservedPaths.Contains(text) && !WitResponseNode.Equals(witResponseNode, witResponseNode2))
				{
					result = true;
					this.Data[text] = witResponseNode2;
					this.Logger.Verbose("Update Context Map Key: '{0}'\nFrom: {1}\nTo: {2}", text, witResponseNode, witResponseNode2, null, "UpdateDataObject", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Features\\Composer\\Composer\\Lib\\Wit.ai\\Features\\composer\\Scripts\\Runtime\\Data\\ComposerContextMap.cs", 310);
					UnityEvent<string, string, string> onContextMapValueChanged = this.OnContextMapValueChanged;
					if (onContextMapValueChanged != null)
					{
						onContextMapValueChanged.Invoke(text, witResponseNode, witResponseNode2);
					}
				}
			}
			return result;
		}

		public void CopyPersistentData(ComposerContextMap otherMap, ComposerService composerTarget)
		{
			if (otherMap.LoadedPlugins == null)
			{
				return;
			}
			this.LoadedPlugins = otherMap.LoadedPlugins;
			foreach (IContextMapReservedPathExtension contextMapReservedPathExtension in otherMap.LoadedPlugins)
			{
				BaseReservedContextPath baseReservedContextPath = (BaseReservedContextPath)contextMapReservedPathExtension;
				baseReservedContextPath.AssignTo(composerTarget);
				baseReservedContextPath.UpdateContextMap();
			}
		}

		internal static HashSet<string> ReservedPaths = new HashSet<string>();
	}
}
