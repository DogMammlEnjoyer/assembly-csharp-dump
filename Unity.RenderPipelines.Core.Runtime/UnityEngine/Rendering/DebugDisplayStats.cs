using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	public abstract class DebugDisplayStats<TProfileId> where TProfileId : Enum
	{
		public abstract void EnableProfilingRecorders();

		public abstract void DisableProfilingRecorders();

		public abstract void RegisterDebugUI(List<DebugUI.Widget> list);

		public abstract void Update();

		protected List<TProfileId> GetProfilerIdsToDisplay()
		{
			List<TProfileId> list = new List<TProfileId>();
			Type type = typeof(TProfileId);
			Func<MemberInfo, bool> <>9__0;
			foreach (object obj in Enum.GetValues(type))
			{
				IEnumerable<MemberInfo> member = type.GetMember(obj.ToString());
				Func<MemberInfo, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((MemberInfo m) => m.DeclaringType == type));
				}
				if (Attribute.GetCustomAttribute(member.First(predicate), typeof(HideInDebugUIAttribute)) == null)
				{
					list.Add((TProfileId)((object)obj));
				}
			}
			return list;
		}

		protected void UpdateDetailedStats(List<TProfileId> samplers)
		{
			this.m_HiddenProfileIds.Clear();
			this.m_TimeSinceLastAvgValue += Time.unscaledDeltaTime;
			this.m_AccumulatedFrames++;
			bool flag = this.m_TimeSinceLastAvgValue >= 1f;
			this.UpdateListOfAveragedProfilerTimings(flag, samplers);
			if (flag)
			{
				this.m_TimeSinceLastAvgValue = 0f;
				this.m_AccumulatedFrames = 0;
			}
		}

		protected DebugUI.Widget BuildDetailedStatsList(string title, List<TProfileId> samplers)
		{
			return new DebugUI.Foldout(title, this.BuildProfilingSamplerWidgetList(samplers), DebugDisplayStats<TProfileId>.k_DetailedStatsColumnLabels, null)
			{
				opened = true
			};
		}

		private void UpdateListOfAveragedProfilerTimings(bool needUpdatingAverages, List<TProfileId> samplers)
		{
			foreach (TProfileId tprofileId in samplers)
			{
				ProfilingSampler profilingSampler = ProfilingSampler.Get<TProfileId>(tprofileId);
				bool flag = true;
				DebugDisplayStats<TProfileId>.AccumulatedTiming accumulatedTiming;
				if (this.m_AccumulatedTiming[0].TryGetValue(tprofileId, out accumulatedTiming))
				{
					accumulatedTiming.accumulatedValue += profilingSampler.cpuElapsedTime;
					flag &= (accumulatedTiming.accumulatedValue == 0f);
				}
				DebugDisplayStats<TProfileId>.AccumulatedTiming accumulatedTiming2;
				if (this.m_AccumulatedTiming[1].TryGetValue(tprofileId, out accumulatedTiming2))
				{
					accumulatedTiming2.accumulatedValue += profilingSampler.inlineCpuElapsedTime;
					flag &= (accumulatedTiming2.accumulatedValue == 0f);
				}
				DebugDisplayStats<TProfileId>.AccumulatedTiming accumulatedTiming3;
				if (this.m_AccumulatedTiming[2].TryGetValue(tprofileId, out accumulatedTiming3))
				{
					accumulatedTiming3.accumulatedValue += profilingSampler.gpuElapsedTime;
					flag &= (accumulatedTiming3.accumulatedValue == 0f);
				}
				if (needUpdatingAverages)
				{
					if (accumulatedTiming != null)
					{
						accumulatedTiming.UpdateLastAverage(this.m_AccumulatedFrames);
					}
					if (accumulatedTiming2 != null)
					{
						accumulatedTiming2.UpdateLastAverage(this.m_AccumulatedFrames);
					}
					if (accumulatedTiming3 != null)
					{
						accumulatedTiming3.UpdateLastAverage(this.m_AccumulatedFrames);
					}
				}
				if (flag)
				{
					this.m_HiddenProfileIds.Add(tprofileId);
				}
			}
		}

		private float GetSamplerTiming(TProfileId samplerId, ProfilingSampler sampler, DebugDisplayStats<TProfileId>.DebugProfilingType type)
		{
			DebugDisplayStats<TProfileId>.AccumulatedTiming accumulatedTiming;
			if (this.averageProfilerTimingsOverASecond && this.m_AccumulatedTiming[(int)type].TryGetValue(samplerId, out accumulatedTiming))
			{
				return accumulatedTiming.lastAverage;
			}
			if (type == DebugDisplayStats<TProfileId>.DebugProfilingType.CPU)
			{
				return sampler.cpuElapsedTime;
			}
			if (type != DebugDisplayStats<TProfileId>.DebugProfilingType.GPU)
			{
				return sampler.inlineCpuElapsedTime;
			}
			return sampler.gpuElapsedTime;
		}

		private ObservableList<DebugUI.Widget> BuildProfilingSamplerWidgetList(IEnumerable<TProfileId> samplers)
		{
			ObservableList<DebugUI.Widget> observableList = new ObservableList<DebugUI.Widget>();
			using (IEnumerator<TProfileId> enumerator = samplers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DebugDisplayStats<TProfileId>.<>c__DisplayClass19_1 CS$<>8__locals1 = new DebugDisplayStats<TProfileId>.<>c__DisplayClass19_1();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.samplerId = enumerator.Current;
					ProfilingSampler sampler = ProfilingSampler.Get<TProfileId>(CS$<>8__locals1.samplerId);
					if (sampler != null)
					{
						sampler.enableRecording = true;
						observableList.Add(new DebugUI.ValueTuple
						{
							displayName = sampler.name,
							isHiddenCallback = (() => CS$<>8__locals1.<>4__this.hideEmptyScopes && CS$<>8__locals1.<>4__this.m_HiddenProfileIds.Contains(CS$<>8__locals1.samplerId)),
							values = (from DebugDisplayStats<TProfileId>.DebugProfilingType e in Enum.GetValues(typeof(DebugDisplayStats<TProfileId>.DebugProfilingType))
							select CS$<>8__locals1.<>4__this.<BuildProfilingSamplerWidgetList>g__CreateWidgetForSampler|19_0(CS$<>8__locals1.samplerId, sampler, e)).ToArray<DebugUI.Value>()
						});
					}
				}
			}
			return observableList;
		}

		[CompilerGenerated]
		private DebugUI.Value <BuildProfilingSamplerWidgetList>g__CreateWidgetForSampler|19_0(TProfileId samplerId, ProfilingSampler sampler, DebugDisplayStats<TProfileId>.DebugProfilingType type)
		{
			Dictionary<TProfileId, DebugDisplayStats<TProfileId>.AccumulatedTiming> dictionary = this.m_AccumulatedTiming[(int)type];
			if (!dictionary.ContainsKey(samplerId))
			{
				dictionary.Add(samplerId, new DebugDisplayStats<TProfileId>.AccumulatedTiming());
			}
			return new DebugUI.Value
			{
				formatString = "{0:F2}ms",
				refreshRate = 0.2f,
				getter = (() => this.GetSamplerTiming(samplerId, sampler, type))
			};
		}

		private static readonly string[] k_DetailedStatsColumnLabels = new string[]
		{
			"CPU",
			"CPUInline",
			"GPU"
		};

		private Dictionary<TProfileId, DebugDisplayStats<TProfileId>.AccumulatedTiming>[] m_AccumulatedTiming = new Dictionary<TProfileId, DebugDisplayStats<TProfileId>.AccumulatedTiming>[]
		{
			new Dictionary<TProfileId, DebugDisplayStats<TProfileId>.AccumulatedTiming>(),
			new Dictionary<TProfileId, DebugDisplayStats<TProfileId>.AccumulatedTiming>(),
			new Dictionary<TProfileId, DebugDisplayStats<TProfileId>.AccumulatedTiming>()
		};

		private float m_TimeSinceLastAvgValue;

		private int m_AccumulatedFrames;

		private HashSet<TProfileId> m_HiddenProfileIds = new HashSet<TProfileId>();

		private const float k_AccumulationTimeInSeconds = 1f;

		protected bool averageProfilerTimingsOverASecond;

		protected bool hideEmptyScopes = true;

		private class AccumulatedTiming
		{
			internal void UpdateLastAverage(int frameCount)
			{
				this.lastAverage = this.accumulatedValue / (float)frameCount;
				this.accumulatedValue = 0f;
			}

			public float accumulatedValue;

			public float lastAverage;
		}

		private enum DebugProfilingType
		{
			CPU,
			InlineCPU,
			GPU
		}
	}
}
