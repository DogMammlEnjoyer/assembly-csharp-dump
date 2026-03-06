using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.Splines
{
	[Serializable]
	public sealed class KnotLinkCollection
	{
		public int Count
		{
			get
			{
				return this.m_KnotsLink.Length;
			}
		}

		private KnotLinkCollection.KnotLink GetKnotLinksInternal(SplineKnotIndex index)
		{
			foreach (KnotLinkCollection.KnotLink knotLink in this.m_KnotsLink)
			{
				if (Array.IndexOf<SplineKnotIndex>(knotLink.Knots, index) >= 0)
				{
					return knotLink;
				}
			}
			return null;
		}

		public bool TryGetKnotLinks(SplineKnotIndex knotIndex, out IReadOnlyList<SplineKnotIndex> linkedKnots)
		{
			linkedKnots = this.GetKnotLinksInternal(knotIndex);
			return linkedKnots != null;
		}

		public IReadOnlyList<SplineKnotIndex> GetKnotLinks(SplineKnotIndex knotIndex)
		{
			IReadOnlyList<SplineKnotIndex> result;
			if (this.TryGetKnotLinks(knotIndex, out result))
			{
				return result;
			}
			return new KnotLinkCollection.KnotLink
			{
				Knots = new SplineKnotIndex[]
				{
					knotIndex
				}
			};
		}

		public void Clear()
		{
			this.m_KnotsLink = new KnotLinkCollection.KnotLink[0];
		}

		public void Link(SplineKnotIndex knotA, SplineKnotIndex knotB)
		{
			if (knotA.Equals(knotB))
			{
				return;
			}
			KnotLinkCollection.KnotLink knotLinksInternal = this.GetKnotLinksInternal(knotA);
			KnotLinkCollection.KnotLink knotLinksInternal2 = this.GetKnotLinksInternal(knotB);
			if (knotLinksInternal != null && knotLinksInternal2 != null)
			{
				if (knotLinksInternal.Equals(knotLinksInternal2))
				{
					return;
				}
				SplineKnotIndex[] array = new SplineKnotIndex[knotLinksInternal.Knots.Length + knotLinksInternal2.Knots.Length];
				Array.Copy(knotLinksInternal.Knots, array, knotLinksInternal.Knots.Length);
				Array.Copy(knotLinksInternal2.Knots, 0, array, knotLinksInternal.Knots.Length, knotLinksInternal2.Knots.Length);
				knotLinksInternal.Knots = array;
				ArrayUtility.Remove<KnotLinkCollection.KnotLink>(ref this.m_KnotsLink, knotLinksInternal2);
				return;
			}
			else
			{
				if (knotLinksInternal2 != null)
				{
					SplineKnotIndex[] knots = knotLinksInternal2.Knots;
					ArrayUtility.Add<SplineKnotIndex>(ref knots, knotA);
					knotLinksInternal2.Knots = knots;
					return;
				}
				if (knotLinksInternal != null)
				{
					SplineKnotIndex[] knots2 = knotLinksInternal.Knots;
					ArrayUtility.Add<SplineKnotIndex>(ref knots2, knotB);
					knotLinksInternal.Knots = knots2;
					return;
				}
				KnotLinkCollection.KnotLink element = new KnotLinkCollection.KnotLink
				{
					Knots = new SplineKnotIndex[]
					{
						knotA,
						knotB
					}
				};
				ArrayUtility.Add<KnotLinkCollection.KnotLink>(ref this.m_KnotsLink, element);
				return;
			}
		}

		public void Unlink(SplineKnotIndex knot)
		{
			KnotLinkCollection.KnotLink knotLinksInternal = this.GetKnotLinksInternal(knot);
			if (knotLinksInternal == null)
			{
				return;
			}
			SplineKnotIndex[] knots = knotLinksInternal.Knots;
			ArrayUtility.Remove<SplineKnotIndex>(ref knots, knot);
			knotLinksInternal.Knots = knots;
			if (knotLinksInternal.Knots.Length < 2)
			{
				ArrayUtility.Remove<KnotLinkCollection.KnotLink>(ref this.m_KnotsLink, knotLinksInternal);
			}
		}

		public void SplineRemoved(int splineIndex)
		{
			List<int> list = new List<int>(1);
			for (int i = this.m_KnotsLink.Length - 1; i >= 0; i--)
			{
				KnotLinkCollection.KnotLink knotLink = this.m_KnotsLink[i];
				list.Clear();
				for (int j = 0; j < knotLink.Knots.Length; j++)
				{
					if (knotLink.Knots[j].Spline == splineIndex)
					{
						list.Add(j);
					}
				}
				if (knotLink.Knots.Length - list.Count < 2)
				{
					ArrayUtility.RemoveAt<KnotLinkCollection.KnotLink>(ref this.m_KnotsLink, i);
				}
				else
				{
					SplineKnotIndex[] knots = knotLink.Knots;
					ArrayUtility.SortedRemoveAt<SplineKnotIndex>(ref knots, list);
					knotLink.Knots = knots;
				}
				for (int k = 0; k < knotLink.Knots.Length; k++)
				{
					SplineKnotIndex splineKnotIndex = knotLink.Knots[k];
					if (splineKnotIndex.Spline > splineIndex)
					{
						knotLink.Knots[k] = new SplineKnotIndex(splineKnotIndex.Spline - 1, splineKnotIndex.Knot);
					}
				}
			}
		}

		public void SplineIndexChanged(int previousIndex, int newIndex)
		{
			for (int i = this.m_KnotsLink.Length - 1; i >= 0; i--)
			{
				KnotLinkCollection.KnotLink knotLink = this.m_KnotsLink[i];
				for (int j = 0; j < knotLink.Knots.Length; j++)
				{
					SplineKnotIndex splineKnotIndex = knotLink.Knots[j];
					if (splineKnotIndex.Spline == previousIndex)
					{
						knotLink.Knots[j] = new SplineKnotIndex(newIndex, splineKnotIndex.Knot);
					}
					else if (splineKnotIndex.Spline > previousIndex && splineKnotIndex.Spline <= newIndex)
					{
						knotLink.Knots[j] = new SplineKnotIndex(splineKnotIndex.Spline - 1, splineKnotIndex.Knot);
					}
					else if (splineKnotIndex.Spline < previousIndex && splineKnotIndex.Spline >= newIndex)
					{
						knotLink.Knots[j] = new SplineKnotIndex(splineKnotIndex.Spline + 1, splineKnotIndex.Knot);
					}
				}
			}
		}

		public void KnotIndexChanged(int splineIndex, int previousKnotIndex, int newKnotIndex)
		{
			this.KnotIndexChanged(new SplineKnotIndex(splineIndex, previousKnotIndex), new SplineKnotIndex(splineIndex, newKnotIndex));
		}

		public void KnotIndexChanged(SplineKnotIndex previousIndex, SplineKnotIndex newIndex)
		{
			if (previousIndex.Knot > newIndex.Knot)
			{
				previousIndex.Knot++;
			}
			else
			{
				newIndex.Knot++;
			}
			this.KnotInserted(newIndex);
			this.Link(previousIndex, newIndex);
			this.KnotRemoved(previousIndex);
		}

		public void KnotRemoved(int splineIndex, int knotIndex)
		{
			this.KnotRemoved(new SplineKnotIndex(splineIndex, knotIndex));
		}

		public void KnotRemoved(SplineKnotIndex index)
		{
			this.Unlink(index);
			this.ShiftKnotIndices(index, -1);
		}

		public void KnotInserted(int splineIndex, int knotIndex)
		{
			this.KnotInserted(new SplineKnotIndex(splineIndex, knotIndex));
		}

		public void KnotInserted(SplineKnotIndex index)
		{
			this.ShiftKnotIndices(index, 1);
		}

		public void ShiftKnotIndices(SplineKnotIndex index, int offset)
		{
			foreach (KnotLinkCollection.KnotLink knotLink in this.m_KnotsLink)
			{
				for (int j = 0; j < knotLink.Knots.Length; j++)
				{
					SplineKnotIndex splineKnotIndex = knotLink.Knots[j];
					if (splineKnotIndex.Spline == index.Spline && splineKnotIndex.Knot >= index.Knot)
					{
						knotLink.Knots[j] = new SplineKnotIndex(splineKnotIndex.Spline, splineKnotIndex.Knot + offset);
					}
				}
			}
		}

		[SerializeField]
		private KnotLinkCollection.KnotLink[] m_KnotsLink = new KnotLinkCollection.KnotLink[0];

		[Serializable]
		private sealed class KnotLink : IReadOnlyList<SplineKnotIndex>, IEnumerable<SplineKnotIndex>, IEnumerable, IReadOnlyCollection<SplineKnotIndex>
		{
			public IEnumerator<SplineKnotIndex> GetEnumerator()
			{
				return ((IEnumerable<SplineKnotIndex>)this.Knots).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.Knots.GetEnumerator();
			}

			public int Count
			{
				get
				{
					return this.Knots.Length;
				}
			}

			public SplineKnotIndex this[int index]
			{
				get
				{
					return this.Knots[index];
				}
			}

			public SplineKnotIndex[] Knots;
		}
	}
}
