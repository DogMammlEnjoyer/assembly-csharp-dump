using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[AddComponentMenu("Splines/Spline Container")]
	[ExecuteAlways]
	public sealed class SplineContainer : MonoBehaviour, ISplineContainer, ISerializationCallbackReceiver
	{
		public static event Action<SplineContainer, int> SplineAdded;

		public static event Action<SplineContainer, int> SplineRemoved;

		public static event Action<SplineContainer, int, int> SplineReordered;

		public IReadOnlyList<Spline> Splines
		{
			get
			{
				ReadOnlyCollection<Spline> result;
				if ((result = this.m_ReadOnlySplines) == null)
				{
					result = (this.m_ReadOnlySplines = new ReadOnlyCollection<Spline>(this.m_Splines));
				}
				return result;
			}
			set
			{
				if (value == null)
				{
					this.m_Splines = Array.Empty<Spline>();
					return;
				}
				this.ClearCaches();
				this.DisposeNativeSplinesCache();
				for (int k = 0; k < this.m_Splines.Length; k++)
				{
					int num = SplineContainer.IndexOf(value, this.m_Splines[k]);
					if (num == -1)
					{
						this.m_RemovedSplinesIndices.Add(k);
					}
					else if (num != k)
					{
						this.m_ReorderedSplinesIndices.Add(new ValueTuple<int, int>(k, num));
					}
				}
				int i;
				Predicate<Spline> <>9__0;
				int i2;
				for (i = 0; i < value.Count; i = i2 + 1)
				{
					Spline[] splines = this.m_Splines;
					Predicate<Spline> match;
					if ((match = <>9__0) == null)
					{
						match = (<>9__0 = ((Spline spline) => spline == value[i]));
					}
					if (Array.FindIndex<Spline>(splines, match) == -1)
					{
						this.m_AddedSplinesIndices.Add(i);
					}
					i2 = i;
				}
				this.m_Splines = new Spline[value.Count];
				for (int j = 0; j < this.m_Splines.Length; j++)
				{
					this.m_Splines[j] = value[j];
					if (this.IsNonUniformlyScaled)
					{
						this.GetOrBakeNativeSpline<Spline>(this.m_Splines[j]);
					}
				}
				this.m_ReadOnlySplines = new ReadOnlyCollection<Spline>(this.m_Splines);
				foreach (int arg in this.m_RemovedSplinesIndices)
				{
					Action<SplineContainer, int> splineRemoved = SplineContainer.SplineRemoved;
					if (splineRemoved != null)
					{
						splineRemoved(this, arg);
					}
				}
				foreach (int arg2 in this.m_AddedSplinesIndices)
				{
					Action<SplineContainer, int> splineAdded = SplineContainer.SplineAdded;
					if (splineAdded != null)
					{
						splineAdded(this, arg2);
					}
				}
				foreach (ValueTuple<int, int> valueTuple in this.m_ReorderedSplinesIndices)
				{
					Action<SplineContainer, int, int> splineReordered = SplineContainer.SplineReordered;
					if (splineReordered != null)
					{
						splineReordered(this, valueTuple.Item1, valueTuple.Item2);
					}
				}
			}
		}

		private static int IndexOf(IReadOnlyList<Spline> self, Spline elementToFind)
		{
			for (int i = 0; i < self.Count; i++)
			{
				if (self[i] == elementToFind)
				{
					return i;
				}
			}
			return -1;
		}

		public KnotLinkCollection KnotLinkCollection
		{
			get
			{
				return this.m_Knots;
			}
		}

		public Spline this[int index]
		{
			get
			{
				return this.m_Splines[index];
			}
		}

		~SplineContainer()
		{
		}

		private void OnEnable()
		{
			Spline.Changed += this.OnSplineChanged;
		}

		private void OnDisable()
		{
			Spline.Changed -= this.OnSplineChanged;
		}

		private void OnDestroy()
		{
			this.DisposeNativeSplinesCache();
		}

		public void Warmup()
		{
			for (int i = 0; i < this.Splines.Count; i++)
			{
				Spline spline = this.Splines[i];
				spline.Warmup();
				this.GetOrBakeNativeSpline<Spline>(spline);
			}
		}

		internal void ClearCaches()
		{
			this.m_ReorderedSplinesIndices.Clear();
			this.m_RemovedSplinesIndices.Clear();
			this.m_AddedSplinesIndices.Clear();
			this.m_ReadOnlySplines = null;
		}

		private void DisposeNativeSplinesCache()
		{
			foreach (KeyValuePair<ISpline, NativeSpline> keyValuePair in this.m_NativeSplinesCache)
			{
				keyValuePair.Value.Dispose();
			}
			this.m_NativeSplinesCache.Clear();
		}

		private void OnSplineChanged(Spline spline, int index, SplineModification modificationType)
		{
			int num = Array.IndexOf<Spline>(this.m_Splines, spline);
			if (num < 0)
			{
				return;
			}
			switch (modificationType)
			{
			case SplineModification.KnotModified:
				this.SetLinkedKnotPosition(new SplineKnotIndex(num, index));
				break;
			case SplineModification.KnotInserted:
			case SplineModification.KnotReordered:
				this.m_Knots.KnotInserted(num, index);
				break;
			case SplineModification.KnotRemoved:
				this.m_Knots.KnotRemoved(num, index);
				break;
			}
			NativeSpline nativeSpline;
			if (this.m_NativeSplinesCache.TryGetValue(spline, out nativeSpline))
			{
				nativeSpline.Dispose();
			}
			this.m_NativeSplinesCache.Remove(spline);
		}

		private void OnKnotModified(Spline spline, int index)
		{
			int num = Array.IndexOf<Spline>(this.m_Splines, spline);
			if (num >= 0)
			{
				this.SetLinkedKnotPosition(new SplineKnotIndex(num, index));
			}
		}

		private bool IsNonUniformlyScaled
		{
			get
			{
				float3 @float = base.transform.lossyScale;
				return !math.all(@float == @float.x);
			}
		}

		public Spline Spline
		{
			get
			{
				if (this.m_Splines.Length == 0)
				{
					return null;
				}
				return this.m_Splines[0];
			}
			set
			{
				if (this.m_Splines.Length != 0)
				{
					this.m_Splines[0] = value;
				}
			}
		}

		public bool Evaluate(float t, out float3 position, out float3 tangent, out float3 upVector)
		{
			return this.Evaluate(0, t, out position, out tangent, out upVector);
		}

		public bool Evaluate(int splineIndex, float t, out float3 position, out float3 tangent, out float3 upVector)
		{
			return this.Evaluate<Spline>(this.m_Splines[splineIndex], t, out position, out tangent, out upVector);
		}

		public bool Evaluate<T>(T spline, float t, out float3 position, out float3 tangent, out float3 upVector) where T : ISpline
		{
			if (spline == null)
			{
				position = float3.zero;
				tangent = new float3(0f, 0f, 1f);
				upVector = new float3(0f, 1f, 0f);
				return false;
			}
			if (this.IsNonUniformlyScaled)
			{
				return this.GetOrBakeNativeSpline<T>(spline).Evaluate(t, out position, out tangent, out upVector);
			}
			bool flag = spline.Evaluate(t, out position, out tangent, out upVector);
			if (flag)
			{
				position = base.transform.TransformPoint(position);
				tangent = base.transform.TransformVector(tangent);
				upVector = base.transform.TransformDirection(upVector);
			}
			return flag;
		}

		public float3 EvaluatePosition(float t)
		{
			return this.EvaluatePosition(0, t);
		}

		public float3 EvaluatePosition(int splineIndex, float t)
		{
			return this.EvaluatePosition<Spline>(this.m_Splines[splineIndex], t);
		}

		public float3 EvaluatePosition<T>(T spline, float t) where T : ISpline
		{
			if (spline == null)
			{
				return float.PositiveInfinity;
			}
			if (this.IsNonUniformlyScaled)
			{
				return this.GetOrBakeNativeSpline<T>(spline).EvaluatePosition(t);
			}
			return base.transform.TransformPoint(spline.EvaluatePosition(t));
		}

		public float3 EvaluateTangent(float t)
		{
			return this.EvaluateTangent(0, t);
		}

		public float3 EvaluateTangent(int splineIndex, float t)
		{
			return this.EvaluateTangent<Spline>(this.m_Splines[splineIndex], t);
		}

		public float3 EvaluateTangent<T>(T spline, float t) where T : ISpline
		{
			if (spline == null)
			{
				return float.PositiveInfinity;
			}
			if (this.IsNonUniformlyScaled)
			{
				return this.GetOrBakeNativeSpline<T>(spline).EvaluateTangent(t);
			}
			return base.transform.TransformVector(spline.EvaluateTangent(t));
		}

		public float3 EvaluateUpVector(float t)
		{
			return this.EvaluateUpVector(0, t);
		}

		public float3 EvaluateUpVector(int splineIndex, float t)
		{
			return this.EvaluateUpVector<Spline>(this.m_Splines[splineIndex], t);
		}

		public float3 EvaluateUpVector<T>(T spline, float t) where T : ISpline
		{
			if (spline == null)
			{
				return float3.zero;
			}
			if (this.IsNonUniformlyScaled)
			{
				return this.GetOrBakeNativeSpline<T>(spline).EvaluateUpVector(t);
			}
			return base.transform.TransformDirection(spline.EvaluateUpVector(t));
		}

		public float3 EvaluateAcceleration(float t)
		{
			return this.EvaluateAcceleration(0, t);
		}

		public float3 EvaluateAcceleration(int splineIndex, float t)
		{
			return this.EvaluateAcceleration<Spline>(this.m_Splines[splineIndex], t);
		}

		public float3 EvaluateAcceleration<T>(T spline, float t) where T : ISpline
		{
			if (spline == null)
			{
				return float3.zero;
			}
			if (this.IsNonUniformlyScaled)
			{
				return this.GetOrBakeNativeSpline<T>(spline).EvaluateAcceleration(t);
			}
			return base.transform.TransformVector(spline.EvaluateAcceleration(t));
		}

		public float CalculateLength()
		{
			return this.CalculateLength(0);
		}

		public float CalculateLength(int splineIndex)
		{
			return this.m_Splines[splineIndex].CalculateLength(base.transform.localToWorldMatrix);
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.m_Spline != null && this.m_Spline.Count > 0)
			{
				if (this.m_Splines == null || this.m_Splines.Length == 0 || (this.m_Splines.Length == 1 && this.m_Splines[0].Count == 0))
				{
					this.m_Splines = new Spline[]
					{
						this.m_Spline
					};
				}
				this.m_Spline = new Spline();
			}
			bool flag = this.m_ReadOnlySplines == null || this.m_ReadOnlySplines.Count != this.m_Splines.Length;
			if (!flag)
			{
				for (int i = 0; i < this.m_Splines.Length; i++)
				{
					if (this.m_ReadOnlySplines[i] != this.m_Splines[i])
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				this.m_ReadOnlySplines = new ReadOnlyCollection<Spline>(this.m_Splines);
			}
		}

		private NativeSpline GetOrBakeNativeSpline<T>(T spline) where T : ISpline
		{
			NativeSpline nativeSpline;
			if (!this.m_NativeSplinesCache.TryGetValue(spline, out nativeSpline))
			{
				this.m_NativeSplinesCacheTransform = base.transform.localToWorldMatrix;
				nativeSpline = new NativeSpline(spline, this.m_NativeSplinesCacheTransform, true, Allocator.Persistent);
				this.m_NativeSplinesCache.Add(spline, nativeSpline);
			}
			else if (!MathUtility.All(this.m_NativeSplinesCacheTransform, base.transform.localToWorldMatrix))
			{
				this.m_NativeSplinesCacheTransform = base.transform.localToWorldMatrix;
				SplineContainer.s_AllocPreventionHelperBuffer.Clear();
				foreach (ISpline spline2 in this.m_NativeSplinesCache.Keys)
				{
					NativeSpline nativeSpline2 = this.m_NativeSplinesCache[spline2];
					NativeSpline nativeSpline3 = new NativeSpline(spline, this.m_NativeSplinesCacheTransform, true, Allocator.Persistent);
					if (spline2 == spline)
					{
						nativeSpline = nativeSpline3;
					}
					nativeSpline2.Dispose();
					SplineContainer.s_AllocPreventionHelperBuffer.Add(new SplineContainer.SplineToNative
					{
						spline = spline2,
						nativeSpline = nativeSpline3
					});
				}
				for (int i = 0; i < SplineContainer.s_AllocPreventionHelperBuffer.Count; i++)
				{
					SplineContainer.SplineToNative splineToNative = SplineContainer.s_AllocPreventionHelperBuffer[i];
					this.m_NativeSplinesCache[splineToNative.spline] = splineToNative.nativeSpline;
				}
			}
			return nativeSpline;
		}

		private const string k_IconPath = "Packages/com.unity.splines/Editor/Editor Resources/Icons/SplineComponent.png";

		[SerializeField]
		[Obsolete]
		[HideInInspector]
		private Spline m_Spline;

		[SerializeField]
		private Spline[] m_Splines = new Spline[]
		{
			new Spline()
		};

		[SerializeField]
		private KnotLinkCollection m_Knots = new KnotLinkCollection();

		[TupleElementNames(new string[]
		{
			"previousIndex",
			"newIndex"
		})]
		private List<ValueTuple<int, int>> m_ReorderedSplinesIndices = new List<ValueTuple<int, int>>();

		private List<int> m_RemovedSplinesIndices = new List<int>();

		private List<int> m_AddedSplinesIndices = new List<int>();

		private ReadOnlyCollection<Spline> m_ReadOnlySplines;

		private Dictionary<ISpline, NativeSpline> m_NativeSplinesCache = new Dictionary<ISpline, NativeSpline>();

		private float4x4 m_NativeSplinesCacheTransform = float4x4.identity;

		private static List<SplineContainer.SplineToNative> s_AllocPreventionHelperBuffer = new List<SplineContainer.SplineToNative>(32);

		private struct SplineToNative
		{
			public ISpline spline;

			public NativeSpline nativeSpline;
		}
	}
}
