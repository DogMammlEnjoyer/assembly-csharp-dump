using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.ResourceManagement.Util
{
	internal class DelayedActionManager : ComponentSingleton<DelayedActionManager>
	{
		private LinkedListNode<DelayedActionManager.DelegateInfo> GetNode(ref DelayedActionManager.DelegateInfo del)
		{
			if (this.m_NodeCache.Count > 0)
			{
				LinkedListNode<DelayedActionManager.DelegateInfo> linkedListNode = this.m_NodeCache.Pop();
				linkedListNode.Value = del;
				return linkedListNode;
			}
			return new LinkedListNode<DelayedActionManager.DelegateInfo>(del);
		}

		public static void Clear()
		{
			if (ComponentSingleton<DelayedActionManager>.Exists)
			{
				ComponentSingleton<DelayedActionManager>.Instance.DestroyWhenComplete();
			}
		}

		private void DestroyWhenComplete()
		{
			this.m_DestroyOnCompletion = true;
		}

		public static void AddAction(Delegate action, float delay = 0f, params object[] parameters)
		{
			ComponentSingleton<DelayedActionManager>.Instance.AddActionInternal(action, delay, parameters);
		}

		private void AddActionInternal(Delegate action, float delay, params object[] parameters)
		{
			DelayedActionManager.DelegateInfo item = new DelayedActionManager.DelegateInfo(action, Time.unscaledTime + delay, parameters);
			if (delay <= 0f)
			{
				this.m_Actions[this.m_CollectionIndex].Add(item);
				return;
			}
			if (this.m_DelayedActions.Count == 0)
			{
				this.m_DelayedActions.AddFirst(this.GetNode(ref item));
				return;
			}
			LinkedListNode<DelayedActionManager.DelegateInfo> linkedListNode = this.m_DelayedActions.Last;
			while (linkedListNode != null && linkedListNode.Value.InvocationTime > item.InvocationTime)
			{
				linkedListNode = linkedListNode.Previous;
			}
			if (linkedListNode == null)
			{
				this.m_DelayedActions.AddFirst(this.GetNode(ref item));
				return;
			}
			this.m_DelayedActions.AddBefore(linkedListNode, this.GetNode(ref item));
		}

		public static bool IsActive
		{
			get
			{
				if (!ComponentSingleton<DelayedActionManager>.Exists)
				{
					return false;
				}
				if (ComponentSingleton<DelayedActionManager>.Instance.m_DelayedActions.Count > 0)
				{
					return true;
				}
				for (int i = 0; i < ComponentSingleton<DelayedActionManager>.Instance.m_Actions.Length; i++)
				{
					if (ComponentSingleton<DelayedActionManager>.Instance.m_Actions[i].Count > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static bool Wait(float timeout = 0f, float timeAdvanceAmount = 0f)
		{
			if (!DelayedActionManager.IsActive)
			{
				return true;
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			float num = Time.unscaledTime;
			do
			{
				ComponentSingleton<DelayedActionManager>.Instance.InternalLateUpdate(num);
				if (timeAdvanceAmount >= 0f)
				{
					num += timeAdvanceAmount;
				}
				else
				{
					num = Time.unscaledTime;
				}
			}
			while (DelayedActionManager.IsActive && (timeout <= 0f || stopwatch.Elapsed.TotalSeconds < (double)timeout));
			return !DelayedActionManager.IsActive;
		}

		private void LateUpdate()
		{
			this.InternalLateUpdate(Time.unscaledTime);
		}

		private void InternalLateUpdate(float t)
		{
			int num = 0;
			while (this.m_DelayedActions.Count > 0 && this.m_DelayedActions.First.Value.InvocationTime <= t)
			{
				this.m_Actions[this.m_CollectionIndex].Add(this.m_DelayedActions.First.Value);
				this.m_NodeCache.Push(this.m_DelayedActions.First);
				this.m_DelayedActions.RemoveFirst();
			}
			do
			{
				int collectionIndex = this.m_CollectionIndex;
				this.m_CollectionIndex = (this.m_CollectionIndex + 1) % 2;
				List<DelayedActionManager.DelegateInfo> list = this.m_Actions[collectionIndex];
				if (list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						list[i].Invoke();
					}
					list.Clear();
				}
				num++;
			}
			while (this.m_Actions[this.m_CollectionIndex].Count > 0);
			if (this.m_DestroyOnCompletion && !DelayedActionManager.IsActive)
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void OnApplicationQuit()
		{
			if (ComponentSingleton<DelayedActionManager>.Exists)
			{
				Object.Destroy(ComponentSingleton<DelayedActionManager>.Instance.gameObject);
			}
		}

		private List<DelayedActionManager.DelegateInfo>[] m_Actions = new List<DelayedActionManager.DelegateInfo>[]
		{
			new List<DelayedActionManager.DelegateInfo>(),
			new List<DelayedActionManager.DelegateInfo>()
		};

		private LinkedList<DelayedActionManager.DelegateInfo> m_DelayedActions = new LinkedList<DelayedActionManager.DelegateInfo>();

		private Stack<LinkedListNode<DelayedActionManager.DelegateInfo>> m_NodeCache = new Stack<LinkedListNode<DelayedActionManager.DelegateInfo>>(10);

		private int m_CollectionIndex;

		private bool m_DestroyOnCompletion;

		private struct DelegateInfo
		{
			public DelegateInfo(Delegate d, float invocationTime, params object[] p)
			{
				this.m_Delegate = d;
				this.m_Id = DelayedActionManager.DelegateInfo.s_Id++;
				this.m_Target = p;
				this.InvocationTime = invocationTime;
			}

			public float InvocationTime { readonly get; private set; }

			public override string ToString()
			{
				if (this.m_Delegate == null || this.m_Delegate.Method.DeclaringType == null)
				{
					return "Null m_delegate for " + this.m_Id.ToString();
				}
				string[] array = new string[8];
				array[0] = this.m_Id.ToString();
				array[1] = " (target=";
				int num = 2;
				object target = this.m_Delegate.Target;
				array[num] = ((target != null) ? target.ToString() : null);
				array[3] = ") ";
				array[4] = this.m_Delegate.Method.DeclaringType.Name;
				array[5] = ".";
				array[6] = this.m_Delegate.Method.Name;
				array[7] = "(";
				string str = string.Concat(array);
				string str2 = "";
				foreach (object obj in this.m_Target)
				{
					str = str + str2 + ((obj != null) ? obj.ToString() : null);
					str2 = ", ";
				}
				return str + ") @" + this.InvocationTime.ToString();
			}

			public void Invoke()
			{
				try
				{
					this.m_Delegate.DynamicInvoke(this.m_Target);
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Exception thrown in DynamicInvoke: {0} {1}", new object[]
					{
						ex,
						this
					});
				}
			}

			private static int s_Id;

			private int m_Id;

			private Delegate m_Delegate;

			private object[] m_Target;
		}
	}
}
