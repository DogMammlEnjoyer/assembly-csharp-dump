using System;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	public struct PokeStateData : IEquatable<PokeStateData>
	{
		public bool meetsRequirements { readonly get; set; }

		public Vector3 pokeInteractionPoint { readonly get; set; }

		public Vector3 axisAlignedPokeInteractionPoint { readonly get; set; }

		public float interactionStrength { readonly get; set; }

		public Vector3 axisNormal { readonly get; set; }

		public Transform target { readonly get; set; }

		public bool Equals(PokeStateData other)
		{
			return this.meetsRequirements == other.meetsRequirements && this.pokeInteractionPoint.Equals(other.pokeInteractionPoint) && this.axisAlignedPokeInteractionPoint.Equals(other.axisAlignedPokeInteractionPoint) && this.interactionStrength.Equals(other.interactionStrength) && this.axisNormal.Equals(other.axisNormal) && this.target == other.target;
		}

		public override bool Equals(object obj)
		{
			if (obj is PokeStateData)
			{
				PokeStateData other = (PokeStateData)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((((17 * 31 + this.meetsRequirements.GetHashCode()) * 31 + this.pokeInteractionPoint.GetHashCode()) * 31 + this.axisAlignedPokeInteractionPoint.GetHashCode()) * 31 + this.interactionStrength.GetHashCode()) * 31 + this.axisNormal.GetHashCode()) * 31 + this.target.GetHashCode();
		}
	}
}
