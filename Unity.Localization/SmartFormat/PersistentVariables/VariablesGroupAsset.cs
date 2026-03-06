using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[CreateAssetMenu(menuName = "Localization/Variables Group")]
	public class VariablesGroupAsset : ScriptableObject, IVariableGroup, IVariable, IDictionary<string, IVariable>, ICollection<KeyValuePair<string, IVariable>>, IEnumerable<KeyValuePair<string, IVariable>>, IEnumerable, ISerializationCallbackReceiver
	{
		public int Count
		{
			get
			{
				return this.m_VariableLookup.Count;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return this.m_VariableLookup.Keys;
			}
		}

		public ICollection<IVariable> Values
		{
			get
			{
				return (from s in this.m_VariableLookup.Values
				select s.variable).ToList<IVariable>();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IVariable this[string name]
		{
			get
			{
				return this.m_VariableLookup[name].variable;
			}
			set
			{
				this.Add(name, value);
			}
		}

		public object GetSourceValue(ISelectorInfo _)
		{
			return this;
		}

		public bool TryGetValue(string name, out IVariable value)
		{
			VariableNameValuePair variableNameValuePair;
			if (this.m_VariableLookup.TryGetValue(name, out variableNameValuePair))
			{
				value = variableNameValuePair.variable;
				return true;
			}
			value = null;
			return false;
		}

		public void Add(string name, IVariable variable)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name", "Name must not be null or empty.");
			}
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			name = name.ReplaceWhiteSpaces("-");
			VariableNameValuePair variableNameValuePair = new VariableNameValuePair
			{
				name = name,
				variable = variable
			};
			this.m_VariableLookup.Add(name, variableNameValuePair);
			this.m_Variables.Add(variableNameValuePair);
		}

		public void Add(KeyValuePair<string, IVariable> item)
		{
			this.Add(item.Key, item.Value);
		}

		public bool Remove(string name)
		{
			VariableNameValuePair item;
			if (this.m_VariableLookup.TryGetValue(name, out item))
			{
				this.m_Variables.Remove(item);
				this.m_VariableLookup.Remove(name);
				return true;
			}
			return false;
		}

		public bool Remove(KeyValuePair<string, IVariable> item)
		{
			return this.Remove(item.Key);
		}

		public bool ContainsKey(string name)
		{
			return this.m_VariableLookup.ContainsKey(name);
		}

		public bool Contains(KeyValuePair<string, IVariable> item)
		{
			IVariable variable;
			return this.TryGetValue(item.Key, out variable) && variable == item.Value;
		}

		public void CopyTo(KeyValuePair<string, IVariable>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			foreach (KeyValuePair<string, VariableNameValuePair> keyValuePair in this.m_VariableLookup)
			{
				array[arrayIndex++] = new KeyValuePair<string, IVariable>(keyValuePair.Key, keyValuePair.Value.variable);
			}
		}

		IEnumerator<KeyValuePair<string, IVariable>> IEnumerable<KeyValuePair<string, IVariable>>.GetEnumerator()
		{
			foreach (KeyValuePair<string, VariableNameValuePair> keyValuePair in this.m_VariableLookup)
			{
				yield return new KeyValuePair<string, IVariable>(keyValuePair.Key, keyValuePair.Value.variable);
			}
			Dictionary<string, VariableNameValuePair>.Enumerator enumerator = default(Dictionary<string, VariableNameValuePair>.Enumerator);
			yield break;
			yield break;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<string, VariableNameValuePair> keyValuePair in this.m_VariableLookup)
			{
				yield return new KeyValuePair<string, IVariable>(keyValuePair.Key, keyValuePair.Value.variable);
			}
			Dictionary<string, VariableNameValuePair>.Enumerator enumerator = default(Dictionary<string, VariableNameValuePair>.Enumerator);
			yield break;
			yield break;
		}

		[Obsolete("Please use ContainsKey instead.", false)]
		public bool ContainsName(string name)
		{
			return this.ContainsKey(name);
		}

		public void Clear()
		{
			this.m_VariableLookup.Clear();
			this.m_Variables.Clear();
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.m_VariableLookup == null)
			{
				this.m_VariableLookup = new Dictionary<string, VariableNameValuePair>();
			}
			this.m_VariableLookup.Clear();
			foreach (VariableNameValuePair variableNameValuePair in this.m_Variables)
			{
				if (!string.IsNullOrEmpty(variableNameValuePair.name))
				{
					this.m_VariableLookup[variableNameValuePair.name] = variableNameValuePair;
				}
			}
		}

		[SerializeField]
		internal List<VariableNameValuePair> m_Variables = new List<VariableNameValuePair>();

		private Dictionary<string, VariableNameValuePair> m_VariableLookup = new Dictionary<string, VariableNameValuePair>();
	}
}
