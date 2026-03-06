using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	public class TargetPositionCache
	{
		internal static TargetPositionCache.Mode CacheMode
		{
			get
			{
				return TargetPositionCache.m_CacheMode;
			}
			set
			{
				if (value == TargetPositionCache.m_CacheMode)
				{
					return;
				}
				TargetPositionCache.m_CacheMode = value;
				switch (value)
				{
				default:
					TargetPositionCache.ClearCache();
					return;
				case TargetPositionCache.Mode.Record:
					TargetPositionCache.ClearCache();
					return;
				case TargetPositionCache.Mode.Playback:
					TargetPositionCache.CreatePlaybackCurves();
					return;
				}
			}
		}

		internal static bool IsRecording
		{
			get
			{
				return TargetPositionCache.UseCache && TargetPositionCache.m_CacheMode == TargetPositionCache.Mode.Record;
			}
		}

		internal static bool CurrentPlaybackTimeValid
		{
			get
			{
				return TargetPositionCache.UseCache && TargetPositionCache.m_CacheMode == TargetPositionCache.Mode.Playback && TargetPositionCache.HasCurrentTime;
			}
		}

		internal static bool IsEmpty
		{
			get
			{
				return TargetPositionCache.CacheTimeRange.IsEmpty;
			}
		}

		internal static TargetPositionCache.TimeRange CacheTimeRange
		{
			get
			{
				return TargetPositionCache.m_CacheTimeRange;
			}
		}

		internal static bool HasCurrentTime
		{
			get
			{
				return TargetPositionCache.m_CacheTimeRange.Contains(TargetPositionCache.CurrentTime);
			}
		}

		internal static void ClearCache()
		{
			TargetPositionCache.m_Cache = ((TargetPositionCache.CacheMode == TargetPositionCache.Mode.Disabled) ? null : new Dictionary<Transform, TargetPositionCache.CacheEntry>());
			TargetPositionCache.m_CacheTimeRange = TargetPositionCache.TimeRange.Empty;
			TargetPositionCache.CurrentTime = 0f;
			TargetPositionCache.CurrentFrame = 0;
			TargetPositionCache.IsCameraCut = false;
		}

		private static void CreatePlaybackCurves()
		{
			if (TargetPositionCache.m_Cache == null)
			{
				TargetPositionCache.m_Cache = new Dictionary<Transform, TargetPositionCache.CacheEntry>();
			}
			Dictionary<Transform, TargetPositionCache.CacheEntry>.Enumerator enumerator = TargetPositionCache.m_Cache.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<Transform, TargetPositionCache.CacheEntry> keyValuePair = enumerator.Current;
				keyValuePair.Value.CreateCurves();
			}
			enumerator.Dispose();
		}

		public static Vector3 GetTargetPosition(Transform target)
		{
			if (!TargetPositionCache.UseCache || TargetPositionCache.CacheMode == TargetPositionCache.Mode.Disabled)
			{
				return target.position;
			}
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Record && !TargetPositionCache.m_CacheTimeRange.IsEmpty && TargetPositionCache.CurrentTime < TargetPositionCache.m_CacheTimeRange.Start - 0.1f)
			{
				TargetPositionCache.ClearCache();
			}
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Playback && !TargetPositionCache.HasCurrentTime)
			{
				return target.position;
			}
			TargetPositionCache.CacheEntry cacheEntry;
			if (!TargetPositionCache.m_Cache.TryGetValue(target, out cacheEntry))
			{
				if (TargetPositionCache.CacheMode != TargetPositionCache.Mode.Record)
				{
					return target.position;
				}
				cacheEntry = new TargetPositionCache.CacheEntry();
				TargetPositionCache.m_Cache.Add(target, cacheEntry);
			}
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Record)
			{
				cacheEntry.AddRawItem(TargetPositionCache.CurrentTime, TargetPositionCache.IsCameraCut, target);
				TargetPositionCache.m_CacheTimeRange.Include(TargetPositionCache.CurrentTime);
				return target.position;
			}
			if (cacheEntry.Curve == null)
			{
				return target.position;
			}
			return cacheEntry.Curve.Evaluate(TargetPositionCache.CurrentTime).Pos;
		}

		public static Quaternion GetTargetRotation(Transform target)
		{
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Disabled)
			{
				return target.rotation;
			}
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Record && !TargetPositionCache.m_CacheTimeRange.IsEmpty && TargetPositionCache.CurrentTime < TargetPositionCache.m_CacheTimeRange.Start - 0.1f)
			{
				TargetPositionCache.ClearCache();
			}
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Playback && !TargetPositionCache.HasCurrentTime)
			{
				return target.rotation;
			}
			TargetPositionCache.CacheEntry cacheEntry;
			if (!TargetPositionCache.m_Cache.TryGetValue(target, out cacheEntry))
			{
				if (TargetPositionCache.CacheMode != TargetPositionCache.Mode.Record)
				{
					return target.rotation;
				}
				cacheEntry = new TargetPositionCache.CacheEntry();
				TargetPositionCache.m_Cache.Add(target, cacheEntry);
			}
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Record)
			{
				if (TargetPositionCache.m_CacheTimeRange.End <= TargetPositionCache.CurrentTime)
				{
					cacheEntry.AddRawItem(TargetPositionCache.CurrentTime, TargetPositionCache.IsCameraCut, target);
					TargetPositionCache.m_CacheTimeRange.Include(TargetPositionCache.CurrentTime);
				}
				return target.rotation;
			}
			return cacheEntry.Curve.Evaluate(TargetPositionCache.CurrentTime).Rot;
		}

		internal static bool UseCache;

		internal const float CacheStepSize = 0.016666668f;

		private static TargetPositionCache.Mode m_CacheMode;

		internal static float CurrentTime;

		internal static int CurrentFrame;

		internal static bool IsCameraCut;

		private static Dictionary<Transform, TargetPositionCache.CacheEntry> m_Cache;

		private static TargetPositionCache.TimeRange m_CacheTimeRange;

		private const float kWraparoundSlush = 0.1f;

		internal enum Mode
		{
			Disabled,
			Record,
			Playback
		}

		private class CacheCurve
		{
			public int Count
			{
				get
				{
					return this.m_Cache.Count;
				}
			}

			public CacheCurve(float startTime, float endTime, float stepSize)
			{
				this.StepSize = stepSize;
				this.StartTime = startTime;
				this.m_Cache = new List<TargetPositionCache.CacheCurve.Item>(Mathf.CeilToInt((this.StepSize * 0.5f + endTime - startTime) / this.StepSize));
			}

			public void Add(TargetPositionCache.CacheCurve.Item item)
			{
				this.m_Cache.Add(item);
			}

			public void AddUntil(TargetPositionCache.CacheCurve.Item item, float time, bool isCut)
			{
				int num = this.m_Cache.Count - 1;
				float num2 = (float)num * this.StepSize;
				float num3 = time - this.StartTime - num2;
				if (isCut)
				{
					for (float num4 = this.StepSize; num4 <= num3; num4 += this.StepSize)
					{
						this.Add(item);
					}
					return;
				}
				TargetPositionCache.CacheCurve.Item a = this.m_Cache[num];
				for (float num5 = this.StepSize; num5 <= num3; num5 += this.StepSize)
				{
					this.Add(TargetPositionCache.CacheCurve.Item.Lerp(a, item, num5 / num3));
				}
			}

			public TargetPositionCache.CacheCurve.Item Evaluate(float time)
			{
				int count = this.m_Cache.Count;
				if (count == 0)
				{
					return TargetPositionCache.CacheCurve.Item.Empty;
				}
				float num = time - this.StartTime;
				int num2 = Mathf.Clamp(Mathf.FloorToInt(num / this.StepSize), 0, count - 1);
				TargetPositionCache.CacheCurve.Item item = this.m_Cache[num2];
				if (num2 == count - 1)
				{
					return item;
				}
				return TargetPositionCache.CacheCurve.Item.Lerp(item, this.m_Cache[num2 + 1], (num - (float)num2 * this.StepSize) / this.StepSize);
			}

			public float StartTime;

			public float StepSize;

			private List<TargetPositionCache.CacheCurve.Item> m_Cache;

			public struct Item
			{
				public static TargetPositionCache.CacheCurve.Item Lerp(TargetPositionCache.CacheCurve.Item a, TargetPositionCache.CacheCurve.Item b, float t)
				{
					return new TargetPositionCache.CacheCurve.Item
					{
						Pos = Vector3.LerpUnclamped(a.Pos, b.Pos, t),
						Rot = Quaternion.SlerpUnclamped(a.Rot, b.Rot, t)
					};
				}

				public static TargetPositionCache.CacheCurve.Item Empty
				{
					get
					{
						return new TargetPositionCache.CacheCurve.Item
						{
							Rot = Quaternion.identity
						};
					}
				}

				public Vector3 Pos;

				public Quaternion Rot;
			}
		}

		private class CacheEntry
		{
			public void AddRawItem(float time, bool isCut, Transform target)
			{
				float num = time - 0.016666668f;
				int num2 = this.RawItems.Count - 1;
				int num3 = num2;
				while (num3 >= 0 && this.RawItems[num3].Time > num)
				{
					num3--;
				}
				if (num3 == num2)
				{
					this.RawItems.Add(new TargetPositionCache.CacheEntry.RecordingItem
					{
						Time = time,
						IsCut = isCut,
						Item = new TargetPositionCache.CacheCurve.Item
						{
							Pos = target.position,
							Rot = target.rotation
						}
					});
					return;
				}
				int num4 = num3 + 2;
				if (num4 <= num2)
				{
					this.RawItems.RemoveRange(num4, this.RawItems.Count - num4);
				}
				this.RawItems[num3 + 1] = new TargetPositionCache.CacheEntry.RecordingItem
				{
					Time = time,
					IsCut = isCut,
					Item = new TargetPositionCache.CacheCurve.Item
					{
						Pos = target.position,
						Rot = target.rotation
					}
				};
			}

			public void CreateCurves()
			{
				int num = this.RawItems.Count - 1;
				float startTime = (num < 0) ? 0f : this.RawItems[0].Time;
				float endTime = (num < 0) ? 0f : this.RawItems[num].Time;
				this.Curve = new TargetPositionCache.CacheCurve(startTime, endTime, 0.016666668f);
				this.Curve.Add((num < 0) ? TargetPositionCache.CacheCurve.Item.Empty : this.RawItems[0].Item);
				for (int i = 1; i <= num; i++)
				{
					this.Curve.AddUntil(this.RawItems[i].Item, this.RawItems[i].Time, this.RawItems[i].IsCut);
				}
				this.RawItems.Clear();
			}

			public TargetPositionCache.CacheCurve Curve;

			private List<TargetPositionCache.CacheEntry.RecordingItem> RawItems = new List<TargetPositionCache.CacheEntry.RecordingItem>();

			private struct RecordingItem
			{
				public float Time;

				public bool IsCut;

				public TargetPositionCache.CacheCurve.Item Item;
			}
		}

		internal struct TimeRange
		{
			public bool IsEmpty
			{
				get
				{
					return this.End < this.Start;
				}
			}

			public bool Contains(float time)
			{
				return time >= this.Start && time <= this.End;
			}

			public static TargetPositionCache.TimeRange Empty
			{
				get
				{
					return new TargetPositionCache.TimeRange
					{
						Start = float.MaxValue,
						End = float.MinValue
					};
				}
			}

			public void Include(float time)
			{
				this.Start = Mathf.Min(this.Start, time);
				this.End = Mathf.Max(this.End, time);
			}

			public float Start;

			public float End;
		}
	}
}
