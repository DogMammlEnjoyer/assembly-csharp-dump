using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[Serializable]
	public class Spline : ISpline, IReadOnlyList<BezierKnot>, IEnumerable<BezierKnot>, IEnumerable, IReadOnlyCollection<BezierKnot>, IList<BezierKnot>, ICollection<BezierKnot>
	{
		private IEnumerable<ISplineModificationHandler> embeddedSplineData
		{
			get
			{
				foreach (SplineDataKeyValuePair<int> splineDataKeyValuePair in this.m_IntData)
				{
					yield return splineDataKeyValuePair.Value;
				}
				IEnumerator<SplineDataKeyValuePair<int>> enumerator = null;
				foreach (SplineDataKeyValuePair<float> splineDataKeyValuePair2 in this.m_FloatData)
				{
					yield return splineDataKeyValuePair2.Value;
				}
				IEnumerator<SplineDataKeyValuePair<float>> enumerator2 = null;
				foreach (SplineDataKeyValuePair<float4> splineDataKeyValuePair3 in this.m_Float4Data)
				{
					yield return splineDataKeyValuePair3.Value;
				}
				IEnumerator<SplineDataKeyValuePair<float4>> enumerator3 = null;
				foreach (SplineDataKeyValuePair<Object> splineDataKeyValuePair4 in this.m_ObjectData)
				{
					yield return splineDataKeyValuePair4.Value;
				}
				IEnumerator<SplineDataKeyValuePair<Object>> enumerator4 = null;
				yield break;
				yield break;
			}
		}

		public bool TryGetFloatData(string key, out SplineData<float> data)
		{
			return this.m_FloatData.TryGetValue(key, out data);
		}

		public bool TryGetFloat4Data(string key, out SplineData<float4> data)
		{
			return this.m_Float4Data.TryGetValue(key, out data);
		}

		public bool TryGetIntData(string key, out SplineData<int> data)
		{
			return this.m_IntData.TryGetValue(key, out data);
		}

		public bool TryGetObjectData(string key, out SplineData<Object> data)
		{
			return this.m_ObjectData.TryGetValue(key, out data);
		}

		public SplineData<float> GetOrCreateFloatData(string key)
		{
			return this.m_FloatData.GetOrCreate(key);
		}

		public SplineData<float4> GetOrCreateFloat4Data(string key)
		{
			return this.m_Float4Data.GetOrCreate(key);
		}

		public SplineData<int> GetOrCreateIntData(string key)
		{
			return this.m_IntData.GetOrCreate(key);
		}

		public SplineData<Object> GetOrCreateObjectData(string key)
		{
			return this.m_ObjectData.GetOrCreate(key);
		}

		public bool RemoveFloatData(string key)
		{
			return this.m_FloatData.Remove(key);
		}

		public bool RemoveFloat4Data(string key)
		{
			return this.m_Float4Data.Remove(key);
		}

		public bool RemoveIntData(string key)
		{
			return this.m_IntData.Remove(key);
		}

		public bool RemoveObjectData(string key)
		{
			return this.m_ObjectData.Remove(key);
		}

		public IEnumerable<string> GetFloatDataKeys()
		{
			return this.m_FloatData.Keys;
		}

		public IEnumerable<string> GetFloat4DataKeys()
		{
			return this.m_Float4Data.Keys;
		}

		public IEnumerable<string> GetIntDataKeys()
		{
			return this.m_IntData.Keys;
		}

		public IEnumerable<string> GetObjectDataKeys()
		{
			return this.m_ObjectData.Keys;
		}

		public IEnumerable<string> GetSplineDataKeys(EmbeddedSplineDataType type)
		{
			switch (type)
			{
			case EmbeddedSplineDataType.Int:
				return this.m_IntData.Keys;
			case EmbeddedSplineDataType.Float:
				return this.m_FloatData.Keys;
			case EmbeddedSplineDataType.Float4:
				return this.m_Float4Data.Keys;
			case EmbeddedSplineDataType.Object:
				return this.m_ObjectData.Keys;
			default:
				throw new InvalidEnumArgumentException();
			}
		}

		public IEnumerable<SplineData<float>> GetFloatDataValues()
		{
			return this.m_FloatData.Values;
		}

		public IEnumerable<SplineData<float4>> GetFloat4DataValues()
		{
			return this.m_Float4Data.Values;
		}

		public IEnumerable<SplineData<int>> GetIntDataValues()
		{
			return this.m_IntData.Values;
		}

		public IEnumerable<SplineData<Object>> GetObjectDataValues()
		{
			return this.m_ObjectData.Values;
		}

		public void SetFloatData(string key, SplineData<float> value)
		{
			this.m_FloatData[key] = value;
		}

		public void SetFloat4Data(string key, SplineData<float4> value)
		{
			this.m_Float4Data[key] = value;
		}

		public void SetIntData(string key, SplineData<int> value)
		{
			this.m_IntData[key] = value;
		}

		public void SetObjectData(string key, SplineData<Object> value)
		{
			this.m_ObjectData[key] = value;
		}

		public int Count
		{
			get
			{
				return this.m_Knots.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		[Obsolete("Deprecated, use Changed instead.")]
		public event Action changed;

		public static event Action<Spline, int, SplineModification> Changed;

		internal void SetDirtyNoNotify()
		{
			this.EnsureMetaDataValid();
			this.m_Length = -1f;
			int i = 0;
			int count = this.m_MetaData.Count;
			while (i < count)
			{
				this.m_MetaData[i].InvalidateCache();
				i++;
			}
		}

		internal void SetDirty(SplineModification modificationEvent, int knotIndex = -1)
		{
			this.SetDirtyNoNotify();
			Action action = this.changed;
			if (action != null)
			{
				action();
			}
			this.OnSplineChanged();
			foreach (ISplineModificationHandler splineModificationHandler in this.embeddedSplineData)
			{
				splineModificationHandler.OnSplineModified(new SplineModificationData(this, modificationEvent, knotIndex, this.m_LastKnotChangeCurveLengths.Item1, this.m_LastKnotChangeCurveLengths.Item2));
			}
			Action<Spline, int, SplineModification> action2 = Spline.Changed;
			if (action2 == null)
			{
				return;
			}
			action2(this, knotIndex, modificationEvent);
		}

		protected virtual void OnSplineChanged()
		{
		}

		private void EnsureMetaDataValid()
		{
			while (this.m_MetaData.Count < this.m_Knots.Count)
			{
				this.m_MetaData.Add(new Spline.MetaData());
			}
		}

		public void EnforceTangentModeNoNotify(int index)
		{
			this.EnforceTangentModeNoNotify(new SplineRange(index, 1));
		}

		public void EnforceTangentModeNoNotify(SplineRange range)
		{
			for (int i = range.Start; i <= range.End; i++)
			{
				this.ApplyTangentModeNoNotify(i, BezierTangent.Out);
			}
		}

		public TangentMode GetTangentMode(int index)
		{
			this.EnsureMetaDataValid();
			if (this.m_MetaData.Count <= 0)
			{
				return TangentMode.Broken;
			}
			return this.m_MetaData[index].Mode;
		}

		public void SetTangentMode(TangentMode mode)
		{
			this.SetTangentMode(new SplineRange(0, this.Count), mode, BezierTangent.Out);
		}

		public void SetTangentMode(int index, TangentMode mode, BezierTangent main = BezierTangent.Out)
		{
			if (this.GetTangentMode(index) == mode)
			{
				return;
			}
			if (index == this.Count - 1 && !this.Closed)
			{
				main = BezierTangent.In;
			}
			this.SetTangentMode(new SplineRange(index, 1), mode, main);
		}

		public void SetTangentMode(SplineRange range, TangentMode mode, BezierTangent main = BezierTangent.Out)
		{
			foreach (int num in range)
			{
				this.CacheKnotOperationCurves(num);
				this.SetTangentModeNoNotify(num, mode, main);
				this.SetDirty(SplineModification.KnotModified, num);
			}
		}

		public void SetTangentModeNoNotify(int index, TangentMode mode, BezierTangent main = BezierTangent.Out)
		{
			this.EnsureMetaDataValid();
			BezierKnot bezierKnot = this.m_Knots[index];
			if (this.m_MetaData[index].Mode == TangentMode.Linear && mode >= TangentMode.Mirrored)
			{
				bezierKnot.TangentIn = SplineUtility.GetExplicitLinearTangent(bezierKnot, this.Previous(index));
				bezierKnot.TangentOut = SplineUtility.GetExplicitLinearTangent(bezierKnot, this.Next(index));
			}
			this.m_MetaData[index].Mode = mode;
			this.m_Knots[index] = bezierKnot;
			this.ApplyTangentModeNoNotify(index, main);
		}

		private void ApplyTangentModeNoNotify(int index, BezierTangent main = BezierTangent.Out)
		{
			BezierKnot bezierKnot = this.m_Knots[index];
			switch (this.GetTangentMode(index))
			{
			case TangentMode.AutoSmooth:
				bezierKnot = SplineUtility.GetAutoSmoothKnot(bezierKnot.Position, this.Previous(index).Position, this.Next(index).Position, math.mul(bezierKnot.Rotation, math.up()), this.m_MetaData[index].Tension);
				break;
			case TangentMode.Linear:
				bezierKnot.TangentIn = float3.zero;
				bezierKnot.TangentOut = float3.zero;
				break;
			case TangentMode.Mirrored:
				bezierKnot = bezierKnot.BakeTangentDirectionToRotation(true, main);
				break;
			case TangentMode.Continuous:
				bezierKnot = bezierKnot.BakeTangentDirectionToRotation(false, main);
				break;
			}
			this.m_Knots[index] = bezierKnot;
			this.SetDirtyNoNotify();
		}

		public float GetAutoSmoothTension(int index)
		{
			return this.m_MetaData[index].Tension;
		}

		public void SetAutoSmoothTension(int index, float tension)
		{
			this.SetAutoSmoothTension(new SplineRange(index, 1), tension);
		}

		public void SetAutoSmoothTension(SplineRange range, float tension)
		{
			this.SetAutoSmoothTensionInternal(range, tension, true);
		}

		public void SetAutoSmoothTensionNoNotify(int index, float tension)
		{
			this.SetAutoSmoothTensionInternal(new SplineRange(index, 1), tension, false);
		}

		public void SetAutoSmoothTensionNoNotify(SplineRange range, float tension)
		{
			this.SetAutoSmoothTensionInternal(range, tension, false);
		}

		private void SetAutoSmoothTensionInternal(SplineRange range, float tension, bool setDirty)
		{
			int i = 0;
			int count = range.Count;
			while (i < count)
			{
				int num = range[i];
				this.CacheKnotOperationCurves(num);
				this.m_MetaData[num].Tension = tension;
				if (this.m_MetaData[num].Mode == TangentMode.AutoSmooth)
				{
					this.ApplyTangentModeNoNotify(num, BezierTangent.Out);
				}
				if (setDirty)
				{
					this.SetDirty(SplineModification.KnotModified, num);
				}
				i++;
			}
		}

		[Obsolete("Use GetTangentMode and SetTangentMode.")]
		public SplineType EditType
		{
			get
			{
				return this.m_EditModeType;
			}
			set
			{
				if (this.m_EditModeType == value)
				{
					return;
				}
				this.m_EditModeType = value;
				TangentMode tangentMode = value.GetTangentMode();
				for (int i = 0; i < this.Count; i++)
				{
					this.SetTangentModeNoNotify(i, tangentMode, BezierTangent.Out);
				}
				this.SetDirty(SplineModification.Default, -1);
			}
		}

		public IEnumerable<BezierKnot> Knots
		{
			get
			{
				return this.m_Knots;
			}
			set
			{
				this.m_Knots = new List<BezierKnot>(value);
				this.m_MetaData = new List<Spline.MetaData>(this.m_Knots.Count);
				this.SetDirty(SplineModification.Default, -1);
			}
		}

		public bool Closed
		{
			get
			{
				return this.m_Closed;
			}
			set
			{
				if (this.m_Closed == value)
				{
					return;
				}
				this.m_Closed = value;
				this.CheckAutoSmoothExtremityKnots();
				this.SetDirty(SplineModification.ClosedModified, -1);
			}
		}

		internal void CheckAutoSmoothExtremityKnots()
		{
			if (this.GetTangentMode(0) == TangentMode.AutoSmooth)
			{
				this.ApplyTangentModeNoNotify(0, BezierTangent.Out);
			}
			if (this.Count > 2 && this.GetTangentMode(this.Count - 1) == TangentMode.AutoSmooth)
			{
				this.ApplyTangentModeNoNotify(this.Count - 1, BezierTangent.Out);
			}
		}

		public int IndexOf(BezierKnot item)
		{
			return this.m_Knots.IndexOf(item);
		}

		public void Insert(int index, BezierKnot knot)
		{
			this.Insert(index, knot, TangentMode.Broken, 0.5f);
		}

		public void Insert(int index, BezierKnot knot, TangentMode mode)
		{
			this.Insert(index, knot, mode, 0.5f);
		}

		public void Insert(int index, BezierKnot knot, TangentMode mode, float tension)
		{
			this.CacheKnotOperationCurves(index);
			this.InsertNoNotify(index, knot, mode, tension);
			this.SetDirty(SplineModification.KnotInserted, index);
		}

		private void InsertNoNotify(int index, BezierKnot knot, TangentMode mode, float tension)
		{
			this.EnsureMetaDataValid();
			this.m_Knots.Insert(index, knot);
			this.m_MetaData.Insert(index, new Spline.MetaData
			{
				Mode = mode,
				Tension = tension
			});
			int num = this.PreviousIndex(index);
			if (num != index)
			{
				this.ApplyTangentModeNoNotify(num, BezierTangent.Out);
			}
			this.ApplyTangentModeNoNotify(index, BezierTangent.Out);
			int num2 = this.NextIndex(index);
			if (num2 != index)
			{
				this.ApplyTangentModeNoNotify(num2, BezierTangent.Out);
			}
		}

		internal void InsertOnCurve(int index, float curveT)
		{
			int index2 = SplineUtility.PreviousIndex(index, this.Count, this.Closed);
			BezierKnot bezierKnot = this.m_Knots[index2];
			BezierKnot bezierKnot2 = this.m_Knots[index];
			BezierCurve curve = new BezierCurve(bezierKnot, this.m_Knots[index]);
			BezierCurve bezierCurve;
			BezierCurve bezierCurve2;
			CurveUtility.Split(curve, curveT, out bezierCurve, out bezierCurve2);
			if (this.GetTangentMode(index2) == TangentMode.Mirrored)
			{
				this.SetTangentMode(index2, TangentMode.Continuous, BezierTangent.Out);
			}
			if (this.GetTangentMode(index) == TangentMode.Mirrored)
			{
				this.SetTangentMode(index, TangentMode.Continuous, BezierTangent.Out);
			}
			if (SplineUtility.AreTangentsModifiable(this.GetTangentMode(index2)))
			{
				bezierKnot.TangentOut = math.mul(math.inverse(bezierKnot.Rotation), bezierCurve.Tangent0);
			}
			if (SplineUtility.AreTangentsModifiable(this.GetTangentMode(index)))
			{
				bezierKnot2.TangentIn = math.mul(math.inverse(bezierKnot2.Rotation), bezierCurve2.Tangent1);
			}
			float3 up = CurveUtility.EvaluateUpVector(curve, curveT, math.rotate(bezierKnot.Rotation, math.up()), math.rotate(bezierKnot2.Rotation, math.up()), true);
			quaternion quaternion = quaternion.LookRotationSafe(math.normalizesafe(bezierCurve2.Tangent0, default(float3)), up);
			quaternion q = math.inverse(quaternion);
			this.SetKnotNoNotify(index2, bezierKnot, BezierTangent.Out);
			this.SetKnotNoNotify(index, bezierKnot2, BezierTangent.Out);
			BezierKnot knot = new BezierKnot(bezierCurve.P3, math.mul(q, bezierCurve.Tangent1), math.mul(q, bezierCurve2.Tangent0), quaternion);
			this.Insert(index, knot);
		}

		public void RemoveAt(int index)
		{
			this.EnsureMetaDataValid();
			this.CacheKnotOperationCurves(index);
			this.m_Knots.RemoveAt(index);
			this.m_MetaData.RemoveAt(index);
			int index2 = Mathf.Clamp(index, 0, this.Count - 1);
			if (this.Count > 0)
			{
				this.ApplyTangentModeNoNotify(this.PreviousIndex(index2), BezierTangent.Out);
				this.ApplyTangentModeNoNotify(index2, BezierTangent.Out);
			}
			this.SetDirty(SplineModification.KnotRemoved, index);
		}

		public BezierKnot this[int index]
		{
			get
			{
				return this.m_Knots[index];
			}
			set
			{
				this.SetKnot(index, value, BezierTangent.Out);
			}
		}

		public void SetKnot(int index, BezierKnot value, BezierTangent main = BezierTangent.Out)
		{
			this.CacheKnotOperationCurves(index);
			this.SetKnotNoNotify(index, value, main);
			this.SetDirty(SplineModification.KnotModified, index);
		}

		public void SetKnotNoNotify(int index, BezierKnot value, BezierTangent main = BezierTangent.Out)
		{
			this.m_Knots[index] = value;
			this.ApplyTangentModeNoNotify(index, main);
			int index2 = this.PreviousIndex(index);
			int index3 = this.NextIndex(index);
			if (this.m_MetaData[index2].Mode == TangentMode.AutoSmooth)
			{
				this.ApplyTangentModeNoNotify(index2, main);
			}
			if (this.m_MetaData[index3].Mode == TangentMode.AutoSmooth)
			{
				this.ApplyTangentModeNoNotify(index3, main);
			}
		}

		public Spline()
		{
		}

		public Spline(int knotCapacity, bool closed = false)
		{
			this.m_Knots = new List<BezierKnot>(knotCapacity);
			this.m_Closed = closed;
		}

		public Spline(IEnumerable<BezierKnot> knots, bool closed = false)
		{
			this.m_Knots = knots.ToList<BezierKnot>();
			this.m_Closed = closed;
		}

		public Spline(IEnumerable<float3> knotPositions, TangentMode tangentMode = TangentMode.AutoSmooth, bool closed = false)
		{
			this.InsertRangeNoNotify(this.Count, knotPositions, tangentMode, false);
			this.m_Closed = closed;
		}

		public Spline(Spline spline)
		{
			this.m_Knots = spline.Knots.ToList<BezierKnot>();
			this.m_Closed = spline.Closed;
			foreach (SplineDataKeyValuePair<int> splineDataKeyValuePair in spline.m_IntData)
			{
				this.m_IntData[splineDataKeyValuePair.Key] = splineDataKeyValuePair.Value;
			}
			foreach (SplineDataKeyValuePair<float> splineDataKeyValuePair2 in spline.m_FloatData)
			{
				this.m_FloatData[splineDataKeyValuePair2.Key] = splineDataKeyValuePair2.Value;
			}
			foreach (SplineDataKeyValuePair<float4> splineDataKeyValuePair3 in spline.m_Float4Data)
			{
				this.m_Float4Data[splineDataKeyValuePair3.Key] = splineDataKeyValuePair3.Value;
			}
			foreach (SplineDataKeyValuePair<Object> splineDataKeyValuePair4 in spline.m_ObjectData)
			{
				this.m_ObjectData[splineDataKeyValuePair4.Key] = splineDataKeyValuePair4.Value;
			}
		}

		public BezierCurve GetCurve(int index)
		{
			int index2 = this.m_Closed ? ((index + 1) % this.m_Knots.Count) : math.min(index + 1, this.m_Knots.Count - 1);
			return new BezierCurve(this.m_Knots[index], this.m_Knots[index2]);
		}

		public float GetCurveLength(int index)
		{
			this.EnsureMetaDataValid();
			DistanceToInterpolation[] distanceToInterpolation = this.m_MetaData[index].DistanceToInterpolation;
			if (distanceToInterpolation[0].Distance < 0f)
			{
				CurveUtility.CalculateCurveLengths(this.GetCurve(index), distanceToInterpolation);
			}
			if (distanceToInterpolation.Length == 0)
			{
				return 0f;
			}
			return distanceToInterpolation[distanceToInterpolation.Length - 1].Distance;
		}

		public float GetLength()
		{
			if (this.m_Length < 0f)
			{
				this.m_Length = 0f;
				int i = 0;
				int num = this.Closed ? this.Count : (this.Count - 1);
				while (i < num)
				{
					this.m_Length += this.GetCurveLength(i);
					i++;
				}
			}
			return this.m_Length;
		}

		private DistanceToInterpolation[] GetCurveDistanceLut(int index)
		{
			if (this.m_MetaData[index].DistanceToInterpolation[0].Distance < 0f)
			{
				CurveUtility.CalculateCurveLengths(this.GetCurve(index), this.m_MetaData[index].DistanceToInterpolation);
			}
			return this.m_MetaData[index].DistanceToInterpolation;
		}

		public float GetCurveInterpolation(int curveIndex, float curveDistance)
		{
			return CurveUtility.GetDistanceToInterpolation<DistanceToInterpolation[]>(this.GetCurveDistanceLut(curveIndex), curveDistance);
		}

		private void WarmUpCurveUps()
		{
			this.EnsureMetaDataValid();
			int i = 0;
			int num = this.Closed ? this.Count : (this.Count - 1);
			while (i < num)
			{
				this.EvaluateUpVectorsForCurve(i, this.m_MetaData[i].UpVectors);
				i++;
			}
		}

		public float3 GetCurveUpVector(int index, float t)
		{
			this.EnsureMetaDataValid();
			float3[] upVectors = this.m_MetaData[index].UpVectors;
			if (math.all(upVectors[0] == float3.zero))
			{
				this.EvaluateUpVectorsForCurve(index, upVectors);
			}
			float num = 1f / (float)(upVectors.Length - 1);
			float num2 = 0f;
			for (int i = 0; i < upVectors.Length; i++)
			{
				if (t <= num2 + num)
				{
					return Vector3.Lerp(upVectors[i], upVectors[i + 1], (t - num2) / num);
				}
				num2 += num;
			}
			return upVectors[upVectors.Length - 1];
		}

		public void Warmup()
		{
			this.GetLength();
			this.WarmUpCurveUps();
		}

		public void Resize(int newSize)
		{
			int count = this.Count;
			newSize = math.max(0, newSize);
			if (newSize == count)
			{
				return;
			}
			if (newSize > count)
			{
				while (this.m_Knots.Count < newSize)
				{
					this.Add(default(BezierKnot));
				}
				return;
			}
			if (newSize < count)
			{
				while (newSize < this.Count)
				{
					this.RemoveAt(this.Count - 1);
				}
				int num = newSize - 1;
				if (num > -1 && num < this.m_Knots.Count)
				{
					this.ApplyTangentModeNoNotify(num, BezierTangent.Out);
				}
			}
		}

		public BezierKnot[] ToArray()
		{
			return this.m_Knots.ToArray();
		}

		public void Copy(Spline copyFrom)
		{
			if (copyFrom == this)
			{
				return;
			}
			this.m_Closed = copyFrom.Closed;
			this.m_Knots.Clear();
			this.m_Knots.AddRange(copyFrom.m_Knots);
			this.m_MetaData.Clear();
			for (int i = 0; i < copyFrom.m_MetaData.Count; i++)
			{
				this.m_MetaData.Add(new Spline.MetaData(copyFrom.m_MetaData[i]));
			}
			this.SetDirty(SplineModification.Default, -1);
		}

		public IEnumerator<BezierKnot> GetEnumerator()
		{
			return this.m_Knots.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.m_Knots.GetEnumerator();
		}

		public void Add(BezierKnot item)
		{
			this.Add(item, TangentMode.Broken);
		}

		public void Add(float3 knotPosition, TangentMode tangentMode = TangentMode.AutoSmooth)
		{
			this.Insert(this.Count, knotPosition, tangentMode);
		}

		public void AddRange(IEnumerable<float3> knotPositions, TangentMode tangentMode = TangentMode.AutoSmooth)
		{
			this.InsertRange(this.Count, knotPositions, tangentMode);
		}

		public void Insert(int index, float3 knotPosition, TangentMode tangentMode = TangentMode.AutoSmooth)
		{
			if (tangentMode == TangentMode.AutoSmooth)
			{
				this.Insert(index, new BezierKnot(knotPosition), tangentMode);
				return;
			}
			this.CacheKnotOperationCurves(index);
			this.InsertNoNotify(index, new BezierKnot(knotPosition), TangentMode.AutoSmooth, 0.33333334f);
			this.SetTangentModeNoNotify(index, tangentMode, BezierTangent.Out);
			this.SetDirty(SplineModification.KnotInserted, index);
		}

		public void InsertRange(int index, IEnumerable<float3> knotPositions, TangentMode tangentMode = TangentMode.AutoSmooth)
		{
			this.InsertRangeNoNotify(index, knotPositions, tangentMode, true);
			this.SetDirty(SplineModification.KnotInserted, -1);
		}

		private void InsertRangeNoNotify(int index, IEnumerable<float3> knotPositions, TangentMode tangentMode = TangentMode.AutoSmooth, bool cacheCurves = false)
		{
			int num = 0;
			foreach (float3 position in knotPositions)
			{
				int index2 = index + num;
				if (cacheCurves)
				{
					this.CacheKnotOperationCurves(index2);
				}
				this.InsertNoNotify(index2, new BezierKnot(position), TangentMode.AutoSmooth, 0.33333334f);
				num++;
			}
			if (tangentMode != TangentMode.AutoSmooth)
			{
				num = 0;
				foreach (float3 @float in knotPositions)
				{
					int index3 = index + num;
					this.SetTangentModeNoNotify(index3, tangentMode, BezierTangent.Out);
					num++;
				}
			}
		}

		public void Add(BezierKnot item, TangentMode mode)
		{
			this.Insert(this.Count, item, mode);
		}

		public void Add(BezierKnot item, TangentMode mode, float tension)
		{
			this.Insert(this.Count, item, mode, tension);
		}

		public void Add(Spline spline)
		{
			for (int i = 0; i < spline.Count; i++)
			{
				this.Insert(this.Count, spline[i], spline.GetTangentMode(i), spline.GetAutoSmoothTension(i));
			}
		}

		public void Clear()
		{
			this.m_Knots.Clear();
			this.m_MetaData.Clear();
			this.SetDirty(SplineModification.KnotRemoved, -1);
		}

		public bool Contains(BezierKnot item)
		{
			return this.m_Knots.Contains(item);
		}

		public void CopyTo(BezierKnot[] array, int arrayIndex)
		{
			this.m_Knots.CopyTo(array, arrayIndex);
		}

		public bool Remove(BezierKnot item)
		{
			int num = this.m_Knots.IndexOf(item);
			if (num >= 0)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		internal void RemoveUnusedSplineData()
		{
			this.m_FloatData.RemoveEmpty();
			this.m_Float4Data.RemoveEmpty();
			this.m_IntData.RemoveEmpty();
			this.m_ObjectData.RemoveEmpty();
		}

		internal void CacheKnotOperationCurves(int index)
		{
			if (this.Count <= 1)
			{
				return;
			}
			this.m_LastKnotChangeCurveLengths.Item1 = this.GetCurveLength(this.PreviousIndex(index));
			if (index < this.Count)
			{
				this.m_LastKnotChangeCurveLengths.Item2 = this.GetCurveLength(index);
			}
		}

		private const TangentMode k_DefaultTangentMode = TangentMode.Broken;

		private const BezierTangent k_DefaultMainTangent = BezierTangent.Out;

		private const int k_BatchModification = -1;

		private const int k_CurveDistanceLutResolution = 30;

		[SerializeField]
		[Obsolete]
		[HideInInspector]
		private SplineType m_EditModeType = SplineType.Bezier;

		[SerializeField]
		private List<BezierKnot> m_Knots = new List<BezierKnot>();

		private float m_Length = -1f;

		[SerializeField]
		[HideInInspector]
		private List<Spline.MetaData> m_MetaData = new List<Spline.MetaData>();

		[SerializeField]
		private bool m_Closed;

		[SerializeField]
		private SplineDataDictionary<int> m_IntData = new SplineDataDictionary<int>();

		[SerializeField]
		private SplineDataDictionary<float> m_FloatData = new SplineDataDictionary<float>();

		[SerializeField]
		private SplineDataDictionary<float4> m_Float4Data = new SplineDataDictionary<float4>();

		[SerializeField]
		private SplineDataDictionary<Object> m_ObjectData = new SplineDataDictionary<Object>();

		[TupleElementNames(new string[]
		{
			"curve0",
			"curve1"
		})]
		private ValueTuple<float, float> m_LastKnotChangeCurveLengths;

		[Serializable]
		private sealed class MetaData
		{
			public DistanceToInterpolation[] DistanceToInterpolation
			{
				get
				{
					if (this.m_DistanceToInterpolation == null || this.m_DistanceToInterpolation.Length != 30)
					{
						this.m_DistanceToInterpolation = new DistanceToInterpolation[30];
						this.InvalidateCache();
					}
					return this.m_DistanceToInterpolation;
				}
			}

			public float3[] UpVectors
			{
				get
				{
					if (this.m_UpVectors == null || this.m_UpVectors.Length != 30)
					{
						this.m_UpVectors = new float3[30];
						this.InvalidateCache();
					}
					return this.m_UpVectors;
				}
			}

			public MetaData()
			{
				this.Mode = TangentMode.Broken;
				this.Tension = 0.5f;
				this.InvalidateCache();
			}

			public MetaData(Spline.MetaData toCopy)
			{
				this.Mode = toCopy.Mode;
				this.Tension = toCopy.Tension;
				Array.Copy(toCopy.DistanceToInterpolation, this.DistanceToInterpolation, this.DistanceToInterpolation.Length);
				Array.Copy(toCopy.UpVectors, this.UpVectors, this.UpVectors.Length);
			}

			public void InvalidateCache()
			{
				this.DistanceToInterpolation[0] = UnityEngine.Splines.DistanceToInterpolation.Invalid;
				this.UpVectors[0] = Vector3.zero;
			}

			public TangentMode Mode;

			public float Tension;

			private DistanceToInterpolation[] m_DistanceToInterpolation = new DistanceToInterpolation[30];

			private float3[] m_UpVectors = new float3[30];
		}
	}
}
