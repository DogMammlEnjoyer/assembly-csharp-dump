using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(1)]
	[Nullable(0)]
	public static class Utilities
	{
		public static string SanitizeString(string str)
		{
			string text = "";
			for (int i = 0; i < str.Length; i++)
			{
				if (char.IsLetterOrDigit(str[i]))
				{
					text += str[i].ToString();
				}
				else if (char.IsWhiteSpace(str[i]))
				{
					text += "-";
				}
			}
			return text;
		}

		private static void StripMeshesForObjectsOfType<[Nullable(2)] T>(GameObject rootObject)
		{
			T[] componentsInChildren = rootObject.GetComponentsInChildren<T>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Component component = componentsInChildren[i] as Component;
				if (!(component == null))
				{
					if (component.gameObject.GetComponent<Renderer>() != null)
					{
						Object.DestroyImmediate(component.gameObject.GetComponent<Renderer>());
					}
					if (component.gameObject.GetComponent<MeshFilter>() != null)
					{
						Object.DestroyImmediate(component.gameObject.GetComponent<MeshFilter>());
					}
				}
			}
		}

		public static string GetSceneNameFromFilePath(string filePath, bool sanitizeName = true)
		{
			string[] array = filePath.Split('/', StringSplitOptions.None);
			string[] array2 = array[array.Length - 1].Split('.', StringSplitOptions.None);
			string text = "";
			for (int i = 0; i < array2.Length - 1; i++)
			{
				text += array2[i];
				if (i < array2.Length - 2)
				{
					text += ".";
				}
			}
			if (!sanitizeName)
			{
				return text;
			}
			return Utilities.SanitizeString(text);
		}
	}
}
