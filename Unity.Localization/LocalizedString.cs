using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Tables;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace UnityEngine.Localization
{
	[UxmlObject]
	[Serializable]
	public class LocalizedString : LocalizedReference, IVariableGroup, IDictionary<string, IVariable>, ICollection<KeyValuePair<string, IVariable>>, IEnumerable<KeyValuePair<string, IVariable>>, IEnumerable, IVariableValueChanged, IVariable, IDisposable
	{
		public event Action<IVariable> ValueChanged;

		internal override bool ForceSynchronous
		{
			get
			{
				return this.WaitForCompletion || LocalizationSettings.StringDatabase.AsynchronousBehaviour == AsynchronousBehaviour.ForceSynchronous;
			}
		}

		public IList<object> Arguments { get; set; }

		public AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult> CurrentLoadingOperationHandle { get; internal set; }

		public event LocalizedString.ChangeHandler StringChanged
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.m_ChangeHandler.Add(value, 5);
				if (this.m_ChangeHandler.Length == 1)
				{
					LocalizationSettings.ValidateSettingsExist("");
					this.ForceUpdate();
					LocalizationSettings.SelectedLocaleChanged += this.m_SelectedLocaleChanged;
					return;
				}
				if (this.CurrentLoadingOperationHandle.IsValid() && this.CurrentLoadingOperationHandle.IsDone)
				{
					value(this.m_CurrentStringChangedValue);
				}
			}
			remove
			{
				this.m_ChangeHandler.RemoveByMovingTail(value);
				if (this.m_ChangeHandler.Length == 0)
				{
					LocalizationSettings.SelectedLocaleChanged -= this.m_SelectedLocaleChanged;
					this.ClearLoadingOperation();
					this.ClearVariableListeners();
				}
			}
		}

		public bool HasChangeHandler
		{
			get
			{
				return this.m_ChangeHandler.Length != 0;
			}
		}

		public LocalizedString()
		{
			this.m_SelectedLocaleChanged = new Action<Locale>(this.HandleLocaleChange);
			this.m_OnVariableChanged = new Action<IVariable>(this.OnVariableChanged);
			this.m_AutomaticLoadingCompleted = new Action<AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult>>(this.AutomaticLoadingCompleted);
			this.m_CompletedSourceValue = new Action<AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult>>(this.CompletedSourceValue);
		}

		public LocalizedString(TableReference tableReference, TableEntryReference entryReference) : this()
		{
			base.TableReference = tableReference;
			base.TableEntryReference = entryReference;
		}

		public bool RefreshString()
		{
			if (this.m_ChangeHandler.Length == 0 || !this.CurrentLoadingOperationHandle.IsValid())
			{
				return false;
			}
			if (this.CurrentLoadingOperationHandle.IsDone)
			{
				StringTableEntry entry = this.CurrentLoadingOperationHandle.Result.Entry;
				FormatCache formatCache = (entry != null) ? entry.GetOrCreateFormatCache() : null;
				if (formatCache != null)
				{
					formatCache.LocalVariables = this;
					formatCache.VariableTriggers.Clear();
				}
				string currentStringChangedValue = LocalizationSettings.StringDatabase.GenerateLocalizedString(this.CurrentLoadingOperationHandle.Result.Table, entry, base.TableReference, base.TableEntryReference, LocalizationSettings.SelectedLocale, this.Arguments);
				if (formatCache != null)
				{
					formatCache.LocalVariables = null;
					List<IVariableValueChanged> variables;
					if (entry == null)
					{
						variables = null;
					}
					else
					{
						FormatCache formatCache2 = entry.FormatCache;
						variables = ((formatCache2 != null) ? formatCache2.VariableTriggers : null);
					}
					this.UpdateVariableListeners(variables);
				}
				this.m_CurrentStringChangedValue = currentStringChangedValue;
				this.InvokeChangeHandler(this.m_CurrentStringChangedValue);
				return true;
			}
			if (this.ForceSynchronous)
			{
				this.CurrentLoadingOperationHandle.WaitForCompletion();
				return true;
			}
			return false;
		}

		public AsyncOperationHandle<string> GetLocalizedStringAsync()
		{
			return this.GetLocalizedStringAsync(this.Arguments);
		}

		public string GetLocalizedString()
		{
			return this.GetLocalizedStringAsync().WaitForCompletion();
		}

		public AsyncOperationHandle<string> GetLocalizedStringAsync(params object[] arguments)
		{
			return this.GetLocalizedStringAsync(arguments);
		}

		public string GetLocalizedString(params object[] arguments)
		{
			return this.GetLocalizedStringAsync(arguments).WaitForCompletion();
		}

		public string GetLocalizedString(IList<object> arguments)
		{
			return this.GetLocalizedStringAsync(arguments).WaitForCompletion();
		}

		public AsyncOperationHandle<string> GetLocalizedStringAsync(IList<object> arguments)
		{
			LocalizationSettings.ValidateSettingsExist("");
			return LocalizationSettings.StringDatabase.GetLocalizedStringAsync(base.TableReference, base.TableEntryReference, arguments, base.LocaleOverride, base.FallbackState, (this.m_LocalVariables.Count > 0) ? this : null);
		}

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
			VariableNameValuePair variableNameValuePair;
			if (this.m_VariableLookup.TryGetValue(name, out variableNameValuePair))
			{
				if (variableNameValuePair.variable == variable)
				{
					return;
				}
				this.m_LocalVariables.Remove(variableNameValuePair);
			}
			VariableNameValuePair variableNameValuePair2 = new VariableNameValuePair
			{
				name = name,
				variable = variable
			};
			this.m_VariableLookup[name] = variableNameValuePair2;
			this.m_LocalVariables.Add(variableNameValuePair2);
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
				this.m_LocalVariables.Remove(item);
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

		public void Clear()
		{
			this.m_VariableLookup.Clear();
			this.m_LocalVariables.Clear();
		}

		public object GetSourceValue(ISelectorInfo selector)
		{
			if (base.IsEmpty)
			{
				throw new DataNotReadyException("{Empty}");
			}
			Locale locale = base.LocaleOverride;
			if (locale == null && selector.FormatDetails.FormatCache != null)
			{
				locale = LocalizationSettings.AvailableLocales.GetLocale(selector.FormatDetails.FormatCache.Table.LocaleIdentifier);
			}
			if (locale == null && LocalizationSettings.SelectedLocaleAsync.IsDone)
			{
				locale = LocalizationSettings.SelectedLocaleAsync.Result;
			}
			if (locale == null)
			{
				throw new DataNotReadyException("{No Available Locale}");
			}
			AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult> tableEntryAsync = LocalizationSettings.StringDatabase.GetTableEntryAsync(base.TableReference, base.TableEntryReference, locale, base.FallbackState);
			if (!tableEntryAsync.IsDone)
			{
				tableEntryAsync.Completed += this.m_CompletedSourceValue;
				throw new DataNotReadyException();
			}
			StringTableEntry entry = tableEntryAsync.Result.Entry;
			if (entry == null)
			{
				throw new DataNotReadyException("{Missing Entry}");
			}
			if (!entry.IsSmart)
			{
				return new LocalizedString.StringTableEntryVariable(LocalizationSettings.StringDatabase.GenerateLocalizedString(tableEntryAsync.Result.Table, entry, base.TableReference, base.TableEntryReference, locale, this.Arguments), entry);
			}
			FormatCache formatCache = (entry != null) ? entry.GetOrCreateFormatCache() : null;
			if (formatCache != null)
			{
				formatCache.VariableTriggers.Clear();
				if (this.m_VariableLookup.Count > 0)
				{
					formatCache.LocalVariables = new LocalizedString.ChainedLocalVariablesGroup(this, selector.FormatDetails.FormatCache.LocalVariables);
				}
				else
				{
					formatCache.LocalVariables = selector.FormatDetails.FormatCache.LocalVariables;
				}
			}
			List<object> list;
			object result;
			using (CollectionPool<List<object>, object>.Get(out list))
			{
				if (selector.CurrentValue != null)
				{
					list.Add(selector.CurrentValue);
				}
				if (this.Arguments != null)
				{
					list.AddRange(this.Arguments);
				}
				string localized = LocalizationSettings.StringDatabase.GenerateLocalizedString(tableEntryAsync.Result.Table, entry, base.TableReference, base.TableEntryReference, locale, list);
				if (formatCache != null)
				{
					formatCache.LocalVariables = null;
					this.UpdateVariableListeners(formatCache.VariableTriggers);
				}
				result = new LocalizedString.StringTableEntryVariable(localized, entry);
			}
			return result;
		}

		private void CompletedSourceValue(AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult> _)
		{
			Action<IVariable> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(this);
		}

		protected internal override void ForceUpdate()
		{
			if (this.m_ChangeHandler.Length != 0)
			{
				this.HandleLocaleChange(null);
			}
			Action<IVariable> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(this);
		}

		private void ClearVariableListeners()
		{
			foreach (IVariableValueChanged variableValueChanged in this.m_UsedVariables)
			{
				variableValueChanged.ValueChanged -= this.m_OnVariableChanged;
			}
			this.m_UsedVariables.Clear();
		}

		private void UpdateVariableListeners(List<IVariableValueChanged> variables)
		{
			this.ClearVariableListeners();
			if (variables == null)
			{
				return;
			}
			foreach (IVariableValueChanged variableValueChanged in variables)
			{
				this.m_UsedVariables.Add(variableValueChanged);
				variableValueChanged.ValueChanged += this.m_OnVariableChanged;
			}
		}

		private void OnVariableChanged(IVariable globalVariable)
		{
			if (this.m_WaitingForVariablesEndUpdate)
			{
				return;
			}
			if (PersistentVariablesSource.IsUpdating)
			{
				this.m_WaitingForVariablesEndUpdate = true;
				PersistentVariablesSource.EndUpdate += this.OnVariablesSourceUpdateCompleted;
				return;
			}
			this.RefreshString();
			Action<IVariable> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(this);
		}

		private void OnVariablesSourceUpdateCompleted()
		{
			PersistentVariablesSource.EndUpdate -= this.OnVariablesSourceUpdateCompleted;
			this.m_WaitingForVariablesEndUpdate = false;
			this.RefreshString();
			Action<IVariable> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(this);
		}

		private void InvokeChangeHandler(string value)
		{
			try
			{
				this.m_ChangeHandler.LockForChanges();
				int length = this.m_ChangeHandler.Length;
				if (length == 1)
				{
					this.m_ChangeHandler.SingleDelegate(value);
				}
				else if (length > 1)
				{
					LocalizedString.ChangeHandler[] multiDelegates = this.m_ChangeHandler.MultiDelegates;
					for (int i = 0; i < length; i++)
					{
						multiDelegates[i](value);
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			this.m_ChangeHandler.UnlockForChanges();
		}

		private void HandleLocaleChange(Locale locale)
		{
			this.ClearLoadingOperation();
			this.m_CurrentStringChangedValue = null;
			if (base.IsEmpty)
			{
				return;
			}
			this.CurrentLoadingOperationHandle = LocalizationSettings.StringDatabase.GetTableEntryAsync(base.TableReference, base.TableEntryReference, base.LocaleOverride, base.FallbackState);
			AddressablesInterface.Acquire(this.CurrentLoadingOperationHandle);
			if (!this.CurrentLoadingOperationHandle.IsDone)
			{
				if (!this.ForceSynchronous)
				{
					this.CurrentLoadingOperationHandle.Completed += this.m_AutomaticLoadingCompleted;
					return;
				}
				this.CurrentLoadingOperationHandle.WaitForCompletion();
			}
			this.AutomaticLoadingCompleted(this.CurrentLoadingOperationHandle);
		}

		private void AutomaticLoadingCompleted(AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult> loadOperation)
		{
			if (loadOperation.Status != AsyncOperationStatus.Succeeded)
			{
				this.CurrentLoadingOperationHandle = default(AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult>);
				return;
			}
			this.RefreshString();
		}

		private void ClearLoadingOperation()
		{
			if (this.CurrentLoadingOperationHandle.IsValid())
			{
				if (!this.CurrentLoadingOperationHandle.IsDone)
				{
					this.CurrentLoadingOperationHandle.Completed -= this.m_AutomaticLoadingCompleted;
				}
				AddressablesInterface.Release(this.CurrentLoadingOperationHandle);
				this.CurrentLoadingOperationHandle = default(AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult>);
			}
		}

		protected override void Reset()
		{
			this.ClearLoadingOperation();
		}

		public override void OnAfterDeserialize()
		{
			this.m_VariableLookup.Clear();
			foreach (VariableNameValuePair variableNameValuePair in this.m_LocalVariables)
			{
				if (!string.IsNullOrEmpty(variableNameValuePair.name))
				{
					this.m_VariableLookup[variableNameValuePair.name] = variableNameValuePair;
				}
			}
		}

		~LocalizedString()
		{
			this.ClearLoadingOperation();
		}

		void IDisposable.Dispose()
		{
			this.m_ChangeHandler.Clear();
			this.ClearLoadingOperation();
			this.ClearVariableListeners();
			LocalizationSettings.SelectedLocaleChanged -= this.m_SelectedLocaleChanged;
			GC.SuppressFinalize(this);
		}

		[UxmlObjectReference("variables")]
		internal List<LocalVariable> LocalVariablesUXML
		{
			get
			{
				return this.m_UxmlLocalVariables;
			}
			set
			{
				this.m_LocalVariables.Clear();
				this.m_UxmlLocalVariables = value;
				if (this.m_UxmlLocalVariables != null)
				{
					foreach (LocalVariable localVariable in this.m_UxmlLocalVariables)
					{
						if (localVariable != null && !string.IsNullOrEmpty(localVariable.Name) && localVariable.Variable != null)
						{
							this.Add(localVariable.Name, localVariable.Variable);
						}
					}
				}
			}
		}

		protected override void Initialize()
		{
			this.StringChanged += this.UpdateBindingValue;
		}

		protected override void Cleanup()
		{
			this.StringChanged -= this.UpdateBindingValue;
		}

		protected override BindingResult Update(in BindingContext context)
		{
			if (base.IsEmpty)
			{
				return new BindingResult(BindingStatus.Success, null);
			}
			if (!this.CurrentLoadingOperationHandle.IsDone)
			{
				return new BindingResult(BindingStatus.Pending, null);
			}
			VisualElement targetElement = context.targetElement;
			BindingId bindingId = context.bindingId;
			PropertyPath propertyPath = bindingId;
			VisitReturnCode errorCode;
			if (ConverterGroups.TrySetValueGlobal<VisualElement, string>(ref targetElement, propertyPath, this.m_CurrentStringChangedValue, out errorCode))
			{
				return new BindingResult(BindingStatus.Success, null);
			}
			return base.CreateErrorResult(context, errorCode, typeof(string));
		}

		private void UpdateBindingValue(string _)
		{
			base.MarkDirty();
		}

		[SerializeField]
		private List<VariableNameValuePair> m_LocalVariables = new List<VariableNameValuePair>();

		private CallbackArray<LocalizedString.ChangeHandler> m_ChangeHandler;

		private string m_CurrentStringChangedValue;

		private readonly Dictionary<string, VariableNameValuePair> m_VariableLookup = new Dictionary<string, VariableNameValuePair>();

		private readonly List<IVariableValueChanged> m_UsedVariables = new List<IVariableValueChanged>();

		private readonly Action<IVariable> m_OnVariableChanged;

		private readonly Action<Locale> m_SelectedLocaleChanged;

		private readonly Action<AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult>> m_AutomaticLoadingCompleted;

		private readonly Action<AsyncOperationHandle<LocalizedDatabase<StringTable, StringTableEntry>.TableEntryResult>> m_CompletedSourceValue;

		private bool m_WaitingForVariablesEndUpdate;

		private List<LocalVariable> m_UxmlLocalVariables;

		public delegate void ChangeHandler(string value);

		private struct StringTableEntryVariable : IVariableGroup
		{
			public StringTableEntryVariable(string localized, StringTableEntry entry)
			{
				this.m_Localized = localized;
				this.m_StringTableEntry = entry;
			}

			public bool TryGetValue(string key, out IVariable value)
			{
				foreach (IMetadata metadata in this.m_StringTableEntry.MetadataEntries)
				{
					IMetadataVariable metadataVariable = metadata as IMetadataVariable;
					if (metadataVariable != null && metadataVariable.VariableName == key)
					{
						value = metadataVariable;
						return true;
					}
				}
				value = null;
				return false;
			}

			public override string ToString()
			{
				return this.m_Localized;
			}

			private readonly string m_Localized;

			private readonly StringTableEntry m_StringTableEntry;
		}

		private struct ChainedLocalVariablesGroup : IVariableGroup
		{
			private IVariableGroup ParentGroup { readonly get; set; }

			private IVariableGroup Group { readonly get; set; }

			public ChainedLocalVariablesGroup(IVariableGroup group, IVariableGroup parent)
			{
				this.Group = group;
				this.ParentGroup = parent;
			}

			public bool TryGetValue(string key, out IVariable value)
			{
				if (this.Group.TryGetValue(key, out value))
				{
					return true;
				}
				if (this.ParentGroup.TryGetValue(key, out value))
				{
					return true;
				}
				value = null;
				return false;
			}
		}

		[CompilerGenerated]
		[Serializable]
		public new class UxmlSerializedData : LocalizedReference.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(LocalizedString.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("LocalVariablesUXML", "variables", null, Array.Empty<string>())
				});
			}

			public override object CreateInstance()
			{
				return new LocalizedString();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				LocalizedString localizedString = (LocalizedString)obj;
				if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.LocalVariablesUXML_UxmlAttributeFlags))
				{
					List<LocalVariable> list = new List<LocalVariable>();
					if (this.LocalVariablesUXML != null)
					{
						for (int i = 0; i < this.LocalVariablesUXML.Count; i++)
						{
							if (this.LocalVariablesUXML[i] != null)
							{
								LocalVariable localVariable = (LocalVariable)this.LocalVariablesUXML[i].CreateInstance();
								this.LocalVariablesUXML[i].Deserialize(localVariable);
								list.Add(localVariable);
							}
							else
							{
								list.Add(null);
							}
						}
					}
					localizedString.LocalVariablesUXML = list;
				}
			}

			[UxmlObjectReference("variables")]
			[SerializeReference]
			private List<LocalVariable.UxmlSerializedData> LocalVariablesUXML;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags LocalVariablesUXML_UxmlAttributeFlags;
		}
	}
}
