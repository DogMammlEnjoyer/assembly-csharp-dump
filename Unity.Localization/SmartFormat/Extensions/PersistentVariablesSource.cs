using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class PersistentVariablesSource : ISource, IDictionary<string, VariablesGroupAsset>, ICollection<KeyValuePair<string, VariablesGroupAsset>>, IEnumerable<KeyValuePair<string, VariablesGroupAsset>>, IEnumerable, ISerializationCallbackReceiver
	{
		public static bool IsUpdating
		{
			get
			{
				return PersistentVariablesSource.s_IsUpdating != 0;
			}
		}

		public int Count
		{
			get
			{
				return this.m_Groups.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return this.m_GroupLookup.Keys;
			}
		}

		public ICollection<VariablesGroupAsset> Values
		{
			get
			{
				return (from k in this.m_GroupLookup.Values
				select k.@group).ToList<VariablesGroupAsset>();
			}
		}

		public VariablesGroupAsset this[string name]
		{
			get
			{
				return this.m_GroupLookup[name].group;
			}
			set
			{
				this.Add(name, value);
			}
		}

		public static event Action EndUpdate;

		public PersistentVariablesSource(SmartFormatter formatter)
		{
			formatter.Parser.AddOperators(".");
		}

		public static void BeginUpdating()
		{
			PersistentVariablesSource.s_IsUpdating++;
		}

		public static void EndUpdating()
		{
			PersistentVariablesSource.s_IsUpdating--;
			if (PersistentVariablesSource.s_IsUpdating != 0)
			{
				if (PersistentVariablesSource.s_IsUpdating < 0)
				{
					Debug.LogWarning("Incorrect number of Begin and End calls to PersistentVariablesSource. BeginUpdating must be called before EndUpdating.");
					PersistentVariablesSource.s_IsUpdating = 0;
				}
				return;
			}
			Action endUpdate = PersistentVariablesSource.EndUpdate;
			if (endUpdate == null)
			{
				return;
			}
			endUpdate();
		}

		public static IDisposable UpdateScope()
		{
			PersistentVariablesSource.BeginUpdating();
			return default(PersistentVariablesSource.ScopedUpdate);
		}

		public bool TryGetValue(string name, out VariablesGroupAsset value)
		{
			PersistentVariablesSource.NameValuePair nameValuePair;
			if (this.m_GroupLookup.TryGetValue(name, out nameValuePair))
			{
				value = nameValuePair.group;
				return true;
			}
			value = null;
			return false;
		}

		public void Add(string name, VariablesGroupAsset group)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name", "Name must not be null or empty.");
			}
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			PersistentVariablesSource.NameValuePair nameValuePair = new PersistentVariablesSource.NameValuePair
			{
				name = name,
				group = group
			};
			name = name.ReplaceWhiteSpaces("-");
			this.m_GroupLookup[name] = nameValuePair;
			this.m_Groups.Add(nameValuePair);
		}

		public void Add(KeyValuePair<string, VariablesGroupAsset> item)
		{
			this.Add(item.Key, item.Value);
		}

		public bool Remove(string name)
		{
			PersistentVariablesSource.NameValuePair item;
			if (this.m_GroupLookup.TryGetValue(name, out item))
			{
				this.m_Groups.Remove(item);
				this.m_GroupLookup.Remove(name);
				return true;
			}
			return false;
		}

		public bool Remove(KeyValuePair<string, VariablesGroupAsset> item)
		{
			return this.Remove(item.Key);
		}

		public void Clear()
		{
			this.m_GroupLookup.Clear();
			this.m_Groups.Clear();
		}

		public bool ContainsKey(string name)
		{
			return this.m_GroupLookup.ContainsKey(name);
		}

		public bool Contains(KeyValuePair<string, VariablesGroupAsset> item)
		{
			VariablesGroupAsset x;
			return this.TryGetValue(item.Key, out x) && x == item.Value;
		}

		public void CopyTo(KeyValuePair<string, VariablesGroupAsset>[] array, int arrayIndex)
		{
			foreach (KeyValuePair<string, PersistentVariablesSource.NameValuePair> keyValuePair in this.m_GroupLookup)
			{
				array[arrayIndex++] = new KeyValuePair<string, VariablesGroupAsset>(keyValuePair.Key, keyValuePair.Value.group);
			}
		}

		IEnumerator<KeyValuePair<string, VariablesGroupAsset>> IEnumerable<KeyValuePair<string, VariablesGroupAsset>>.GetEnumerator()
		{
			foreach (KeyValuePair<string, PersistentVariablesSource.NameValuePair> keyValuePair in this.m_GroupLookup)
			{
				yield return new KeyValuePair<string, VariablesGroupAsset>(keyValuePair.Key, keyValuePair.Value.group);
			}
			Dictionary<string, PersistentVariablesSource.NameValuePair>.Enumerator enumerator = default(Dictionary<string, PersistentVariablesSource.NameValuePair>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<string, PersistentVariablesSource.NameValuePair> keyValuePair in this.m_GroupLookup)
			{
				yield return new KeyValuePair<string, VariablesGroupAsset>(keyValuePair.Key, keyValuePair.Value.group);
			}
			Dictionary<string, PersistentVariablesSource.NameValuePair>.Enumerator enumerator = default(Dictionary<string, PersistentVariablesSource.NameValuePair>.Enumerator);
			yield break;
			yield break;
		}

		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
		{
			string selectorText = selectorInfo.SelectorText;
			IVariableGroup variableGroup = selectorInfo.CurrentValue as IVariableGroup;
			if (variableGroup != null && PersistentVariablesSource.EvaluateLocalGroup(selectorInfo, variableGroup))
			{
				return true;
			}
			if (selectorInfo.SelectorOperator == "")
			{
				FormatCache formatCache = selectorInfo.FormatDetails.FormatCache;
				if (PersistentVariablesSource.EvaluateLocalGroup(selectorInfo, (formatCache != null) ? formatCache.LocalVariables : null))
				{
					return true;
				}
			}
			VariablesGroupAsset result;
			if (this.TryGetValue(selectorText, out result))
			{
				selectorInfo.Result = result;
				return true;
			}
			return false;
		}

		private static bool EvaluateLocalGroup(ISelectorInfo selectorInfo, IVariableGroup variablleGroup)
		{
			if (variablleGroup == null)
			{
				return false;
			}
			IVariable variable;
			if (variablleGroup != null && variablleGroup.TryGetValue(selectorInfo.SelectorText, out variable))
			{
				FormatCache formatCache = selectorInfo.FormatDetails.FormatCache;
				if (formatCache != null)
				{
					IVariableValueChanged variableValueChanged = variable as IVariableValueChanged;
					if (variableValueChanged != null && !formatCache.VariableTriggers.Contains(variableValueChanged))
					{
						formatCache.VariableTriggers.Add(variableValueChanged);
					}
				}
				selectorInfo.Result = variable.GetSourceValue(selectorInfo);
				return true;
			}
			return false;
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.m_GroupLookup == null)
			{
				this.m_GroupLookup = new Dictionary<string, PersistentVariablesSource.NameValuePair>();
			}
			this.m_GroupLookup.Clear();
			foreach (PersistentVariablesSource.NameValuePair nameValuePair in this.m_Groups)
			{
				if (!string.IsNullOrEmpty(nameValuePair.name))
				{
					this.m_GroupLookup[nameValuePair.name] = nameValuePair;
				}
			}
		}

		[SerializeField]
		private List<PersistentVariablesSource.NameValuePair> m_Groups = new List<PersistentVariablesSource.NameValuePair>();

		private Dictionary<string, PersistentVariablesSource.NameValuePair> m_GroupLookup = new Dictionary<string, PersistentVariablesSource.NameValuePair>();

		internal static int s_IsUpdating;

		[Serializable]
		private class NameValuePair
		{
			public string name;

			[SerializeReference]
			public VariablesGroupAsset group;
		}

		public struct ScopedUpdate : IDisposable
		{
			public void Dispose()
			{
				PersistentVariablesSource.EndUpdating();
			}
		}
	}
}
