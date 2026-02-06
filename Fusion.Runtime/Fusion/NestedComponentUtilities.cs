using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion
{
	public static class NestedComponentUtilities
	{
		public static T EnsureRootComponentExists<T, TStopOn>(this Transform transform) where T : Component where TStopOn : Component
		{
			TStopOn parentComponent = transform.GetParentComponent<TStopOn>();
			bool flag = parentComponent;
			T result;
			if (flag)
			{
				T t;
				bool flag2 = parentComponent.TryGetComponent<T>(out t);
				if (flag2)
				{
					result = t;
				}
				else
				{
					result = parentComponent.gameObject.AddComponent<T>();
				}
			}
			else
			{
				result = default(T);
			}
			return result;
		}

		public static T GetParentComponent<T>(this Transform t) where T : Component
		{
			T t2;
			bool flag = t.TryGetComponent<T>(out t2);
			T result;
			if (flag)
			{
				result = t2;
			}
			else
			{
				Transform parent = t.parent;
				while (parent)
				{
					T result2;
					bool flag2 = parent.TryGetComponent<T>(out result2);
					if (flag2)
					{
						return result2;
					}
					parent = parent.parent;
				}
				result = default(T);
			}
			return result;
		}

		public static void GetNestedComponentsInParents<T>(this Transform t, List<T> list) where T : Component
		{
			list.Clear();
			while (t != null)
			{
				T item;
				bool flag = t.TryGetComponent<T>(out item);
				if (flag)
				{
					list.Add(item);
				}
				t = t.parent;
			}
		}

		public static T GetNestedComponentInChildren<T, TStopOn>(this Transform t, bool includeInactive) where T : class where TStopOn : class
		{
			T t2;
			bool flag = t.TryGetComponent<T>(out t2);
			T result;
			if (flag)
			{
				result = t2;
			}
			else
			{
				NestedComponentUtilities.nodesQueue.Clear();
				NestedComponentUtilities.nodesQueue.Enqueue(t);
				while (NestedComponentUtilities.nodesQueue.Count > 0)
				{
					Transform transform = NestedComponentUtilities.nodesQueue.Dequeue();
					int i = 0;
					int childCount = transform.childCount;
					while (i < childCount)
					{
						Transform child = transform.GetChild(i);
						bool flag2 = !includeInactive && !child.gameObject.activeSelf;
						if (!flag2)
						{
							TStopOn tstopOn;
							bool flag3 = child.TryGetComponent<TStopOn>(out tstopOn);
							if (!flag3)
							{
								T result2;
								bool flag4 = child.TryGetComponent<T>(out result2);
								if (flag4)
								{
									return result2;
								}
								NestedComponentUtilities.nodesQueue.Enqueue(child);
							}
						}
						i++;
					}
				}
				result = default(T);
			}
			return result;
		}

		public static T GetNestedComponentInParent<T, TStopOn>(this Transform t) where T : class where TStopOn : class
		{
			Transform transform = t;
			T result;
			for (;;)
			{
				bool flag = transform.TryGetComponent<T>(out result);
				if (flag)
				{
					break;
				}
				TStopOn tstopOn;
				bool flag2 = transform.TryGetComponent<TStopOn>(out tstopOn);
				if (flag2)
				{
					goto Block_2;
				}
				transform = transform.parent;
				if (transform == null)
				{
					goto Block_3;
				}
			}
			return result;
			Block_2:
			return default(T);
			Block_3:
			return default(T);
		}

		public static T GetNestedComponentInParents<T, TStopOn>(this Transform t) where T : class where TStopOn : class
		{
			T t2;
			bool flag = t.TryGetComponent<T>(out t2);
			T result;
			if (flag)
			{
				result = t2;
			}
			else
			{
				Transform parent = t.parent;
				while (parent)
				{
					T result2;
					bool flag2 = parent.TryGetComponent<T>(out result2);
					if (flag2)
					{
						return result2;
					}
					TStopOn tstopOn;
					bool flag3 = parent.TryGetComponent<TStopOn>(out tstopOn);
					if (flag3)
					{
						return default(T);
					}
					parent = parent.parent;
				}
				result = default(T);
			}
			return result;
		}

		public static void GetNestedComponentsInParents<T, TStop>(this Transform t, List<T> list) where T : class where TStop : class
		{
			t.GetComponents<T>(list);
			TStop tstop;
			bool flag = t.TryGetComponent<TStop>(out tstop);
			if (!flag)
			{
				Transform parent = t.parent;
				bool flag2 = parent == null;
				if (!flag2)
				{
					NestedComponentUtilities.nodeStack.Clear();
					bool flag4;
					do
					{
						NestedComponentUtilities.nodeStack.Push(parent);
						bool flag3 = parent.TryGetComponent<TStop>(out tstop);
						if (flag3)
						{
							break;
						}
						parent = parent.parent;
						flag4 = (parent == null);
					}
					while (!flag4);
					bool flag5 = NestedComponentUtilities.nodeStack.Count == 0;
					if (!flag5)
					{
						List<T> list2 = NestedComponentUtilities.RecyclableList<T>.List;
						try
						{
							while (NestedComponentUtilities.nodeStack.Count > 0)
							{
								Transform transform = NestedComponentUtilities.nodeStack.Pop();
								transform.GetComponents<T>(list2);
								list.AddRange(list2);
							}
						}
						finally
						{
							list2.Clear();
						}
					}
				}
			}
		}

		public static List<T> GetNestedComponentsInChildren<T, TStopOn>(this Transform t, List<T> list, bool includeInactive = true) where T : class where TStopOn : class
		{
			NestedComponentUtilities.nodesQueue.Clear();
			bool flag = list == null;
			if (flag)
			{
				list = new List<T>();
			}
			t.GetComponents<T>(list);
			int i = 0;
			int childCount = t.childCount;
			while (i < childCount)
			{
				Transform child = t.GetChild(i);
				bool flag2 = !includeInactive && !child.gameObject.activeSelf;
				if (!flag2)
				{
					TStopOn tstopOn;
					bool flag3 = child.TryGetComponent<TStopOn>(out tstopOn);
					if (!flag3)
					{
						NestedComponentUtilities.nodesQueue.Enqueue(child);
					}
				}
				i++;
			}
			List<T> list2 = NestedComponentUtilities.RecyclableList<T>.List;
			try
			{
				while (NestedComponentUtilities.nodesQueue.Count > 0)
				{
					Transform transform = NestedComponentUtilities.nodesQueue.Dequeue();
					transform.GetComponents<T>(list2);
					list.AddRange(list2);
					int j = 0;
					int childCount2 = transform.childCount;
					while (j < childCount2)
					{
						Transform child2 = transform.GetChild(j);
						bool flag4 = !includeInactive && !child2.gameObject.activeSelf;
						if (!flag4)
						{
							TStopOn tstopOn;
							bool flag5 = child2.TryGetComponent<TStopOn>(out tstopOn);
							if (!flag5)
							{
								NestedComponentUtilities.nodesQueue.Enqueue(child2);
							}
						}
						j++;
					}
				}
			}
			finally
			{
				list2.Clear();
			}
			return list;
		}

		public static List<T> GetNestedComponentsInChildren<T>(this Transform t, List<T> list, bool includeInactive = true, params Type[] stopOn) where T : class
		{
			NestedComponentUtilities.nodesQueue.Clear();
			t.GetComponents<T>(list);
			int i = 0;
			int childCount = t.childCount;
			while (i < childCount)
			{
				Transform child = t.GetChild(i);
				bool flag = !includeInactive && !child.gameObject.activeSelf;
				if (!flag)
				{
					int j = 0;
					int num = stopOn.Length;
					while (j < num)
					{
						Component component;
						bool flag2 = child.TryGetComponent(stopOn[j], out component);
						if (flag2)
						{
						}
						j++;
					}
					NestedComponentUtilities.nodesQueue.Enqueue(child);
				}
				i++;
			}
			List<T> list2 = NestedComponentUtilities.RecyclableList<T>.List;
			try
			{
				while (NestedComponentUtilities.nodesQueue.Count > 0)
				{
					Transform transform = NestedComponentUtilities.nodesQueue.Dequeue();
					transform.GetComponents<T>(list2);
					list.AddRange(list2);
					int k = 0;
					int childCount2 = transform.childCount;
					while (k < childCount2)
					{
						Transform child2 = transform.GetChild(k);
						bool flag3 = !includeInactive && !child2.gameObject.activeSelf;
						if (!flag3)
						{
							int l = 0;
							int num2 = stopOn.Length;
							while (l < num2)
							{
								Component component;
								bool flag4 = child2.TryGetComponent(stopOn[l], out component);
								if (flag4)
								{
								}
								l++;
							}
							NestedComponentUtilities.nodesQueue.Enqueue(child2);
						}
						k++;
					}
				}
			}
			finally
			{
				list2.Clear();
			}
			return list;
		}

		public static void GetNestedComponentsInChildren<T, TSearch, TStop>(this Transform t, bool includeInactive, List<T> list) where T : class where TSearch : class
		{
			list.Clear();
			bool flag = !includeInactive && !t.gameObject.activeSelf;
			if (!flag)
			{
				List<TSearch> list2 = NestedComponentUtilities.RecyclableList<TSearch>.List;
				NestedComponentUtilities.nodesQueue.Clear();
				NestedComponentUtilities.nodesQueue.Enqueue(t);
				try
				{
					while (NestedComponentUtilities.nodesQueue.Count > 0)
					{
						Transform transform = NestedComponentUtilities.nodesQueue.Dequeue();
						transform.GetComponents<TSearch>(list2);
						foreach (TSearch tsearch in list2)
						{
							T t2 = tsearch as T;
							bool flag2 = t2 != null;
							if (flag2)
							{
								list.Add(t2);
							}
						}
						int i = 0;
						int childCount = transform.childCount;
						while (i < childCount)
						{
							Transform child = transform.GetChild(i);
							bool flag3 = !includeInactive && !child.gameObject.activeSelf;
							if (!flag3)
							{
								bool flag4 = child.GetComponent<TStop>() != null;
								if (!flag4)
								{
									NestedComponentUtilities.nodesQueue.Enqueue(child);
								}
							}
							i++;
						}
					}
				}
				finally
				{
					list2.Clear();
				}
			}
		}

		public static T[] FindObjectsOfTypeInOrder<T>(this Scene scene, bool includeInactive = false) where T : class
		{
			List<T> list = new List<T>();
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				list.AddRange(gameObject.GetComponentsInChildren<T>(includeInactive));
			}
			return list.ToArray();
		}

		public static void FindObjectsOfTypeInOrder<T>(this Scene scene, List<T> list, bool includeInactive = false) where T : class
		{
			list.Clear();
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				list.AddRange(gameObject.GetComponentsInChildren<T>(includeInactive));
			}
		}

		public static TCast[] FindObjectsOfTypeInOrder<T, TCast>(this Scene scene, bool includeInactive = false) where T : class where TCast : class
		{
			List<TCast> list = new List<TCast>();
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				T[] componentsInChildren = gameObject.GetComponentsInChildren<T>(includeInactive);
				foreach (T t in componentsInChildren)
				{
					TCast tcast = t as TCast;
					bool flag = tcast != null;
					if (flag)
					{
						list.Add(tcast);
					}
				}
			}
			return list.ToArray();
		}

		public static void FindObjectsOfTypeInOrder<T, TCast>(this Scene scene, List<TCast> list, bool includeInactive = false) where T : class where TCast : class
		{
			list.Clear();
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				T[] componentsInChildren = gameObject.GetComponentsInChildren<T>(includeInactive);
				foreach (T t in componentsInChildren)
				{
					TCast tcast = t as TCast;
					bool flag = tcast != null;
					if (flag)
					{
						list.Add(tcast);
					}
				}
			}
		}

		private static Queue<Transform> nodesQueue = new Queue<Transform>();

		private static Stack<Transform> nodeStack = new Stack<Transform>();

		private static class RecyclableList<T> where T : class
		{
			public static List<T> List = new List<T>();
		}
	}
}
