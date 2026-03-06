using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

internal class DelegateList<T>
{
	public DelegateList(Func<Action<T>, LinkedListNode<Action<T>>> acquireFunc, Action<LinkedListNode<Action<T>>> releaseFunc)
	{
		if (acquireFunc == null)
		{
			throw new ArgumentNullException("acquireFunc");
		}
		if (releaseFunc == null)
		{
			throw new ArgumentNullException("releaseFunc");
		}
		this.m_acquireFunc = acquireFunc;
		this.m_releaseFunc = releaseFunc;
	}

	public int Count
	{
		get
		{
			if (this.m_callbacks != null)
			{
				return this.m_callbacks.Count;
			}
			return 0;
		}
	}

	public void Add(Action<T> action)
	{
		LinkedListNode<Action<T>> node = this.m_acquireFunc(action);
		if (this.m_callbacks == null)
		{
			this.m_callbacks = new LinkedList<Action<T>>();
		}
		this.m_callbacks.AddLast(node);
	}

	public void Remove(Action<T> action)
	{
		if (this.m_callbacks == null)
		{
			return;
		}
		LinkedListNode<Action<T>> linkedListNode = this.m_callbacks.First;
		while (linkedListNode != null)
		{
			if (linkedListNode.Value == action)
			{
				if (this.m_invoking)
				{
					linkedListNode.Value = null;
					return;
				}
				this.m_callbacks.Remove(linkedListNode);
				this.m_releaseFunc(linkedListNode);
				return;
			}
			else
			{
				linkedListNode = linkedListNode.Next;
			}
		}
	}

	public void Invoke(T res)
	{
		if (this.m_callbacks == null)
		{
			return;
		}
		this.m_invoking = true;
		for (LinkedListNode<Action<T>> linkedListNode = this.m_callbacks.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value != null)
			{
				try
				{
					linkedListNode.Value(res);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
		this.m_invoking = false;
		LinkedListNode<Action<T>> next;
		for (LinkedListNode<Action<T>> linkedListNode2 = this.m_callbacks.First; linkedListNode2 != null; linkedListNode2 = next)
		{
			next = linkedListNode2.Next;
			if (linkedListNode2.Value == null)
			{
				this.m_callbacks.Remove(linkedListNode2);
				this.m_releaseFunc(linkedListNode2);
			}
		}
	}

	public void Clear()
	{
		if (this.m_callbacks == null)
		{
			return;
		}
		LinkedListNode<Action<T>> next;
		for (LinkedListNode<Action<T>> linkedListNode = this.m_callbacks.First; linkedListNode != null; linkedListNode = next)
		{
			next = linkedListNode.Next;
			this.m_callbacks.Remove(linkedListNode);
			this.m_releaseFunc(linkedListNode);
		}
	}

	public static DelegateList<T> CreateWithGlobalCache()
	{
		if (!GlobalLinkedListNodeCache<Action<T>>.CacheExists)
		{
			GlobalLinkedListNodeCache<Action<T>>.SetCacheSize(32);
		}
		return new DelegateList<T>(new Func<Action<T>, LinkedListNode<Action<T>>>(GlobalLinkedListNodeCache<Action<T>>.Acquire), new Action<LinkedListNode<Action<T>>>(GlobalLinkedListNodeCache<Action<T>>.Release));
	}

	private Func<Action<T>, LinkedListNode<Action<T>>> m_acquireFunc;

	private Action<LinkedListNode<Action<T>>> m_releaseFunc;

	private LinkedList<Action<T>> m_callbacks;

	private bool m_invoking;
}
