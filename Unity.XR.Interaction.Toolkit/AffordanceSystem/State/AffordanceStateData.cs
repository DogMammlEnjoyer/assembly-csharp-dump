using System;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public readonly struct AffordanceStateData : IEquatable<AffordanceStateData>
	{
		public byte stateIndex { get; }

		public byte stateTransitionIncrement { get; }

		public float stateTransitionAmountFloat
		{
			get
			{
				return (float)this.stateTransitionIncrement / 255f;
			}
		}

		public AffordanceStateData(byte stateIndex, float transitionAmount)
		{
			this = new AffordanceStateData(stateIndex, (byte)(Mathf.Clamp01(transitionAmount) * 255f));
		}

		public AffordanceStateData(byte stateIndex, byte transitionIncrement)
		{
			this.stateIndex = stateIndex;
			this.stateTransitionIncrement = transitionIncrement;
		}

		public bool Equals(AffordanceStateData other)
		{
			return this.stateIndex == other.stateIndex && this.stateTransitionIncrement == other.stateTransitionIncrement;
		}

		public override bool Equals(object obj)
		{
			if (obj is AffordanceStateData)
			{
				AffordanceStateData other = (AffordanceStateData)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (17 * 31 + this.stateIndex.GetHashCode()) * 31 + this.stateTransitionIncrement.GetHashCode();
		}

		public const byte totalStateTransitionIncrements = 255;
	}
}
