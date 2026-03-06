using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	internal static class InternalUtility
	{
		public static T[] GetComponents<T>(GameObject go) where T : Component
		{
			return go.transform.GetComponentsInChildren<T>();
		}

		public static T[] GetComponents<T>(this IEnumerable<Transform> transforms) where T : Component
		{
			List<T> list = new List<T>();
			foreach (Transform transform in transforms)
			{
				list.AddRange(transform.GetComponentsInChildren<T>());
			}
			return list.ToArray();
		}

		public static GameObject EmptyGameObjectWithTransform(Transform t)
		{
			return new GameObject
			{
				transform = 
				{
					localPosition = t.localPosition,
					localRotation = t.localRotation,
					localScale = t.localScale
				}
			};
		}

		public static GameObject MeshGameObjectWithTransform(string name, Transform t, Mesh mesh, Material mat, bool inheritParent)
		{
			GameObject gameObject = InternalUtility.EmptyGameObjectWithTransform(t);
			gameObject.name = name;
			gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
			gameObject.AddComponent<MeshRenderer>().sharedMaterial = mat;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			if (inheritParent)
			{
				gameObject.transform.SetParent(t.parent, false);
			}
			return gameObject;
		}

		public static T NextEnumValue<T>(this T current) where T : IConvertible
		{
			Array values = Enum.GetValues(current.GetType());
			int i = 0;
			int length = values.Length;
			while (i < length)
			{
				if (current.Equals(values.GetValue(i)))
				{
					return (T)((object)values.GetValue((i + 1) % length));
				}
				i++;
			}
			return current;
		}

		public static string ControlKeyString(char character)
		{
			if (character == '⌘')
			{
				return "Control";
			}
			if (character == '⇧')
			{
				return "Shift";
			}
			if (character == '⌥')
			{
				return "Alt";
			}
			if (character == '⎇')
			{
				return "Alt";
			}
			if (character == '⌫')
			{
				return "Delete";
			}
			return character.ToString();
		}

		public static bool TryParseColor(string value, ref Color col)
		{
			string valid = "01234567890.,";
			value = new string((from c in value
			where valid.Contains(c)
			select c).ToArray<char>());
			string[] array = value.Split(',', StringSplitOptions.None);
			if (array.Length < 4)
			{
				return false;
			}
			try
			{
				float r = float.Parse(array[0]);
				float g = float.Parse(array[1]);
				float b = float.Parse(array[2]);
				float a = float.Parse(array[3]);
				col.r = r;
				col.g = g;
				col.b = b;
				col.a = a;
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static T DemandComponent<T>(this Component component) where T : Component
		{
			return component.gameObject.DemandComponent<T>();
		}

		public static T DemandComponent<T>(this GameObject gameObject) where T : Component
		{
			T result;
			if (!gameObject.TryGetComponent<T>(out result))
			{
				result = gameObject.AddComponent<T>();
			}
			return result;
		}
	}
}
