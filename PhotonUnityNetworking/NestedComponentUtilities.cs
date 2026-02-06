using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun
{
	public static class NestedComponentUtilities
	{
		public static T EnsureRootComponentExists<T, NestedT>(this Transform transform) where T : Component where NestedT : Component
		{
			NestedT parentComponent = transform.GetParentComponent<NestedT>();
			if (!parentComponent)
			{
				return default(T);
			}
			T component = parentComponent.GetComponent<T>();
			if (component)
			{
				return component;
			}
			return parentComponent.gameObject.AddComponent<T>();
		}

		public static T GetParentComponent<T>(this Transform t) where T : Component
		{
			T component = t.GetComponent<T>();
			if (component)
			{
				return component;
			}
			Transform parent = t.parent;
			while (parent)
			{
				component = parent.GetComponent<T>();
				if (component)
				{
					return component;
				}
				parent = parent.parent;
			}
			return default(T);
		}

		public static void GetNestedComponentsInParents<T>(this Transform t, List<T> list) where T : Component
		{
			list.Clear();
			while (t != null)
			{
				T component = t.GetComponent<T>();
				if (component)
				{
					list.Add(component);
				}
				t = t.parent;
			}
		}

		public static T GetNestedComponentInChildren<T, NestedT>(this Transform t, bool includeInactive) where T : class where NestedT : class
		{
			T component = t.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
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
					if ((includeInactive || child.gameObject.activeSelf) && child.GetComponent<NestedT>() == null)
					{
						component = child.GetComponent<T>();
						if (component != null)
						{
							return component;
						}
						NestedComponentUtilities.nodesQueue.Enqueue(child);
					}
					i++;
				}
			}
			return component;
		}

		public static T GetNestedComponentInParent<T, NestedT>(this Transform t) where T : class where NestedT : class
		{
			T t2 = default(T);
			Transform transform = t;
			for (;;)
			{
				t2 = transform.GetComponent<T>();
				if (t2 != null)
				{
					break;
				}
				if (transform.GetComponent<NestedT>() != null)
				{
					goto Block_2;
				}
				transform = transform.parent;
				if (transform == null)
				{
					goto Block_3;
				}
			}
			return t2;
			Block_2:
			return default(T);
			Block_3:
			return default(T);
		}

		public static T GetNestedComponentInParents<T, NestedT>(this Transform t) where T : class where NestedT : class
		{
			T component = t.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			for (Transform parent = t.parent; parent != null; parent = parent.parent)
			{
				component = parent.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
				if (parent.GetComponent<NestedT>() != null)
				{
					return default(T);
				}
			}
			return default(T);
		}

		public static void GetNestedComponentsInParents<T, NestedT>(this Transform t, List<T> list) where T : class where NestedT : class
		{
			t.GetComponents<T>(list);
			if (t.GetComponent<NestedT>() != null)
			{
				return;
			}
			Transform parent = t.parent;
			if (parent == null)
			{
				return;
			}
			NestedComponentUtilities.nodeStack.Clear();
			do
			{
				NestedComponentUtilities.nodeStack.Push(parent);
				if (parent.GetComponent<NestedT>() != null)
				{
					break;
				}
				parent = parent.parent;
			}
			while (parent != null);
			if (NestedComponentUtilities.nodeStack.Count == 0)
			{
				return;
			}
			Type typeFromHandle = typeof(T);
			List<T> list2;
			if (!NestedComponentUtilities.searchLists.ContainsKey(typeFromHandle))
			{
				list2 = new List<T>();
				NestedComponentUtilities.searchLists.Add(typeFromHandle, list2);
			}
			else
			{
				list2 = (NestedComponentUtilities.searchLists[typeFromHandle] as List<T>);
			}
			while (NestedComponentUtilities.nodeStack.Count > 0)
			{
				NestedComponentUtilities.nodeStack.Pop().GetComponents<T>(list2);
				list.AddRange(list2);
			}
		}

		public static List<T> GetNestedComponentsInChildren<T, NestedT>(this Transform t, List<T> list, bool includeInactive = true) where T : class where NestedT : class
		{
			Type typeFromHandle = typeof(T);
			List<T> list2;
			if (!NestedComponentUtilities.searchLists.ContainsKey(typeFromHandle))
			{
				NestedComponentUtilities.searchLists.Add(typeFromHandle, list2 = new List<T>());
			}
			else
			{
				list2 = (NestedComponentUtilities.searchLists[typeFromHandle] as List<T>);
			}
			NestedComponentUtilities.nodesQueue.Clear();
			if (list == null)
			{
				list = new List<T>();
			}
			t.GetComponents<T>(list);
			int i = 0;
			int childCount = t.childCount;
			while (i < childCount)
			{
				Transform child = t.GetChild(i);
				if ((includeInactive || child.gameObject.activeSelf) && child.GetComponent<NestedT>() == null)
				{
					NestedComponentUtilities.nodesQueue.Enqueue(child);
				}
				i++;
			}
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
					if ((includeInactive || child2.gameObject.activeSelf) && child2.GetComponent<NestedT>() == null)
					{
						NestedComponentUtilities.nodesQueue.Enqueue(child2);
					}
					j++;
				}
			}
			return list;
		}

		public static List<T> GetNestedComponentsInChildren<T>(this Transform t, List<T> list, bool includeInactive = true, params Type[] stopOn) where T : class
		{
			Type typeFromHandle = typeof(T);
			List<T> list2;
			if (!NestedComponentUtilities.searchLists.ContainsKey(typeFromHandle))
			{
				NestedComponentUtilities.searchLists.Add(typeFromHandle, list2 = new List<T>());
			}
			else
			{
				list2 = (NestedComponentUtilities.searchLists[typeFromHandle] as List<T>);
			}
			NestedComponentUtilities.nodesQueue.Clear();
			t.GetComponents<T>(list);
			int i = 0;
			int childCount = t.childCount;
			while (i < childCount)
			{
				Transform child = t.GetChild(i);
				if (includeInactive || child.gameObject.activeSelf)
				{
					bool flag = false;
					int j = 0;
					int num = stopOn.Length;
					while (j < num)
					{
						if (child.GetComponent(stopOn[j]) != null)
						{
							flag = true;
							break;
						}
						j++;
					}
					if (!flag)
					{
						NestedComponentUtilities.nodesQueue.Enqueue(child);
					}
				}
				i++;
			}
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
					if (includeInactive || child2.gameObject.activeSelf)
					{
						bool flag2 = false;
						int l = 0;
						int num2 = stopOn.Length;
						while (l < num2)
						{
							if (child2.GetComponent(stopOn[l]) != null)
							{
								flag2 = true;
								break;
							}
							l++;
						}
						if (!flag2)
						{
							NestedComponentUtilities.nodesQueue.Enqueue(child2);
						}
					}
					k++;
				}
			}
			return list;
		}

		public static void GetNestedComponentsInChildren<T, SearchT, NestedT>(this Transform t, bool includeInactive, List<T> list) where T : class where SearchT : class
		{
			list.Clear();
			if (!includeInactive && !t.gameObject.activeSelf)
			{
				return;
			}
			Type typeFromHandle = typeof(SearchT);
			List<SearchT> list2;
			if (!NestedComponentUtilities.searchLists.ContainsKey(typeFromHandle))
			{
				NestedComponentUtilities.searchLists.Add(typeFromHandle, list2 = new List<SearchT>());
			}
			else
			{
				list2 = (NestedComponentUtilities.searchLists[typeFromHandle] as List<SearchT>);
			}
			NestedComponentUtilities.nodesQueue.Clear();
			NestedComponentUtilities.nodesQueue.Enqueue(t);
			while (NestedComponentUtilities.nodesQueue.Count > 0)
			{
				Transform transform = NestedComponentUtilities.nodesQueue.Dequeue();
				list2.Clear();
				transform.GetComponents<SearchT>(list2);
				foreach (SearchT searchT in list2)
				{
					T t2 = searchT as T;
					if (t2 != null)
					{
						list.Add(t2);
					}
				}
				int i = 0;
				int childCount = transform.childCount;
				while (i < childCount)
				{
					Transform child = transform.GetChild(i);
					if ((includeInactive || child.gameObject.activeSelf) && child.GetComponent<NestedT>() == null)
					{
						NestedComponentUtilities.nodesQueue.Enqueue(child);
					}
					i++;
				}
			}
		}

		private static Queue<Transform> nodesQueue = new Queue<Transform>();

		public static Dictionary<Type, ICollection> searchLists = new Dictionary<Type, ICollection>();

		private static Stack<Transform> nodeStack = new Stack<Transform>();
	}
}
