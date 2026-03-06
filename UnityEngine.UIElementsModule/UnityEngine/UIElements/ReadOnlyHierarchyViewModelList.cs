using System;
using System.Collections;
using Unity.Hierarchy;

namespace UnityEngine.UIElements
{
	internal class ReadOnlyHierarchyViewModelList : IList, ICollection, IEnumerable
	{
		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public int Count
		{
			get
			{
				return this.m_HierarchyViewModel.Count;
			}
		}

		public bool Contains(object value)
		{
			bool result;
			if (value is HierarchyNode)
			{
				HierarchyNode hierarchyNode = (HierarchyNode)value;
				result = this.m_HierarchyViewModel.Contains(hierarchyNode);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public int IndexOf(object value)
		{
			int result;
			if (value is HierarchyNode)
			{
				HierarchyNode hierarchyNode = (HierarchyNode)value;
				result = this.m_HierarchyViewModel.IndexOf(hierarchyNode);
			}
			else
			{
				result = BaseTreeView.invalidId;
			}
			return result;
		}

		public ReadOnlyHierarchyViewModelList(HierarchyViewModel viewModel)
		{
			this.m_HierarchyViewModel = viewModel;
		}

		public unsafe object this[int index]
		{
			get
			{
				return *this.m_HierarchyViewModel[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public unsafe void CopyTo(Array array, int index)
		{
			for (int i = index; i < this.m_HierarchyViewModel.Count; i++)
			{
				array.SetValue(*this.m_HierarchyViewModel[i], i - index);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new ReadOnlyHierarchyViewModelList.Enumerator(this.m_HierarchyViewModel);
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public int Add(object value)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		public void Remove(object value)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		private readonly HierarchyViewModel m_HierarchyViewModel;

		private struct Enumerator : IEnumerator
		{
			public Enumerator(HierarchyViewModel hierarchyViewModel)
			{
				this.m_HierarchyViewModel = hierarchyViewModel;
				this.m_Enumerator = hierarchyViewModel.GetEnumerator();
			}

			public unsafe object Current
			{
				get
				{
					return *this.m_Enumerator.Current;
				}
			}

			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_Enumerator = this.m_HierarchyViewModel.GetEnumerator();
			}

			private readonly HierarchyViewModel m_HierarchyViewModel;

			private HierarchyViewModel.Enumerator m_Enumerator;
		}
	}
}
