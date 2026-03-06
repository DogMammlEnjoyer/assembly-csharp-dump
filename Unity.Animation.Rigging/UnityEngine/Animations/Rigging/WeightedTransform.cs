using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct WeightedTransform : ITransformProvider, IWeightProvider, IEquatable<WeightedTransform>
	{
		public WeightedTransform(Transform transform, float weight)
		{
			this.transform = transform;
			this.weight = Mathf.Clamp01(weight);
		}

		public static WeightedTransform Default(float weight)
		{
			return new WeightedTransform(null, weight);
		}

		public bool Equals(WeightedTransform other)
		{
			return this.transform == other.transform && this.weight == other.weight;
		}

		Transform ITransformProvider.transform
		{
			get
			{
				return this.transform;
			}
			set
			{
				this.transform = value;
			}
		}

		float IWeightProvider.weight
		{
			get
			{
				return this.weight;
			}
			set
			{
				this.weight = Mathf.Clamp01(value);
			}
		}

		public Transform transform;

		public float weight;
	}
}
