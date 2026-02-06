using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Cysharp.Text;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace GorillaExtensions
{
	public static class GTExt
	{
		public static T GetComponentInHierarchy<T>(this Scene scene, bool includeInactive = true) where T : Component
		{
			if (!scene.IsValid())
			{
				return default(T);
			}
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					component = componentsInChildren[j].GetComponent<T>();
					if (component != null)
					{
						return component;
					}
				}
			}
			return default(T);
		}

		public static List<T> GetComponentsInHierarchy<T>(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			List<T> list = new List<T>(capacity);
			if (!scene.IsValid())
			{
				return list;
			}
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				T[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren<T>(includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		public static List<Object> GetComponentsInHierarchy(this Scene scene, Type type, bool includeInactive = true, int capacity = 64)
		{
			List<Object> list = new List<Object>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				Component[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren(type, includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			return scene.GetComponentsInHierarchy(includeInactive, capacity);
		}

		public static List<T> GetComponentsInHierarchyUntil<T, TStop1>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2, TStop3>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		public static List<T> GetComponentsInChildrenUntil<T, TStop1>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component
		{
			GTExt.<>c__DisplayClass7_0<T, TStop1> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && root.GetComponent<TStop1>() != null)
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		public static PooledObject<List<T>> GTGetComponentsListPool<T>(this Component root, bool includeInactive, out List<T> pooledList) where T : Component
		{
			PooledObject<List<T>> result = UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out pooledList);
			root.GetComponentsInChildren<T>(includeInactive, pooledList);
			return result;
		}

		public static PooledObject<List<T>> GTGetComponentsListPool<T>(this Component root, out List<T> pooledList) where T : Component
		{
			PooledObject<List<T>> result = UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out pooledList);
			root.GetComponentsInChildren<T>(pooledList);
			return result;
		}

		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component
		{
			GTExt.<>c__DisplayClass10_0<T, TStop1, TStop2> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			GTExt.<>c__DisplayClass11_0<T, TStop1, TStop2, TStop3> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null || root.GetComponent<TStop3>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		public static void GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, out List<T> out_included, out HashSet<T> out_excluded, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			out_included = root.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
			out_excluded = new HashSet<T>(root.GetComponentsInChildren<T>(includeInactive));
			out_excluded.ExceptWith(new HashSet<T>(out_included));
		}

		private static void _GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(Transform currentTransform, List<T> included, List<Component> excluded, bool includeInactive) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if (includeInactive || transform.gameObject.activeSelf)
				{
					Component item;
					if (GTExt._HasAnyComponents<TStop1, TStop2, TStop3>(transform, out item))
					{
						excluded.Add(item);
					}
					else
					{
						T component = transform.GetComponent<T>();
						if (component != null)
						{
							included.Add(component);
						}
						GTExt._GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(transform, included, excluded, includeInactive);
					}
				}
			}
		}

		private static bool _HasAnyComponents<TStop1, TStop2, TStop3>(Component component, out Component stopComponent) where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			stopComponent = component.GetComponent<TStop1>();
			if (stopComponent != null)
			{
				return true;
			}
			stopComponent = component.GetComponent<TStop2>();
			if (stopComponent != null)
			{
				return true;
			}
			stopComponent = component.GetComponent<TStop3>();
			return stopComponent != null;
		}

		public static T GetComponentWithRegex<T>(this Component root, string regexString) where T : Component
		{
			T[] componentsInChildren = root.GetComponentsInChildren<T>();
			Regex regex = new Regex(regexString);
			foreach (T t in componentsInChildren)
			{
				if (regex.IsMatch(t.name))
				{
					return t;
				}
			}
			return default(T);
		}

		private static List<T> GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, string regexString, bool includeInactive, int capacity = 64) where T : Component
		{
			List<T> result = new List<T>(capacity);
			Regex regex = new Regex(regexString);
			GTExt.GetComponentsWithRegex_Internal<T>(allComponents, regex, ref result);
			return result;
		}

		private static void GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, Regex regex, ref List<T> foundComponents) where T : Component
		{
			foreach (T t in allComponents)
			{
				string name = t.name;
				if (regex.IsMatch(name))
				{
					foundComponents.Add(t);
				}
			}
		}

		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(scene.GetComponentsInHierarchy(includeInactive, capacity), regexString, includeInactive, capacity);
		}

		public static List<T> GetComponentsWithRegex<T>(this Component root, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(root.GetComponentsInChildren<T>(includeInactive), regexString, includeInactive, capacity);
		}

		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string regexString, bool includeInactive = true, int capacity = 64)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexString, includeInactive, capacity);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		public static void GetComponentsWithRegex_Internal<T>(this List<T> allComponents, Regex[] regexes, int maxCount, ref List<T> foundComponents) where T : Component
		{
			if (maxCount == 0)
			{
				return;
			}
			int num = 0;
			foreach (T t in allComponents)
			{
				for (int i = 0; i < regexes.Length; i++)
				{
					if (regexes[i].IsMatch(t.name))
					{
						foundComponents.Add(t);
						num++;
						if (maxCount > 0 && num >= maxCount)
						{
							return;
						}
					}
				}
			}
		}

		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1, int capacity = 64) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, capacity);
			List<T> result = new List<T>(componentsInHierarchy.Count);
			Regex[] array = new Regex[regexStrings.Length];
			for (int i = 0; i < regexStrings.Length; i++)
			{
				array[i] = new Regex(regexStrings[i]);
			}
			componentsInHierarchy.GetComponentsWithRegex_Internal(array, maxCount, ref result);
			return result;
		}

		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, 64);
			List<T> list = new List<T>(componentsInHierarchy.Count);
			if (maxCount == 0)
			{
				return list;
			}
			int num = 0;
			foreach (T t in componentsInHierarchy)
			{
				bool flag = false;
				foreach (string pattern in regexStrings)
				{
					if (!flag && Regex.IsMatch(t.name, pattern))
					{
						foreach (string pattern2 in excludeRegexStrings)
						{
							if (!flag)
							{
								flag = Regex.IsMatch(t.name, pattern2);
							}
						}
						if (!flag)
						{
							list.Add(t);
							num++;
							if (maxCount > 0 && num >= maxCount)
							{
								return list;
							}
						}
					}
				}
			}
			return list;
		}

		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, includeInactive, maxCount, 64);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, excludeRegexStrings, includeInactive, maxCount);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		public static List<T> GetComponentsByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			T[] componentsInChildren = xform.GetComponentsInChildren<T>(includeInactive);
			List<T> list = new List<T>(componentsInChildren.Length);
			foreach (T t in componentsInChildren)
			{
				if (t.name == name)
				{
					list.Add(t);
				}
			}
			return list;
		}

		public static T GetComponentByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			foreach (T t in xform.GetComponentsInChildren<T>(includeInactive))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return default(T);
		}

		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, string name, bool includeInactive = true)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				if (gameObject.name.Contains(name))
				{
					list.Add(gameObject);
				}
				foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(includeInactive))
				{
					if (transform.name.Contains(name))
					{
						list.Add(transform.gameObject);
					}
				}
			}
			return list;
		}

		public static T GetOrAddComponent<T>(this GameObject gameObject, ref T component) where T : Component
		{
			if (component == null)
			{
				component = gameObject.GetOrAddComponent<T>();
			}
			return component;
		}

		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			T result;
			if (!gameObject.TryGetComponent<T>(out result))
			{
				result = gameObject.AddComponent<T>();
			}
			return result;
		}

		public static void SetLossyScale(this Transform transform, Vector3 scale)
		{
			scale = transform.InverseTransformVector(scale);
			Vector3 lossyScale = transform.lossyScale;
			transform.localScale = new Vector3(scale.x / lossyScale.x, scale.y / lossyScale.y, scale.z / lossyScale.z);
		}

		public static Quaternion TransformRotation(this Transform transform, Quaternion localRotation)
		{
			return transform.rotation * localRotation;
		}

		public static Quaternion InverseTransformRotation(this Transform transform, Quaternion localRotation)
		{
			return Quaternion.Inverse(transform.rotation) * localRotation;
		}

		public static Vector3 ProjectOnPlane(this Vector3 point, Vector3 planeAnchorPosition, Vector3 planeNormal)
		{
			return planeAnchorPosition + Vector3.ProjectOnPlane(point - planeAnchorPosition, planeNormal);
		}

		public static void ForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T obj = list[i];
				try
				{
					action(obj);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		public static void AddSortedUnique<T>(this List<T> list, T item)
		{
			int num = list.BinarySearch(item);
			if (num < 0)
			{
				list.Insert(~num, item);
			}
		}

		public static void RemoveSorted<T>(this List<T> list, T item)
		{
			int num = list.BinarySearch(item);
			if (num >= 0)
			{
				list.RemoveAt(num);
			}
		}

		public static bool ContainsSorted<T>(this List<T> list, T item)
		{
			return list.BinarySearch(item) >= 0;
		}

		public static void SafeForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T obj = list[i];
				try
				{
					action(obj);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		public static T[] Filled<T>(this T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = value;
			}
			return array;
		}

		public static bool CompareAs255Unclamped(this Color a, Color b)
		{
			int num = (int)(a.r * 255f);
			int num2 = (int)(a.g * 255f);
			int num3 = (int)(a.b * 255f);
			int num4 = (int)(a.a * 255f);
			int num5 = (int)(b.r * 255f);
			int num6 = (int)(b.g * 255f);
			int num7 = (int)(b.b * 255f);
			int num8 = (int)(b.a * 255f);
			return num == num5 && num2 == num6 && num3 == num7 && num4 == num8;
		}

		public static Quaternion QuaternionFromToVec(Vector3 toVector, Vector3 fromVector)
		{
			Vector3 vector = Vector3.Cross(fromVector, toVector);
			Debug.Log(vector);
			Debug.Log(vector.magnitude);
			Debug.Log(Vector3.Dot(fromVector, toVector) + 1f);
			Quaternion quaternion = new Quaternion(vector.x, vector.y, vector.z, 1f + Vector3.Dot(toVector, fromVector));
			Debug.Log(quaternion);
			Debug.Log(quaternion.eulerAngles);
			Debug.Log(quaternion.normalized);
			return quaternion.normalized;
		}

		public static Vector3 Position(this Matrix4x4 matrix)
		{
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}

		public static Vector3 Scale(this Matrix4x4 m)
		{
			Vector3 result = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
			if (Vector3.Cross(m.GetColumn(0), m.GetColumn(1)).normalized != m.GetColumn(2).normalized)
			{
				result.x *= -1f;
			}
			return result;
		}

		public static void SetLocalRelativeToParentMatrixWithParityAxis(this Matrix4x4 matrix, GTExt.ParityOptions parity = GTExt.ParityOptions.XFlip)
		{
		}

		public static void MultiplyInPlaceWith(this Vector3 a, in Vector3 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			a.z *= b.z;
		}

		public static void DecomposeWithXFlip(this Matrix4x4 matrix, out Vector3 transformation, out Quaternion rotation, out Vector3 scale)
		{
			Matrix4x4 matrix2 = matrix;
			bool flag = matrix2.ValidTRS();
			transformation = matrix2.Position();
			Quaternion quaternion;
			if (!flag)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				int num = 2;
				Vector3 forward = matrix2.GetColumnNoCopy(num);
				int num2 = 1;
				quaternion = Quaternion.LookRotation(forward, matrix2.GetColumnNoCopy(num2));
			}
			rotation = quaternion;
			Vector3 vector;
			if (!flag)
			{
				vector = Vector3.zero;
			}
			else
			{
				Matrix4x4 matrix4x = matrix;
				vector = matrix4x.lossyScale;
			}
			scale = vector;
		}

		public static void SetLocalMatrixRelativeToParentWithXParity(this Transform transform, in Matrix4x4 matrix4X4)
		{
			Vector3 localPosition;
			Quaternion localRotation;
			Vector3 localScale;
			matrix4X4.DecomposeWithXFlip(out localPosition, out localRotation, out localScale);
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			transform.localScale = localScale;
		}

		public static Matrix4x4 Matrix4x4Scale(in Vector3 vector)
		{
			Matrix4x4 result;
			result.m00 = vector.x;
			result.m01 = 0f;
			result.m02 = 0f;
			result.m03 = 0f;
			result.m10 = 0f;
			result.m11 = vector.y;
			result.m12 = 0f;
			result.m13 = 0f;
			result.m20 = 0f;
			result.m21 = 0f;
			result.m22 = vector.z;
			result.m23 = 0f;
			result.m30 = 0f;
			result.m31 = 0f;
			result.m32 = 0f;
			result.m33 = 1f;
			return result;
		}

		public static Vector4 GetColumnNoCopy(this Matrix4x4 matrix, in int index)
		{
			switch (index)
			{
			case 0:
				return new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30);
			case 1:
				return new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31);
			case 2:
				return new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32);
			case 3:
				return new Vector4(matrix.m03, matrix.m13, matrix.m23, matrix.m33);
			default:
				throw new IndexOutOfRangeException("Invalid column index!");
			}
		}

		public static Quaternion RotationWithScaleContext(this Matrix4x4 m, in Vector3 scale)
		{
			Matrix4x4 matrix4x = m * GTExt.Matrix4x4Scale(scale);
			int num = 2;
			Vector3 forward = matrix4x.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(forward, matrix4x.GetColumnNoCopy(num2));
		}

		public static Quaternion Rotation(this Matrix4x4 m)
		{
			int num = 2;
			Vector3 forward = m.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(forward, m.GetColumnNoCopy(num2));
		}

		public static Vector3 x0y(this Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		public static Vector3 x0y(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		public static Vector3 xy0(this Vector2 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		public static Vector3 xy0(this Vector3 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		public static Vector3 xz0(this Vector3 v)
		{
			return new Vector3(v.x, v.z, 0f);
		}

		public static Vector3 x0z(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		public static Matrix4x4 LocalMatrixRelativeToParentNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
		}

		public static Matrix4x4 LocalMatrixRelativeToParentWithScale(this Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.localToWorldMatrix;
			}
			return transform.parent.worldToLocalMatrix * transform.localToWorldMatrix;
		}

		public static void SetLocalMatrixRelativeToParent(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
			transform.localScale = matrix.Scale();
		}

		public static void SetLocalMatrixRelativeToParentNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
		}

		public static void SetLocalToWorldMatrixNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.Rotation();
		}

		public static Matrix4x4 localToWorldNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		}

		public static void SetLocalToWorldMatrixWithScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.rotation;
			transform.SetLossyScale(matrix.lossyScale);
		}

		public static Matrix4x4 Matrix4X4LerpNoScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.rotation, b.rotation, t), b.lossyScale);
		}

		public static Matrix4x4 LerpTo(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpNoScale(a, b, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNaN(this Vector3 v)
		{
			return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNan(this Quaternion q)
		{
			return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInfinity(this Vector3 v)
		{
			return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInfinity(this Quaternion q)
		{
			return float.IsInfinity(q.x) || float.IsInfinity(q.y) || float.IsInfinity(q.z) || float.IsInfinity(q.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ValuesInRange(this Vector3 v, in float maxVal)
		{
			return Mathf.Abs(v.x) < maxVal && Mathf.Abs(v.y) < maxVal && Mathf.Abs(v.z) < maxVal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(this Vector3 v, in float maxVal = 10000f)
		{
			return !v.IsNaN() && !v.IsInfinity() && v.ValuesInRange(maxVal);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetValidWithFallback(this Vector3 v, in Vector3 safeVal)
		{
			float num = 10000f;
			if (!v.IsValid(num))
			{
				return safeVal;
			}
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetValueSafe(this Vector3 v, in Vector3 newVal)
		{
			float num = 10000f;
			if (newVal.IsValid(num))
			{
				v = newVal;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(this Quaternion q)
		{
			return !q.IsNan() && !q.IsInfinity();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion GetValidWithFallback(this Quaternion q, in Quaternion safeVal)
		{
			if (!q.IsValid())
			{
				return safeVal;
			}
			return q;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetValueSafe(this Quaternion q, in Quaternion newVal)
		{
			if (newVal.IsValid())
			{
				q = newVal;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 ClampMagnitudeSafe(this Vector2 v2, float magnitude)
		{
			if (!float.IsFinite(v2.x))
			{
				v2.x = 0f;
			}
			if (!float.IsFinite(v2.y))
			{
				v2.y = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			return Vector2.ClampMagnitude(v2, magnitude);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClampThisMagnitudeSafe(this Vector2 v2, float magnitude)
		{
			if (!float.IsFinite(v2.x))
			{
				v2.x = 0f;
			}
			if (!float.IsFinite(v2.y))
			{
				v2.y = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			v2 = Vector2.ClampMagnitude(v2, magnitude);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ClampMagnitudeSafe(this Vector3 v3, float magnitude)
		{
			if (!float.IsFinite(v3.x))
			{
				v3.x = 0f;
			}
			if (!float.IsFinite(v3.y))
			{
				v3.y = 0f;
			}
			if (!float.IsFinite(v3.z))
			{
				v3.z = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			return Vector3.ClampMagnitude(v3, magnitude);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClampThisMagnitudeSafe(this Vector3 v3, float magnitude)
		{
			if (!float.IsFinite(v3.x))
			{
				v3.x = 0f;
			}
			if (!float.IsFinite(v3.y))
			{
				v3.y = 0f;
			}
			if (!float.IsFinite(v3.z))
			{
				v3.z = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			v3 = Vector3.ClampMagnitude(v3, magnitude);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MinSafe(this float value, float min)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			if (value >= min)
			{
				return min;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMinSafe(this float value, float min)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			value = ((value < min) ? value : min);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MinSafe(this double value, float min)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)min))
			{
				min = 0f;
			}
			if (value >= (double)min)
			{
				return (double)min;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMinSafe(this double value, float min)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)min))
			{
				min = 0f;
			}
			value = ((value < (double)min) ? value : ((double)min));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MaxSafe(this float value, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			if (value <= max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMaxSafe(this float value, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			value = ((value > max) ? value : max);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MaxSafe(this double value, float max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)max))
			{
				max = 0f;
			}
			if (value <= (double)max)
			{
				return (double)max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMaxSafe(this double value, float max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)max))
			{
				max = 0f;
			}
			value = ((value > (double)max) ? value : ((double)max));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ClampSafe(this float value, float min, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ClampSafe(this double value, double min, double max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite(min))
			{
				min = 0.0;
			}
			if (!double.IsFinite(max))
			{
				max = 0.0;
			}
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetFinite(this float value)
		{
			if (!float.IsFinite(value))
			{
				return 0f;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double GetFinite(this double value)
		{
			if (!double.IsFinite(value))
			{
				return 0.0;
			}
			return value;
		}

		public static Matrix4x4 Matrix4X4LerpHandleNegativeScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.Rotation(), b.Rotation(), t), b.lossyScale);
		}

		public static Matrix4x4 LerpTo_HandleNegativeScale(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpHandleNegativeScale(a, b, t);
		}

		public static Vector3 LerpToUnclamped(this Vector3 a, in Vector3 b, float t)
		{
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		public static string ToLongString(this Vector3 self)
		{
			return string.Format("[{0}, {1}, {2}]", self.x, self.y, self.z);
		}

		public static int GetRandomIndex<T>(this IReadOnlyList<T> self)
		{
			return Random.Range(0, self.Count);
		}

		public static T GetRandomItem<T>(this IReadOnlyList<T> self)
		{
			return self[self.GetRandomIndex<T>()];
		}

		public static Vector2 xx(this float v)
		{
			return new Vector2(v, v);
		}

		public static Vector2 xx(this Vector2 v)
		{
			return new Vector2(v.x, v.x);
		}

		public static Vector2 xy(this Vector2 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector2 yy(this Vector2 v)
		{
			return new Vector2(v.y, v.y);
		}

		public static Vector2 xx(this Vector3 v)
		{
			return new Vector2(v.x, v.x);
		}

		public static Vector2 xy(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector2 xz(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector2 yy(this Vector3 v)
		{
			return new Vector2(v.y, v.y);
		}

		public static Vector2 yz(this Vector3 v)
		{
			return new Vector2(v.y, v.z);
		}

		public static Vector2 zz(this Vector3 v)
		{
			return new Vector2(v.z, v.z);
		}

		public static Vector2 xx(this Vector4 v)
		{
			return new Vector2(v.x, v.x);
		}

		public static Vector2 xy(this Vector4 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector2 xz(this Vector4 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector2 xw(this Vector4 v)
		{
			return new Vector2(v.x, v.w);
		}

		public static Vector2 yy(this Vector4 v)
		{
			return new Vector2(v.y, v.y);
		}

		public static Vector2 yz(this Vector4 v)
		{
			return new Vector2(v.y, v.z);
		}

		public static Vector2 yw(this Vector4 v)
		{
			return new Vector2(v.y, v.w);
		}

		public static Vector2 zz(this Vector4 v)
		{
			return new Vector2(v.z, v.z);
		}

		public static Vector2 zw(this Vector4 v)
		{
			return new Vector2(v.z, v.w);
		}

		public static Vector2 ww(this Vector4 v)
		{
			return new Vector2(v.w, v.w);
		}

		public static Vector3 xxx(this float v)
		{
			return new Vector3(v, v, v);
		}

		public static Vector3 xxx(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		public static Vector3 xxy(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		public static Vector3 xyy(this Vector2 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		public static Vector3 yyy(this Vector2 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		public static Vector3 xxx(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		public static Vector3 xxy(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		public static Vector3 xxz(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		public static Vector3 xyy(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		public static Vector3 xyz(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static Vector3 xzz(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		public static Vector3 yyy(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		public static Vector3 yyz(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		public static Vector3 yzz(this Vector3 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		public static Vector3 zzz(this Vector3 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		public static Vector3 xxx(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		public static Vector3 xxy(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		public static Vector3 xxz(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		public static Vector3 xxw(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.w);
		}

		public static Vector3 xyy(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		public static Vector3 xyz(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static Vector3 xyw(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.w);
		}

		public static Vector3 xzz(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		public static Vector3 xzw(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.w);
		}

		public static Vector3 xww(this Vector4 v)
		{
			return new Vector3(v.x, v.w, v.w);
		}

		public static Vector3 yyy(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		public static Vector3 yyz(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		public static Vector3 yyw(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.w);
		}

		public static Vector3 yzz(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		public static Vector3 yzw(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.w);
		}

		public static Vector3 yww(this Vector4 v)
		{
			return new Vector3(v.y, v.w, v.w);
		}

		public static Vector3 zzz(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		public static Vector3 zzw(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.w);
		}

		public static Vector3 zww(this Vector4 v)
		{
			return new Vector3(v.z, v.w, v.w);
		}

		public static Vector3 www(this Vector4 v)
		{
			return new Vector3(v.w, v.w, v.w);
		}

		public static Vector4 xxxx(this float v)
		{
			return new Vector4(v, v, v, v);
		}

		public static Vector4 xxxx(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		public static Vector4 xxxy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		public static Vector4 xxyy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		public static Vector4 xyyy(this Vector2 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		public static Vector4 yyyy(this Vector2 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		public static Vector4 xxxx(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		public static Vector4 xxxy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		public static Vector4 xxxz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		public static Vector4 xxyy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		public static Vector4 xxyz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		public static Vector4 xxzz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		public static Vector4 xyyy(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		public static Vector4 xyyz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		public static Vector4 xyzz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		public static Vector4 xzzz(this Vector3 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		public static Vector4 yyyy(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		public static Vector4 yyyz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		public static Vector4 yyzz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		public static Vector4 yzzz(this Vector3 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		public static Vector4 zzzz(this Vector3 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		public static Vector4 xxxx(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		public static Vector4 xxxy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		public static Vector4 xxxz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		public static Vector4 xxxw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.w);
		}

		public static Vector4 xxyy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		public static Vector4 xxyz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		public static Vector4 xxyw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.w);
		}

		public static Vector4 xxzz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		public static Vector4 xxzw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.w);
		}

		public static Vector4 xxww(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.w, v.w);
		}

		public static Vector4 xyyy(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		public static Vector4 xyyz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		public static Vector4 xyyw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.w);
		}

		public static Vector4 xyzz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		public static Vector4 xyzw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		public static Vector4 xyww(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.w, v.w);
		}

		public static Vector4 xzzz(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		public static Vector4 xzzw(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.w);
		}

		public static Vector4 xzww(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.w, v.w);
		}

		public static Vector4 xwww(this Vector4 v)
		{
			return new Vector4(v.x, v.w, v.w, v.w);
		}

		public static Vector4 yyyy(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		public static Vector4 yyyz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		public static Vector4 yyyw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.w);
		}

		public static Vector4 yyzz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		public static Vector4 yyzw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.w);
		}

		public static Vector4 yyww(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.w, v.w);
		}

		public static Vector4 yzzz(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		public static Vector4 yzzw(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.w);
		}

		public static Vector4 yzww(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.w, v.w);
		}

		public static Vector4 ywww(this Vector4 v)
		{
			return new Vector4(v.y, v.w, v.w, v.w);
		}

		public static Vector4 zzzz(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		public static Vector4 zzzw(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.w);
		}

		public static Vector4 zzww(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.w, v.w);
		}

		public static Vector4 zwww(this Vector4 v)
		{
			return new Vector4(v.z, v.w, v.w, v.w);
		}

		public static Vector4 wwww(this Vector4 v)
		{
			return new Vector4(v.w, v.w, v.w, v.w);
		}

		public static Vector4 WithX(this Vector4 v, float x)
		{
			return new Vector4(x, v.y, v.z, v.w);
		}

		public static Vector4 WithY(this Vector4 v, float y)
		{
			return new Vector4(v.x, y, v.z, v.w);
		}

		public static Vector4 WithZ(this Vector4 v, float z)
		{
			return new Vector4(v.x, v.y, z, v.w);
		}

		public static Vector4 WithW(this Vector4 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		public static Vector3 WithX(this Vector3 v, float x)
		{
			return new Vector3(x, v.y, v.z);
		}

		public static Vector3 WithY(this Vector3 v, float y)
		{
			return new Vector3(v.x, y, v.z);
		}

		public static Vector3 WithZ(this Vector3 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		public static Vector4 WithW(this Vector3 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		public static Vector2 WithX(this Vector2 v, float x)
		{
			return new Vector2(x, v.y);
		}

		public static Vector2 WithY(this Vector2 v, float y)
		{
			return new Vector2(v.x, y);
		}

		public static Vector3 WithZ(this Vector2 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		public static bool IsShorterThan(this Vector2 v, float len)
		{
			return v.sqrMagnitude < len * len;
		}

		public static bool IsShorterThan(this Vector2 v, Vector2 v2)
		{
			return v.sqrMagnitude < v2.sqrMagnitude;
		}

		public static bool IsShorterThan(this Vector3 v, float len)
		{
			return v.sqrMagnitude < len * len;
		}

		public static bool IsShorterThan(this Vector3 v, Vector3 v2)
		{
			return v.sqrMagnitude < v2.sqrMagnitude;
		}

		public static bool IsLongerThan(this Vector2 v, float len)
		{
			return v.sqrMagnitude > len * len;
		}

		public static bool IsLongerThan(this Vector2 v, Vector2 v2)
		{
			return v.sqrMagnitude > v2.sqrMagnitude;
		}

		public static bool IsLongerThan(this Vector3 v, float len)
		{
			return v.sqrMagnitude > len * len;
		}

		public static bool IsLongerThan(this Vector3 v, Vector3 v2)
		{
			return v.sqrMagnitude > v2.sqrMagnitude;
		}

		public static Vector3 Normalize(this Vector3 value, out float existingMagnitude)
		{
			existingMagnitude = Vector3.Magnitude(value);
			if (existingMagnitude > 1E-05f)
			{
				return value / existingMagnitude;
			}
			return Vector3.zero;
		}

		public static Vector3 GetClosestPoint(this Ray ray, Vector3 target)
		{
			float d = Vector3.Dot(target - ray.origin, ray.direction);
			return ray.origin + ray.direction * d;
		}

		public static float GetClosestDistSqr(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).sqrMagnitude;
		}

		public static float GetClosestDistance(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).magnitude;
		}

		public static Vector3 ProjectToPlane(this Ray ray, Vector3 planeOrigin, Vector3 planeNormalMustBeLength1)
		{
			Vector3 rhs = planeOrigin - ray.origin;
			float d = Vector3.Dot(planeNormalMustBeLength1, rhs);
			float d2 = Vector3.Dot(planeNormalMustBeLength1, ray.direction);
			return ray.origin + ray.direction * d / d2;
		}

		public static Vector3 ProjectToLine(this Ray ray, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 normalized = (lineEnd - lineStart).normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.Cross(ray.direction, normalized), normalized).normalized;
			return ray.ProjectToPlane(lineStart, normalized2);
		}

		public static bool IsNull(this Object mono)
		{
			return mono == null || !mono;
		}

		public static bool IsNotNull(this Object mono)
		{
			return !mono.IsNull();
		}

		public static string GetPath(this Transform transform)
		{
			string text = transform.name;
			while (transform.parent)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "/" + text;
		}

		public static string GetPathQ(this Transform transform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				transform.GetPathQ(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public static void GetPathQ(this Transform transform, ref Utf16ValueStringBuilder sb)
		{
			sb.Append("\"");
			int length = sb.Length;
			do
			{
				if (sb.Length > length)
				{
					sb.Insert(length, "/");
				}
				sb.Insert(length, transform.name);
				transform = transform.parent;
			}
			while (transform != null);
			sb.Append("\"");
		}

		public static string GetPath(this Transform transform, int maxDepth)
		{
			string text = transform.name;
			int num = 0;
			while (transform.parent && num < maxDepth)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
				num++;
			}
			return "/" + text;
		}

		public static string GetPath(this Transform transform, Transform stopper)
		{
			string text = transform.name;
			while (transform.parent && transform.parent != stopper)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "/" + text;
		}

		public static string GetPath(this GameObject gameObject)
		{
			return gameObject.transform.GetPath();
		}

		public static void GetPath(this GameObject gameObject, ref Utf16ValueStringBuilder sb)
		{
			gameObject.transform.GetPathQ(ref sb);
		}

		public static string GetPath(this GameObject gameObject, int limit)
		{
			return gameObject.transform.GetPath(limit);
		}

		public static string[] GetPaths(this GameObject[] gobj)
		{
			string[] array = new string[gobj.Length];
			for (int i = 0; i < gobj.Length; i++)
			{
				array[i] = gobj[i].GetPath();
			}
			return array;
		}

		public static string[] GetPaths(this Transform[] xform)
		{
			string[] array = new string[xform.Length];
			for (int i = 0; i < xform.Length; i++)
			{
				array[i] = xform[i].GetPath();
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetRelativePath(string fromPath, string toPath, ref Utf16ValueStringBuilder ZStringBuilder)
		{
			if (string.IsNullOrEmpty(fromPath) || string.IsNullOrEmpty(toPath))
			{
				return;
			}
			int num = 0;
			while (num < fromPath.Length && fromPath[num] == '/')
			{
				num++;
			}
			int num2 = 0;
			while (num2 < toPath.Length && toPath[num2] == '/')
			{
				num2++;
			}
			int num3 = -1;
			int num4 = Mathf.Min(fromPath.Length - num, toPath.Length - num2);
			bool flag = true;
			for (int i = 0; i < num4; i++)
			{
				if (fromPath[num + i] != toPath[num2 + i])
				{
					flag = false;
					break;
				}
				if (fromPath[num + i] == '/')
				{
					num3 = i;
				}
			}
			if (flag && fromPath.Length - num > num4)
			{
				flag = (fromPath[num + num4] == '/');
			}
			else if (flag && toPath.Length - num2 > num4)
			{
				flag = (toPath[num2 + num4] == '/');
			}
			num3 = (flag ? num4 : num3);
			int num5 = (num3 < fromPath.Length - num) ? (num3 + 1) : (fromPath.Length - num);
			int num6 = (num3 < toPath.Length - num2) ? (num3 + 1) : (toPath.Length - num2);
			if (num5 < fromPath.Length - num)
			{
				ZStringBuilder.Append("../");
				for (int j = num5; j < fromPath.Length - num; j++)
				{
					if (fromPath[num + j] == '/')
					{
						ZStringBuilder.Append("../");
					}
				}
			}
			else
			{
				ZStringBuilder.Append((toPath.Length - num2 - num6 > 0) ? "./" : ".");
			}
			ZStringBuilder.Append(toPath, num2 + num6, toPath.Length - (num2 + num6));
		}

		public static string GetRelativePath(string fromPath, string toPath)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				GTExt.GetRelativePath(fromPath, toPath, ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
				utf16ValueStringBuilder.Dispose();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetRelativePath(this Transform fromXform, Transform toXform, ref Utf16ValueStringBuilder ZStringBuilder)
		{
			GTExt.GetRelativePath(fromXform.GetPath(), toXform.GetPath(), ref ZStringBuilder);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRelativePath(this Transform fromXform, Transform toXform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				fromXform.GetRelativePath(toXform, ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
				utf16ValueStringBuilder.Dispose();
			}
			return result;
		}

		public static void GetPathWithSiblingIndexes(this Transform transform, ref Utf16ValueStringBuilder strBuilder)
		{
			int length = strBuilder.Length;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		public static string GetComponentPath(this Component component, int maxDepth = 2147483647)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPath(ref utf16ValueStringBuilder, maxDepth);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public static string GetComponentPath<T>(this T component, int maxDepth = 2147483647) where T : Component
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPath(ref utf16ValueStringBuilder, maxDepth);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public static void GetComponentPath<T>(this T component, ref Utf16ValueStringBuilder strBuilder, int maxDepth = 2147483647) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			if (maxDepth > 0)
			{
				strBuilder.Append("/");
			}
			strBuilder.Append("->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			if (maxDepth <= 0)
			{
				return;
			}
			int num = 0;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				num++;
				if (maxDepth <= num)
				{
					break;
				}
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		public static void GetComponentPathWithSiblingIndexes<T>(this T component, ref Utf16ValueStringBuilder strBuilder) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			strBuilder.Append("/->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		public static string GetComponentPathWithSiblingIndexes<T>(this T component) where T : Component
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPathWithSiblingIndexes(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public static T GetComponentByPath<T>(this GameObject root, string path) where T : Component
		{
			string[] array = path.Split(new string[]
			{
				"/->/"
			}, StringSplitOptions.None);
			if (array.Length < 2)
			{
				return default(T);
			}
			string[] array2 = array[0].Split(new string[]
			{
				"/"
			}, StringSplitOptions.RemoveEmptyEntries);
			Transform transform = root.transform;
			for (int i = 1; i < array2.Length; i++)
			{
				string n = array2[i];
				transform = transform.Find(n);
				if (transform == null)
				{
					return default(T);
				}
			}
			Type type = Type.GetType(array[1].Split('#', StringSplitOptions.None)[0]);
			if (type == null)
			{
				return default(T);
			}
			Component component = transform.GetComponent(type);
			if (component == null)
			{
				return default(T);
			}
			return component as T;
		}

		public static int GetDepth(this Transform xform)
		{
			int num = 0;
			Transform parent = xform.parent;
			while (parent != null)
			{
				num++;
				parent = parent.parent;
			}
			return num;
		}

		public static string GetPathWithSiblingIndexes(this Transform transform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				transform.GetPathWithSiblingIndexes(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		public static void GetPathWithSiblingIndexes(this GameObject gameObject, ref Utf16ValueStringBuilder stringBuilder)
		{
			gameObject.transform.GetPathWithSiblingIndexes(ref stringBuilder);
		}

		public static string GetPathWithSiblingIndexes(this GameObject gameObject)
		{
			return gameObject.transform.GetPathWithSiblingIndexes();
		}

		public static void SetFromMatrix(this Transform transform, Matrix4x4 matrix, bool useLocal = false)
		{
			if (useLocal)
			{
				transform.localPosition = matrix.GetPosition();
				transform.localRotation = matrix.rotation;
				transform.localScale = matrix.lossyScale;
				return;
			}
			transform.position = matrix.GetPosition();
			transform.rotation = matrix.rotation;
			transform.SetScaleFromMatrix(matrix);
		}

		public static void SetScale(this Transform transform, Vector3 scale)
		{
			if (transform.parent)
			{
				transform.localScale = (transform.parent.worldToLocalMatrix * Matrix4x4.TRS(transform.position, transform.rotation, scale)).lossyScale;
				return;
			}
			transform.localScale = scale;
		}

		public static void SetScaleFromMatrix(this Transform transform, Matrix4x4 matrix)
		{
			if (transform.parent)
			{
				transform.localScale = (transform.parent.worldToLocalMatrix * matrix).lossyScale;
				return;
			}
			transform.localScale = matrix.lossyScale;
		}

		public static void AddDictValue(Transform xForm, Dictionary<string, Transform> dict)
		{
			GTExt.caseSenseInner.Add(xForm, dict);
		}

		public static void ClearDicts()
		{
			GTExt.caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
			GTExt.caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
		}

		public static bool TryFindByExactPath([NotNull] string path, out Transform result, FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			if (findObjectsInactive != FindObjectsInactive.Exclude)
			{
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded && sceneAt.TryFindByExactPath(path, out result))
					{
						return true;
					}
				}
				result = null;
				return false;
			}
			if (path[0] != '/')
			{
				path = "/" + path;
			}
			GameObject gameObject = GameObject.Find(path);
			if (gameObject)
			{
				result = gameObject.transform;
				return true;
			}
			result = null;
			return false;
		}

		public static bool TryFindByExactPath(this Scene scene, string path, out Transform result)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			return scene.TryFindByExactPath(splitPath, out result);
		}

		private static bool TryFindByExactPath(this Scene scene, IReadOnlyList<string> splitPath, out Transform result)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (GTExt.TryFindByExactPath_Internal(rootGameObjects[i].transform, splitPath, 0, out result))
				{
					return true;
				}
			}
			result = null;
			return false;
		}

		public static bool TryFindByExactPath(this Transform rootXform, string path, out Transform result)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			using (IEnumerator enumerator = rootXform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, 0, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		public static bool TryFindByExactPath(this Transform rootXform, IReadOnlyList<string> splitPath, out Transform result)
		{
			using (IEnumerator enumerator = rootXform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, 0, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		private static bool TryFindByExactPath_Internal(Transform current, IReadOnlyList<string> splitPath, int index, out Transform result)
		{
			if (current.name != splitPath[index])
			{
				result = null;
				return false;
			}
			if (index == splitPath.Count - 1)
			{
				result = current;
				return true;
			}
			using (IEnumerator enumerator = current.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, index + 1, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		public static bool TryFindByPath(string globPath, out Transform result, bool caseSensitive = false)
		{
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return GTExt._TryFindByPath(null, pathPartsRegex, -1, out result, caseSensitive, true, globPath);
		}

		public static bool TryFindByPath(this Scene scene, string globPath, out Transform result, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return scene.TryFindByPath(pathPartsRegex, out result, globPath, caseSensitive);
		}

		private static bool TryFindByPath(this Scene scene, IReadOnlyList<string> pathPartsRegex, out Transform result, string globPath, bool caseSensitive = false)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (GTExt._TryFindByPath(rootGameObjects[i].transform, pathPartsRegex, 0, out result, caseSensitive, false, globPath))
				{
					return true;
				}
			}
			result = null;
			return false;
		}

		public static bool TryFindByPath(this Transform rootXform, string globPath, out Transform result, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
			}
			char c = globPath[0];
			if (c != ' ' && c != '\n' && c != '\t')
			{
				c = globPath[globPath.Length - 1];
				if (c != ' ' && c != '\n' && c != '\t')
				{
					string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
					return GTExt._TryFindByPath(rootXform, pathPartsRegex, -1, out result, caseSensitive, false, globPath);
				}
			}
			throw new Exception("TryFindByPath: Provided globPath cannot end or start with whitespace.\nProvided globPath=\"" + globPath + "\"");
		}

		public static List<string> ShowAllStringsUsed()
		{
			return GTExt.allStringsUsed.Keys.ToList<string>();
		}

		private static bool _TryFindByPath(Transform current, IReadOnlyList<string> pathPartsRegex, int index, out Transform result, bool caseSensitive, bool isAtSceneLevel, string joinedPath)
		{
			if (joinedPath != null && !GTExt.allStringsUsed.ContainsKey(joinedPath))
			{
				GTExt.allStringsUsed[joinedPath] = joinedPath;
			}
			if (caseSensitive)
			{
				if (GTExt.caseSenseInner.ContainsKey(current))
				{
					if (GTExt.caseSenseInner[current].ContainsKey(joinedPath))
					{
						result = GTExt.caseSenseInner[current][joinedPath];
						return true;
					}
				}
				else
				{
					GTExt.caseSenseInner[current] = new Dictionary<string, Transform>();
				}
			}
			else if (GTExt.caseInsenseInner.ContainsKey(current))
			{
				if (GTExt.caseInsenseInner[current].ContainsKey(joinedPath))
				{
					result = GTExt.caseInsenseInner[current][joinedPath];
					return true;
				}
			}
			else
			{
				GTExt.caseInsenseInner[current] = new Dictionary<string, Transform>();
			}
			string a;
			if (isAtSceneLevel)
			{
				index = ((index == -1) ? 0 : index);
				a = pathPartsRegex[index];
				if (a == ".." || a == "..**" || a == "**..")
				{
					result = null;
					return false;
				}
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
						for (int j = 0; j < rootGameObjects.Length; j++)
						{
							if (GTExt._TryFindByPath(rootGameObjects[j].transform, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
							{
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
						}
					}
				}
			}
			if (index != -1)
			{
				a = pathPartsRegex[index];
				if (!(a == "."))
				{
					if (!(a == ".."))
					{
						if (a == "**")
						{
							goto IL_50A;
						}
						if (!(a == "..**") && !(a == "**.."))
						{
							if (!Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
							{
								goto IL_8CB;
							}
							if (index == pathPartsRegex.Count - 1)
							{
								result = current;
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
							using (IEnumerator enumerator = current.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index + 1, out result, caseSensitive, false, joinedPath))
									{
										if (caseSensitive)
										{
											GTExt.caseSenseInner[current][joinedPath] = result;
										}
										else
										{
											GTExt.caseInsenseInner[current][joinedPath] = result;
										}
										return true;
									}
								}
							}
							goto IL_8CB;
						}
						else
						{
							string a2;
							do
							{
								index++;
								if (index >= pathPartsRegex.Count)
								{
									break;
								}
								a2 = pathPartsRegex[index];
							}
							while (a2 == "..**" || a2 == "**..");
							if (index == pathPartsRegex.Count)
							{
								result = current.root;
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
							Transform parent = current.parent;
							while (parent)
							{
								if (GTExt._TryFindByPath(parent, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
								{
									if (caseSensitive)
									{
										GTExt.caseSenseInner[current][joinedPath] = result;
									}
									else
									{
										GTExt.caseInsenseInner[current][joinedPath] = result;
									}
									return true;
								}
								using (IEnumerator enumerator = parent.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
										{
											if (caseSensitive)
											{
												GTExt.caseSenseInner[current][joinedPath] = result;
											}
											else
											{
												GTExt.caseInsenseInner[current][joinedPath] = result;
											}
											return true;
										}
									}
								}
								parent = parent.parent;
							}
							if (parent != null)
							{
								goto IL_8CB;
							}
							bool result2 = GTExt._TryFindByPath(current.root, pathPartsRegex, index, out result, caseSensitive, true, joinedPath);
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
								return result2;
							}
							GTExt.caseInsenseInner[current][joinedPath] = result;
							return result2;
						}
					}
				}
				else
				{
					while (pathPartsRegex[index] == ".")
					{
						if (index == pathPartsRegex.Count - 1)
						{
							result = current;
							return true;
						}
						index++;
					}
					if (GTExt._TryFindByPath(current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
					{
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
					using (IEnumerator enumerator = current.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
							{
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
						}
						goto IL_8CB;
					}
				}
				Transform transform = current;
				int num = index;
				while (pathPartsRegex[num] == "..")
				{
					if (num + 1 >= pathPartsRegex.Count)
					{
						result = transform.parent;
						return result != null;
					}
					if (transform.parent == null)
					{
						bool result3 = GTExt._TryFindByPath(transform, pathPartsRegex, num + 1, out result, caseSensitive, true, joinedPath);
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
							return result3;
						}
						GTExt.caseInsenseInner[current][joinedPath] = result;
						return result3;
					}
					else
					{
						transform = transform.parent;
						num++;
					}
				}
				using (IEnumerator enumerator = transform.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, num, out result, caseSensitive, false, joinedPath))
						{
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
							}
							else
							{
								GTExt.caseInsenseInner[current][joinedPath] = result;
							}
							return true;
						}
					}
					goto IL_8CB;
				}
				IL_50A:
				if (index == pathPartsRegex.Count - 1)
				{
					result = ((current.childCount > 0) ? current.GetChild(0) : null);
					return current.childCount > 0;
				}
				if (index <= pathPartsRegex.Count - 1 && Regex.IsMatch(current.name, pathPartsRegex[index + 1], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
				{
					if (index + 2 == pathPartsRegex.Count)
					{
						result = current;
						return true;
					}
					using (IEnumerator enumerator = current.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
							{
								return true;
							}
						}
					}
				}
				Transform transform2;
				if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out transform2, caseSensitive))
				{
					if (index + 2 == pathPartsRegex.Count)
					{
						result = transform2;
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
					if (GTExt._TryFindByPath(transform2, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
					{
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
				}
				IL_8CB:
				result = null;
				if (caseSensitive)
				{
					GTExt.caseSenseInner[current][joinedPath] = result;
				}
				else
				{
					GTExt.caseInsenseInner[current][joinedPath] = result;
				}
				return false;
			}
			if (pathPartsRegex.Count == 0)
			{
				result = null;
				return false;
			}
			a = pathPartsRegex[0];
			if (!(a == ".") && !(a == "..") && !(a == "..**") && !(a == "**.."))
			{
				using (IEnumerator enumerator = current.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath))
						{
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
							}
							else
							{
								GTExt.caseInsenseInner[current][joinedPath] = result;
							}
							return true;
						}
					}
				}
				result = null;
				if (caseSensitive)
				{
					GTExt.caseSenseInner[current][joinedPath] = result;
				}
				else
				{
					GTExt.caseInsenseInner[current][joinedPath] = result;
				}
				return false;
			}
			bool result4 = GTExt._TryFindByPath(current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath);
			if (caseSensitive)
			{
				GTExt.caseSenseInner[current][joinedPath] = result;
				return result4;
			}
			GTExt.caseInsenseInner[current][joinedPath] = result;
			return result4;
		}

		private static bool _TryBreadthFirstSearchNames(Transform root, string regexPattern, out Transform result, bool caseSensitive)
		{
			Queue<Transform> queue = new Queue<Transform>();
			using (IEnumerator enumerator = root.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform item = (Transform)obj;
					queue.Enqueue(item);
				}
				goto IL_9B;
			}
			IL_3D:
			Transform transform = queue.Dequeue();
			if (Regex.IsMatch(transform.name, regexPattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
			{
				result = transform;
				return true;
			}
			foreach (object obj2 in transform)
			{
				Transform item2 = (Transform)obj2;
				queue.Enqueue(item2);
			}
			IL_9B:
			if (queue.Count <= 0)
			{
				result = null;
				return false;
			}
			goto IL_3D;
		}

		public static T[] FindComponentsByExactPath<T>(string path) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						list.AddRange(sceneAt.FindComponentsByExactPath(path));
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		public static T[] FindComponentsByExactPath<T>(this Scene scene, string path) where T : Component
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			return scene.FindComponentsByExactPath(splitPath);
		}

		private static T[] FindComponentsByExactPath<T>(this Scene scene, string[] splitPath) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					GTExt._FindComponentsByExactPath<T>(rootGameObjects[i].transform, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string path) where T : Component
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				foreach (object obj in rootXform)
				{
					GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string[] splitPath) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				foreach (object obj in rootXform)
				{
					GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		private static void _FindComponentsByExactPath<T>(Transform current, string[] splitPath, int index, List<T> components) where T : Component
		{
			if (current.name != splitPath[index])
			{
				return;
			}
			if (index == splitPath.Length - 1)
			{
				T component = current.GetComponent<T>();
				if (component)
				{
					components.Add(component);
				}
				return;
			}
			foreach (object obj in current)
			{
				GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, index + 1, components);
			}
		}

		public static T[] FindComponentsByPathInLoadedScenes<T>(string wildcardPath, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(wildcardPath);
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
						for (int j = 0; j < rootGameObjects.Length; j++)
						{
							GTExt._FindComponentsByPath<T>(rootGameObjects[j].transform, pathPartsRegex, list, caseSensitive);
						}
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		public static T[] FindComponentsByPath<T>(this Scene scene, string globPath, bool caseSensitive = false) where T : Component
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return scene.FindComponentsByPath(pathPartsRegex, caseSensitive);
		}

		private static T[] FindComponentsByPath<T>(this Scene scene, string[] pathPartsRegex, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					GTExt._FindComponentsByPath<T>(rootGameObjects[i].transform, pathPartsRegex, list, caseSensitive);
				}
				result = list.ToArray();
			}
			return result;
		}

		public static T[] FindComponentsByPath<T>(this Transform rootXform, string globPath, bool caseSensitive = false) where T : Component
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return rootXform.FindComponentsByPath(pathPartsRegex, caseSensitive);
		}

		public static T[] FindComponentsByPath<T>(this Transform rootXform, string[] pathPartsRegex, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GTExt._FindComponentsByPath<T>(rootXform, pathPartsRegex, list, caseSensitive);
				result = list.ToArray();
			}
			return result;
		}

		public static void _FindComponentsByPath<T>(Transform current, string[] pathPartsRegex, List<T> components, bool caseSensitive) where T : Component
		{
			List<Transform> list;
			using (UnityEngine.Pool.CollectionPool<List<Transform>, Transform>.Get(out list))
			{
				list.EnsureCapacity(64);
				if (GTExt._TryFindAllByPath(current, pathPartsRegex, 0, list, caseSensitive, false))
				{
					for (int i = 0; i < list.Count; i++)
					{
						T[] components2 = list[i].GetComponents<T>();
						components.AddRange(components2);
					}
				}
			}
		}

		private static bool _TryFindAllByPath(Transform current, IReadOnlyList<string> pathPartsRegex, int index, List<Transform> results, bool caseSensitive, bool isAtSceneLevel = false)
		{
			bool flag = false;
			string a;
			if (isAtSceneLevel)
			{
				a = pathPartsRegex[index];
				if (a == ".." || a == "..**" || a == "**..")
				{
					return false;
				}
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						foreach (GameObject gameObject in sceneAt.GetRootGameObjects())
						{
							flag |= GTExt._TryFindAllByPath(gameObject.transform, pathPartsRegex, index, results, caseSensitive, false);
						}
					}
				}
			}
			a = pathPartsRegex[index];
			if (!(a == "."))
			{
				if (!(a == ".."))
				{
					Transform transform2;
					if (!(a == "**"))
					{
						if (!(a == "..**") && !(a == "**.."))
						{
							if (Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
							{
								if (index == pathPartsRegex.Count - 1)
								{
									results.Add(current);
									return true;
								}
								foreach (object obj in current)
								{
									Transform current2 = (Transform)obj;
									flag |= GTExt._TryFindAllByPath(current2, pathPartsRegex, index + 1, results, caseSensitive, false);
								}
							}
						}
						else
						{
							int k;
							for (k = index + 1; k < pathPartsRegex.Count; k++)
							{
								string a2 = pathPartsRegex[k];
								if (!(a2 == "..**") && !(a2 == "**.."))
								{
									break;
								}
							}
							if (k == pathPartsRegex.Count)
							{
								results.Add(current.root);
								return true;
							}
							Transform transform = current;
							while (transform)
							{
								flag |= GTExt._TryFindAllByPath(transform, pathPartsRegex, index + 1, results, caseSensitive, false);
								transform = transform.parent;
							}
						}
					}
					else if (index == pathPartsRegex.Count - 1)
					{
						for (int l = 0; l < current.childCount; l++)
						{
							results.Add(current.GetChild(l));
							flag = true;
						}
					}
					else if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out transform2, caseSensitive))
					{
						if (index + 2 == pathPartsRegex.Count)
						{
							results.Add(transform2);
							return true;
						}
						flag |= GTExt._TryFindAllByPath(transform2, pathPartsRegex, index + 2, results, caseSensitive, false);
					}
				}
				else if (current.parent)
				{
					if (index == pathPartsRegex.Count - 1)
					{
						results.Add(current.parent);
						return true;
					}
					flag |= GTExt._TryFindAllByPath(current.parent, pathPartsRegex, index + 1, results, caseSensitive, false);
				}
			}
			else
			{
				if (index == pathPartsRegex.Count - 1)
				{
					results.Add(current);
					return true;
				}
				flag |= GTExt._TryFindAllByPath(current, pathPartsRegex, index + 1, results, caseSensitive, false);
			}
			return flag;
		}

		public static string[] _GlobPathToPathPartsRegex(string path)
		{
			string[] array = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0)
				{
					string a = array[i];
					if (a == "**" || a == "..**" || a == "**..")
					{
						a = array[i - 1];
						if (a == "**" || a == "..**" || a == "**..")
						{
							num++;
						}
					}
				}
				array[i - num] = array[i];
			}
			if (num > 0)
			{
				Array.Resize<string>(ref array, array.Length - num);
			}
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = GTExt._GlobPathPartToRegex(array[j]);
			}
			return array;
		}

		private static string _GlobPathPartToRegex(string pattern)
		{
			if (pattern == "." || pattern == ".." || pattern == "**" || pattern == "..**" || pattern == "**.." || pattern.StartsWith("^"))
			{
				return pattern;
			}
			return "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
		}

		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass7_0<T, TStop1> A_2) where T : Component where TStop1 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(transform, ref components, ref A_2);
				}
			}
		}

		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass10_0<T, TStop1, TStop2> A_2) where T : Component where TStop1 : Component where TStop2 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null) && !(transform.GetComponent<TStop2>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(transform, ref components, ref A_2);
				}
			}
		}

		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass11_0<T, TStop1, TStop2, TStop3> A_2) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null) && !(transform.GetComponent<TStop2>() != null) && !(transform.GetComponent<TStop3>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(transform, ref components, ref A_2);
				}
			}
		}

		private static Dictionary<Transform, Dictionary<string, Transform>> caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();

		private static Dictionary<Transform, Dictionary<string, Transform>> caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();

		public static Dictionary<string, string> allStringsUsed = new Dictionary<string, string>();

		public enum ParityOptions
		{
			XFlip,
			YFlip,
			ZFlip,
			AllFlip
		}
	}
}
