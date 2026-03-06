using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.UIR
{
	internal class ImplicitPool<T> where T : class
	{
		public ImplicitPool(Func<T> createAction, Action<T> resetAction, int startCapacity, int maxCapacity)
		{
			Debug.Assert(createAction != null);
			Debug.Assert(startCapacity > 0);
			Debug.Assert(startCapacity <= maxCapacity);
			Debug.Assert(maxCapacity > 0);
			this.m_List = new List<T>(0);
			this.m_StartCapacity = startCapacity;
			this.m_MaxCapacity = maxCapacity;
			this.m_CreateAction = createAction;
			this.m_ResetAction = resetAction;
		}

		public T Get()
		{
			bool flag = this.m_UsedCount < this.m_List.Count;
			T result;
			if (flag)
			{
				List<T> list = this.m_List;
				int usedCount = this.m_UsedCount;
				this.m_UsedCount = usedCount + 1;
				result = list[usedCount];
			}
			else
			{
				bool flag2 = this.m_UsedCount < this.m_MaxCapacity;
				if (flag2)
				{
					int b = Mathf.Max(this.m_StartCapacity, this.m_UsedCount);
					int a = this.m_MaxCapacity - this.m_UsedCount;
					int num = Mathf.Min(a, b);
					this.m_List.Capacity = this.m_UsedCount + num;
					T t = this.m_CreateAction();
					this.m_List.Add(t);
					this.m_UsedCount++;
					for (int i = 1; i < num; i++)
					{
						this.m_List.Add(this.m_CreateAction());
					}
					result = t;
				}
				else
				{
					result = this.m_CreateAction();
				}
			}
			return result;
		}

		public void ReturnAll()
		{
			Debug.Assert(this.m_List.Count <= this.m_MaxCapacity);
			bool flag = this.m_ResetAction != null;
			if (flag)
			{
				for (int i = 0; i < this.m_UsedCount; i++)
				{
					this.m_ResetAction(this.m_List[i]);
				}
			}
			this.m_UsedCount = 0;
		}

		private readonly int m_StartCapacity;

		private readonly int m_MaxCapacity;

		private Func<T> m_CreateAction;

		private Action<T> m_ResetAction;

		private List<T> m_List;

		private int m_UsedCount;
	}
}
