using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	[HelpURL("UIE-VisualTree-landing")]
	[Serializable]
	public class VisualTreeAsset : ScriptableObject
	{
		public bool importedWithErrors
		{
			get
			{
				return this.m_ImportedWithErrors;
			}
			internal set
			{
				this.m_ImportedWithErrors = value;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool importerWithUpdatedUrls
		{
			get
			{
				return this.m_HasUpdatedUrls;
			}
			set
			{
				this.m_HasUpdatedUrls = value;
			}
		}

		public bool importedWithWarnings
		{
			get
			{
				return this.m_ImportedWithWarnings;
			}
			internal set
			{
				this.m_ImportedWithWarnings = value;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int GetNextChildSerialNumber()
		{
			List<VisualElementAsset> visualElementAssets = this.m_VisualElementAssets;
			int num = (visualElementAssets != null) ? visualElementAssets.Count : 0;
			int num2 = num;
			List<TemplateAsset> templateAssets = this.m_TemplateAssets;
			num = num2 + ((templateAssets != null) ? templateAssets.Count : 0);
			bool flag = this.m_UxmlObjectEntries != null;
			if (flag)
			{
				num += this.m_UxmlObjectEntries.Count;
				foreach (VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry in this.m_UxmlObjectEntries)
				{
					bool flag2 = uxmlObjectEntry.uxmlObjectAssets != null;
					if (flag2)
					{
						num += uxmlObjectEntry.uxmlObjectAssets.Count;
					}
				}
			}
			return num;
		}

		internal List<VisualTreeAsset.UsingEntry> usings
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				return this.m_Usings;
			}
		}

		public IEnumerable<VisualTreeAsset> templateDependencies
		{
			get
			{
				bool flag = this.m_Usings.Count == 0;
				if (flag)
				{
					yield break;
				}
				HashSet<VisualTreeAsset> sent = new HashSet<VisualTreeAsset>();
				foreach (VisualTreeAsset.UsingEntry entry in this.m_Usings)
				{
					bool flag2 = entry.asset != null && !sent.Contains(entry.asset);
					if (flag2)
					{
						sent.Add(entry.asset);
						yield return entry.asset;
					}
					else
					{
						bool flag3 = !string.IsNullOrEmpty(entry.path);
						if (flag3)
						{
							VisualTreeAsset vta = Panel.LoadResource(entry.path, typeof(VisualTreeAsset), 1f) as VisualTreeAsset;
							bool flag4 = vta != null && !sent.Contains(entry.asset);
							if (flag4)
							{
								sent.Add(entry.asset);
								yield return vta;
							}
							vta = null;
						}
					}
					entry = default(VisualTreeAsset.UsingEntry);
				}
				List<VisualTreeAsset.UsingEntry>.Enumerator enumerator = default(List<VisualTreeAsset.UsingEntry>.Enumerator);
				yield break;
				yield break;
			}
		}

		public IEnumerable<StyleSheet> stylesheets
		{
			get
			{
				HashSet<StyleSheet> sent = new HashSet<StyleSheet>();
				foreach (VisualElementAsset vea in this.m_VisualElementAssets)
				{
					bool hasStylesheets = vea.hasStylesheets;
					if (hasStylesheets)
					{
						foreach (StyleSheet stylesheet in vea.stylesheets)
						{
							bool flag = !sent.Contains(stylesheet);
							if (flag)
							{
								sent.Add(stylesheet);
								yield return stylesheet;
							}
							stylesheet = null;
						}
						List<StyleSheet>.Enumerator enumerator2 = default(List<StyleSheet>.Enumerator);
					}
					bool hasStylesheetPaths = vea.hasStylesheetPaths;
					if (hasStylesheetPaths)
					{
						foreach (string stylesheetPath in vea.stylesheetPaths)
						{
							StyleSheet stylesheet2 = Panel.LoadResource(stylesheetPath, typeof(StyleSheet), 1f) as StyleSheet;
							bool flag2 = stylesheet2 != null && !sent.Contains(stylesheet2);
							if (flag2)
							{
								sent.Add(stylesheet2);
								yield return stylesheet2;
							}
							stylesheet2 = null;
							stylesheetPath = null;
						}
						List<string>.Enumerator enumerator3 = default(List<string>.Enumerator);
					}
					vea = null;
				}
				List<VisualElementAsset>.Enumerator enumerator = default(List<VisualElementAsset>.Enumerator);
				yield break;
				yield break;
			}
		}

		internal List<VisualElementAsset> visualElementAssets
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				return this.m_VisualElementAssets;
			}
		}

		internal List<TemplateAsset> templateAssets
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				return this.m_TemplateAssets;
			}
		}

		internal List<VisualTreeAsset.UxmlObjectEntry> uxmlObjectEntries
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				return this.m_UxmlObjectEntries;
			}
		}

		internal List<int> uxmlObjectIds
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				return this.m_UxmlObjectIds;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void RemoveElementAndDependencies(VisualElementAsset asset)
		{
			bool flag = asset == null;
			if (!flag)
			{
				this.m_VisualElementAssets.Remove(asset);
				this.RemoveUxmlObjectEntryDependencies(asset.id);
			}
		}

		internal void RegisterUxmlObject(UxmlObjectAsset uxmlObjectAsset)
		{
			VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry = this.GetUxmlObjectEntry(uxmlObjectAsset.parentId);
			bool flag = uxmlObjectEntry.uxmlObjectAssets != null;
			if (flag)
			{
				uxmlObjectEntry.uxmlObjectAssets.Add(uxmlObjectAsset);
			}
			else
			{
				this.m_UxmlObjectEntries.Add(new VisualTreeAsset.UxmlObjectEntry(uxmlObjectAsset.parentId, new List<UxmlObjectAsset>
				{
					uxmlObjectAsset
				}));
				this.m_UxmlObjectIds.Add(uxmlObjectAsset.id);
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal UxmlObjectAsset AddUxmlObject(UxmlAsset parent, string fieldUxmlName, string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
		{
			VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry = this.GetUxmlObjectEntry(parent.id);
			bool flag = uxmlObjectEntry.uxmlObjectAssets == null;
			if (flag)
			{
				uxmlObjectEntry = new VisualTreeAsset.UxmlObjectEntry(parent.id, new List<UxmlObjectAsset>());
				this.m_UxmlObjectEntries.Add(uxmlObjectEntry);
			}
			bool flag2 = string.IsNullOrEmpty(fieldUxmlName);
			UxmlObjectAsset result;
			if (flag2)
			{
				UxmlObjectAsset uxmlObjectAsset = new UxmlObjectAsset(fullTypeName, false, xmlNamespace);
				uxmlObjectAsset.parentId = parent.id;
				uxmlObjectAsset.id = this.GetNextUxmlAssetId(parent.id);
				this.m_UxmlObjectIds.Add(uxmlObjectAsset.id);
				uxmlObjectEntry.uxmlObjectAssets.Add(uxmlObjectAsset);
				result = uxmlObjectAsset;
			}
			else
			{
				UxmlObjectAsset uxmlObjectAsset2 = uxmlObjectEntry.GetField(fieldUxmlName);
				bool flag3 = uxmlObjectAsset2 == null;
				if (flag3)
				{
					uxmlObjectAsset2 = new UxmlObjectAsset(fieldUxmlName, true, xmlNamespace);
					uxmlObjectEntry.uxmlObjectAssets.Add(uxmlObjectAsset2);
					uxmlObjectAsset2.parentId = parent.id;
					uxmlObjectAsset2.id = this.GetNextUxmlAssetId(parent.id);
					this.m_UxmlObjectIds.Add(uxmlObjectAsset2.id);
				}
				result = this.AddUxmlObject(uxmlObjectAsset2, null, fullTypeName, xmlNamespace);
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal int GetNextUxmlAssetId(int parentId)
		{
			int hashCode = Guid.NewGuid().GetHashCode();
			return (this.GetNextChildSerialNumber() + 585386304) * -1521134295 + parentId + hashCode;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void RemoveUxmlObject(int id, bool onlyIfIsField = false)
		{
			for (int i = 0; i < this.m_UxmlObjectEntries.Count; i++)
			{
				VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry = this.m_UxmlObjectEntries[i];
				int j = 0;
				while (j < uxmlObjectEntry.uxmlObjectAssets.Count)
				{
					UxmlObjectAsset uxmlObjectAsset = uxmlObjectEntry.uxmlObjectAssets[j];
					bool flag = uxmlObjectAsset.id == id;
					if (flag)
					{
						bool flag2 = onlyIfIsField && !uxmlObjectAsset.isField;
						if (flag2)
						{
							return;
						}
						uxmlObjectEntry.uxmlObjectAssets.RemoveAt(j);
						this.RemoveUxmlObjectEntryDependencies(uxmlObjectAsset.id);
						bool flag3 = uxmlObjectEntry.uxmlObjectAssets.Count == 0;
						if (flag3)
						{
							int index = this.m_UxmlObjectEntries.IndexOf(uxmlObjectEntry);
							this.m_UxmlObjectEntries.RemoveAt(index);
							this.m_UxmlObjectIds.RemoveAt(index);
							this.RemoveUxmlObject(uxmlObjectEntry.parentId, true);
						}
						return;
					}
					else
					{
						j++;
					}
				}
			}
		}

		private void RemoveUxmlObjectEntryDependencies(int parentId)
		{
			bool flag = this.m_UxmlObjectEntries.Count == 0;
			if (!flag)
			{
				List<VisualTreeAsset.UxmlObjectEntry> list = CollectionPool<List<VisualTreeAsset.UxmlObjectEntry>, VisualTreeAsset.UxmlObjectEntry>.Get();
				foreach (VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry in this.m_UxmlObjectEntries)
				{
					bool flag2 = parentId == uxmlObjectEntry.parentId;
					if (flag2)
					{
						list.Add(uxmlObjectEntry);
					}
				}
				foreach (VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry2 in list)
				{
					int index = this.m_UxmlObjectEntries.IndexOf(uxmlObjectEntry2);
					this.m_UxmlObjectEntries.RemoveAt(index);
					this.m_UxmlObjectIds.RemoveAt(index);
					foreach (UxmlObjectAsset uxmlObjectAsset in uxmlObjectEntry2.uxmlObjectAssets)
					{
						this.RemoveUxmlObjectEntryDependencies(uxmlObjectAsset.id);
					}
				}
				CollectionPool<List<VisualTreeAsset.UxmlObjectEntry>, VisualTreeAsset.UxmlObjectEntry>.Release(list);
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void CollectUxmlObjectAssets(UxmlAsset parent, string fieldName, List<UxmlObjectAsset> foundEntries)
		{
			bool flag = parent == null;
			if (!flag)
			{
				foreach (VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry in this.m_UxmlObjectEntries)
				{
					bool flag2 = uxmlObjectEntry.parentId == parent.id;
					if (flag2)
					{
						bool flag3 = !string.IsNullOrEmpty(fieldName);
						if (flag3)
						{
							UxmlObjectAsset field = uxmlObjectEntry.GetField(fieldName);
							bool flag4 = field != null;
							if (flag4)
							{
								this.CollectUxmlObjectAssets(field, null, foundEntries);
							}
						}
						else
						{
							foreach (UxmlObjectAsset uxmlObjectAsset in uxmlObjectEntry.uxmlObjectAssets)
							{
								bool flag5 = !uxmlObjectAsset.isField;
								if (flag5)
								{
									foundEntries.Add(uxmlObjectAsset);
								}
							}
						}
						break;
					}
				}
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void SetUxmlObjectAssets(UxmlAsset parent, string fieldName, List<UxmlObjectAsset> entries)
		{
			foreach (VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry in this.m_UxmlObjectEntries)
			{
				bool flag = uxmlObjectEntry.parentId == parent.id;
				if (flag)
				{
					bool flag2 = !string.IsNullOrEmpty(fieldName);
					if (flag2)
					{
						UxmlObjectAsset field = uxmlObjectEntry.GetField(fieldName);
						bool flag3 = field != null;
						if (flag3)
						{
							this.SetUxmlObjectAssets(field, null, entries);
						}
					}
					else
					{
						for (int i = uxmlObjectEntry.uxmlObjectAssets.Count - 1; i >= 0; i--)
						{
							bool flag4 = !uxmlObjectEntry.uxmlObjectAssets[i].isField;
							if (flag4)
							{
								uxmlObjectEntry.uxmlObjectAssets.RemoveAt(i);
							}
						}
						uxmlObjectEntry.uxmlObjectAssets.AddRange(entries);
						bool flag5 = uxmlObjectEntry.uxmlObjectAssets.Count == 0;
						if (flag5)
						{
							int index = this.m_UxmlObjectEntries.IndexOf(uxmlObjectEntry);
							this.m_UxmlObjectEntries.RemoveAt(index);
							this.m_UxmlObjectIds.RemoveAt(index);
							this.RemoveUxmlObject(uxmlObjectEntry.parentId, true);
						}
					}
					break;
				}
			}
		}

		internal List<T> GetUxmlObjects<T>(IUxmlAttributes asset, CreationContext cc) where T : new()
		{
			UxmlAsset uxmlAsset = asset as UxmlAsset;
			bool flag = uxmlAsset != null;
			if (flag)
			{
				VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry = this.GetUxmlObjectEntry(uxmlAsset.id);
				bool flag2 = uxmlObjectEntry.uxmlObjectAssets != null;
				if (flag2)
				{
					List<T> list = null;
					foreach (UxmlObjectAsset uxmlObjectAsset in uxmlObjectEntry.uxmlObjectAssets)
					{
						IBaseUxmlObjectFactory uxmlObjectFactory = this.GetUxmlObjectFactory(uxmlObjectAsset);
						IUxmlObjectFactory<T> uxmlObjectFactory2 = uxmlObjectFactory as IUxmlObjectFactory<T>;
						bool flag3 = uxmlObjectFactory2 == null;
						if (!flag3)
						{
							T item = uxmlObjectFactory2.CreateObject(uxmlObjectAsset, cc);
							bool flag4 = list == null;
							if (flag4)
							{
								list = new List<T>
								{
									item
								};
							}
							else
							{
								list.Add(item);
							}
						}
					}
					return list;
				}
			}
			return null;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool AssetEntryExists(string path, Type type)
		{
			foreach (VisualTreeAsset.AssetEntry assetEntry in this.m_AssetEntries)
			{
				bool flag = assetEntry.path == path && assetEntry.type == type;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void RegisterAssetEntry(string path, Type type, Object asset)
		{
			this.m_AssetEntries.Add(new VisualTreeAsset.AssetEntry(path, type, asset));
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal void TransferAssetEntries(VisualTreeAsset otherVta)
		{
			this.m_AssetEntries.Clear();
			this.m_AssetEntries.AddRange(otherVta.m_AssetEntries);
		}

		internal T GetAsset<T>(string path) where T : Object
		{
			return this.GetAsset(path, typeof(T)) as T;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal Object GetAsset(string path, Type type)
		{
			foreach (VisualTreeAsset.AssetEntry assetEntry in this.m_AssetEntries)
			{
				bool flag = assetEntry.path == path && type.IsAssignableFrom(assetEntry.type);
				if (flag)
				{
					return assetEntry.asset;
				}
			}
			return null;
		}

		internal Type GetAssetType(string path)
		{
			foreach (VisualTreeAsset.AssetEntry assetEntry in this.m_AssetEntries)
			{
				bool flag = assetEntry.path == path;
				if (flag)
				{
					return assetEntry.type;
				}
			}
			return null;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal VisualTreeAsset.UxmlObjectEntry GetUxmlObjectEntry(int id)
		{
			bool flag = this.m_UxmlObjectEntries != null;
			if (flag)
			{
				foreach (VisualTreeAsset.UxmlObjectEntry uxmlObjectEntry in this.m_UxmlObjectEntries)
				{
					bool flag2 = uxmlObjectEntry.parentId == id;
					if (flag2)
					{
						return uxmlObjectEntry;
					}
				}
			}
			return default(VisualTreeAsset.UxmlObjectEntry);
		}

		internal IBaseUxmlObjectFactory GetUxmlObjectFactory(UxmlObjectAsset uxmlObjectAsset)
		{
			List<IBaseUxmlObjectFactory> list;
			bool flag = !UxmlObjectFactoryRegistry.factories.TryGetValue(uxmlObjectAsset.fullTypeName, out list);
			IBaseUxmlObjectFactory result;
			if (flag)
			{
				Debug.LogErrorFormat("Element '{0}' has no registered factory method.", new object[]
				{
					uxmlObjectAsset.fullTypeName
				});
				result = null;
			}
			else
			{
				IBaseUxmlObjectFactory baseUxmlObjectFactory = null;
				CreationContext cc = new CreationContext(this);
				foreach (IBaseUxmlObjectFactory baseUxmlObjectFactory2 in list)
				{
					bool flag2 = baseUxmlObjectFactory2.AcceptsAttributeBag(uxmlObjectAsset, cc);
					if (flag2)
					{
						baseUxmlObjectFactory = baseUxmlObjectFactory2;
						break;
					}
				}
				bool flag3 = baseUxmlObjectFactory == null;
				if (flag3)
				{
					Debug.LogErrorFormat("Element '{0}' has a no factory that accept the set of XML attributes specified.", new object[]
					{
						uxmlObjectAsset.fullTypeName
					});
					result = null;
				}
				else
				{
					result = baseUxmlObjectFactory;
				}
			}
			return result;
		}

		internal List<VisualTreeAsset.SlotDefinition> slots
		{
			get
			{
				return this.m_Slots;
			}
		}

		internal int contentContainerId
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				return this.m_ContentContainerId;
			}
			set
			{
				this.m_ContentContainerId = value;
			}
		}

		public TemplateContainer Instantiate()
		{
			TemplateContainer templateContainer = new TemplateContainer(base.name, this);
			try
			{
				CreationContext cc = new CreationContext(VisualTreeAsset.s_TemporarySlotInsertionPoints, null, null, null, null, VisualTreeAsset.s_VeaIdsPath, null);
				this.CloneTree(templateContainer, cc);
			}
			finally
			{
				VisualTreeAsset.s_TemporarySlotInsertionPoints.Clear();
				VisualTreeAsset.s_VeaIdsPath.Clear();
			}
			return templateContainer;
		}

		public TemplateContainer Instantiate(string bindingPath)
		{
			TemplateContainer templateContainer = this.Instantiate();
			templateContainer.bindingPath = bindingPath;
			return templateContainer;
		}

		public TemplateContainer CloneTree()
		{
			return this.Instantiate();
		}

		public TemplateContainer CloneTree(string bindingPath)
		{
			return this.Instantiate(bindingPath);
		}

		public void CloneTree(VisualElement target)
		{
			int num;
			int num2;
			this.CloneTree(target, out num, out num2);
		}

		public void CloneTree(VisualElement target, out int firstElementIndex, out int elementAddedCount)
		{
			bool flag = target == null;
			if (flag)
			{
				throw new ArgumentNullException("target");
			}
			firstElementIndex = target.childCount;
			try
			{
				CreationContext cc = new CreationContext(VisualTreeAsset.s_TemporarySlotInsertionPoints, null, null, null, null, VisualTreeAsset.s_VeaIdsPath, null);
				this.CloneTree(target, cc);
			}
			finally
			{
				elementAddedCount = target.childCount - firstElementIndex;
				VisualTreeAsset.s_TemporarySlotInsertionPoints.Clear();
				VisualTreeAsset.s_VeaIdsPath.Clear();
			}
		}

		internal void CloneTree(VisualElement target, CreationContext cc)
		{
			bool flag = target == null;
			if (flag)
			{
				throw new ArgumentNullException("target");
			}
			bool flag2 = (this.visualElementAssets == null || this.visualElementAssets.Count <= 0) && (this.templateAssets == null || this.templateAssets.Count <= 0);
			if (!flag2)
			{
				Dictionary<int, List<VisualElementAsset>> dictionary = new Dictionary<int, List<VisualElementAsset>>();
				int num = (this.visualElementAssets == null) ? 0 : this.visualElementAssets.Count;
				int num2 = (this.templateAssets == null) ? 0 : this.templateAssets.Count;
				for (int i = 0; i < num + num2; i++)
				{
					VisualElementAsset visualElementAsset = (i < num) ? this.visualElementAssets[i] : this.templateAssets[i - num];
					List<VisualElementAsset> list;
					bool flag3 = !dictionary.TryGetValue(visualElementAsset.parentId, out list);
					if (flag3)
					{
						list = new List<VisualElementAsset>();
						dictionary[visualElementAsset.parentId] = list;
					}
					list.Add(visualElementAsset);
				}
				List<VisualElementAsset> list2;
				dictionary.TryGetValue(0, out list2);
				bool flag4 = list2 == null || list2.Count == 0;
				if (!flag4)
				{
					Debug.Assert(list2.Count == 1);
					VisualElementAsset visualElementAsset2 = list2[0];
					VisualTreeAsset.AssignClassListFromAssetToElement(visualElementAsset2, target);
					VisualTreeAsset.AssignStyleSheetFromAssetToElement(visualElementAsset2, target);
					list2.Clear();
					dictionary.TryGetValue(visualElementAsset2.id, out list2);
					bool flag5 = list2 == null || list2.Count == 0;
					if (!flag5)
					{
						list2.Sort(new Comparison<VisualElementAsset>(VisualTreeAsset.CompareForOrder));
						foreach (VisualElementAsset visualElementAsset3 in list2)
						{
							Assert.IsNotNull<VisualElementAsset>(visualElementAsset3);
							bool flag6 = false;
							bool flag7 = visualElementAsset3 is TemplateAsset;
							if (flag7)
							{
								cc.veaIdsPath.Add(visualElementAsset3.id);
								flag6 = true;
							}
							CreationContext context = new CreationContext(cc.slotInsertionPoints, cc.attributeOverrides, cc.serializedDataOverrides, this, target, cc.veaIdsPath, null);
							VisualElement visualElement = this.CloneSetupRecursively(visualElementAsset3, dictionary, context);
							bool flag8 = flag6;
							if (flag8)
							{
								cc.veaIdsPath.Remove(visualElementAsset3.id);
							}
							bool flag9 = visualElement == null;
							if (!flag9)
							{
								visualElement.visualTreeAssetSource = this;
								target.hierarchy.Add(visualElement);
							}
						}
					}
				}
			}
		}

		private VisualElement CloneSetupRecursively(VisualElementAsset root, Dictionary<int, List<VisualElementAsset>> idToChildren, CreationContext context)
		{
			bool skipClone = root.skipClone;
			VisualElement result;
			if (skipClone)
			{
				result = null;
			}
			else
			{
				VisualElement visualElement = VisualTreeAsset.Create(root, context);
				bool flag = visualElement == null;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = root.id == context.visualTreeAsset.contentContainerId;
					if (flag2)
					{
						TemplateContainer templateContainer = context.target as TemplateContainer;
						bool flag3 = templateContainer != null;
						if (flag3)
						{
							templateContainer.SetContentContainer(visualElement);
						}
						else
						{
							Debug.LogError("Trying to clone a VisualTreeAsset with a custom content container into a element which is not a template container");
						}
					}
					string key;
					bool flag4 = context.slotInsertionPoints != null && this.TryGetSlotInsertionPoint(root.id, out key);
					if (flag4)
					{
						context.slotInsertionPoints.Add(key, visualElement);
					}
					bool flag5 = root.ruleIndex != -1;
					if (flag5)
					{
						bool flag6 = this.inlineSheet == null;
						if (flag6)
						{
							Debug.LogWarning("VisualElementAsset has a RuleIndex but no inlineStyleSheet");
						}
						else
						{
							StyleRule rule = this.inlineSheet.rules[root.ruleIndex];
							visualElement.SetInlineRule(this.inlineSheet, rule);
						}
					}
					TemplateAsset templateAsset = root as TemplateAsset;
					List<VisualElementAsset> list;
					bool flag7 = idToChildren.TryGetValue(root.id, out list);
					if (flag7)
					{
						list.Sort(new Comparison<VisualElementAsset>(VisualTreeAsset.CompareForOrder));
						using (List<VisualElementAsset>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								VisualElementAsset childVea = enumerator.Current;
								bool flag8 = false;
								bool flag9 = childVea is TemplateAsset;
								if (flag9)
								{
									context.veaIdsPath.Add(childVea.id);
									flag8 = true;
								}
								VisualElement visualElement2 = this.CloneSetupRecursively(childVea, idToChildren, context);
								bool flag10 = flag8;
								if (flag10)
								{
									context.veaIdsPath.Remove(childVea.id);
								}
								bool flag11 = visualElement2 == null;
								if (!flag11)
								{
									bool flag12 = templateAsset == null;
									if (flag12)
									{
										visualElement.Add(visualElement2);
									}
									else
									{
										int num = (templateAsset.slotUsages == null) ? -1 : templateAsset.slotUsages.FindIndex((VisualTreeAsset.SlotUsageEntry u) => u.assetId == childVea.id);
										bool flag13 = num != -1;
										if (flag13)
										{
											string slotName = templateAsset.slotUsages[num].slotName;
											Assert.IsFalse(string.IsNullOrEmpty(slotName), "a lost name should not be null or empty, this probably points to an importer or serialization bug");
											VisualElement visualElement3;
											bool flag14 = context.slotInsertionPoints == null || !context.slotInsertionPoints.TryGetValue(slotName, out visualElement3);
											if (flag14)
											{
												Debug.LogErrorFormat("Slot '{0}' was not found. Existing slots: {1}", new object[]
												{
													slotName,
													(context.slotInsertionPoints == null) ? string.Empty : string.Join(", ", context.slotInsertionPoints.Keys.ToArray<string>())
												});
												visualElement.Add(visualElement2);
											}
											else
											{
												visualElement3.Add(visualElement2);
											}
										}
										else
										{
											visualElement.Add(visualElement2);
										}
									}
								}
							}
						}
					}
					bool flag15 = templateAsset != null && context.slotInsertionPoints != null;
					if (flag15)
					{
						context.slotInsertionPoints.Clear();
					}
					result = visualElement;
				}
			}
			return result;
		}

		internal static int CompareForOrder(VisualElementAsset a, VisualElementAsset b)
		{
			return a.orderInDocument.CompareTo(b.orderInDocument);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool TryGetSlotInsertionPoint(int insertionPointId, out string slotName)
		{
			for (int i = 0; i < this.m_Slots.Count; i++)
			{
				VisualTreeAsset.SlotDefinition slotDefinition = this.m_Slots[i];
				bool flag = slotDefinition.insertionPointId == insertionPointId;
				if (flag)
				{
					slotName = slotDefinition.name;
					return true;
				}
			}
			slotName = null;
			return false;
		}

		internal bool TryGetUsingEntry(string templateName, out VisualTreeAsset.UsingEntry entry)
		{
			entry = default(VisualTreeAsset.UsingEntry);
			bool flag = this.m_Usings.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int num = this.m_Usings.BinarySearch(new VisualTreeAsset.UsingEntry(templateName, string.Empty), VisualTreeAsset.UsingEntry.comparer);
				bool flag2 = num < 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					entry = this.m_Usings[num];
					result = true;
				}
			}
			return result;
		}

		private void RemoveUsingEntry(VisualTreeAsset.UsingEntry entry)
		{
			this.m_Usings.Remove(entry);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal VisualTreeAsset ResolveTemplate(string templateName)
		{
			VisualTreeAsset.UsingEntry usingEntry;
			bool flag = !this.TryGetUsingEntry(templateName, out usingEntry);
			VisualTreeAsset result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = usingEntry.asset;
				if (flag2)
				{
					result = usingEntry.asset;
				}
				else
				{
					string path = usingEntry.path;
					result = (Panel.LoadResource(path, typeof(VisualTreeAsset), 1f) as VisualTreeAsset);
				}
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal static VisualElement Create(VisualElementAsset asset, CreationContext ctx)
		{
			VisualTreeAsset.<>c__DisplayClass82_0 CS$<>8__locals1;
			CS$<>8__locals1.asset = asset;
			bool flag = CS$<>8__locals1.asset.serializedData != null;
			VisualElement result;
			if (flag)
			{
				result = CS$<>8__locals1.asset.Instantiate(ctx);
			}
			else
			{
				List<IUxmlFactory> list;
				bool flag2 = !VisualElementFactoryRegistry.TryGetValue(CS$<>8__locals1.asset.fullTypeName, out list);
				if (flag2)
				{
					bool flag3 = CS$<>8__locals1.asset.fullTypeName.StartsWith("UnityEngine.Experimental.UIElements.") || CS$<>8__locals1.asset.fullTypeName.StartsWith("UnityEditor.Experimental.UIElements.");
					if (flag3)
					{
						string fullTypeName = CS$<>8__locals1.asset.fullTypeName.Replace(".Experimental.UIElements", ".UIElements");
						bool flag4 = !VisualElementFactoryRegistry.TryGetValue(fullTypeName, out list);
						if (flag4)
						{
							return VisualTreeAsset.<Create>g__CreateError|82_0(ref CS$<>8__locals1);
						}
					}
					else
					{
						bool flag5 = CS$<>8__locals1.asset.fullTypeName == "UXML";
						if (!flag5)
						{
							return VisualTreeAsset.<Create>g__CreateError|82_0(ref CS$<>8__locals1);
						}
						VisualElementFactoryRegistry.TryGetValue(typeof(UxmlRootElementFactory).Namespace + "." + CS$<>8__locals1.asset.fullTypeName, out list);
					}
				}
				IUxmlFactory uxmlFactory = null;
				foreach (IUxmlFactory uxmlFactory2 in list)
				{
					bool flag6 = uxmlFactory2.AcceptsAttributeBag(CS$<>8__locals1.asset, ctx);
					if (flag6)
					{
						uxmlFactory = uxmlFactory2;
						break;
					}
				}
				bool flag7 = uxmlFactory == null;
				if (flag7)
				{
					Debug.LogErrorFormat("Element '{0}' has a no factory that accept the set of XML attributes specified.", new object[]
					{
						CS$<>8__locals1.asset.fullTypeName
					});
					result = new Label(string.Format("Type with no factory: '{0}'", CS$<>8__locals1.asset.fullTypeName));
				}
				else
				{
					VisualElement visualElement = uxmlFactory.Create(CS$<>8__locals1.asset, ctx);
					bool flag8 = visualElement != null;
					if (flag8)
					{
						VisualTreeAsset.AssignClassListFromAssetToElement(CS$<>8__locals1.asset, visualElement);
						VisualTreeAsset.AssignStyleSheetFromAssetToElement(CS$<>8__locals1.asset, visualElement);
					}
					result = visualElement;
				}
			}
			return result;
		}

		private static void AssignClassListFromAssetToElement(VisualElementAsset asset, VisualElement element)
		{
			bool flag = asset.classes != null;
			if (flag)
			{
				for (int i = 0; i < asset.classes.Length; i++)
				{
					element.AddToClassList(asset.classes[i]);
				}
			}
		}

		private static void AssignStyleSheetFromAssetToElement(VisualElementAsset asset, VisualElement element)
		{
			bool hasStylesheetPaths = asset.hasStylesheetPaths;
			if (hasStylesheetPaths)
			{
				for (int i = 0; i < asset.stylesheetPaths.Count; i++)
				{
					element.AddStyleSheetPath(asset.stylesheetPaths[i]);
				}
			}
			bool hasStylesheets = asset.hasStylesheets;
			if (hasStylesheets)
			{
				for (int j = 0; j < asset.stylesheets.Count; j++)
				{
					bool flag = asset.stylesheets[j] != null;
					if (flag)
					{
						element.styleSheets.Add(asset.stylesheets[j]);
					}
				}
			}
		}

		public int contentHash
		{
			get
			{
				return this.m_ContentHash;
			}
			set
			{
				this.m_ContentHash = value;
			}
		}

		[CompilerGenerated]
		internal static VisualElement <Create>g__CreateError|82_0(ref VisualTreeAsset.<>c__DisplayClass82_0 A_0)
		{
			Debug.LogErrorFormat(VisualTreeAsset.NoRegisteredFactoryErrorMessage, new object[]
			{
				A_0.asset.fullTypeName
			});
			return new Label(string.Format("Unknown type: '{0}'", A_0.asset.fullTypeName));
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal static string LinkedVEAInTemplatePropertyName = "--unity-linked-vea-in-template";

		internal static string NoRegisteredFactoryErrorMessage = "Element '{0}' is missing a UxmlElementAttribute and has no registered factory method. Please ensure that you have the correct namespace imported.";

		[SerializeField]
		private bool m_ImportedWithErrors;

		[SerializeField]
		private bool m_HasUpdatedUrls;

		[SerializeField]
		private bool m_ImportedWithWarnings;

		private static readonly Dictionary<string, VisualElement> s_TemporarySlotInsertionPoints = new Dictionary<string, VisualElement>();

		private static readonly List<int> s_VeaIdsPath = new List<int>();

		[SerializeField]
		private List<VisualTreeAsset.UsingEntry> m_Usings = new List<VisualTreeAsset.UsingEntry>();

		[SerializeField]
		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal StyleSheet inlineSheet;

		[SerializeField]
		internal List<VisualElementAsset> m_VisualElementAssets = new List<VisualElementAsset>();

		[SerializeField]
		internal List<TemplateAsset> m_TemplateAssets = new List<TemplateAsset>();

		[SerializeField]
		private List<VisualTreeAsset.UxmlObjectEntry> m_UxmlObjectEntries = new List<VisualTreeAsset.UxmlObjectEntry>();

		[SerializeField]
		private List<int> m_UxmlObjectIds = new List<int>();

		[SerializeField]
		private List<VisualTreeAsset.AssetEntry> m_AssetEntries = new List<VisualTreeAsset.AssetEntry>();

		[SerializeField]
		private List<VisualTreeAsset.SlotDefinition> m_Slots = new List<VisualTreeAsset.SlotDefinition>();

		[SerializeField]
		private int m_ContentContainerId;

		[SerializeField]
		private int m_ContentHash;

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[Serializable]
		internal struct UsingEntry
		{
			public UsingEntry(string alias, string path)
			{
				this.alias = alias;
				this.path = path;
				this.asset = null;
			}

			public UsingEntry(string alias, VisualTreeAsset asset)
			{
				this.alias = alias;
				this.path = null;
				this.asset = asset;
			}

			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			internal static readonly IComparer<VisualTreeAsset.UsingEntry> comparer = new VisualTreeAsset.UsingEntryComparer();

			[SerializeField]
			public string alias;

			[SerializeField]
			public string path;

			[SerializeField]
			public VisualTreeAsset asset;
		}

		private class UsingEntryComparer : IComparer<VisualTreeAsset.UsingEntry>
		{
			public int Compare(VisualTreeAsset.UsingEntry x, VisualTreeAsset.UsingEntry y)
			{
				return string.CompareOrdinal(x.alias, y.alias);
			}
		}

		[Serializable]
		internal struct SlotDefinition
		{
			[SerializeField]
			public string name;

			[SerializeField]
			public int insertionPointId;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[Serializable]
		internal struct SlotUsageEntry
		{
			public SlotUsageEntry(string slotName, int assetId)
			{
				this.slotName = slotName;
				this.assetId = assetId;
			}

			[SerializeField]
			public string slotName;

			[SerializeField]
			public int assetId;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[Serializable]
		internal struct UxmlObjectEntry
		{
			public UxmlObjectEntry(int parentId, List<UxmlObjectAsset> uxmlObjectAssets)
			{
				this.parentId = parentId;
				this.uxmlObjectAssets = uxmlObjectAssets;
			}

			public UxmlObjectAsset GetField(string fieldName)
			{
				foreach (UxmlObjectAsset uxmlObjectAsset in this.uxmlObjectAssets)
				{
					bool flag = uxmlObjectAsset.isField && uxmlObjectAsset.fullTypeName == fieldName;
					if (flag)
					{
						return uxmlObjectAsset;
					}
				}
				return null;
			}

			public override string ToString()
			{
				string format = "UxmlObjectEntry parent:{0} ({1})";
				object arg = this.parentId;
				List<UxmlObjectAsset> list = this.uxmlObjectAssets;
				return string.Format(format, arg, (list != null) ? new int?(list.Count) : null);
			}

			[SerializeField]
			public int parentId;

			[SerializeField]
			public List<UxmlObjectAsset> uxmlObjectAssets;
		}

		[Serializable]
		private struct AssetEntry
		{
			public Type type
			{
				get
				{
					Type result;
					if ((result = this.m_CachedType) == null)
					{
						result = (this.m_CachedType = Type.GetType(this.m_TypeFullName));
					}
					return result;
				}
			}

			public string path
			{
				get
				{
					return this.m_Path;
				}
			}

			public Object asset
			{
				get
				{
					bool isSet = this.m_AssetReference.isSet;
					Object result;
					if (isSet)
					{
						result = this.m_AssetReference.asset;
					}
					else
					{
						result = null;
					}
					return result;
				}
			}

			public AssetEntry(string path, Type type, Object asset)
			{
				this.m_Path = path;
				this.m_TypeFullName = type.AssemblyQualifiedName;
				this.m_CachedType = type;
				this.m_AssetReference = asset;
				this.m_InstanceID = ((asset != null) ? asset.GetInstanceID() : 0);
			}

			[SerializeField]
			private string m_Path;

			[SerializeField]
			private string m_TypeFullName;

			[SerializeField]
			private LazyLoadReference<Object> m_AssetReference;

			[SerializeField]
			private int m_InstanceID;

			private Type m_CachedType;
		}
	}
}
