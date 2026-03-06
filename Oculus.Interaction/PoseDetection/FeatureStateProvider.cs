using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Oculus.Interaction.PoseDetection
{
	public class FeatureStateProvider<[IsUnmanaged] TFeature, TFeatureState> where TFeature : struct, ValueType, Enum where TFeatureState : IEquatable<TFeatureState>
	{
		public int LastUpdatedFrameId { get; set; }

		private int EnumToInt(TFeature value)
		{
			return this._featureToInt(value);
		}

		public FeatureStateProvider(Func<TFeature, float?> valueReader, Func<TFeature, int> featureToInt, Func<float> timeProvider)
		{
			this._valueReader = valueReader;
			this._featureToInt = featureToInt;
			this._timeProvider = timeProvider;
		}

		public void InitializeThresholds(IFeatureThresholds<TFeature, TFeatureState> featureThresholds)
		{
			this._featureThresholds = featureThresholds;
			this._featureToThresholds = this.ValidateFeatureThresholds(featureThresholds.FeatureStateThresholds);
			this.InitializeStates();
		}

		public IFeatureStateThresholds<TFeature, TFeatureState>[] ValidateFeatureThresholds(IReadOnlyList<IFeatureStateThresholds<TFeature, TFeatureState>> featureStateThresholdsList)
		{
			IFeatureStateThresholds<TFeature, TFeatureState>[] array = new IFeatureStateThresholds<TFeature, TFeatureState>[Enum.GetNames(typeof(TFeature)).Length];
			foreach (IFeatureStateThresholds<TFeature, TFeatureState> featureStateThresholds in featureStateThresholdsList)
			{
				int num = this.EnumToInt(featureStateThresholds.Feature);
				array[num] = featureStateThresholds;
				for (int i = 0; i < featureStateThresholds.Thresholds.Count; i++)
				{
					IFeatureStateThreshold<TFeatureState> featureStateThreshold = featureStateThresholds.Thresholds[i];
					float toFirstWhenBelow = featureStateThreshold.ToFirstWhenBelow;
					float toSecondWhenAbove = featureStateThreshold.ToSecondWhenAbove;
				}
			}
			foreach (IFeatureStateThresholds<TFeature, TFeatureState> featureStateThresholds2 in array)
			{
			}
			return array;
		}

		private void InitializeStates()
		{
			this._featureToCurrentState = new FeatureStateProvider<TFeature, TFeatureState>.FeatureStateSnapshot[FeatureStateProvider<TFeature, TFeatureState>.FeatureEnumValues.Length];
			foreach (TFeature value in FeatureStateProvider<TFeature, TFeatureState>.FeatureEnumValues)
			{
				int num = this.EnumToInt(value);
				FeatureStateProvider<TFeature, TFeatureState>.FeatureStateSnapshot[] featureToCurrentState = this._featureToCurrentState;
				int num2 = num;
				featureToCurrentState[num2].State = default(TFeatureState);
				featureToCurrentState[num2].DesiredState = default(TFeatureState);
				featureToCurrentState[num2].DesiredStateEntryTime = 0.0;
			}
		}

		private ref IFeatureStateThresholds<TFeature, TFeatureState> GetFeatureThresholds(TFeature feature)
		{
			return ref this._featureToThresholds[this.EnumToInt(feature)];
		}

		public unsafe TFeatureState GetCurrentFeatureState(TFeature feature)
		{
			ref FeatureStateProvider<TFeature, TFeatureState>.FeatureStateSnapshot ptr = ref this._featureToCurrentState[this.EnumToInt(feature)];
			if (ptr.LastUpdatedFrameId == this.LastUpdatedFrameId)
			{
				return ptr.State;
			}
			float? num = this._valueReader(feature);
			if (num == null)
			{
				return ptr.State;
			}
			ptr.LastUpdatedFrameId = this.LastUpdatedFrameId;
			IReadOnlyList<IFeatureStateThreshold<TFeatureState>> thresholds = this.GetFeatureThresholds(feature)->Thresholds;
			TFeatureState tfeatureState;
			if (!ptr.HasCurrentState)
			{
				tfeatureState = this.ReadDesiredState(num.Value, thresholds);
			}
			else
			{
				tfeatureState = this.ReadDesiredState(num.Value, thresholds, ptr.State);
			}
			if (tfeatureState.Equals(ptr.State))
			{
				return ptr.State;
			}
			float num2 = this._timeProvider();
			if (!tfeatureState.Equals(ptr.DesiredState))
			{
				ptr.DesiredStateEntryTime = (double)num2;
				ptr.DesiredState = tfeatureState;
			}
			if (ptr.DesiredStateEntryTime + this._featureThresholds.MinTimeInState <= (double)num2)
			{
				ptr.HasCurrentState = true;
				ptr.State = tfeatureState;
			}
			return ptr.State;
		}

		private TFeatureState ReadDesiredState(float value, IReadOnlyList<IFeatureStateThreshold<TFeatureState>> featureStateThresholds, TFeatureState previousState)
		{
			TFeatureState tfeatureState = previousState;
			for (int i = 0; i < featureStateThresholds.Count; i++)
			{
				IFeatureStateThreshold<TFeatureState> featureStateThreshold = featureStateThresholds[i];
				if (tfeatureState.Equals(featureStateThreshold.FirstState) && value > featureStateThreshold.ToSecondWhenAbove)
				{
					return featureStateThreshold.SecondState;
				}
				if (tfeatureState.Equals(featureStateThreshold.SecondState) && value < featureStateThreshold.ToFirstWhenBelow)
				{
					return featureStateThreshold.FirstState;
				}
			}
			return previousState;
		}

		private TFeatureState ReadDesiredState(float value, IReadOnlyList<IFeatureStateThreshold<TFeatureState>> featureStateThresholds)
		{
			TFeatureState result = default(TFeatureState);
			for (int i = 0; i < featureStateThresholds.Count; i++)
			{
				IFeatureStateThreshold<TFeatureState> featureStateThreshold = featureStateThresholds[i];
				if (value <= featureStateThreshold.ToSecondWhenAbove)
				{
					result = featureStateThreshold.FirstState;
					break;
				}
				result = featureStateThreshold.SecondState;
			}
			return result;
		}

		public void ReadTouchedFeatureStates()
		{
			for (int i = 0; i < this._featureToCurrentState.Length; i++)
			{
				if (this._featureToCurrentState[i].LastUpdatedFrameId != 0)
				{
					this.GetCurrentFeatureState(FeatureStateProvider<TFeature, TFeatureState>.FeatureEnumValues[i]);
				}
			}
		}

		private FeatureStateProvider<TFeature, TFeatureState>.FeatureStateSnapshot[] _featureToCurrentState;

		private IFeatureStateThresholds<TFeature, TFeatureState>[] _featureToThresholds;

		private readonly Func<TFeature, float?> _valueReader;

		private readonly Func<TFeature, int> _featureToInt;

		private readonly Func<float> _timeProvider;

		private static readonly TFeature[] FeatureEnumValues = (TFeature[])Enum.GetValues(typeof(TFeature));

		private IFeatureThresholds<TFeature, TFeatureState> _featureThresholds;

		private struct FeatureStateSnapshot
		{
			public bool HasCurrentState;

			public TFeatureState State;

			public TFeatureState DesiredState;

			public int LastUpdatedFrameId;

			public double DesiredStateEntryTime;
		}
	}
}
