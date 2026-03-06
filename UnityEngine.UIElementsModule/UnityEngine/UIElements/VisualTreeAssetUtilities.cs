using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	internal static class VisualTreeAssetUtilities
	{
		public static UxmlAsset GetParentAsset(this VisualTreeAsset vta, UxmlAsset asset)
		{
			int parentId = asset.parentId;
			int num = vta.visualElementAssets.FindIndex((VisualElementAsset ua) => ua.id == parentId);
			bool flag = num >= 0;
			UxmlAsset result;
			if (flag)
			{
				result = vta.visualElementAssets[num];
			}
			else
			{
				num = vta.templateAssets.FindIndex((TemplateAsset ta) => ta.id == parentId);
				bool flag2 = num >= 0;
				if (flag2)
				{
					result = vta.templateAssets[num];
				}
				else
				{
					Predicate<UxmlObjectAsset> <>9__2;
					for (int i = 0; i < vta.uxmlObjectEntries.Count; i++)
					{
						List<UxmlObjectAsset> uxmlObjectAssets = vta.uxmlObjectEntries[i].uxmlObjectAssets;
						List<UxmlObjectAsset> list = uxmlObjectAssets;
						Predicate<UxmlObjectAsset> match;
						if ((match = <>9__2) == null)
						{
							match = (<>9__2 = ((UxmlObjectAsset ua) => ua.id == parentId));
						}
						num = list.FindIndex(match);
						bool flag3 = num >= 0;
						if (flag3)
						{
							return uxmlObjectAssets[num];
						}
					}
					result = null;
				}
			}
			return result;
		}

		public static IEnumerable<string> EnumerateEnclosingNamespaces(string fullTypeName)
		{
			int startIndex = fullTypeName.Length - 1;
			for (;;)
			{
				int lastDot = fullTypeName.LastIndexOf(".", startIndex, StringComparison.Ordinal);
				bool flag = lastDot >= 0;
				if (!flag)
				{
					break;
				}
				yield return fullTypeName.Substring(0, lastDot);
				startIndex = lastDot - 1;
			}
			yield break;
			yield break;
		}

		public static UxmlNamespaceDefinition FindUxmlNamespaceDefinitionFromPrefix(this VisualTreeAsset vta, UxmlAsset asset, string prefix)
		{
			for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = vta.GetParentAsset(uxmlAsset))
			{
				for (int i = 0; i < uxmlAsset.namespaceDefinitions.Count; i++)
				{
					UxmlNamespaceDefinition uxmlNamespaceDefinition = uxmlAsset.namespaceDefinitions[i];
					bool flag = string.Compare(uxmlNamespaceDefinition.prefix, prefix, StringComparison.Ordinal) == 0;
					if (flag)
					{
						return uxmlNamespaceDefinition;
					}
				}
			}
			return UxmlNamespaceDefinition.Empty;
		}

		public static UxmlNamespaceDefinition FindUxmlNamespaceDefinitionForTypeName(this VisualTreeAsset vta, UxmlAsset asset, string fullTypeName)
		{
			List<UxmlNamespaceDefinition> list;
			UxmlNamespaceDefinition empty;
			using (CollectionPool<List<UxmlNamespaceDefinition>, UxmlNamespaceDefinition>.Get(out list))
			{
				for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = vta.GetParentAsset(uxmlAsset))
				{
					list.AddRange(uxmlAsset.namespaceDefinitions);
				}
				bool flag = list.Count == 0;
				if (flag)
				{
					empty = UxmlNamespaceDefinition.Empty;
				}
				else
				{
					foreach (string value in VisualTreeAssetUtilities.EnumerateEnclosingNamespaces(fullTypeName))
					{
						for (int i = 0; i < list.Count; i++)
						{
							bool flag2 = list[i].resolvedNamespace.Equals(value, StringComparison.Ordinal);
							if (flag2)
							{
								return list[i];
							}
						}
					}
					empty = UxmlNamespaceDefinition.Empty;
				}
			}
			return empty;
		}

		public static void GatherUxmlNamespaceDefinitions(this VisualTreeAsset vta, UxmlAsset asset, List<UxmlNamespaceDefinition> definitions)
		{
			for (UxmlAsset uxmlAsset = asset; uxmlAsset != null; uxmlAsset = vta.GetParentAsset(uxmlAsset))
			{
				definitions.InsertRange(0, uxmlAsset.namespaceDefinitions);
			}
		}
	}
}
